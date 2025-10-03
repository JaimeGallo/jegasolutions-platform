using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using JEGASolutions.ReportBuilder.Core.Dto;
using JEGASolutions.ReportBuilder.Core.Interfaces;
using System.Security.Claims;

namespace JEGASolutions.ReportBuilder.API.Controllers
{
    /// <summary>
    /// Controller para gestión de Plantillas Consolidadas con aislamiento multi-tenant
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ConsolidatedTemplatesController : ControllerBase
    {
        private readonly IConsolidatedTemplateService _consolidatedTemplateService;
        private readonly ILogger<ConsolidatedTemplatesController> _logger;

        public ConsolidatedTemplatesController(
            IConsolidatedTemplateService consolidatedTemplateService,
            ILogger<ConsolidatedTemplatesController> logger)
        {
            _consolidatedTemplateService = consolidatedTemplateService;
            _logger = logger;
        }

        // ==================== HELPER METHODS ====================

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : 0;
        }

        private int GetCurrentTenantId()
        {
            // Por ahora retornamos 1 (default tenant)
            // En implementación completa, esto vendría del JWT o del usuario
            return 1;
        }

        private bool IsAdmin()
        {
            return User.IsInRole("Admin");
        }

        // ==================== ADMIN ENDPOINTS ====================

        /// <summary>
        /// [ADMIN] Crea nueva plantilla consolidada con secciones
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateConsolidatedTemplate([FromBody] ConsolidatedTemplateCreateDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var tenantId = GetCurrentTenantId();

                var result = await _consolidatedTemplateService.CreateConsolidatedTemplateAsync(dto, userId, tenantId);
                
                _logger.LogInformation(
                    "Plantilla consolidada creada: {TemplateId} por usuario {UserId} en tenant {TenantId}", 
                    result.Id, userId, tenantId);

                return CreatedAtAction(
                    nameof(GetConsolidatedTemplateById), 
                    new { id = result.Id }, 
                    result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Error de validación al crear plantilla consolidada");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear plantilla consolidada");
                return StatusCode(500, new { message = "Error al crear plantilla consolidada" });
            }
        }

        /// <summary>
        /// [ADMIN] Obtiene todas las plantillas consolidadas del tenant
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllConsolidatedTemplates([FromQuery] string? status = null)
        {
            try
            {
                var tenantId = GetCurrentTenantId();
                var templates = await _consolidatedTemplateService.GetAllConsolidatedTemplatesAsync(tenantId, status);

                _logger.LogInformation(
                    "Listado de plantillas consolidadas obtenido para tenant {TenantId}, filtro status: {Status}", 
                    tenantId, status ?? "all");

                return Ok(templates);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener plantillas consolidadas");
                return StatusCode(500, new { message = "Error al obtener plantillas consolidadas" });
            }
        }

        /// <summary>
        /// [ADMIN] Obtiene detalle completo de plantilla consolidada
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetConsolidatedTemplateById(int id)
        {
            try
            {
                var tenantId = GetCurrentTenantId();
                var template = await _consolidatedTemplateService.GetConsolidatedTemplateByIdAsync(id, tenantId);

                if (template == null)
                {
                    return NotFound(new { message = "Plantilla consolidada no encontrada" });
                }

                return Ok(template);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener plantilla consolidada {TemplateId}", id);
                return StatusCode(500, new { message = "Error al obtener plantilla consolidada" });
            }
        }

        /// <summary>
        /// [ADMIN] Actualiza plantilla consolidada
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateConsolidatedTemplate(int id, [FromBody] ConsolidatedTemplateUpdateDto dto)
        {
            try
            {
                if (id != dto.Id)
                {
                    return BadRequest(new { message = "El ID de la URL no coincide con el ID del DTO" });
                }

                var userId = GetCurrentUserId();
                var tenantId = GetCurrentTenantId();

                var result = await _consolidatedTemplateService.UpdateConsolidatedTemplateAsync(dto, userId, tenantId);
                
                _logger.LogInformation(
                    "Plantilla consolidada actualizada: {TemplateId} por usuario {UserId}", 
                    result.Id, userId);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Error de validación al actualizar plantilla consolidada {TemplateId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar plantilla consolidada {TemplateId}", id);
                return StatusCode(500, new { message = "Error al actualizar plantilla consolidada" });
            }
        }

        /// <summary>
        /// [ADMIN] Elimina plantilla consolidada (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConsolidatedTemplate(int id)
        {
            try
            {
                var tenantId = GetCurrentTenantId();
                var result = await _consolidatedTemplateService.DeleteConsolidatedTemplateAsync(id, tenantId);

                if (!result)
                {
                    return NotFound(new { message = "Plantilla consolidada no encontrada" });
                }

                _logger.LogInformation(
                    "Plantilla consolidada eliminada: {TemplateId} en tenant {TenantId}", 
                    id, tenantId);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar plantilla consolidada {TemplateId}", id);
                return StatusCode(500, new { message = "Error al eliminar plantilla consolidada" });
            }
        }

        /// <summary>
        /// [ADMIN] Agrega nueva sección a plantilla consolidada existente
        /// </summary>
        [HttpPost("{templateId}/sections")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddSectionToTemplate(int templateId, [FromBody] ConsolidatedTemplateSectionCreateDto dto)
        {
            try
            {
                var tenantId = GetCurrentTenantId();
                var result = await _consolidatedTemplateService.AddSectionToTemplateAsync(templateId, dto, tenantId);

                _logger.LogInformation(
                    "Sección agregada a plantilla consolidada {TemplateId}: {SectionId}", 
                    templateId, result.Id);

                return CreatedAtAction(
                    nameof(GetMyTaskDetail), 
                    new { sectionId = result.Id }, 
                    result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Error al agregar sección a plantilla {TemplateId}", templateId);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar sección a plantilla {TemplateId}", templateId);
                return StatusCode(500, new { message = "Error al agregar sección" });
            }
        }

        /// <summary>
        /// [ADMIN] Actualiza estado de sección
        /// </summary>
        [HttpPut("sections/{sectionId}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateSectionStatus(int sectionId, [FromBody] ConsolidatedTemplateSectionUpdateStatusDto dto)
        {
            try
            {
                if (sectionId != dto.SectionId)
                {
                    return BadRequest(new { message = "El ID de la URL no coincide con el ID del DTO" });
                }

                var tenantId = GetCurrentTenantId();
                var result = await _consolidatedTemplateService.UpdateSectionStatusAsync(dto, tenantId);

                if (!result)
                {
                    return NotFound(new { message = "Sección no encontrada" });
                }

                _logger.LogInformation(
                    "Estado de sección actualizado: {SectionId} a {Status}", 
                    sectionId, dto.Status);

                return Ok(new { message = "Estado actualizado correctamente" });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Error al actualizar estado de sección {SectionId}", sectionId);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar estado de sección {SectionId}", sectionId);
                return StatusCode(500, new { message = "Error al actualizar estado de sección" });
            }
        }

        /// <summary>
        /// [ADMIN] Obtiene estadísticas de plantillas consolidadas (Dashboard)
        /// </summary>
        [HttpGet("stats")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetStats()
        {
            try
            {
                var tenantId = GetCurrentTenantId();
                var stats = await _consolidatedTemplateService.GetConsolidatedTemplateStatsAsync(tenantId);

                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estadísticas de plantillas consolidadas");
                return StatusCode(500, new { message = "Error al obtener estadísticas" });
            }
        }

        /// <summary>
        /// [ADMIN] Obtiene secciones próximas a vencer
        /// </summary>
        [HttpGet("upcoming-deadlines")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUpcomingDeadlines([FromQuery] int daysAhead = 7)
        {
            try
            {
                var tenantId = GetCurrentTenantId();
                var sections = await _consolidatedTemplateService.GetUpcomingDeadlinesAsync(tenantId, daysAhead);

                return Ok(sections);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener deadlines próximos");
                return StatusCode(500, new { message = "Error al obtener deadlines" });
            }
        }

        /// <summary>
        /// [ADMIN] Obtiene secciones vencidas
        /// </summary>
        [HttpGet("overdue-sections")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetOverdueSections()
        {
            try
            {
                var tenantId = GetCurrentTenantId();
                var sections = await _consolidatedTemplateService.GetOverdueSectionsAsync(tenantId);

                return Ok(sections);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener secciones vencidas");
                return StatusCode(500, new { message = "Error al obtener secciones vencidas" });
            }
        }

        // ==================== USER ENDPOINTS ====================

        /// <summary>
        /// [USER] Obtiene "Mis Tareas" - secciones asignadas al área del usuario
        /// </summary>
        [HttpGet("my-tasks")]
        public async Task<IActionResult> GetMyTasks()
        {
            try
            {
                var userId = GetCurrentUserId();
                var tenantId = GetCurrentTenantId();

                var tasks = await _consolidatedTemplateService.GetMyTasksAsync(userId, tenantId);

                _logger.LogInformation(
                    "Tareas obtenidas para usuario {UserId} en tenant {TenantId}: {TaskCount} tareas", 
                    userId, tenantId, tasks.Count);

                return Ok(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener mis tareas para usuario {UserId}", GetCurrentUserId());
                return StatusCode(500, new { message = "Error al obtener tareas" });
            }
        }

        /// <summary>
        /// [USER] Obtiene detalle de una tarea específica
        /// </summary>
        [HttpGet("my-tasks/{sectionId}")]
        public async Task<IActionResult> GetMyTaskDetail(int sectionId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var tenantId = GetCurrentTenantId();

                var task = await _consolidatedTemplateService.GetMyTaskDetailAsync(sectionId, userId, tenantId);

                if (task == null)
                {
                    return NotFound(new { message = "Tarea no encontrada o no tienes acceso a ella" });
                }

                return Ok(task);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener detalle de tarea {SectionId}", sectionId);
                return StatusCode(500, new { message = "Error al obtener detalle de tarea" });
            }
        }

        /// <summary>
        /// [USER] Actualiza contenido de sección (guardar progreso o completar)
        /// </summary>
        [HttpPut("sections/{sectionId}/content")]
        public async Task<IActionResult> UpdateSectionContent(int sectionId, [FromBody] ConsolidatedTemplateSectionUpdateContentDto dto)
        {
            try
            {
                if (sectionId != dto.SectionId)
                {
                    return BadRequest(new { message = "El ID de la URL no coincide con el ID del DTO" });
                }

                var userId = GetCurrentUserId();
                var tenantId = GetCurrentTenantId();

                var result = await _consolidatedTemplateService.UpdateSectionContentAsync(dto, userId, tenantId);

                _logger.LogInformation(
                    "Contenido de sección actualizado: {SectionId} por usuario {UserId}, completada: {IsCompleted}", 
                    sectionId, userId, dto.MarkAsCompleted);

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Acceso no autorizado a sección {SectionId} por usuario {UserId}", sectionId, GetCurrentUserId());
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Error al actualizar contenido de sección {SectionId}", sectionId);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar contenido de sección {SectionId}", sectionId);
                return StatusCode(500, new { message = "Error al actualizar contenido" });
            }
        }

        /// <summary>
        /// [USER] Marca sección como "en progreso" (empezar a trabajar)
        /// </summary>
        [HttpPost("sections/{sectionId}/start")]
        public async Task<IActionResult> StartWorkingOnSection(int sectionId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var tenantId = GetCurrentTenantId();

                var result = await _consolidatedTemplateService.StartWorkingOnSectionAsync(sectionId, userId, tenantId);

                if (!result)
                {
                    return NotFound(new { message = "Sección no encontrada o no tienes acceso a ella" });
                }

                _logger.LogInformation(
                    "Usuario {UserId} comenzó a trabajar en sección {SectionId}", 
                    userId, sectionId);

                return Ok(new { message = "Sección marcada como en progreso" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al iniciar trabajo en sección {SectionId}", sectionId);
                return StatusCode(500, new { message = "Error al iniciar trabajo en sección" });
            }
        }

        /// <summary>
        /// [USER] Completa sección
        /// </summary>
        [HttpPost("sections/{sectionId}/complete")]
        public async Task<IActionResult> CompleteSection(int sectionId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var tenantId = GetCurrentTenantId();

                var result = await _consolidatedTemplateService.CompleteSectionAsync(sectionId, userId, tenantId);

                if (!result)
                {
                    return NotFound(new { message = "Sección no encontrada o no tienes acceso a ella" });
                }

                _logger.LogInformation(
                    "Usuario {UserId} completó sección {SectionId}", 
                    userId, sectionId);

                return Ok(new { message = "Sección completada correctamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al completar sección {SectionId}", sectionId);
                return StatusCode(500, new { message = "Error al completar sección" });
            }
        }
    }
}

