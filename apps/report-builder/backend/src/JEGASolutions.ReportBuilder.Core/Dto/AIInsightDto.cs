namespace JEGASolutions.ReportBuilder.Core.Dto
{
    public class AIInsightDto
    {
        public int Id { get; set; }
        public int ReportSubmissionId { get; set; }
        public string InsightType { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public decimal ConfidenceScore { get; set; }
        public string? Metadata { get; set; }
        public DateTime GeneratedAt { get; set; }
        public string? AIModel { get; set; }
    }

    public class AIAnalysisRequestDto
    {
        public int ReportSubmissionId { get; set; }
        public string AnalysisType { get; set; } = string.Empty; // trend, anomaly, summary, recommendation
        public Dictionary<string, object> Parameters { get; set; } = new();
    }

    public class AIAnalysisResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<AIInsightDto> Insights { get; set; } = new();
        public string? ErrorDetails { get; set; }
    }
}
