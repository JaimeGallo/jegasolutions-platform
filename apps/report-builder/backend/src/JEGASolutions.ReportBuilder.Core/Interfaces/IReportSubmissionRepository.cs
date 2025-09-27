using JEGASolutions.ReportBuilder.Core.Entities.Models;

namespace JEGASolutions.ReportBuilder.Core.Interfaces
{
    public interface IReportSubmissionRepository
    {
        Task<ReportSubmission?> GetByIdAsync(int id, int tenantId);
        Task<List<ReportSubmission>> GetByTenantAsync(int tenantId);
        Task<List<ReportSubmission>> GetByAreaAsync(int areaId, int tenantId);
        Task<List<ReportSubmission>> GetByUserAsync(int userId, int tenantId);
        Task<ReportSubmission> CreateAsync(ReportSubmission submission);
        Task<ReportSubmission> UpdateAsync(ReportSubmission submission);
        Task<bool> DeleteAsync(int id, int tenantId);
        Task<bool> ExistsAsync(int id, int tenantId);
    }
}
