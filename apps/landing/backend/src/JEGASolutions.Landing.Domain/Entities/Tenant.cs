using System.ComponentModel.DataAnnotations;

namespace JEGASolutions.Landing.Domain.Entities;

public class Tenant
{
    public int Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string CompanyName { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Subdomain { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public string? ConnectionString { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual ICollection<TenantModule> Modules { get; set; } = new List<TenantModule>();
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}