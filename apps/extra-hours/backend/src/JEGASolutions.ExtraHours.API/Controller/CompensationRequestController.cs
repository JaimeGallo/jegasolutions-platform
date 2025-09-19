using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using JEGASolutions.ExtraHours.API.Model;
using JEGASolutions.ExtraHours.API.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using JEGASolutions.ExtraHours.API.Dto;
using Microsoft.EntityFrameworkCore;
using JEGASolutions.ExtraHours.API.Data;
using System.Security.Claims;

namespace JEGASolutions.ExtraHours.API.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompensationRequestController : ControllerBase
    {
        private readonly ICompensationRequestService _service;
        private readonly IEmployeeService _employeeService;
        private readonly AppDbContext _context;
        public CompensationRequestController(ICompensationRequestService service, IEmployeeService employeeService, AppDbContext context)
        {
            _service = service;
            _employeeService = employeeService;
            _context = context;
        }

        // GET: api/CompensationRequest/all
        [HttpGet("all")]
        [Authorize(Roles = "superusuario")]
        public async Task<ActionResult<IEnumerable<object>>> GetAllCompensationRequests([FromQuery] string? startDate = null, [FromQuery] string? endDate = null)
        {
            try
            {
                DateTime? start = null;
                DateTime? end = null;

                if (!string.IsNullOrEmpty(startDate) && DateTime.TryParse(startDate, out var parsedStart))
                {
                    start = parsedStart;
                }

                if (!string.IsNullOrEmpty(endDate) && DateTime.TryParse(endDate, out var parsedEnd))
                {
                    end = parsedEnd.Date.AddDays(1).AddSeconds(-1); // Final del día
                }

                var requests = await _service.GetAllFilteredAsync(start, end);

                // Devolver datos enriquecidos para el frontend
                var result = requests.Select(r => new
                {
                    r.id,
                    r.EmployeeId,
                    employeeName = r.Employee?.name,
                    r.WorkDate,
                    r.RequestedCompensationDate,
                    r.Status,
                    r.Justification,
                    r.ApprovedById,
                    approvedByName = r.ApprovedBy?.name, // User.Name en lugar de Employee.name
                    r.RequestedAt,
                    r.DecidedAt
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno del servidor: " + ex.Message });
            }
        }

        // GET: api/CompensationRequest/manager
        [HttpGet("manager")]
        [Authorize(Roles = "manager,superusuario")]
        public async Task<ActionResult<IEnumerable<object>>> GetByManager([FromQuery] string? startDate = null, [FromQuery] string? endDate = null)
        {
            try
            {
                var managerIdStr = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
                if (string.IsNullOrEmpty(managerIdStr))
                {
                    return Unauthorized(new { error = "No se pudo obtener el ID del manager autenticado." });
                }

                long managerId = long.Parse(managerIdStr);

                DateTime? start = null;
                DateTime? end = null;

                if (!string.IsNullOrEmpty(startDate) && DateTime.TryParse(startDate, out var parsedStart))
                {
                    start = parsedStart;
                }

                if (!string.IsNullOrEmpty(endDate) && DateTime.TryParse(endDate, out var parsedEnd))
                {
                    end = parsedEnd.Date.AddDays(1).AddSeconds(-1); // Final del día
                }

                var requests = await _service.GetByManagerFilteredAsync(managerId, start, end);

                // Devolver datos enriquecidos para el frontend
                var result = requests.Select(r => new
                {
                    r.id,
                    r.EmployeeId,
                    employeeName = r.Employee?.name,
                    r.WorkDate,
                    r.RequestedCompensationDate,
                    r.Status,
                    r.Justification,
                    r.ApprovedById,
                    approvedByName = r.ApprovedBy?.name, // User.Name en lugar de Employee.name
                    r.RequestedAt,
                    r.DecidedAt
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno del servidor: " + ex.Message });
            }
        }

        // PUT: api/CompensationRequest/{id}/status
        [HttpPut("{id}/status")]
        [Authorize(Roles = "manager,superusuario")]
        public async Task<ActionResult<CompensationRequest>> UpdateStatus(int id, [FromBody] StatusUpdateDto dto)
        {
            try
            {
                // Obtener información del usuario autenticado
                var userIdStr = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
                var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                if (string.IsNullOrEmpty(userIdStr))
                {
                    return Unauthorized(new { error = "No se pudo obtener el ID del usuario autenticado." });
                }

                long userId = long.Parse(userIdStr);

                // Verificar que la solicitud existe y obtener datos
                var request = await _service.GetByIdAsync(id);
                if (request == null)
                {
                    return NotFound(new { error = "Solicitud no encontrada" });
                }

                // Verificar que la solicitud esté pendiente
                if (request.Status != "Pending")
                {
                    return BadRequest(new { error = "Solo se pueden aprobar/rechazar solicitudes en estado 'Pending'" });
                }

                // Verificar permisos según el rol
                if (userRole?.ToLower() == "manager")
                {
                    var employee = await _employeeService.GetByIdAsync(request.EmployeeId);
                    if (employee == null || employee.manager_id != userId)
                    {
                        return Forbid();
                    }
                }

                // Validar que si es rechazo, debe tener justificación
                if (dto.Status == "Rejected" && string.IsNullOrWhiteSpace(dto.Justification))
                {
                    return BadRequest(new { error = "Debe proporcionar una justificación para rechazar la solicitud" });
                }

                var updated = await _service.UpdateStatusAsync(id, dto.Status, dto.Justification, userId);
                if (updated == null)
                {
                    return NotFound();
                }

                return Ok(updated);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno del servidor: " + ex.Message });
            }
        }

        // PUT: api/CompensationRequest/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "manager,superusuario")]
        public async Task<ActionResult<CompensationRequest>> Update(int id, [FromBody] CompensationRequestDto dto)
        {
            // Solo permitir edición si la solicitud está pendiente
            var existing = await _service.GetByIdAsync(id);
            if (existing == null) return NotFound();
            if (existing.Status != "Pending")
                return BadRequest(new { error = "Solo se pueden editar solicitudes en estado 'Pending'" });

            // Solo superusuario puede editar cualquier solicitud, manager solo las de sus empleados
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (userRole?.ToLower() == "manager")
            {
                var employee = await _employeeService.GetByIdAsync(existing.EmployeeId);
                if (employee == null || employee.manager_id.ToString() != userId)
                    return Forbid();
            }

            // Validar fechas
            if (!DateTime.TryParse(dto.WorkDate, out var workDate))
                return BadRequest(new { error = $"Formato de fecha inválido para WorkDate: '{dto.WorkDate}'" });
            if (!DateTime.TryParse(dto.RequestedCompensationDate, out var requestedCompensationDate))
                return BadRequest(new { error = $"Formato de fecha inválido para RequestedCompensationDate: '{dto.RequestedCompensationDate}'" });
            if (workDate > DateTime.Now)
                return BadRequest(new { error = "La fecha de trabajo no puede ser futura" });
            if (requestedCompensationDate <= workDate)
                return BadRequest(new { error = "La fecha de compensación debe ser posterior a la fecha de trabajo" });

            // Actualizar campos editables
            existing.WorkDate = workDate;
            existing.RequestedCompensationDate = requestedCompensationDate;
            existing.Justification = string.IsNullOrWhiteSpace(dto.Justification) ? null : dto.Justification.Trim();

            var updated = await _service.UpdateAsync(existing);
            return Ok(updated);
        }

        // POST: api/CompensationRequest
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<CompensationRequest>> Create([FromBody] CompensationRequestDto dto)
        {
            try
            {
                // Obtener ID del empleado desde el token (igual que en ExtraHourController)
                var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
                var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { error = "No se pudo obtener el ID del usuario logueado." });
                }

                long currentUserId = long.Parse(userId);
                long employeeId;

                // Validar según el rol (igual que en ExtraHourController)
                if (userRole?.ToLower() == "superusuario")
                {
                    var targetEmployeeExists = await _employeeService.EmployeeExistsAsync(dto.EmployeeId);
                    if (!targetEmployeeExists)
                    {
                        return BadRequest(new { error = "El empleado no existe" });
                    }
                    employeeId = dto.EmployeeId;
                }
                else
                {
                    employeeId = currentUserId;
                    var employee = await _employeeService.GetByIdAsync(currentUserId);
                    if (employee == null || employee.manager_id == null)
                    {
                        return BadRequest(new { error = "El empleado no tiene un manager asignado" });
                    }

                    // Verificar que el empleado no esté intentando crear una solicitud para otro empleado
                    if (dto.EmployeeId > 0 && currentUserId != dto.EmployeeId)
                    {
                        return Forbid();
                    }
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validar que las fechas no estén vacías
                if (string.IsNullOrWhiteSpace(dto.WorkDate))
                {
                    return BadRequest(new { error = "La fecha de trabajo es requerida" });
                }

                if (string.IsNullOrWhiteSpace(dto.RequestedCompensationDate))
                {
                    return BadRequest(new { error = "La fecha de compensación solicitada es requerida" });
                }

                DateTime workDate;
                DateTime requestedCompensationDate;

                // Intentar convertir WorkDate
                if (!DateTime.TryParse(dto.WorkDate, out workDate))
                {
                    return BadRequest(new { error = $"Formato de fecha inválido para WorkDate: '{dto.WorkDate}'" });
                }

                // Intentar convertir RequestedCompensationDate
                if (!DateTime.TryParse(dto.RequestedCompensationDate, out requestedCompensationDate))
                {
                    return BadRequest(new { error = $"Formato de fecha inválido para RequestedCompensationDate: '{dto.RequestedCompensationDate}'" });
                }

                // Validaciones de negocio
                if (workDate > DateTime.Now)
                {
                    return BadRequest(new { error = "La fecha de trabajo no puede ser futura" });
                }

                if (requestedCompensationDate <= workDate)
                {
                    return BadRequest(new { error = "La fecha de compensación debe ser posterior a la fecha de trabajo" });
                }

                // Asegurar que Justification no sea una cadena vacía
                string? justification = string.IsNullOrWhiteSpace(dto.Justification) ? null : dto.Justification.Trim();

                var request = new CompensationRequest
                {
                    EmployeeId = employeeId,
                    WorkDate = workDate,
                    RequestedCompensationDate = requestedCompensationDate,
                    Status = "Pending", // Estado inicial fijo
                    Justification = justification, // null si está vacío, sino el valor trimmed
                    ApprovedById = null, // Siempre null para nuevas solicitudes
                    RequestedAt = DateTime.UtcNow, // Timestamp de creación
                    DecidedAt = null // Siempre null para nuevas solicitudes
                };

                var created = await _service.CreateAsync(request);

                return CreatedAtAction(nameof(GetById), new { id = created.id }, created);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // GET: api/CompensationRequest
        [HttpGet]
        [Authorize(Roles = "manager,superusuario")]
        public async Task<ActionResult<IEnumerable<CompensationRequest>>> GetAll()
        {
            var list = await _service.GetAllAsync();
            return Ok(list);
        }

        // GET: api/CompensationRequest/{id}
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<CompensationRequest>> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        // GET: api/CompensationRequest/employee/{employeeId}
        [HttpGet("employee/{employeeId}")]
        [Authorize(Roles = "empleado,manager,superusuario")]
        public async Task<ActionResult<IEnumerable<CompensationRequest>>> GetByEmployeeId(long employeeId)
        {
            var list = await _service.GetByEmployeeIdAsync(employeeId);
            return Ok(list);
        }

        // DELETE: api/CompensationRequest/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "manager,superusuario")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Verificar que la solicitud existe
                var request = await _service.GetByIdAsync(id);
                if (request == null)
                {
                    return NotFound(new { error = "Solicitud no encontrada" });
                }

                // Verificar permisos según el rol
                var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
                var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                if (userRole?.ToLower() == "manager")
                {
                    var employee = await _employeeService.GetByIdAsync(request.EmployeeId);
                    if (employee == null || employee.manager_id.ToString() != userId)
                    {
                        return Forbid();
                    }
                }

                var deleted = await _service.DeleteAsync(id);
                if (!deleted)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno del servidor: " + ex.Message });
            }
        }
    }

    public class StatusUpdateDto
    {
        public string Status { get; set; } = string.Empty;
        public string? Justification { get; set; }
        public long? ApprovedById { get; set; }
    }
}