using JEGASolutions.TenantDashboard.Core.Entities;

namespace JEGASolutions.TenantDashboard.Core.Interfaces;

public interface IModuleService
{
    Task<List<TenantModule>> GetTenantModulesAsync(int tenantId);
    Task<TenantModule?> GetTenantModuleAsync(int tenantId, string moduleName);
    Task<TenantModule> AddModuleToTenantAsync(int tenantId, string moduleName);
    Task<bool> RemoveModuleFromTenantAsync(int tenantId, string moduleName);
    Task<bool> UpdateModuleStatusAsync(int tenantId, string moduleName, string status);
}
