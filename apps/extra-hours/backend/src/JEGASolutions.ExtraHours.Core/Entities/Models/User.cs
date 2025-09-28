using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JEGASolutions.ExtraHours.Core.Entities;

namespace JEGASolutions.ExtraHours.Core.Entities.Models
{
    [Table("users")]
    public class User : TenantEntity
    {
        [Key]
        [Column("id")]
        public long id { get; set; }

        [Required, EmailAddress]
        [Column("email")]
        public string email { get; set; } = string.Empty;

        [Required]
        [Column("name")]
        public string name { get; set; } = string.Empty;

        [Required]
        [Column("password")]
        public string passwordHash { get; set; } = string.Empty;

        [Required]
        [Column("role")]
        public string role { get; set; } = string.Empty;

        [Required]
        [Column("username")]
        public string username { get; set; } = string.Empty;
    }
}
