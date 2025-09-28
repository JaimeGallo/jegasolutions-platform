using JEGASolutions.TenantDashboard.Core.Entities;

namespace JEGASolutions.TenantDashboard.Core.Interfaces;

public interface ITenantService
{
    Task<Tenant?> GetTenantBySubdomainAsync(string subdomain);
    Task<Tenant?> GetTenantByIdAsync(int id);
    Task<List<Tenant>> GetAllTenantsAsync();
    Task<Tenant> CreateTenantAsync(Tenant tenant);
    Task<Tenant> UpdateTenantAsync(Tenant tenant);
    Task<bool> DeleteTenantAsync(int id);
}
