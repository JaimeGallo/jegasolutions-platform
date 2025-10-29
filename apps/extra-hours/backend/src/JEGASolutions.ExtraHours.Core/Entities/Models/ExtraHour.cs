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

        [Column("diurnal")]
        public double diurnal { get; set; } = 0;

        [Column("nocturnal")]
        public double nocturnal { get; set; } = 0;

        [Column("diurnal_holiday")]
        public double diurnalHoliday { get; set; } = 0;

        [Column("nocturnal_holiday")]
        public double nocturnalHoliday { get; set; } = 0;

        [Column("total_hours")]
        public double extraHours { get; set; } = 0;

        [Column("type")]
        public string? type { get; set; } = "extra"; // Tipo general

        [Column("status")]
        [Required]
        public string status { get; set; } = "pending"; // pending, approved, rejected

        [Column("notes")]
        public string? observations { get; set; }

        [Column("approved_by")]
        public long? ApprovedByManagerId { get; set; }

        [Column("approved_at")]
        public DateTime? approvedAt { get; set; }

        [NotMapped]
        public bool approved
        {
            get => status.ToLower() == "approved";
            set => status = value ? "approved" : "pending";
        }

        [ForeignKey("id")]
        public Employee? employee { get; set; }

        [ForeignKey("ApprovedByManagerId")]
        public User? ApprovedByManager { get; set; }
    }
}
