using Microsoft.EntityFrameworkCore;
using JEGASolutions.TenantDashboard.Core.DTOs;
using JEGASolutions.TenantDashboard.Core.Interfaces;
using JEGASolutions.TenantDashboard.Infrastructure.Data;

namespace JEGASolutions.TenantDashboard.Infrastructure.Services;

public class DashboardService : IDashboardService
{
    private readonly TenantDashboardDbContext _context;

    public DashboardService(TenantDashboardDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardStatsDto> GetDashboardStatsAsync(int tenantId)
    {
        var modules = await _context.TenantModules
            .Where(tm => tm.TenantId == tenantId)
            .ToListAsync();

        var users = await _context.Users
            .Where(u => u.TenantId == tenantId && u.IsActive)
            .ToListAsync();

        var lastActivity = await GetLastActivityAsync(tenantId);
        var moduleUsage = await GetModuleUsageStatsAsync(tenantId);

        return new DashboardStatsDto
        {
            TotalModules = modules.Count,
            ActiveModules = modules.Count(m => m.Status == "ACTIVE"),
            TotalUsers = users.Count,
            LastActivity = lastActivity,
            ModuleUsage = moduleUsage
        };
    }

    public async Task<Dictionary<string, int>> GetModuleUsageStatsAsync(int tenantId)
    {
        // This would typically come from usage analytics
        // For now, return mock data
        var modules = await _context.TenantModules
            .Where(tm => tm.TenantId == tenantId && tm.Status == "ACTIVE")
            .Select(tm => tm.ModuleName)
            .ToListAsync();

        var usage = new Dictionary<string, int>();
        foreach (var module in modules)
        {
            usage[module] = Random.Shared.Next(10, 100); // Mock usage data
        }

        return usage;
    }

    public async Task<DateTime> GetLastActivityAsync(int tenantId)
    {
        var lastLogin = await _context.Users
            .Where(u => u.TenantId == tenantId)
            .MaxAsync(u => u.LastLoginAt);

        return lastLogin ?? DateTime.UtcNow.AddDays(-1);
    }
}
