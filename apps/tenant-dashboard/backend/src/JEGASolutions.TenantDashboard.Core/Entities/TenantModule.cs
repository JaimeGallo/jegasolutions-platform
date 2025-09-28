using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JEGASolutions.TenantDashboard.Core.Entities;

[Table("tenant_modules")]
public class TenantModule
{
    [Key]
    public int Id { get; set; }

    [Required]
    [Column("tenant_id")]
    public int TenantId { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("module_name")]
    public string ModuleName { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    [Column("status")]
    public string Status { get; set; } = "ACTIVE";

    [Column("purchased_at")]
    public DateTime PurchasedAt { get; set; } = DateTime.UtcNow;

    [Column("expires_at")]
    public DateTime? ExpiresAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    [ForeignKey("TenantId")]
    public virtual Tenant Tenant { get; set; } = null!;
}
