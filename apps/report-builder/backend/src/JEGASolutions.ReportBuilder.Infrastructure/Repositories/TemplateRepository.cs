using Microsoft.EntityFrameworkCore;
using JEGASolutions.ReportBuilder.Core.Entities.Models;
using JEGASolutions.ReportBuilder.Core.Interfaces;
using JEGASolutions.ReportBuilder.Data;

namespace JEGASolutions.ReportBuilder.Infrastructure.Repositories
{
    public class TemplateRepository : ITemplateRepository
    {
        private readonly AppDbContext _context;

        public TemplateRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Template?> GetByIdAsync(int id, int tenantId)
        {
            return await _context.Templates
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id && t.TenantId == tenantId);
        }

        public async Task<List<Template>> GetByTenantAsync(int tenantId)
        {
            return await _context.Templates
                .AsNoTracking()
                .Where(t => t.TenantId == tenantId)
                .OrderBy(t => t.Name)
                .ToListAsync();
        }

        public async Task<List<Template>> GetByAreaAsync(int areaId, int tenantId)
        {
            return await _context.Templates
                .AsNoTracking()
                .Where(t => t.AreaId == areaId && t.TenantId == tenantId)
                .OrderBy(t => t.Name)
                .ToListAsync();
        }

        public async Task<List<Template>> GetByTypeAsync(string type, int tenantId)
        {
            return await _context.Templates
                .AsNoTracking()
                .Where(t => t.TenantId == tenantId && t.Type == type)
                .OrderBy(t => t.Name)
                .ToListAsync();
        }

        public async Task<Template> CreateAsync(Template template)
        {
            _context.Templates.Add(template);
            await _context.SaveChangesAsync();
            return template;
        }

        public async Task<Template> UpdateAsync(Template template)
        {
            _context.Entry(template).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return template;
        }

        public async Task<bool> DeleteAsync(int id, int tenantId)
        {
            var template = await _context.Templates
                .FirstOrDefaultAsync(t => t.Id == id && t.TenantId == tenantId);
            
            if (template == null)
                return false;

            template.MarkAsDeleted();
            _context.Entry(template).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id, int tenantId)
        {
            return await _context.Templates
                .AnyAsync(t => t.Id == id && t.TenantId == tenantId);
        }
    }
}
