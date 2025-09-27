using Microsoft.EntityFrameworkCore;
using JEGASolutions.ReportBuilder.Core.Entities.Models;
using JEGASolutions.ReportBuilder.Core.Interfaces;
using JEGASolutions.ReportBuilder.Data;

namespace JEGASolutions.ReportBuilder.Infrastructure.Repositories
{
    public class ReportSubmissionRepository : IReportSubmissionRepository
    {
        private readonly AppDbContext _context;

        public ReportSubmissionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ReportSubmission?> GetByIdAsync(int id, int tenantId)
        {
            return await _context.ReportSubmissions
                .AsNoTracking()
                .Include(rs => rs.Template)
                .Include(rs => rs.Area)
                .FirstOrDefaultAsync(rs => rs.Id == id && rs.TenantId == tenantId);
        }

        public async Task<List<ReportSubmission>> GetByTenantAsync(int tenantId)
        {
            return await _context.ReportSubmissions
                .AsNoTracking()
                .Include(rs => rs.Template)
                .Include(rs => rs.Area)
                .Where(rs => rs.TenantId == tenantId)
                .OrderByDescending(rs => rs.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<ReportSubmission>> GetByAreaAsync(int areaId, int tenantId)
        {
            return await _context.ReportSubmissions
                .AsNoTracking()
                .Include(rs => rs.Template)
                .Include(rs => rs.Area)
                .Where(rs => rs.AreaId == areaId && rs.TenantId == tenantId)
                .OrderByDescending(rs => rs.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<ReportSubmission>> GetByUserAsync(int userId, int tenantId)
        {
            return await _context.ReportSubmissions
                .AsNoTracking()
                .Include(rs => rs.Template)
                .Include(rs => rs.Area)
                .Where(rs => rs.SubmittedByUserId == userId && rs.TenantId == tenantId)
                .OrderByDescending(rs => rs.CreatedAt)
                .ToListAsync();
        }

        public async Task<ReportSubmission> CreateAsync(ReportSubmission submission)
        {
            _context.ReportSubmissions.Add(submission);
            await _context.SaveChangesAsync();
            return submission;
        }

        public async Task<ReportSubmission> UpdateAsync(ReportSubmission submission)
        {
            _context.Entry(submission).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return submission;
        }

        public async Task<bool> DeleteAsync(int id, int tenantId)
        {
            var submission = await _context.ReportSubmissions
                .FirstOrDefaultAsync(rs => rs.Id == id && rs.TenantId == tenantId);
            
            if (submission == null)
                return false;

            submission.MarkAsDeleted();
            _context.Entry(submission).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id, int tenantId)
        {
            return await _context.ReportSubmissions
                .AnyAsync(rs => rs.Id == id && rs.TenantId == tenantId);
        }
    }
}
