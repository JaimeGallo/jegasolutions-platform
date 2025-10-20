using JEGASolutions.ExtraHours.Core.Entities.Models;
using Microsoft.AspNetCore.Mvc;
using JEGASolutions.ExtraHours.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace JEGASolutions.ExtraHours.API.Controller
{
    [Route("api/config")]
    [ApiController]
    public class ExtraHoursConfigController : ControllerBase
    {
        private readonly IExtraHoursConfigService _configService;
        private readonly ILogger<ExtraHoursConfigController> _logger;

        public ExtraHoursConfigController(
            IExtraHoursConfigService configService,
            ILogger<ExtraHoursConfigController> logger)
        {
            _configService = configService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetConfig()
        {
            try
            {
                var config = await _configService.GetConfigAsync();
                if (config == null)
                {
                    return NotFound(new { error = "Configuración no encontrada" });
                }

                _logger.LogInformation("✅ Config retrieved: weeklyLimit={Weekly}, diurnalEnd={End}",
                    config.weeklyExtraHoursLimit, config.diurnalEnd);
                return Ok(config);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting config");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPut]
        [Authorize(Roles = "superusuario")]
        public async Task<IActionResult> UpdateConfig([FromBody] ExtraHoursConfig config)
        {
            try
            {
                // Log de debugging
                var userRole = User.FindFirst("role")?.Value;
                var userId = User.FindFirst("userId")?.Value ?? User.FindFirst("id")?.Value;
                var userName = User.Identity?.Name;
                
                _logger.LogInformation($"🔍 UpdateConfig called by - Role: {userRole}, UserId: {userId}, Name: {userName}");
                
                if (config == null)
                {
                    return BadRequest(new { error = "Datos de configuración no pueden ser nulos" });
                }

                var updatedConfig = await _configService.UpdateConfigAsync(config);
                return Ok(updatedConfig);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error updating config");
                return StatusCode(500, new 
                { 
                    error = "Error actualizando la configuración",
                    details = ex.Message 
                });
            }
        }
    }
}
