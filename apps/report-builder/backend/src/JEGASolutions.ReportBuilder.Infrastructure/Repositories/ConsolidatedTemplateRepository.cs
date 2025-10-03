using JEGASolutions.ReportBuilder.Core.Entities.Models;
using JEGASolutions.ReportBuilder.Core.Interfaces;
using JEGASolutions.ReportBuilder.Data;
using Microsoft.EntityFrameworkCore;

namespace JEGASolutions.ReportBuilder.Infrastructure.Repositories
{
    public class ConsolidatedTemplateRepository : IConsolidatedTemplateRepository
    {
        private readonly AppDbContext _context;

        public ConsolidatedTemplateRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ConsolidatedTemplate> CreateAsync(ConsolidatedTemplate template)
        {
            _context.ConsolidatedTemplates.Add(template);
            await _context.SaveChangesAsync();
            return template;
        }

        public async Task<ConsolidatedTemplate> UpdateAsync(ConsolidatedTemplate template)
        {
            template.MarkAsUpdated();
            _context.ConsolidatedTemplates.Update(template);
            await _context.SaveChangesAsync();
            return template;
        }

        public async Task<bool> DeleteAsync(int id, int tenantId)
        {
            var template = await GetByIdAsync(id, tenantId);
            if (template == null) return false;

            template.MarkAsDeleted();
            _context.ConsolidatedTemplates.Update(template);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ConsolidatedTemplate?> GetByIdAsync(int id, int tenantId)
        {
            return await _context.ConsolidatedTemplates
                .Include(ct => ct.Sections)
                    .ThenInclude(s => s.Area)
                .Include(ct => ct.Sections)
                    .ThenInclude(s => s.CompletedByUser)
                .Include(ct => ct.CreatedByUser)
                .FirstOrDefaultAsync(ct => ct.Id == id && ct.TenantId == tenantId && ct.DeletedAt == null);
        }

        public async Task<List<ConsolidatedTemplate>> GetAllAsync(int tenantId, string? status = null)
        {
            var query = _context.ConsolidatedTemplates
                .Include(ct => ct.Sections)
                .Include(ct => ct.CreatedByUser)
                .Where(ct => ct.TenantId == tenantId && ct.DeletedAt == null);

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(ct => ct.Status == status);
            }

            return await query
                .OrderByDescending(ct => ct.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<ConsolidatedTemplate>> GetByPeriodAsync(string period, int tenantId)
        {
            return await _context.ConsolidatedTemplates
                .Include(ct => ct.Sections)
                .Include(ct => ct.CreatedByUser)
                .Where(ct => ct.Period == period && ct.TenantId == tenantId && ct.DeletedAt == null)
                .OrderByDescending(ct => ct.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<ConsolidatedTemplate>> GetByCreatorAsync(int userId, int tenantId)
        {
            return await _context.ConsolidatedTemplates
                .Include(ct => ct.Sections)
                .Where(ct => ct.CreatedByUserId == userId && ct.TenantId == tenantId && ct.DeletedAt == null)
                .OrderByDescending(ct => ct.CreatedAt)
                .ToListAsync();
        }

        public async Task<Dictionary<string, int>> GetCountByStatusAsync(int tenantId)
        {
            return await _context.ConsolidatedTemplates
                .Where(ct => ct.TenantId == tenantId && ct.DeletedAt == null)
                .GroupBy(ct => ct.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Status, x => x.Count);
        }

        public async Task<List<ConsolidatedTemplate>> GetWithUpcomingDeadlinesAsync(int tenantId, int daysAhead = 7)
        {
            var futureDate = DateTime.UtcNow.AddDays(daysAhead);
            
            return await _context.ConsolidatedTemplates
                .Include(ct => ct.Sections)
                    .ThenInclude(s => s.Area)
                .Where(ct => ct.TenantId == tenantId 
                    && ct.DeletedAt == null
                    && ct.Deadline.HasValue 
                    && ct.Deadline.Value <= futureDate
                    && ct.Status != "completed")
                .OrderBy(ct => ct.Deadline)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(string name, string period, int tenantId, int? excludeId = null)
        {
            var query = _context.ConsolidatedTemplates
                .Where(ct => ct.Name == name 
                    && ct.Period == period 
                    && ct.TenantId == tenantId 
                    && ct.DeletedAt == null);

            if (excludeId.HasValue)
            {
                query = query.Where(ct => ct.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }
    }
}

