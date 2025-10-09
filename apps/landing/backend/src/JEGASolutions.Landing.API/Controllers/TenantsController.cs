using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JEGASolutions.Landing.Infrastructure.Data;
using Microsoft.Extensions.Logging;

namespace JEGASolutions.Landing.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TenantsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TenantsController> _logger;

        public TenantsController(
            ApplicationDbContext context,
            ILogger<TenantsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene un tenant por subdomain - PARA TENANT DASHBOARD
        /// </summary>
        [HttpGet("by-subdomain/{subdomain}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetBySubdomain(string subdomain)
        {
            try
            {
                _logger.LogInformation("🔍 Buscando tenant: {Subdomain}", subdomain);

                var tenant = await _context.Tenants
                    .FirstOrDefaultAsync(t => t.Subdomain == subdomain);

                if (tenant == null)
                {
                    _logger.LogWarning("❌ Tenant no encontrado: {Subdomain}", subdomain);
                    return NotFound(new {
                        message = $"Tenant '{subdomain}' no encontrado",
                        subdomain = subdomain
                    });
                }

                if (!tenant.IsActive)
                {
                    _logger.LogWarning("⚠️ Tenant inactivo: {Subdomain}", subdomain);
                    return BadRequest(new {
                        message = $"Tenant '{subdomain}' está inactivo"
                    });
                }

                var userCount = await _context.Users
                    .CountAsync(u => u.TenantId == tenant.Id && u.IsActive);

                var response = new
                {
                    id = tenant.Id,
                    companyName = tenant.CompanyName,
                    subdomain = tenant.Subdomain,
                    isActive = tenant.IsActive,
                    createdAt = tenant.CreatedAt,
                    userCount = userCount
                };

                _logger.LogInformation("✅ Tenant encontrado: {CompanyName}", tenant.CompanyName);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Error al buscar tenant: {Subdomain}", subdomain);
                return StatusCode(500, new {
                    message = "Error interno al buscar tenant",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Obtiene módulos de un tenant - PARA TENANT DASHBOARD
        /// </summary>
        [HttpGet("{id}/modules")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetModules(int id)
        {
            try
            {
                _logger.LogInformation("📦 Buscando módulos para tenant: {TenantId}", id);

                var tenant = await _context.Tenants.FindAsync(id);
                if (tenant == null)
                {
                    return NotFound(new { message = $"Tenant {id} no encontrado" });
                }

                var modules = await _context.TenantModules
                    .Where(tm => tm.TenantId == id)
                    .Select(tm => new
                    {
                        id = tm.Id,
                        moduleName = tm.ModuleName,
                        status = tm.Status,
                        purchasedAt = tm.PurchasedAt,
                         expiresAt = (DateTime?)null,
                        // URLs y metadata
                        route = GetModuleRoute(tm.ModuleName),
                        url = GetModuleUrl(tm.ModuleName),
                        description = GetModuleDescription(tm.ModuleName),
                        icon = GetModuleIcon(tm.ModuleName)
                    })
                    .ToListAsync();

                _logger.LogInformation("✅ Encontrados {Count} módulos", modules.Count);
                return Ok(modules);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Error al obtener módulos");
                return StatusCode(500, new {
                    message = "Error al obtener módulos",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Obtiene estadísticas del tenant
        /// </summary>
        [HttpGet("{id}/stats")]
        public async Task<ActionResult> GetStats(int id)
        {
            try
            {
                var tenant = await _context.Tenants.FindAsync(id);
                if (tenant == null)
                {
                    return NotFound(new { message = $"Tenant {id} no encontrado" });
                }

                var stats = new
                {
                    totalModules = await _context.TenantModules
                        .CountAsync(tm => tm.TenantId == id),
                    activeModules = await _context.TenantModules
                        .CountAsync(tm => tm.TenantId == id && tm.Status == "active"),
                    totalUsers = await _context.Users
                        .CountAsync(u => u.TenantId == id && u.IsActive),
                    lastActivity = DateTime.UtcNow
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estadísticas");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // ============ MÉTODOS AUXILIARES ============

        private static string GetModuleRoute(string moduleName)
        {
            return moduleName switch
            {
                "Extra Hours" => "/extra-hours",
                "Report Builder" => "/report-builder",
                _ => "/"
            };
        }

        private static string GetModuleUrl(string moduleName)
        {
            return moduleName switch
            {
                "Extra Hours" => "https://extrahours.jegasolutions.co",
                "Report Builder" => "https://reportbuilder.jegasolutions.co",
                _ => "https://www.jegasolutions.co"
            };
        }

        private static string GetModuleDescription(string moduleName)
        {
            return moduleName switch
            {
                "Extra Hours" => "Gestión completa de horas extra, compensaciones y aprobaciones",
                "Report Builder" => "Generación inteligente de reportes con múltiples IAs",
                _ => "Módulo del sistema"
            };
        }

        private static string GetModuleIcon(string moduleName)
        {
            return moduleName switch
            {
                "Extra Hours" => "clock",
                "Report Builder" => "file-text",
                _ => "package"
            };
        }
    }
}
