using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JEGASolutions.ExtraHours.Core.Entities;

namespace JEGASolutions.ExtraHours.Core.Entities.Models
{
    [Table("extra_hours")]
    public class ExtraHour : TenantEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int registry { get; set; }

        [Column("employee_id")]
        public long id { get; set; }

        [Column("date")]
        public DateTime date { get; set; }

        [Column("start_time")]
        public TimeSpan startTime { get; set; }

        [Column("end_time")]
        public TimeSpan endTime { get; set; }

        [Column("total_hours")]
        public double? extraHours { get; set; }

        [Column("type")]
        public string? type { get; set; }

        [Column("status")]
        public string? status { get; set; }

        [Column("notes")]
        public string? observations { get; set; }

        [Column("approved_by")]
        public long? ApprovedByManagerId { get; set; }

        [Column("approved_at")]
        public DateTime? approvedAt { get; set; }

        // Computed properties for backward compatibility (not mapped to DB)
        [NotMapped]
        public double diurnal { get; set; }

        [NotMapped]
        public double nocturnal { get; set; }

        [NotMapped]
        public double diurnalHoliday { get; set; }

        [NotMapped]
        public double nocturnalHoliday { get; set; }

        [NotMapped]
        public bool approved
        {
            get => status?.ToLower() == "approved";
            set => status = value ? "approved" : "pending";
        }

        [ForeignKey("id")]
        public Employee? employee { get; set; }

        [ForeignKey("ApprovedByManagerId")]
        public User? ApprovedByManager { get; set; }
    }
}
