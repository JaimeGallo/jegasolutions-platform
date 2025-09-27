namespace JEGASolutions.ReportBuilder.Core.Dto
{
    public class ReportSubmissionDto
    {
        public int Id { get; set; }
        public int TemplateId { get; set; }
        public string TemplateName { get; set; } = string.Empty;
        public int AreaId { get; set; }
        public string AreaName { get; set; } = string.Empty;
        public int SubmittedByUserId { get; set; }
        public string SubmittedByUserName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime? SubmittedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public int? ApprovedByUserId { get; set; }
        public string? ApprovedByUserName { get; set; }
        public string? RejectionReason { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class ReportSubmissionCreateDto
    {
        public int TemplateId { get; set; }
        public int AreaId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
    }

    public class ReportSubmissionUpdateDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? RejectionReason { get; set; }
    }
}
