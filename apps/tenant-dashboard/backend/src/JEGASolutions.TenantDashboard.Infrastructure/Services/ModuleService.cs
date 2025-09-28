using Microsoft.EntityFrameworkCore;
using JEGASolutions.TenantDashboard.Core.Entities;
using JEGASolutions.TenantDashboard.Core.Interfaces;
using JEGASolutions.TenantDashboard.Infrastructure.Data;

namespace JEGASolutions.TenantDashboard.Infrastructure.Services;

public class ModuleService : IModuleService
{
    private readonly TenantDashboardDbContext _context;

    public ModuleService(TenantDashboardDbContext context)
    {
        _context = context;
    }

    public async Task<List<TenantModule>> GetTenantModulesAsync(int tenantId)
    {
        return await _context.TenantModules
            .Where(tm => tm.TenantId == tenantId)
            .ToListAsync();
    }

    public async Task<TenantModule?> GetTenantModuleAsync(int tenantId, string moduleName)
    {
        return await _context.TenantModules
            .FirstOrDefaultAsync(tm => tm.TenantId == tenantId && tm.ModuleName == moduleName);
    }

    public async Task<TenantModule> AddModuleToTenantAsync(int tenantId, string moduleName)
    {
        var tenantModule = new TenantModule
        {
            TenantId = tenantId,
            ModuleName = moduleName,
            Status = "ACTIVE",
            PurchasedAt = DateTime.UtcNow
        };

        _context.TenantModules.Add(tenantModule);
        await _context.SaveChangesAsync();
        return tenantModule;
    }

    public async Task<bool> RemoveModuleFromTenantAsync(int tenantId, string moduleName)
    {
        var tenantModule = await _context.TenantModules
            .FirstOrDefaultAsync(tm => tm.TenantId == tenantId && tm.ModuleName == moduleName);

        if (tenantModule == null)
            return false;

        _context.TenantModules.Remove(tenantModule);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateModuleStatusAsync(int tenantId, string moduleName, string status)
    {
        var tenantModule = await _context.TenantModules
            .FirstOrDefaultAsync(tm => tm.TenantId == tenantId && tm.ModuleName == moduleName);

        if (tenantModule == null)
            return false;

        tenantModule.Status = status;
        tenantModule.UpdatedAt = DateTime.UtcNow;
        
        _context.TenantModules.Update(tenantModule);
        await _context.SaveChangesAsync();
        return true;
    }
}
