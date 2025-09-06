namespace JEGASolutions.Landing.Application.DTOs;

public class PaymentResponseDto
{
    public int Id { get; set; }

    public string Reference { get; set; } = string.Empty;

    /// <summary>
    /// Estado del pago (APPROVED, DECLINED, PENDING, FAILED, CANCELLED, etc.)
    /// </summary>
    public string Status { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public string Currency { get; set; } = "COP";

    public string? CustomerEmail { get; set; }

    public string? CustomerName { get; set; }

    public string? CustomerPhone { get; set; }

    public string? WompiTransactionId { get; set; }

    public string? CheckoutUrl { get; set; } // útil para frontend cuando creas el pago

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? Metadata { get; set; }
}
