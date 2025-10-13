using JEGASolutions.ReportBuilder.Core.Dto;
using JEGASolutions.ReportBuilder.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JEGASolutions.ReportBuilder.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ExcelUploadsController : ControllerBase
    {
        private readonly IExcelProcessorService _excelProcessorService;
        private readonly ILogger<ExcelUploadsController> _logger;

        public ExcelUploadsController(
            IExcelProcessorService excelProcessorService,
            ILogger<ExcelUploadsController> logger)
        {
            _excelProcessorService = excelProcessorService;
            _logger = logger;
        }

        /// <summary>
        /// Sube y procesa un archivo Excel
        /// </summary>
        [HttpPost("upload")]
        public async Task<ActionResult<ExcelUploadDetailDto>> UploadExcel(
            [FromBody] ExcelUploadCreateDto dto)
        {
            try
            {
                var userId = GetUserIdFromClaims();
                var tenantId = GetTenantIdFromClaims();

                _logger.LogInformation(
                    "Upload Excel iniciado por usuario {UserId} en tenant {TenantId}", 
                    userId, tenantId);

                var result = await _excelProcessorService.UploadAndProcessExcelAsync(
                    dto, userId, tenantId);

                return CreatedAtAction(
                    nameof(GetExcelUploadById), 
                    new { id = result.Id }, 
                    result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validación de upload falló");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en UploadExcel");
                return StatusCode(500, new { message = "Error interno al procesar el archivo" });
            }
        }

        /// <summary>
        /// Obtiene todos los uploads de Excel del tenant
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<ExcelUploadListDto>>> GetExcelUploads(
            [FromQuery] int? areaId = null,
            [FromQuery] string? period = null)
        {
            try
            {
                var tenantId = GetTenantIdFromClaims();
                
                var uploads = await _excelProcessorService.GetExcelUploadsAsync(
                    tenantId, areaId, period);

                return Ok(uploads);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en GetExcelUploads");
                return StatusCode(500, new { message = "Error al obtener uploads" });
            }
        }

        /// <summary>
        /// Obtiene detalle de un upload específico
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ExcelUploadDetailDto>> GetExcelUploadById(int id)
        {
            try
            {
                var tenantId = GetTenantIdFromClaims();
                
                var upload = await _excelProcessorService.GetExcelUploadByIdAsync(id, tenantId);
                
                if (upload == null)
                {
                    return NotFound(new { message = $"Upload {id} no encontrado" });
                }

                return Ok(upload);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en GetExcelUploadById");
                return StatusCode(500, new { message = "Error al obtener detalle del upload" });
            }
        }

        /// <summary>
        /// Obtiene uploads por área específica
        /// </summary>
        [HttpGet("area/{areaId}")]
        public async Task<ActionResult<List<ExcelUploadListDto>>> GetByArea(int areaId)
        {
            try
            {
                var tenantId = GetTenantIdFromClaims();
                
                var uploads = await _excelProcessorService.GetExcelUploadsAsync(
                    tenantId, areaId: areaId);

                return Ok(uploads);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en GetByArea");
                return StatusCode(500, new { message = "Error al obtener uploads por área" });
            }
        }

        /// <summary>
        /// Elimina un upload (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "superusuario")]
        public async Task<IActionResult> DeleteExcelUpload(int id)
        {
            try
            {
                var tenantId = GetTenantIdFromClaims();
                
                var success = await _excelProcessorService.DeleteExcelUploadAsync(id, tenantId);
                
                if (!success)
                {
                    return NotFound(new { message = $"Upload {id} no encontrado" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en DeleteExcelUpload");
                return StatusCode(500, new { message = "Error al eliminar upload" });
            }
        }

        /// <summary>
        /// Re-procesa un archivo Excel ya subido
        /// </summary>
        [HttpPost("{id}/reprocess")]
        public async Task<ActionResult<ExcelProcessingResultDto>> ReprocessExcel(int id)
        {
            try
            {
                var tenantId = GetTenantIdFromClaims();
                
                var result = await _excelProcessorService.ReprocessExcelAsync(id, tenantId);
                
                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en ReprocessExcel");
                return StatusCode(500, new { message = "Error al reprocesar archivo" });
            }
        }

        /// <summary>
        /// Solicita análisis AI de datos de Excel
        /// </summary>
        [HttpPost("analyze-ai")]
        public async Task<ActionResult<ExcelAIAnalysisResultDto>> RequestAIAnalysis(
            [FromBody] ExcelAIAnalysisRequestDto dto)
        {
            try
            {
                var tenantId = GetTenantIdFromClaims();
                
                var result = await _excelProcessorService.RequestAIAnalysisAsync(dto, tenantId);
                
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validación de análisis AI falló");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en RequestAIAnalysis");
                return StatusCode(500, new { message = "Error al solicitar análisis AI" });
            }
        }

        // Helper methods para extraer claims del JWT
        private int GetUserIdFromClaims()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                              ?? User.FindFirst("sub")?.Value
                              ?? User.FindFirst("userId")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                throw new UnauthorizedAccessException("User ID no encontrado en token");
            }

            return userId;
        }

        private int GetTenantIdFromClaims()
        {
            var tenantIdClaim = User.FindFirst("tenantId")?.Value
                                ?? User.FindFirst("tenant_id")?.Value;

            if (string.IsNullOrEmpty(tenantIdClaim) || !int.TryParse(tenantIdClaim, out int tenantId))
            {
                throw new UnauthorizedAccessException("Tenant ID no encontrado en token");
            }

            return tenantId;
        }
    }
}

