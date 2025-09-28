using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JEGASolutions.TenantDashboard.Core.Entities;

[Table("tenants")]
public class Tenant
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(255)]
    [Column("company_name")]
    public string CompanyName { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    [Column("subdomain")]
    public string Subdomain { get; set; } = string.Empty;

    [Required]
    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual ICollection<TenantModule> Modules { get; set; } = new List<TenantModule>();
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
