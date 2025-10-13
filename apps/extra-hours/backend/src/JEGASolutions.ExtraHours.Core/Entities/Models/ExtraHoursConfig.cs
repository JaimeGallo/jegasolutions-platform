using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JEGASolutions.ExtraHours.Core.Entities;

namespace JEGASolutions.ExtraHours.Core.Entities.Models
{
    [Table("extra_hours_config")]
    public class ExtraHoursConfig : TenantEntity
    {
        [Key]
        [Column("id")]
        public long id { get; set; }

        [Required]
        [Column("weekly_extra_hours_limit")]
        public double weeklyExtraHoursLimit { get; set; }

        [Required]
        [Column("diurnal_multiplier")]
        public double diurnalMultiplier { get; set; }

        [Required]
        [Column("nocturnal_multiplier")]
        public double nocturnalMultiplier { get; set; }

        [Required]
        [Column("diurnal_holiday_multiplier")]
        public double diurnalHolidayMultiplier { get; set; }

        [Required]
        [Column("nocturnal_holiday_multiplier")]
        public double nocturnalHolidayMultiplier { get; set; }

        [Required]
        [Column("diurnal_start", TypeName = "time")]
        public TimeSpan diurnalStart { get; set; }

        [Required]
        [Column("diurnal_end", TypeName = "time")]
        public TimeSpan diurnalEnd { get; set; }
    }
}
