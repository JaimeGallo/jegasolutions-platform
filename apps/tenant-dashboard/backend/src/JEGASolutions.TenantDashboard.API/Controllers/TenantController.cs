using Microsoft.AspNetCore.Mvc;
using JEGASolutions.TenantDashboard.Core.Interfaces;
using JEGASolutions.TenantDashboard.Core.DTOs;

namespace JEGASolutions.TenantDashboard.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TenantController : ControllerBase
{
    private readonly ITenantService _tenantService;
    private readonly IModuleService _moduleService;
    private readonly IDashboardService _dashboardService;

    public TenantController(
        ITenantService tenantService,
        IModuleService moduleService,
        IDashboardService dashboardService)
    {
        _tenantService = tenantService;
        _moduleService = moduleService;
        _dashboardService = dashboardService;
    }

    [HttpGet("{subdomain}")]
    public async Task<ActionResult<TenantDashboardDto>> GetTenantDashboard(string subdomain)
    {
        try
        {
            var tenant = await _tenantService.GetTenantBySubdomainAsync(subdomain);
            if (tenant == null)
            {
                return NotFound($"Tenant with subdomain '{subdomain}' not found");
            }

            var modules = await _moduleService.GetTenantModulesAsync(tenant.Id);
            var stats = await _dashboardService.GetDashboardStatsAsync(tenant.Id);

            var dashboard = new TenantDashboardDto
            {
                Tenant = new TenantDto
                {
                    Id = tenant.Id,
                    CompanyName = tenant.CompanyName,
                    Subdomain = tenant.Subdomain,
                    IsActive = tenant.IsActive,
                    CreatedAt = tenant.CreatedAt
                },
                Modules = modules.Select(m => new ModuleDto
                {
                    Id = m.Id,
                    ModuleName = m.ModuleName,
                    Status = m.Status,
                    PurchasedAt = m.PurchasedAt,
                    ExpiresAt = m.ExpiresAt
                }).ToList(),
                Stats = stats
            };

            return Ok(dashboard);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("{subdomain}/modules")]
    public async Task<ActionResult<List<ModuleDto>>> GetTenantModules(string subdomain)
    {
        try
        {
            var tenant = await _tenantService.GetTenantBySubdomainAsync(subdomain);
            if (tenant == null)
            {
                return NotFound($"Tenant with subdomain '{subdomain}' not found");
            }

            var modules = await _moduleService.GetTenantModulesAsync(tenant.Id);
            var moduleDtos = modules.Select(m => new ModuleDto
            {
                Id = m.Id,
                ModuleName = m.ModuleName,
                Status = m.Status,
                PurchasedAt = m.PurchasedAt,
                ExpiresAt = m.ExpiresAt
            }).ToList();

            return Ok(moduleDtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("{subdomain}/stats")]
    public async Task<ActionResult<DashboardStatsDto>> GetTenantStats(string subdomain)
    {
        try
        {
            var tenant = await _tenantService.GetTenantBySubdomainAsync(subdomain);
            if (tenant == null)
            {
                return NotFound($"Tenant with subdomain '{subdomain}' not found");
            }

            var stats = await _dashboardService.GetDashboardStatsAsync(tenant.Id);
            return Ok(stats);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}
