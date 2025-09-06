using System.ComponentModel.DataAnnotations;

namespace JEGASolutions.Landing.Domain.Entities;

public class TenantModule
{
    public int Id { get; set; }

    public int TenantId { get; set; }

    [Required]
    [MaxLength(100)]
    public string ModuleName { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = "ACTIVE";

    public DateTime PurchasedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    // Navigation property
    public virtual Tenant Tenant { get; set; } = null!;
}