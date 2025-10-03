using Microsoft.AspNetCore.Mvc;
using JEGASolutions.ReportBuilder.Core.Interfaces;
using JEGASolutions.ReportBuilder.Core.Dto;
using Microsoft.AspNetCore.Authorization;

namespace JEGASolutions.ReportBuilder.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TemplatesController : ControllerBase
    {
        private readonly ITemplateService _templateService;
        private readonly ILogger<TemplatesController> _logger;

        public TemplatesController(ITemplateService templateService, ILogger<TemplatesController> logger)
        {
            _templateService = templateService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las plantillas del tenant
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<TemplateDto>>> GetTemplates()
        {
            try
            {
                var tenantId = GetTenantIdFromClaims();
                var templates = await _templateService.GetTemplatesByTenantAsync(tenantId);
                
                var templateDtos = templates.Select(t => new TemplateDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Type = t.Type,
                    Version = t.Version,
                    AreaId = t.AreaId,
                    AreaName = t.Area?.Name,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt ?? t.CreatedAt
                }).ToList();

                return Ok(templateDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener plantillas");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene una plantilla específica
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<TemplateDetailDto>> GetTemplate(int id)
        {
            try
            {
                var tenantId = GetTenantIdFromClaims();
                var template = await _templateService.GetTemplateByIdAsync(id, tenantId);
                
                var result = new TemplateDetailDto
                {
                    Id = template.Id,
                    Name = template.Name,
                    AreaId = template.AreaId,
                    AreaName = template.Area?.Name,
                    Configuration = template.Configuration,
                    CreatedAt = template.CreatedAt,
                    UpdatedAt = template.UpdatedAt ?? template.CreatedAt
                };

                return Ok(result);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener plantilla con ID {id}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene plantillas por tipo
        /// </summary>
        [HttpGet("by-type/{type}")]
        public async Task<ActionResult<List<TemplateDto>>> GetTemplatesByType(string type)
        {
            try
            {
                var tenantId = GetTenantIdFromClaims();
                var templates = await _templateService.GetTemplatesByTypeAsync(type, tenantId);
                
                var templateDtos = templates.Select(t => new TemplateDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Type = t.Type,
                    Version = t.Version,
                    AreaId = t.AreaId,
                    AreaName = t.Area?.Name,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt ?? t.CreatedAt
                }).ToList();

                return Ok(templateDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener plantillas de tipo {type}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Crea una nueva plantilla
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<TemplateDetailDto>> CreateTemplate([FromBody] TemplateCreateDto templateDto)
        {
            try
            {
                var tenantId = GetTenantIdFromClaims();
                var template = await _templateService.CreateTemplateAsync(templateDto, tenantId);
                
                var result = new TemplateDetailDto
                {
                    Id = template.Id,
                    Name = template.Name,
                    AreaId = template.AreaId,
                    AreaName = template.Area?.Name,
                    Configuration = template.Configuration,
                    CreatedAt = template.CreatedAt,
                    UpdatedAt = template.UpdatedAt ?? template.CreatedAt
                };

                return CreatedAtAction(nameof(GetTemplate), new { id = template.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear plantilla");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Actualiza una plantilla existente
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTemplate(int id, [FromBody] TemplateUpdateDto templateDto)
        {
            try
            {
                if (id != templateDto.Id)
                {
                    return BadRequest("ID de plantilla no coincide");
                }

                var tenantId = GetTenantIdFromClaims();
                await _templateService.UpdateTemplateAsync(templateDto, tenantId);
                return NoContent();
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar plantilla ID {id}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Elimina una plantilla
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTemplate(int id)
        {
            try
            {
                var tenantId = GetTenantIdFromClaims();
                var deleted = await _templateService.DeleteTemplateAsync(id, tenantId);
                
                if (!deleted)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar plantilla ID {id}");
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
