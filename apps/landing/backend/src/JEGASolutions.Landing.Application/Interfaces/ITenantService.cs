using JEGASolutions.Landing.Domain.Entities;

namespace JEGASolutions.Landing.Application.Interfaces;

public interface ITenantService
{
    Task<Tenant?> GetTenantBySubdomainAsync(string subdomain);
    Task<Tenant?> GetTenantByIdAsync(int tenantId);
    Task<Tenant> CreateTenantAsync(string companyName, string email, string contactName);
    Task<IEnumerable<Tenant>> GetAllTenantsAsync();
    Task<bool> UpdateTenantAsync(Tenant tenant);
    Task<bool> DeleteTenantAsync(int tenantId);
    Task<bool> TenantExistsAsync(string subdomain);
}