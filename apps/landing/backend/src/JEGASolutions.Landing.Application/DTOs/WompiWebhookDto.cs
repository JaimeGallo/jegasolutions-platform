namespace JEGASolutions.Landing.Application.DTOs;

public class WompiWebhookDto
{
    public string Event { get; set; } = string.Empty;
    public WompiTransactionDataDto Data { get; set; } = new();
    public string Environment { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string SentAt { get; set; } = string.Empty;
}

public class WompiTransactionDataDto
{
    public string Id { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int AmountInCents { get; set; }
    public string Currency { get; set; } = string.Empty;
    public WompiCustomerDataDto Customer { get; set; } = new();
    public WompiPaymentMethodDataDto PaymentMethod { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime FinalizedAt { get; set; }
}

public class WompiCustomerDataDto
{
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}

public class WompiPaymentMethodDataDto
{
    public string Type { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string Installments { get; set; } = string.Empty;
}

public class WompiTransactionResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int AmountInCents { get; set; }
    public string Currency { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime FinalizedAt { get; set; }
}