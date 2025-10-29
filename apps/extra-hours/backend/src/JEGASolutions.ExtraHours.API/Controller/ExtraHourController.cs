using System.Security.Claims;
using JEGASolutions.ExtraHours.Core.Entities.Models;
using JEGASolutions.ExtraHours.Core.Interfaces;
using JEGASolutions.ExtraHours.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JEGASolutions.ExtraHours.API.Controller
{
    [Route("api/extra-hour")]
    [ApiController]
    public class ExtraHourController : ControllerBase
    {
        private readonly IExtraHourService _extraHourService;
        private readonly IEmployeeService _employeeService;
        private readonly IExtraHourCalculationService _calculationService;
        private readonly ITenantContextService _tenantContextService;

        public ExtraHourController(
            IExtraHourService extraHourService,
            IEmployeeService employeeService,
            IExtraHourCalculationService calculationService,
            ITenantContextService tenantContextService)
        {
            _extraHourService = extraHourService;
            _employeeService = employeeService;
            _calculationService = calculationService;
            _tenantContextService = tenantContextService;
        }

        [HttpPost("calculate")]
        public async Task<IActionResult> CalculateExtraHours([FromBody] ExtraHourCalculationRequest request)
        {
            if (request == null)
                return BadRequest(new { error = "Los datos de solicitud no pueden ser nulos" });

            try
            {
                // ✅ Try to extract tenant_id from JWT token (if user is authenticated)
                int tenantId = 1; // Default tenant
                bool hasTenantId = false;

                if (User.Identity?.IsAuthenticated == true)
                {
                    // Try multiple claim names for compatibility with different token issuers
                    var tenantIdClaim = User.Claims.FirstOrDefault(c => c.Type == "tenant_id")
                                     ?? User.Claims.FirstOrDefault(c => c.Type == "TenantId")
                                     ?? User.Claims.FirstOrDefault(c => c.Type == "tenantId"); // Landing API uses this format

                    if (tenantIdClaim != null && int.TryParse(tenantIdClaim.Value, out int parsedTenantId))
                    {
                        tenantId = parsedTenantId;
                        hasTenantId = true;
                        Console.WriteLine($"🔍 CalculateExtraHours - Authenticated user, TenantId: {tenantId}");
                    }
                    else
                    {
                        Console.WriteLine("⚠️ User authenticated but no tenant_id in token, using default tenant");
                    }
                }
                else
                {
                    Console.WriteLine("ℹ️ CalculateExtraHours - Unauthenticated request, using default tenant configuration");
                }

                // ✅ Use tenant-specific configuration (or default if not authenticated)
                var calculation = await _calculationService.DetermineExtraHourTypeAsync(
                    request.Date,
                    request.StartTime,
                    request.EndTime,
                    tenantId);

                return Ok(calculation);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                // If tenant configuration not found, provide helpful message
                return NotFound(new {
                    error = "Configuración no encontrada. Por favor, contacte al administrador.",
                    details = ex.Message
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al calcular horas extra: {ex.Message}");
                return StatusCode(500, new { error = "Error interno del servidor" });
            }
        }

        [HttpGet("manager/employees-extra-hours")]
        [Authorize(Roles = "manager")]
        public async Task<IActionResult> GetEmployeesExtraHoursByManager([FromQuery] string? startDate = null, [FromQuery] string? endDate = null)
        {
            // ✅ Extract tenant_id from JWT token and configure tenant context
            var tenantIdClaim = User.Claims.FirstOrDefault(c => c.Type == "tenant_id")
                             ?? User.Claims.FirstOrDefault(c => c.Type == "TenantId")
                             ?? User.Claims.FirstOrDefault(c => c.Type == "tenantId");

            if (tenantIdClaim != null && int.TryParse(tenantIdClaim.Value, out int tenantId))
            {
                _tenantContextService.SetCurrentTenantId(tenantId);
                Console.WriteLine($"✅ Tenant context configured: {tenantId}");
            }

            var managerId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (string.IsNullOrEmpty(managerId))
            {
                return Unauthorized(new { error = "No se pudo obtener el ID del manager logueado." });
            }

            long managerIdLong = long.Parse(managerId);
            Console.WriteLine($"[DEBUG] Manager ID: {managerIdLong}");

            var employees = await _employeeService.GetEmployeesByManagerIdAsync(managerIdLong);
            Console.WriteLine($"[DEBUG] Empleados encontrados: {employees?.Count ?? 0}");

            if (employees == null || !employees.Any())
            {
                Console.WriteLine("[DEBUG] No se encontraron empleados para este manager");
                return Ok(new List<object>());
            }

            var result = new List<object>();

            foreach (var employee in employees)
            {
                Console.WriteLine($"[DEBUG] Procesando empleado: {employee.name} (ID: {employee.id})");
                IEnumerable<ExtraHour> extraHours;

                // Si se proporcionan fechas, filtrar por rango de fechas
                if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate) &&
                    DateTime.TryParse(startDate, out var start) && DateTime.TryParse(endDate, out var end))
                {
                    // Obtener horas extras dentro del rango de fechas para este empleado
                    extraHours = await _extraHourService.FindExtraHoursByIdAndDateRangeAsync(employee.id, start, end);
                    Console.WriteLine($"[DEBUG] Horas extra encontradas con fechas: {extraHours?.Count() ?? 0}");
                }
                else
                {
                    // Si no hay fechas, obtener todas las horas extras de este empleado
                    extraHours = await _extraHourService.FindExtraHoursByIdAsync(employee.id);
                    Console.WriteLine($"[DEBUG] Horas extra encontradas sin fechas: {extraHours?.Count() ?? 0}");
                }

                if (extraHours != null && extraHours.Any())
                {
                    foreach (var extraHour in extraHours)
                    {
                        Console.WriteLine($"[DEBUG] Procesando hora extra: Registry={extraHour.registry}, Diurnal={extraHour.diurnal}, Nocturnal={extraHour.nocturnal}");
                        result.Add(new
                        {
                            id = employee.id,
                            name = employee.name,
                            position = employee.position,
                            salary = employee.salary,
                            manager = new { name = employee.manager?.User?.name ?? "Sin asignar" },
                            registry = extraHour.registry,
                            diurnal = extraHour.diurnal,
                            nocturnal = extraHour.nocturnal,
                            diurnalHoliday = extraHour.diurnalHoliday,
                            nocturnalHoliday = extraHour.nocturnalHoliday,
                            extrasHours = extraHour.extraHours,
                            date = extraHour.date.ToString("yyyy-MM-dd"),
                            startTime = extraHour.startTime,
                            endTime = extraHour.endTime,
                            approved = extraHour.approved,
                            approvedByManagerId = extraHour.ApprovedByManagerId,
                            approvedByManagerName = extraHour.ApprovedByManager?.User?.name ?? "No aprobado",
                            observations = extraHour.observations
                        });
                    }
                }
            }

            Console.WriteLine($"[DEBUG] Total de registros en resultado: {result.Count}");
            return Ok(result);

        }

        [HttpGet("id/{id}")]
        public async Task<IActionResult> GetExtraHoursById(long id)
        {
            var extraHours = await _extraHourService.FindExtraHoursByIdAsync(id);
            if (extraHours == null || !extraHours.Any())
                return Ok(new List<ExtraHour>());

            // Obtener la información del empleado
            var employee = await _employeeService.GetByIdAsync(id);
            if (employee == null)
                return NotFound(new { error = "Empleado no encontrado" });

            var result = new List<object>();

            foreach (var extraHour in extraHours)
            {
                result.Add(new
                {
                    id = employee.id,
                    name = employee.name,
                    position = employee.position,
                    salary = employee.salary,
                    manager = new { name = employee.manager?.User?.name ?? "Sin asignar" },
                    registry = extraHour.registry,
                    diurnal = extraHour.diurnal,
                    nocturnal = extraHour.nocturnal,
                    diurnalHoliday = extraHour.diurnalHoliday,
                    nocturnalHoliday = extraHour.nocturnalHoliday,
                    extraHours = extraHour.extraHours,
                    date = extraHour.date.ToString("yyyy-MM-dd"),
                    startTime = extraHour.startTime,
                    endTime = extraHour.endTime,
                    approved = extraHour.approved,
                    approvedByManagerId = extraHour.ApprovedByManagerId,
                    approvedByManagerName = extraHour.ApprovedByManager?.User?.name ?? "No aprobado",
                    observations = extraHour.observations
                });
            }

            return Ok(result);

        }


        [HttpGet("date-range-with-employee")]
        public async Task<IActionResult> GetExtraHoursByDateRangeWithEmployee(
            [FromQuery] string startDate,
            [FromQuery] string endDate)
        {
            if (string.IsNullOrEmpty(startDate) || string.IsNullOrEmpty(endDate))
                return BadRequest(new { error = "startDate y endDate son requeridos" });

            if (!DateTime.TryParse(startDate, out var start) || !DateTime.TryParse(endDate, out var end))
                return BadRequest(new { error = "Formato de fecha inválido" });

            var extraHours = await _extraHourService.FindByDateRangeAsync(start, end);
            if (extraHours == null || !extraHours.Any())
                return NotFound(new { error = "No se encontraron horas extra en el rango de fechas" });

            var result = new List<object>();

            foreach (var extraHour in extraHours)
            {
                var employee = await _employeeService.GetByIdAsync(extraHour.id); // Obtener el empleado por ID
                if (employee == null)
                    continue;

                result.Add(new
                {
                    id = employee.id,
                    name = employee.name,
                    position = employee.position,
                    salary = employee.salary,
                    manager = new { name = employee.manager?.User?.name ?? "Sin asignar" },
                    registry = extraHour.registry,
                    diurnal = extraHour.diurnal,
                    nocturnal = extraHour.nocturnal,
                    diurnalHoliday = extraHour.diurnalHoliday,
                    nocturnalHoliday = extraHour.nocturnalHoliday,
                    extraHours = extraHour.extraHours,
                    date = extraHour.date.ToString("yyyy-MM-dd"),
                    startTime = extraHour.startTime,
                    endTime = extraHour.endTime,
                    approved = extraHour.approved,
                    approvedByManagerId = extraHour.ApprovedByManagerId,
                    approvedByManagerName = extraHour.ApprovedByManager?.User?.name ?? "No aprobado",
                    observations = extraHour.observations
                });
            }

            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateExtraHour([FromBody] ExtraHour extraHour, IEmailService emailService)
        {
            // ✅ CRITICAL FIX: Extract tenant_id from JWT token
            // Try multiple claim names for compatibility with different token issuers
            var tenantIdClaim = User.Claims.FirstOrDefault(c => c.Type == "tenant_id")
                             ?? User.Claims.FirstOrDefault(c => c.Type == "TenantId")
                             ?? User.Claims.FirstOrDefault(c => c.Type == "tenantId"); // Landing API uses this format

            if (tenantIdClaim == null || !int.TryParse(tenantIdClaim.Value, out int tenantId))
            {
                Console.WriteLine("❌ ERROR: Tenant ID no encontrado en el token");
                return BadRequest(new { error = "Tenant ID no encontrado en el token. Por favor, inicie sesión nuevamente." });
            }

            // ✅ CRITICAL FIX: Configure tenant context before calling any services
            _tenantContextService.SetCurrentTenantId(tenantId);
            Console.WriteLine($"✅ Tenant context configured: {tenantId}");

            // Obtener ID del empleado desde el token
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            var userRole = User.Claims.FirstOrDefault(c => c.Type == "role")?.Value;

            Console.WriteLine($"🔍 CreateExtraHour - TenantId: {tenantId}, UserId: {userId}, Role: {userRole}");

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { error = "No se pudo obtener el ID del usuario logueado." });
            }

            if (extraHour == null)
            {
                return BadRequest(new { error = "Datos de horas extra no pueden ser nulos" });
            }

            long currentUserId = long.Parse(userId);
            long employeeId;

            if (userRole?.ToLower() == "superusuario")
            {
                // ✅ For superusers, verify that the employee exists AND belongs to the same tenant
                var targetEmployee = await _employeeService.GetByIdAsync(extraHour.id);

                if (targetEmployee == null)
                {
                    Console.WriteLine($"❌ Empleado {extraHour.id} no existe");
                    return BadRequest(new { error = "El empleado no existe" });
                }

                // ✅ CRITICAL: Verify employee belongs to the same tenant
                if (targetEmployee.TenantId != tenantId)
                {
                    Console.WriteLine($"❌ SECURITY: Empleado {extraHour.id} pertenece a tenant {targetEmployee.TenantId}, pero usuario está en tenant {tenantId}");
                    return Forbid();
                }

                employeeId = extraHour.id;
                Console.WriteLine($"✅ Superusuario creando horas extra para empleado {employeeId} en tenant {tenantId}");
            }
            else
            {
                employeeId = currentUserId;
                var employee = await _employeeService.GetByIdAsync(currentUserId);

                if (employee == null)
                {
                    Console.WriteLine($"❌ Empleado {currentUserId} no encontrado");
                    return BadRequest(new { error = "Empleado no encontrado" });
                }

                // ✅ CRITICAL: Verify employee belongs to the same tenant
                if (employee.TenantId != tenantId)
                {
                    Console.WriteLine($"❌ SECURITY: Empleado {currentUserId} pertenece a tenant {employee.TenantId}, pero token tiene tenant {tenantId}");
                    return Forbid();
                }

                if (employee.manager_id == null)
                {
                    return BadRequest(new { error = "El empleado no tiene un manager asignado" });
                }

                if (extraHour.id > 0 && currentUserId != extraHour.id)
                {
                    return Forbid();
                }

                extraHour.id = employeeId;
            }

            if (extraHour.startTime == TimeSpan.Zero)
                return BadRequest(new { error = "Formato de startTime inválido" });

            if (extraHour.endTime == TimeSpan.Zero)
                return BadRequest(new { error = "Formato de endTime inválido" });

            // ✅ CRITICAL: Assign tenant_id to the extra hour record
            extraHour.TenantId = tenantId;
            extraHour.approved = false;
            extraHour.ApprovedByManagerId = null;

            // Realizar el cálculo automático en el backend
            try
            {
                // ✅ Use the new method that filters by tenant_id
                var calculation = await _calculationService.DetermineExtraHourTypeAsync(
                    extraHour.date,
                    extraHour.startTime,
                    extraHour.endTime,
                    tenantId);

                // Actualizar los valores calculados
                extraHour.diurnal = calculation.diurnal;
                extraHour.nocturnal = calculation.nocturnal;
                extraHour.diurnalHoliday = calculation.diurnalHoliday;
                extraHour.nocturnalHoliday = calculation.nocturnalHoliday;
                extraHour.extraHours = calculation.extraHours;

                Console.WriteLine($"📝 Guardando horas extra para empleado {employeeId}, tenant {tenantId}");
                Console.WriteLine($"   Cálculo: Diurnal={calculation.diurnal}, Nocturnal={calculation.nocturnal}, Total={calculation.extraHours}");

                var savedExtraHour = await _extraHourService.AddExtraHourAsync(extraHour);

                Console.WriteLine($"✅ Horas extra guardadas exitosamente: Registry={savedExtraHour.registry}, EmployeeId={savedExtraHour.id}, TenantId={savedExtraHour.TenantId}");

                var employee = await _employeeService.GetByIdAsync(employeeId);

                // Enviar correo al manager si existe
                if (employee?.manager_id != null)
                {
                    var managerEmail = employee?.manager?.User?.email ?? string.Empty;

                    if (!string.IsNullOrEmpty(managerEmail))
                    {
                        var emailSubject = $"Nuevo Registro de Horas Extra - {employee?.name ?? string.Empty}";
                        var emailBody = $@"
                <html>
                <body>
                    <h2>Registro de Horas Extra</h2>
                    <p><strong>Empleado:</strong> {employee?.name ?? string.Empty}</p>
                    <p><strong>Fecha:</strong> {extraHour.date:yyyy-MM-dd}</p>
                    <p><strong>Hora de Inicio:</strong> {extraHour.startTime}</p>
                    <p><strong>Hora de Fin:</strong> {extraHour.endTime}</p>
                    <p><strong>Total Horas Extra:</strong> {extraHour.extraHours}</p>
                    <p><strong>Horas Diurnas:</strong> {extraHour.diurnal}</p>
                    <p><strong>Horas Nocturnas:</strong> {extraHour.nocturnal}</p>
                    <p><strong>Horas Diurnas Festivas:</strong> {extraHour.diurnalHoliday}</p>
                    <p><strong>Horas Nocturnas Festivas:</strong> {extraHour.nocturnalHoliday}</p>
                    <p><strong>Observaciones:</strong> {extraHour.observations}</p>
                    <br/>
                    <p>Por favor, revise y apruebe las horas extra registradas.</p>
                </body>
                </html>";

                        try
                        {
                            await emailService.SendEmailAsync(managerEmail, emailSubject, emailBody);
                            Console.WriteLine($"Correo enviado exitosamente a: {managerEmail}");
                        }
                        catch (Exception ex)
                        {
                            // Registrar el error pero no fallar la operación principal
                            Console.WriteLine($"Error enviando correo: {ex.Message}");
                        }
                    }
                }

                return Created("", savedExtraHour);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar horas extra: {ex.Message}");
                return StatusCode(500, new { error = "Error interno del servidor" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllExtraHours()
        {
            var extraHours = await _extraHourService.GetAllAsync();
            return Ok(extraHours);
        }

        [HttpPatch("{registry}/approve")]
        [Authorize(Roles = "manager, superusuario")]
        public async Task<IActionResult> ApproveExtraHour(long registry)
        {
            // ✅ Extract tenant_id from JWT token and configure tenant context
            var tenantIdClaim = User.Claims.FirstOrDefault(c => c.Type == "tenant_id")
                             ?? User.Claims.FirstOrDefault(c => c.Type == "TenantId")
                             ?? User.Claims.FirstOrDefault(c => c.Type == "tenantId");

            if (tenantIdClaim != null && int.TryParse(tenantIdClaim.Value, out int tenantId))
            {
                _tenantContextService.SetCurrentTenantId(tenantId);
                Console.WriteLine($"✅ Tenant context configured: {tenantId}");
            }

            var extraHour = await _extraHourService.FindByRegistryAsync(registry);
            if (extraHour == null)
                return NotFound(new { error = "Registro de horas extra no encontrado" });

            // Obtener ID del manager desde el token
            var managerId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (string.IsNullOrEmpty(managerId))
            {
                return Unauthorized(new { error = "No se pudo obtener el ID del manager logueado." });
            }

            long managerIdLong = long.Parse(managerId);

            extraHour.approved = true;
            extraHour.ApprovedByManagerId = managerIdLong;

            await _extraHourService.UpdateExtraHourAsync(extraHour);

            var updatedExtraHour = await _extraHourService.FindByRegistryAsync(registry);


            return Ok(extraHour);
        }

        [HttpPut("{registry}/update")]
        [Authorize(Roles = "manager, superusuario")]
        public async Task<IActionResult> UpdateExtraHour(long registry, [FromBody] ExtraHour extraHourDetails)
        {
            // ✅ Extract tenant_id from JWT token and configure tenant context
            var tenantIdClaim = User.Claims.FirstOrDefault(c => c.Type == "tenant_id")
                             ?? User.Claims.FirstOrDefault(c => c.Type == "TenantId")
                             ?? User.Claims.FirstOrDefault(c => c.Type == "tenantId");

            if (tenantIdClaim != null && int.TryParse(tenantIdClaim.Value, out int tenantId))
            {
                _tenantContextService.SetCurrentTenantId(tenantId);
                Console.WriteLine($"✅ Tenant context configured: {tenantId}");
            }

            var existingExtraHour = await _extraHourService.FindByRegistryAsync(registry);
            if (existingExtraHour == null)
                return NotFound(new { error = "Registro de horas extra no encontrado" });

            existingExtraHour.diurnal = extraHourDetails.diurnal;
            existingExtraHour.nocturnal = extraHourDetails.nocturnal;
            existingExtraHour.diurnalHoliday = extraHourDetails.diurnalHoliday;
            existingExtraHour.nocturnalHoliday = extraHourDetails.nocturnalHoliday;
            existingExtraHour.extraHours = extraHourDetails.diurnal +
                                           extraHourDetails.nocturnal +
                                           extraHourDetails.diurnalHoliday +
                                           extraHourDetails.nocturnalHoliday;
            existingExtraHour.date = extraHourDetails.date;
            existingExtraHour.observations = extraHourDetails.observations;

            await _extraHourService.UpdateExtraHourAsync(existingExtraHour);
            return Ok(existingExtraHour);
        }

        [HttpDelete("{registry}/delete")]
        [Authorize(Roles = "manager, superusuario")]
        public async Task<IActionResult> DeleteExtraHour(long registry)
        {
            // ✅ Extract tenant_id from JWT token and configure tenant context
            var tenantIdClaim = User.Claims.FirstOrDefault(c => c.Type == "tenant_id")
                             ?? User.Claims.FirstOrDefault(c => c.Type == "TenantId")
                             ?? User.Claims.FirstOrDefault(c => c.Type == "tenantId");

            if (tenantIdClaim != null && int.TryParse(tenantIdClaim.Value, out int tenantId))
            {
                _tenantContextService.SetCurrentTenantId(tenantId);
                Console.WriteLine($"✅ Tenant context configured: {tenantId}");
            }

            var deleted = await _extraHourService.DeleteExtraHourByRegistryAsync(registry);
            if (!deleted)
                return NotFound(new { error = "Registro de horas extra no encontrado" });

            return Ok(new { message = "Registro eliminado exitosamente" });
        }

        [HttpGet("all-employees-extra-hours")]
        [Authorize(Roles = "superusuario")]
        public async Task<IActionResult> GetAllEmployeesExtraHours([FromQuery] string? startDate = null, [FromQuery] string? endDate = null)
        {
            // ✅ Extract tenant_id from JWT token and configure tenant context
            var tenantIdClaim = User.Claims.FirstOrDefault(c => c.Type == "tenant_id")
                             ?? User.Claims.FirstOrDefault(c => c.Type == "TenantId")
                             ?? User.Claims.FirstOrDefault(c => c.Type == "tenantId");

            if (tenantIdClaim != null && int.TryParse(tenantIdClaim.Value, out int tenantId))
            {
                _tenantContextService.SetCurrentTenantId(tenantId);
                Console.WriteLine($"✅ Tenant context configured: {tenantId}");
            }

            var result = new List<object>();
            var allEmployees = await _employeeService.GetAllAsync();

            foreach (var employee in allEmployees)
            {
                IEnumerable<ExtraHour> extraHours;

                if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate) &&
           DateTime.TryParse(startDate, out var start) && DateTime.TryParse(endDate, out var end))
                {
                    extraHours = await _extraHourService.FindExtraHoursByIdAndDateRangeAsync(employee.id, start, end);
                }
                else
                {
                    extraHours = await _extraHourService.FindExtraHoursByIdAsync(employee.id);
                }

                if (extraHours != null && extraHours.Any())
                {
                    foreach (var extraHour in extraHours)
                    {
                        result.Add(new
                        {
                            id = employee.id,
                            name = employee.name,
                            position = employee.position,
                            salary = employee.salary,
                            manager = new { name = employee.manager?.User?.name ?? "Sin asignar" },
                            registry = extraHour.registry,
                            diurnal = extraHour.diurnal,
                            nocturnal = extraHour.nocturnal,
                            diurnalHoliday = extraHour.diurnalHoliday,
                            nocturnalHoliday = extraHour.nocturnalHoliday,
                            extrasHours = extraHour.extraHours,
                            date = extraHour.date.ToString("yyyy-MM-dd"),
                            startTime = extraHour.startTime,
                            endTime = extraHour.endTime,
                            approved = extraHour.approved,
                            approvedByManagerId = extraHour.ApprovedByManagerId,
                            approvedByManagerName = extraHour.ApprovedByManager?.User?.name ?? "No aprobado",
                            observations = extraHour.observations
                        });
                    }
                }
            }

            return Ok(result);

        }


    }

    public class ExtraHourCalculationRequest
    {
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
