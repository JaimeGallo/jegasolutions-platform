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
        [Authorize]
        public async Task<IActionResult> GetConfig()
        {
            try
            {
                // ✅ CRITICAL FIX: Extract tenant_id from JWT token
                // Try multiple claim names for compatibility with different token issuers
                var tenantIdClaim = User.FindFirst("tenant_id") 
                                 ?? User.FindFirst("TenantId") 
                                 ?? User.FindFirst("tenantId"); // Landing API uses this format
                if (tenantIdClaim == null || !int.TryParse(tenantIdClaim.Value, out int tenantId))
                {
                    _logger.LogWarning("⚠️ Tenant ID not found in token");
                    return BadRequest(new { error = "Tenant ID no encontrado en el token" });
                }

                _logger.LogInformation("🔍 Getting config for tenant {TenantId}", tenantId);

                // ✅ Use the new method that filters by tenant_id
                var config = await _configService.GetConfigByTenantAsync(tenantId);
                if (config == null)
                {
                    return NotFound(new { error = "Configuración no encontrada para tu organización" });
                }

                _logger.LogInformation(
                    "✅ Config retrieved for tenant {TenantId}: weeklyLimit={Weekly}, diurnalEnd={End}",
                    tenantId, config.weeklyExtraHoursLimit, config.diurnalEnd);

                return Ok(config);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "⚠️ Config not found");
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting config");
                return StatusCode(500, new { error = "Error interno del servidor" });
            }
        }

        [HttpPut]
        [Authorize(Roles = "superusuario")]
        public async Task<IActionResult> UpdateConfig([FromBody] ExtraHoursConfig config)
        {
            try
            {
                // ✅ Extract tenant_id from JWT token
                // Try multiple claim names for compatibility with different token issuers
                var tenantIdClaim = User.FindFirst("tenant_id") 
                                 ?? User.FindFirst("TenantId") 
                                 ?? User.FindFirst("tenantId"); // Landing API uses this format
                if (tenantIdClaim == null || !int.TryParse(tenantIdClaim.Value, out int tenantId))
                {
                    _logger.LogWarning("⚠️ Tenant ID not found in token");
                    return BadRequest(new { error = "Tenant ID no encontrado en el token" });
                }

                // Log de debugging
                var userRole = User.FindFirst("role")?.Value;
                var userId = User.FindFirst("userId")?.Value ?? User.FindFirst("id")?.Value;
                var userName = User.Identity?.Name;

                _logger.LogInformation(
                    "🔍 UpdateConfig called by - Role: {Role}, UserId: {UserId}, Name: {Name}, TenantId: {TenantId}",
                    userRole, userId, userName, tenantId);

                if (config == null)
                {
                    return BadRequest(new { error = "Datos de configuración no pueden ser nulos" });
                }

                // ✅ CRITICAL: Ensure the config has the correct tenant_id
                config.TenantId = tenantId;

                var updatedConfig = await _configService.UpdateConfigAsync(config);

                _logger.LogInformation(
                    "✅ Config updated successfully for tenant {TenantId}",
                    tenantId);

                return Ok(updatedConfig);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "⚠️ Invalid operation");
                return BadRequest(new { error = ex.Message });
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
