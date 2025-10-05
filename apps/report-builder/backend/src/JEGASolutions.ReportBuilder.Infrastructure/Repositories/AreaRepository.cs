using JEGASolutions.ReportBuilder.Core.Entities.Models;
using JEGASolutions.ReportBuilder.Core.Interfaces;
using JEGASolutions.ReportBuilder.Data;
using Microsoft.EntityFrameworkCore;

namespace JEGASolutions.ReportBuilder.Infrastructure.Repositories
{
    public class AreaRepository : IAreaRepository
    {
        private readonly AppDbContext _context;

        public AreaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Area?> GetByIdAsync(int id, int tenantId)
        {
            return await _context.Areas
                .FirstOrDefaultAsync(a => a.Id == id && a.TenantId == tenantId && a.DeletedAt == null);
        }

        public async Task<List<Area>> GetAllAsync(int tenantId)
        {
            return await _context.Areas
                .Where(a => a.TenantId == tenantId && a.DeletedAt == null)
                .OrderBy(a => a.Name)
                .ToListAsync();
        }

        public async Task<Area> CreateAsync(Area area)
        {
            _context.Areas.Add(area);
            await _context.SaveChangesAsync();
            return area;
        }

        public async Task<Area> UpdateAsync(Area area)
        {
            area.MarkAsUpdated();
            _context.Areas.Update(area);
            await _context.SaveChangesAsync();
            return area;
        }

        public async Task<bool> DeleteAsync(int id, int tenantId)
        {
            var area = await GetByIdAsync(id,tenantId);
            if (area == null || area.TenantId != tenantId) return false;

            area.MarkAsDeleted();
            _context.Areas.Update(area);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

