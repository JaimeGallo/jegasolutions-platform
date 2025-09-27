using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JEGASolutions.ReportBuilder.Core.Entities;

namespace JEGASolutions.ReportBuilder.Core.Entities.Models
{
    [Table("ai_insights")]
    public class AIInsight : TenantEntity
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("report_submission_id")]
        public int ReportSubmissionId { get; set; }

        [ForeignKey("ReportSubmissionId")]
        public virtual ReportSubmission ReportSubmission { get; set; } = null!;

        [Required]
        [Column("insight_type")]
        [StringLength(50)]
        public string InsightType { get; set; } = string.Empty; // trend, anomaly, recommendation, summary

        [Required]
        [Column("title")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Column("content", TypeName = "text")]
        public string Content { get; set; } = string.Empty;

        [Column("confidence_score")]
        public decimal ConfidenceScore { get; set; }

        [Column("metadata", TypeName = "jsonb")]
        public string? Metadata { get; set; }

        [Column("generated_at")]
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

        [Column("ai_model")]
        [StringLength(100)]
        public string? AIModel { get; set; }
    }
}
