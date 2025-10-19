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

        public ExtraHoursConfigController(IExtraHoursConfigService configService)
        {
            _configService = configService;
        }

        [HttpGet]
        public async Task<IActionResult> GetConfig()
        {
            var config = await _configService.GetConfigAsync();
            if (config == null)
            {
                return NotFound(new { error = "Configuración no encontrada" });
            }
            return Ok(config);
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

            var updatedConfig = await _configService.UpdateConfigAsync(config);
            return Ok(updatedConfig);

        }
    }
}
