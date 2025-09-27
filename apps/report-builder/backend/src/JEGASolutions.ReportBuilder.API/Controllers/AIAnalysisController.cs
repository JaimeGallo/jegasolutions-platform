using Microsoft.AspNetCore.Mvc;
using JEGASolutions.ReportBuilder.Core.Interfaces;
using JEGASolutions.ReportBuilder.Core.Dto;
using Microsoft.AspNetCore.Authorization;

namespace JEGASolutions.ReportBuilder.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AIAnalysisController : ControllerBase
    {
        private readonly IAIAnalysisService _aiAnalysisService;
        private readonly ILogger<AIAnalysisController> _logger;

        public AIAnalysisController(IAIAnalysisService aiAnalysisService, ILogger<AIAnalysisController> logger)
        {
            _aiAnalysisService = aiAnalysisService;
            _logger = logger;
        }

        /// <summary>
        /// Analiza un reporte con IA
        /// </summary>
        [HttpPost("analyze")]
        public async Task<ActionResult<AIAnalysisResultDto>> AnalyzeReport([FromBody] AIAnalysisRequestDto request)
        {
            try
            {
                var tenantId = GetTenantIdFromClaims();
                var result = await _aiAnalysisService.AnalyzeReportAsync(request, tenantId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al analizar reporte");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene insights para un reporte específico
        /// </summary>
        [HttpGet("insights/{reportSubmissionId}")]
        public async Task<ActionResult<List<AIInsightDto>>> GetInsightsForReport(int reportSubmissionId)
        {
            try
            {
                var tenantId = GetTenantIdFromClaims();
                var insights = await _aiAnalysisService.GetInsightsForReportAsync(reportSubmissionId, tenantId);
                return Ok(insights);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener insights para reporte {reportSubmissionId}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene insights por tipo
        /// </summary>
        [HttpGet("insights/by-type/{insightType}")]
        public async Task<ActionResult<List<AIInsightDto>>> GetInsightsByType(string insightType)
        {
            try
            {
                var tenantId = GetTenantIdFromClaims();
                var insights = await _aiAnalysisService.GetInsightsByTypeAsync(insightType, tenantId);
                return Ok(insights);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener insights de tipo {insightType}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Genera insights automáticamente para un reporte
        /// </summary>
        [HttpPost("generate-insights/{reportSubmissionId}")]
        public async Task<ActionResult> GenerateInsights(int reportSubmissionId)
        {
            try
            {
                var tenantId = GetTenantIdFromClaims();
                var success = await _aiAnalysisService.GenerateInsightsAsync(reportSubmissionId, tenantId);
                
                if (!success)
                {
                    return BadRequest("No se pudieron generar los insights");
                }

                return Ok(new { message = "Insights generados exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al generar insights para reporte {reportSubmissionId}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Elimina un insight específico
        /// </summary>
        [HttpDelete("insights/{insightId}")]
        public async Task<IActionResult> DeleteInsight(int insightId)
        {
            try
            {
                var tenantId = GetTenantIdFromClaims();
                var deleted = await _aiAnalysisService.DeleteInsightAsync(insightId, tenantId);
                
                if (!deleted)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar insight {insightId}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        private int GetTenantIdFromClaims()
        {
            var claim = User.FindFirst("tenant_id");
            return int.Parse(claim?.Value ?? "0");
        }
    }
}
