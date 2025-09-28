using Microsoft.EntityFrameworkCore;
using JEGASolutions.TenantDashboard.Core.Entities;
using JEGASolutions.TenantDashboard.Core.Interfaces;
using JEGASolutions.TenantDashboard.Infrastructure.Data;

namespace JEGASolutions.TenantDashboard.Infrastructure.Services;

public class TenantService : ITenantService
{
    private readonly TenantDashboardDbContext _context;

    public TenantService(TenantDashboardDbContext context)
    {
        _context = context;
    }

    public async Task<Tenant?> GetTenantBySubdomainAsync(string subdomain)
    {
        return await _context.Tenants
            .Include(t => t.Modules)
            .Include(t => t.Users)
            .FirstOrDefaultAsync(t => t.Subdomain == subdomain);
    }

    public async Task<Tenant?> GetTenantByIdAsync(int id)
    {
        return await _context.Tenants
            .Include(t => t.Modules)
            .Include(t => t.Users)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<List<Tenant>> GetAllTenantsAsync()
    {
        return await _context.Tenants
            .Include(t => t.Modules)
            .Include(t => t.Users)
            .ToListAsync();
    }

    public async Task<Tenant> CreateTenantAsync(Tenant tenant)
    {
        _context.Tenants.Add(tenant);
        await _context.SaveChangesAsync();
        return tenant;
    }

    public async Task<Tenant> UpdateTenantAsync(Tenant tenant)
    {
        tenant.UpdatedAt = DateTime.UtcNow;
        _context.Tenants.Update(tenant);
        await _context.SaveChangesAsync();
        return tenant;
    }

    public async Task<bool> DeleteTenantAsync(int id)
    {
        var tenant = await _context.Tenants.FindAsync(id);
        if (tenant == null)
            return false;

        _context.Tenants.Remove(tenant);
        await _context.SaveChangesAsync();
        return true;
    }
}
