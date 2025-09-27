using JEGASolutions.ReportBuilder.Core.Entities.Models;
using JEGASolutions.ReportBuilder.Core.Interfaces;
using JEGASolutions.ReportBuilder.Core.Dto;

namespace JEGASolutions.ReportBuilder.Core.Services
{
    public class ReportSubmissionService : IReportSubmissionService
    {
        private readonly IReportSubmissionRepository _reportSubmissionRepository;

        public ReportSubmissionService(IReportSubmissionRepository reportSubmissionRepository)
        {
            _reportSubmissionRepository = reportSubmissionRepository;
        }

        public async Task<ReportSubmission> GetReportSubmissionByIdAsync(int submissionId, int tenantId)
        {
            var submission = await _reportSubmissionRepository.GetByIdAsync(submissionId, tenantId);
            if (submission == null)
                throw new InvalidOperationException("Report submission not found");
            return submission;
        }

        public async Task<List<ReportSubmission>> GetReportSubmissionsByTenantAsync(int tenantId)
        {
            return await _reportSubmissionRepository.GetByTenantAsync(tenantId);
        }

        public async Task<List<ReportSubmission>> GetReportSubmissionsByAreaAsync(int areaId, int tenantId)
        {
            return await _reportSubmissionRepository.GetByAreaAsync(areaId, tenantId);
        }

        public async Task<List<ReportSubmission>> GetReportSubmissionsByUserAsync(int userId, int tenantId)
        {
            return await _reportSubmissionRepository.GetByUserAsync(userId, tenantId);
        }

        public async Task<ReportSubmission> CreateReportSubmissionAsync(ReportSubmissionCreateDto createDto, int userId, int tenantId)
        {
            var submission = new ReportSubmission
            {
                TenantId = tenantId,
                TemplateId = createDto.TemplateId,
                AreaId = createDto.AreaId,
                SubmittedByUserId = userId,
                Title = createDto.Title,
                Content = createDto.Content,
                Status = "draft",
                PeriodStart = createDto.PeriodStart,
                PeriodEnd = createDto.PeriodEnd
            };

            return await _reportSubmissionRepository.CreateAsync(submission);
        }

        public async Task<ReportSubmission> UpdateReportSubmissionAsync(ReportSubmissionUpdateDto updateDto, int tenantId)
        {
            var existingSubmission = await _reportSubmissionRepository.GetByIdAsync(updateDto.Id, tenantId);
            if (existingSubmission == null)
                throw new InvalidOperationException("Report submission not found");

            existingSubmission.Title = updateDto.Title;
            existingSubmission.Content = updateDto.Content;
            existingSubmission.Status = updateDto.Status;
            existingSubmission.RejectionReason = updateDto.RejectionReason;
            existingSubmission.MarkAsUpdated();

            return await _reportSubmissionRepository.UpdateAsync(existingSubmission);
        }

        public async Task<bool> SubmitReportAsync(int submissionId, int tenantId)
        {
            var submission = await _reportSubmissionRepository.GetByIdAsync(submissionId, tenantId);
            if (submission == null)
                return false;

            submission.Status = "submitted";
            submission.SubmittedAt = DateTime.UtcNow;
            submission.MarkAsUpdated();

            await _reportSubmissionRepository.UpdateAsync(submission);
            return true;
        }

        public async Task<bool> ApproveReportAsync(int submissionId, int approvedByUserId, int tenantId)
        {
            var submission = await _reportSubmissionRepository.GetByIdAsync(submissionId, tenantId);
            if (submission == null)
                return false;

            submission.Status = "approved";
            submission.ApprovedAt = DateTime.UtcNow;
            submission.ApprovedByUserId = approvedByUserId;
            submission.MarkAsUpdated();

            await _reportSubmissionRepository.UpdateAsync(submission);
            return true;
        }

        public async Task<bool> RejectReportAsync(int submissionId, string rejectionReason, int tenantId)
        {
            var submission = await _reportSubmissionRepository.GetByIdAsync(submissionId, tenantId);
            if (submission == null)
                return false;

            submission.Status = "rejected";
            submission.RejectionReason = rejectionReason;
            submission.MarkAsUpdated();

            await _reportSubmissionRepository.UpdateAsync(submission);
            return true;
        }

        public async Task<bool> DeleteReportSubmissionAsync(int submissionId, int tenantId)
        {
            return await _reportSubmissionRepository.DeleteAsync(submissionId, tenantId);
        }
    }
}
