using JEGASolutions.ReportBuilder.Core.Entities.Models;
using JEGASolutions.ReportBuilder.Core.Dto;

namespace JEGASolutions.ReportBuilder.Core.Interfaces
{
    public interface IReportSubmissionService
    {
        Task<ReportSubmission> GetReportSubmissionByIdAsync(int submissionId, int tenantId);
        Task<List<ReportSubmission>> GetReportSubmissionsByTenantAsync(int tenantId);
        Task<List<ReportSubmission>> GetReportSubmissionsByAreaAsync(int areaId, int tenantId);
        Task<List<ReportSubmission>> GetReportSubmissionsByUserAsync(int userId, int tenantId);
        Task<ReportSubmission> CreateReportSubmissionAsync(ReportSubmissionCreateDto createDto, int userId, int tenantId);
        Task<ReportSubmission> UpdateReportSubmissionAsync(ReportSubmissionUpdateDto updateDto, int tenantId);
        Task<bool> SubmitReportAsync(int submissionId, int tenantId);
        Task<bool> ApproveReportAsync(int submissionId, int approvedByUserId, int tenantId);
        Task<bool> RejectReportAsync(int submissionId, string rejectionReason, int tenantId);
        Task<bool> DeleteReportSubmissionAsync(int submissionId, int tenantId);
    }
}
