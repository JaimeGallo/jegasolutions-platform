using JEGASolutions.ReportBuilder.Core.Entities.Models;
using JEGASolutions.ReportBuilder.Core.Interfaces;
using JEGASolutions.ReportBuilder.Data;
using Microsoft.EntityFrameworkCore;

namespace JEGASolutions.ReportBuilder.Infrastructure.Repositories
{
    public class ConsolidatedTemplateSectionRepository : IConsolidatedTemplateSectionRepository
    {
        private readonly AppDbContext _context;

        public ConsolidatedTemplateSectionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ConsolidatedTemplateSection> CreateAsync(ConsolidatedTemplateSection section)
        {
            _context.ConsolidatedTemplateSections.Add(section);
            await _context.SaveChangesAsync();
            return section;
        }

        public async Task<ConsolidatedTemplateSection> UpdateAsync(ConsolidatedTemplateSection section)
        {
            section.MarkAsUpdated();
            _context.ConsolidatedTemplateSections.Update(section);
            await _context.SaveChangesAsync();
            return section;
        }

        public async Task<bool> DeleteAsync(int id, int tenantId)
        {
            var section = await GetByIdAsync(id, tenantId);
            if (section == null) return false;

            section.MarkAsDeleted();
            _context.ConsolidatedTemplateSections.Update(section);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ConsolidatedTemplateSection?> GetByIdAsync(int id, int tenantId)
        {
            return await _context.ConsolidatedTemplateSections
                .Include(s => s.ConsolidatedTemplate)
                .Include(s => s.Area)
                .Include(s => s.CompletedByUser)
                .FirstOrDefaultAsync(s => s.Id == id && s.TenantId == tenantId && s.DeletedAt == null);
        }

        public async Task<List<ConsolidatedTemplateSection>> GetByTemplateIdAsync(int templateId, int tenantId)
        {
            return await _context.ConsolidatedTemplateSections
                .Include(s => s.Area)
                .Include(s => s.CompletedByUser)
                .Where(s => s.ConsolidatedTemplateId == templateId 
                    && s.TenantId == tenantId 
                    && s.DeletedAt == null)
                .OrderBy(s => s.Order)
                .ToListAsync();
        }

        public async Task<List<ConsolidatedTemplateSection>> GetByAreaIdAsync(int areaId, int tenantId, string? status = null)
        {
            var query = _context.ConsolidatedTemplateSections
                .Include(s => s.ConsolidatedTemplate)
                .Include(s => s.CompletedByUser)
                .Where(s => s.AreaId == areaId 
                    && s.TenantId == tenantId 
                    && s.DeletedAt == null);

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(s => s.Status == status);
            }

            return await query
                .OrderBy(s => s.SectionDeadline ?? DateTime.MaxValue)
                .ToListAsync();
        }

        public async Task<List<ConsolidatedTemplateSection>> GetCompletedByUserAsync(int userId, int tenantId)
        {
            return await _context.ConsolidatedTemplateSections
                .Include(s => s.ConsolidatedTemplate)
                .Include(s => s.Area)
                .Where(s => s.CompletedByUserId == userId 
                    && s.TenantId == tenantId 
                    && s.DeletedAt == null
                    && s.Status == "completed")
                .OrderByDescending(s => s.CompletedAt)
                .ToListAsync();
        }

        public async Task<List<ConsolidatedTemplateSection>> GetOverdueSectionsAsync(int tenantId)
        {
            var now = DateTime.UtcNow;
            
            return await _context.ConsolidatedTemplateSections
                .Include(s => s.ConsolidatedTemplate)
                .Include(s => s.Area)
                .Where(s => s.TenantId == tenantId 
                    && s.DeletedAt == null
                    && s.SectionDeadline.HasValue
                    && s.SectionDeadline.Value < now
                    && s.Status != "completed")
                .OrderBy(s => s.SectionDeadline)
                .ToListAsync();
        }

        public async Task<List<ConsolidatedTemplateSection>> GetUpcomingDeadlinesAsync(int tenantId, int daysAhead = 7)
        {
            var now = DateTime.UtcNow;
            var futureDate = now.AddDays(daysAhead);
            
            return await _context.ConsolidatedTemplateSections
                .Include(s => s.ConsolidatedTemplate)
                .Include(s => s.Area)
                .Where(s => s.TenantId == tenantId 
                    && s.DeletedAt == null
                    && s.SectionDeadline.HasValue
                    && s.SectionDeadline.Value >= now
                    && s.SectionDeadline.Value <= futureDate
                    && s.Status != "completed")
                .OrderBy(s => s.SectionDeadline)
                .ToListAsync();
        }

        public async Task<Dictionary<string, int>> GetCountByStatusAsync(int tenantId)
        {
            return await _context.ConsolidatedTemplateSections
                .Where(s => s.TenantId == tenantId && s.DeletedAt == null)
                .GroupBy(s => s.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Status, x => x.Count);
        }

        public async Task<Dictionary<int, int>> GetCountByAreaAsync(int tenantId)
        {
            return await _context.ConsolidatedTemplateSections
                .Where(s => s.TenantId == tenantId && s.DeletedAt == null)
                .GroupBy(s => s.AreaId)
                .Select(g => new { AreaId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.AreaId, x => x.Count);
        }

        public async Task<(int Total, int Completed)> GetTemplateProgressAsync(int templateId, int tenantId)
        {
            var sections = await _context.ConsolidatedTemplateSections
                .Where(s => s.ConsolidatedTemplateId == templateId 
                    && s.TenantId == tenantId 
                    && s.DeletedAt == null)
                .ToListAsync();

            var total = sections.Count;
            var completed = sections.Count(s => s.Status == "completed");

            return (total, completed);
        }

        public async Task<bool> UserHasAccessToSectionAsync(int sectionId, int userId, int tenantId)
        {
            // Verificar que la sección existe y pertenece al tenant
            var section = await _context.ConsolidatedTemplateSections
                .Include(s => s.Area)
                .FirstOrDefaultAsync(s => s.Id == sectionId 
                    && s.TenantId == tenantId 
                    && s.DeletedAt == null);

            if (section == null) return false;

            // Verificar que el usuario pertenece al área de la sección
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId 
                    && u.TenantId == tenantId 
                    && u.DeletedAt == null);

            if (user == null) return false;

            // Por ahora, verificamos que ambos existen y pertenecen al mismo tenant
            // En una implementación completa, necesitaríamos una relación User -> Area
            return true;
        }
    }
}

