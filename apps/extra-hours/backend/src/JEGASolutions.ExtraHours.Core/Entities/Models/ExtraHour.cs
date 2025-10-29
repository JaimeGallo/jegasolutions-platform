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
        [Column("registry")]
        public int registry { get; set; }

        public long id { get; set; }
        public DateTime date { get; set; }
        public TimeSpan startTime { get; set; }
        public TimeSpan endTime { get; set; }


        public double diurnal { get; set; }
        public double nocturnal { get; set; }
        public double diurnalHoliday { get; set; }
        public double nocturnalHoliday { get; set; }
        public double extraHours { get; set; }
        public string? observations { get; set; }

        [Required]
        public bool approved { get; set; } = false;

        [Column("approved_by_id")]
        public long? ApprovedByManagerId { get; set; }

        [ForeignKey("id")]
        public Employee? employee { get; set; }

        [ForeignKey("ApprovedByManagerId")]
        public Manager? ApprovedByManager { get; set; }
    }
}
