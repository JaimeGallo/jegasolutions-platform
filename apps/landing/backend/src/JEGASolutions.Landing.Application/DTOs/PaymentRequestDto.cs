using System.ComponentModel.DataAnnotations;

namespace JEGASolutions.Landing.Application.DTOs;

public class PaymentRequestDto
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Reference { get; set; } = string.Empty;

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal Amount { get; set; }

    [Required]
    [StringLength(3, MinimumLength = 3, ErrorMessage = "Currency must be a valid ISO 4217 code (e.g., COP, USD)")]
    public string Currency { get; set; } = "COP";

    [Required]
    [EmailAddress]
    [StringLength(255)]
    public string CustomerEmail { get; set; } = string.Empty;

    [Required]
    [StringLength(255, MinimumLength = 2)]
    public string CustomerName { get; set; } = string.Empty;

    [Phone]
    [StringLength(20)]
    public string? CustomerPhone { get; set; }

    [Url]
    [StringLength(500)]
    public string? RedirectUrl { get; set; }

    public string? Metadata { get; set; } // JSON string para datos adicionales (opcional)
}
