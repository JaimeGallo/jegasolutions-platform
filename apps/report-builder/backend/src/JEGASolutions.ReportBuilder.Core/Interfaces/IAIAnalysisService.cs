using JEGASolutions.ReportBuilder.Core.Dto;

namespace JEGASolutions.ReportBuilder.Core.Interfaces
{
    public interface IAIAnalysisService
    {
        Task<AIAnalysisResultDto> AnalyzeReportAsync(AIAnalysisRequestDto request, int tenantId);
        Task<List<AIInsightDto>> GetInsightsForReportAsync(int reportSubmissionId, int tenantId);
        Task<List<AIInsightDto>> GetInsightsByTypeAsync(string insightType, int tenantId);
        Task<bool> GenerateInsightsAsync(int reportSubmissionId, int tenantId);
        Task<bool> DeleteInsightAsync(int insightId, int tenantId);
    }
}
