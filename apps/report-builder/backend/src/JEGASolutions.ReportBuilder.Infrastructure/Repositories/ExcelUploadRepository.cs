using JEGASolutions.ReportBuilder.Core.Entities.Models;
using JEGASolutions.ReportBuilder.Core.Interfaces;
using JEGASolutions.ReportBuilder.Data;
using Microsoft.EntityFrameworkCore;

namespace JEGASolutions.ReportBuilder.Infrastructure.Repositories
{
    public class ExcelUploadRepository : IExcelUploadRepository
    {
        private readonly AppDbContext _context;

        public ExcelUploadRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ExcelUpload> CreateAsync(ExcelUpload upload)
        {
            _context.ExcelUploads.Add(upload);
            await _context.SaveChangesAsync();
            return upload;
        }

        public async Task<ExcelUpload> UpdateAsync(ExcelUpload upload)
        {
            upload.MarkAsUpdated();
            _context.ExcelUploads.Update(upload);
            await _context.SaveChangesAsync();
            return upload;
        }

        public async Task<bool> DeleteAsync(int id, int tenantId)
        {
            var upload = await GetByIdAsync(id, tenantId);
            if (upload == null) return false;

            upload.MarkAsDeleted();
            _context.ExcelUploads.Update(upload);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ExcelUpload?> GetByIdAsync(int id, int tenantId)
        {
            return await _context.ExcelUploads
                .Include(e => e.Area)
                .Include(e => e.UploadedByUser)
                .FirstOrDefaultAsync(e => e.Id == id && e.TenantId == tenantId && e.DeletedAt == null);
        }

        public async Task<List<ExcelUpload>> GetAllAsync(int tenantId, int? areaId = null, string? period = null)
        {
            var query = _context.ExcelUploads
                .Include(e => e.Area)
                .Include(e => e.UploadedByUser)
                .Where(e => e.TenantId == tenantId && e.DeletedAt == null);

            if (areaId.HasValue)
            {
                query = query.Where(e => e.AreaId == areaId.Value);
            }

            if (!string.IsNullOrEmpty(period))
            {
                query = query.Where(e => e.Period == period);
            }

            return await query
                .OrderByDescending(e => e.UploadDate)
                .ToListAsync();
        }

        public async Task<List<ExcelUpload>> GetByAreaIdAsync(int areaId, int tenantId)
        {
            return await _context.ExcelUploads
                .Include(e => e.UploadedByUser)
                .Where(e => e.AreaId == areaId && e.TenantId == tenantId && e.DeletedAt == null)
                .OrderByDescending(e => e.UploadDate)
                .ToListAsync();
        }

        public async Task<List<ExcelUpload>> GetByPeriodAsync(string period, int tenantId)
        {
            return await _context.ExcelUploads
                .Include(e => e.Area)
                .Include(e => e.UploadedByUser)
                .Where(e => e.Period == period && e.TenantId == tenantId && e.DeletedAt == null)
                .OrderByDescending(e => e.UploadDate)
                .ToListAsync();
        }

        public async Task<List<ExcelUpload>> GetByUploaderAsync(int userId, int tenantId)
        {
            return await _context.ExcelUploads
                .Include(e => e.Area)
                .Where(e => e.UploadedByUserId == userId && e.TenantId == tenantId && e.DeletedAt == null)
                .OrderByDescending(e => e.UploadDate)
                .ToListAsync();
        }

        public async Task<List<ExcelUpload>> GetByProcessingStatusAsync(string status, int tenantId)
        {
            return await _context.ExcelUploads
                .Include(e => e.Area)
                .Include(e => e.UploadedByUser)
                .Where(e => e.ProcessingStatus == status && e.TenantId == tenantId && e.DeletedAt == null)
                .OrderByDescending(e => e.UploadDate)
                .ToListAsync();
        }

        public async Task<List<ExcelUpload>> GetPendingProcessingAsync(int tenantId)
        {
            return await _context.ExcelUploads
                .Include(e => e.Area)
                .Include(e => e.UploadedByUser)
                .Where(e => e.ProcessingStatus == "pending" && e.TenantId == tenantId && e.DeletedAt == null)
                .OrderBy(e => e.UploadDate)
                .ToListAsync();
        }

        public async Task<Dictionary<string, int>> GetCountByStatusAsync(int tenantId)
        {
            return await _context.ExcelUploads
                .Where(e => e.TenantId == tenantId && e.DeletedAt == null)
                .GroupBy(e => e.ProcessingStatus)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Status, x => x.Count);
        }

        public async Task<bool> ExistsAsync(string fileName, string period, int areaId, int tenantId)
        {
            return await _context.ExcelUploads
                .AnyAsync(e => e.FileName == fileName 
                    && e.Period == period 
                    && e.AreaId == areaId 
                    && e.TenantId == tenantId 
                    && e.DeletedAt == null);
        }
    }
}

