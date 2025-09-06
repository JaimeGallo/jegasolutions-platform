using System.ComponentModel.DataAnnotations;

namespace JEGASolutions.Landing.Domain.Entities;

public class Lead
{
    public int Id { get; set; }

    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(255)]
    public string Email { get; set; } = string.Empty;

    [StringLength(20)]
    public string? Phone { get; set; }

    [StringLength(255)]
    public string? Company { get; set; }

    [StringLength(50)]
    public string? Position { get; set; }

    public string? Message { get; set; }

    [StringLength(50)]
    public string Source { get; set; } = "LANDING_FORM";

    [StringLength(20)]
    public string Status { get; set; } = "NEW";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ContactedAt { get; set; }
}