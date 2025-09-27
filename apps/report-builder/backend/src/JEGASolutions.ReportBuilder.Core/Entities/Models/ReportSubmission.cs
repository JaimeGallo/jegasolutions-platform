using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JEGASolutions.ReportBuilder.Core.Entities;

namespace JEGASolutions.ReportBuilder.Core.Entities.Models
{
    [Table("report_submissions")]
    public class ReportSubmission : TenantEntity
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("template_id")]
        public int TemplateId { get; set; }

        [ForeignKey("TemplateId")]
        public virtual Template Template { get; set; } = null!;

        [Required]
        [Column("area_id")]
        public int AreaId { get; set; }

        [ForeignKey("AreaId")]
        public virtual Area Area { get; set; } = null!;

        [Required]
        [Column("submitted_by_user_id")]
        public int SubmittedByUserId { get; set; }

        [Column("title")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Column("content", TypeName = "text")]
        public string Content { get; set; } = string.Empty;

        [Column("status")]
        [StringLength(20)]
        public string Status { get; set; } = "draft"; // draft, submitted, approved, rejected

        [Column("submitted_at")]
        public DateTime? SubmittedAt { get; set; }

        [Column("approved_at")]
        public DateTime? ApprovedAt { get; set; }

        [Column("approved_by_user_id")]
        public int? ApprovedByUserId { get; set; }

        [Column("rejection_reason", TypeName = "text")]
        public string? RejectionReason { get; set; }

        [Column("period_start")]
        public DateTime PeriodStart { get; set; }

        [Column("period_end")]
        public DateTime PeriodEnd { get; set; }
    }
}
