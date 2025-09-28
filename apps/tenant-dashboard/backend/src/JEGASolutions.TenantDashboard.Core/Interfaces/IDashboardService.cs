using JEGASolutions.TenantDashboard.Core.DTOs;

namespace JEGASolutions.TenantDashboard.Core.Interfaces;

public interface IDashboardService
{
    Task<DashboardStatsDto> GetDashboardStatsAsync(int tenantId);
    Task<Dictionary<string, int>> GetModuleUsageStatsAsync(int tenantId);
    Task<DateTime> GetLastActivityAsync(int tenantId);
}
