namespace JEGASolutions.TenantDashboard.Core.DTOs;

public class TenantDto
{
    public int Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string Subdomain { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class TenantModuleDto
{
    public int Id { get; set; }
    public string ModuleName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime PurchasedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

public class ModuleDto
{
    public int Id { get; set; }
    public string ModuleName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime PurchasedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

public class DashboardStatsDto
{
    public int TotalModules { get; set; }
    public int ActiveModules { get; set; }
    public int TotalUsers { get; set; }
    public DateTime LastActivity { get; set; }
    public Dictionary<string, int> ModuleUsage { get; set; } = new();
}

public class TenantDashboardDto
{
    public TenantDto Tenant { get; set; } = null!;
    public List<ModuleDto> Modules { get; set; } = new();
    public DashboardStatsDto Stats { get; set; } = null!;
}
