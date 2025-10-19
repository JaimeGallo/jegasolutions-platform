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
            // ✅ AGREGAR LOGS PARA DEBUGGING
            var userRole = User.FindFirst("role")?.Value;
            var userId = User.FindFirst("id")?.Value;
            var tenantId = User.FindFirst("tenant_id")?.Value ?? User.FindFirst("tenantId")?.Value;

            Console.WriteLine($"[UpdateConfig] User: {userId}, Role: {userRole}, TenantId: {tenantId}");
            Console.WriteLine($"[UpdateConfig] All claims: {string.Join(", ", User.Claims.Select(c => $"{c.Type}={c.Value}"))}");

            if (config == null)
                return BadRequest(new { error = "Datos de configuración no pueden ser nulos" });
            try
            {
                if (config == null)
                {
                    return BadRequest(new { error = "Datos de configuración no pueden ser nulos" });
                }

                _logger.LogInformation("📝 UPDATING CONFIG - User: {User}", User.Identity?.Name);
                _logger.LogInformation("📝 User authenticated: {Auth}", User.Identity?.IsAuthenticated);
                _logger.LogInformation("📝 User roles: {Roles}", 
                    string.Join(", ", User.Claims.Where(c => c.Type == "role").Select(c => c.Value)));
                _logger.LogInformation("📝 New values - weeklyLimit: {Weekly}, diurnalEnd: {End}", 
                    config.weeklyExtraHoursLimit, config.diurnalEnd);

                var updatedConfig = await _configService.UpdateConfigAsync(config);
                
                _logger.LogInformation("✅✅✅ CONFIG UPDATED SUCCESSFULLY!");
                _logger.LogInformation("✅ Final values - weeklyLimit: {Weekly}, diurnalEnd: {End}", 
                    updatedConfig.weeklyExtraHoursLimit, updatedConfig.diurnalEnd);
                
                return Ok(updatedConfig);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error updating config");
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
