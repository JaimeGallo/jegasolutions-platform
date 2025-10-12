namespace JEGASolutions.Landing.Domain.Entities;

/// <summary>
/// Representa el acceso de un usuario a un módulo específico
/// Usado para SSO y control de permisos por módulo
/// </summary>
public class UserModuleAccess
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int TenantId { get; set; }
    public string ModuleName { get; set; } = string.Empty;  // "Extra Hours", "Report Builder"
    public string Role { get; set; } = string.Empty;         // "employee", "manager", "admin"
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public User? User { get; set; }
    public Tenant? Tenant { get; set; }
}

