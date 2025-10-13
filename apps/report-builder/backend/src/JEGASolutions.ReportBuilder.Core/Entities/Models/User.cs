using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JEGASolutions.ReportBuilder.Core.Entities;

namespace JEGASolutions.ReportBuilder.Core.Entities.Models
{
    [Table("users")]
    public class User : TenantEntity
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        [Column("email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Column("password_hash")]
        public string PasswordHash { get; set; } = string.Empty;

        [StringLength(255)]
        [Column("full_name")]
        public string FullName { get; set; } = string.Empty;

        [StringLength(20)]
        [Column("role")]
        public string Role { get; set; } = "User"; // superusuario, User

        [Column("is_active")]
        public bool IsActive { get; set; } = true;
    }
}

