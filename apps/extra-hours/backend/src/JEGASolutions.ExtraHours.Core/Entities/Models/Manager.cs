using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JEGASolutions.ExtraHours.Core.Entities;

namespace JEGASolutions.ExtraHours.Core.Entities.Models
{
    [Table("managers")]
    public class Manager : TenantEntity
    {

        [Key]
        [Column("manager_id")]
        public long manager_id { get; set; }

        [ForeignKey("manager_id")]
        public User? User { get; set; }

        [Required]
        [Column("manager_name")]
        public string manager_name { get; set; } = string.Empty;
    }
}
