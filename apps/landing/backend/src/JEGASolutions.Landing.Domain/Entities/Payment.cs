using System.ComponentModel.DataAnnotations;

namespace JEGASolutions.Landing.Domain.Entities;

public class Payment
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Reference { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = "PENDING";

    public decimal Amount { get; set; }

    [MaxLength(255)]
    public string? CustomerEmail { get; set; }

    [MaxLength(255)]
    public string? CustomerName { get; set; }

    public string? CustomerPhone { get; set; }

    [MaxLength(20)]
    public string? WompiTransactionId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public string? Metadata { get; set; } // JSON string for additional data
}