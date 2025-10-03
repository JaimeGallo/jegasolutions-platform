using Microsoft.AspNetCore.Mvc;
using JEGASolutions.ReportBuilder.Core.Interfaces;
using JEGASolutions.ReportBuilder.Core.Dto;
using Microsoft.AspNetCore.Authorization;

namespace JEGASolutions.ReportBuilder.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReportSubmissionsController : ControllerBase
    {
        private readonly IReportSubmissionService _reportSubmissionService;
        private readonly ILogger<ReportSubmissionsController> _logger;

        public ReportSubmissionsController(IReportSubmissionService reportSubmissionService, ILogger<ReportSubmissionsController> logger)
        {
            _reportSubmissionService = reportSubmissionService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las presentaciones de reportes del tenant
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<ReportSubmissionDto>>> GetReportSubmissions()
        {
            try
            {
                var tenantId = GetTenantIdFromClaims();
                var submissions = await _reportSubmissionService.GetReportSubmissionsByTenantAsync(tenantId);
                
                var submissionDtos = submissions.Select(s => new ReportSubmissionDto
                {
                    Id = s.Id,
                    TemplateId = s.TemplateId,
                    TemplateName = s.Template?.Name ?? "",
                    AreaId = s.AreaId,
                    AreaName = s.Area?.Name ?? "",
                    SubmittedByUserId = s.SubmittedByUserId,
                    SubmittedByUserName = "", // Would need to join with user table
                    Title = s.Title,
                    Content = s.Content,
                    Status = s.Status,
                    SubmittedAt = s.SubmittedAt,
                    ApprovedAt = s.ApprovedAt,
                    ApprovedByUserId = s.ApprovedByUserId,
                    ApprovedByUserName = "", // Would need to join with user table
                    RejectionReason = s.RejectionReason,
                    PeriodStart = s.PeriodStart,
                    PeriodEnd = s.PeriodEnd,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt ?? s.CreatedAt
                }).ToList();

                return Ok(submissionDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener presentaciones de reportes");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene una presentación específica
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ReportSubmissionDto>> GetReportSubmission(int id)
        {
            try
            {
                var tenantId = GetTenantIdFromClaims();
                var submission = await _reportSubmissionService.GetReportSubmissionByIdAsync(id, tenantId);
                
                var result = new ReportSubmissionDto
                {
                    Id = submission.Id,
                    TemplateId = submission.TemplateId,
                    TemplateName = submission.Template?.Name ?? "",
                    AreaId = submission.AreaId,
                    AreaName = submission.Area?.Name ?? "",
                    SubmittedByUserId = submission.SubmittedByUserId,
                    SubmittedByUserName = "", // Would need to join with user table
                    Title = submission.Title,
                    Content = submission.Content,
                    Status = submission.Status,
                    SubmittedAt = submission.SubmittedAt,
                    ApprovedAt = submission.ApprovedAt,
                    ApprovedByUserId = submission.ApprovedByUserId,
                    ApprovedByUserName = "", // Would need to join with user table
                    RejectionReason = submission.RejectionReason,
                    PeriodStart = submission.PeriodStart,
                    PeriodEnd = submission.PeriodEnd,
                    CreatedAt = submission.CreatedAt,
                    UpdatedAt = submission.UpdatedAt ?? submission.CreatedAt
                };

                return Ok(result);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener presentación con ID {id}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Crea una nueva presentación de reporte
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ReportSubmissionDto>> CreateReportSubmission([FromBody] ReportSubmissionCreateDto createDto)
        {
            try
            {
                var tenantId = GetTenantIdFromClaims();
                var userId = GetUserIdFromClaims();
                var submission = await _reportSubmissionService.CreateReportSubmissionAsync(createDto, userId, tenantId);
                
                var result = new ReportSubmissionDto
                {
                    Id = submission.Id,
                    TemplateId = submission.TemplateId,
                    TemplateName = submission.Template?.Name ?? "",
                    AreaId = submission.AreaId,
                    AreaName = submission.Area?.Name ?? "",
                    SubmittedByUserId = submission.SubmittedByUserId,
                    SubmittedByUserName = "", // Would need to join with user table
                    Title = submission.Title,
                    Content = submission.Content,
                    Status = submission.Status,
                    SubmittedAt = submission.SubmittedAt,
                    ApprovedAt = submission.ApprovedAt,
                    ApprovedByUserId = submission.ApprovedByUserId,
                    ApprovedByUserName = "", // Would need to join with user table
                    RejectionReason = submission.RejectionReason,
                    PeriodStart = submission.PeriodStart,
                    PeriodEnd = submission.PeriodEnd,
                    CreatedAt = submission.CreatedAt,
                    UpdatedAt = submission.UpdatedAt ?? submission.CreatedAt
                };

                return CreatedAtAction(nameof(GetReportSubmission), new { id = submission.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear presentación de reporte");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Actualiza una presentación existente
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReportSubmission(int id, [FromBody] ReportSubmissionUpdateDto updateDto)
        {
            try
            {
                if (id != updateDto.Id)
                {
                    return BadRequest("ID de presentación no coincide");
                }

                var tenantId = GetTenantIdFromClaims();
                await _reportSubmissionService.UpdateReportSubmissionAsync(updateDto, tenantId);
                return NoContent();
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar presentación ID {id}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Envía un reporte para revisión
        /// </summary>
        [HttpPost("{id}/submit")]
        public async Task<IActionResult> SubmitReport(int id)
        {
            try
            {
                var tenantId = GetTenantIdFromClaims();
                var submitted = await _reportSubmissionService.SubmitReportAsync(id, tenantId);
                
                if (!submitted)
                {
                    return NotFound();
                }

                return Ok(new { message = "Reporte enviado exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al enviar reporte ID {id}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Aprueba un reporte
        /// </summary>
        [HttpPost("{id}/approve")]
        public async Task<IActionResult> ApproveReport(int id)
        {
            try
            {
                var tenantId = GetTenantIdFromClaims();
                var userId = GetUserIdFromClaims();
                var approved = await _reportSubmissionService.ApproveReportAsync(id, userId, tenantId);
                
                if (!approved)
                {
                    return NotFound();
                }

                return Ok(new { message = "Reporte aprobado exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al aprobar reporte ID {id}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Rechaza un reporte
        /// </summary>
        [HttpPost("{id}/reject")]
        public async Task<IActionResult> RejectReport(int id, [FromBody] RejectReportRequest request)
        {
            try
            {
                var tenantId = GetTenantIdFromClaims();
                var rejected = await _reportSubmissionService.RejectReportAsync(id, request.Reason, tenantId);
                
                if (!rejected)
                {
                    return NotFound();
                }

                return Ok(new { message = "Reporte rechazado exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al rechazar reporte ID {id}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Elimina una presentación
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReportSubmission(int id)
        {
            try
            {
                var tenantId = GetTenantIdFromClaims();
                var deleted = await _reportSubmissionService.DeleteReportSubmissionAsync(id, tenantId);
                
                if (!deleted)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar presentación ID {id}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        private int GetTenantIdFromClaims()
        {
            var claim = User.FindFirst("tenant_id");
            return int.Parse(claim?.Value ?? "0");
        }

        private int GetUserIdFromClaims()
        {
            var claim = User.FindFirst("user_id");
            return int.Parse(claim?.Value ?? "0");
        }
    }

    public class RejectReportRequest
    {
        public string Reason { get; set; } = string.Empty;
    }
}
