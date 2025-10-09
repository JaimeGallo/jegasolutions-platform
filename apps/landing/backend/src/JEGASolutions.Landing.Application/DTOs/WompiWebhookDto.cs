using System.Text.Json.Serialization;

namespace JEGASolutions.Landing.Application.DTOs;

/// <summary>
/// DTO para el webhook enviado por Wompi
/// </summary>
public class WompiWebhookDto
{
    [JsonPropertyName("event")]
    public string Event { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public WompiWebhookDataDto Data { get; set; } = new();

    [JsonPropertyName("environment")]
    public string Environment { get; set; } = string.Empty;

    [JsonPropertyName("timestamp")]
    public long? Timestamp { get; set; }

    [JsonPropertyName("sent_at")]
    public string SentAt { get; set; } = string.Empty;
}

/// <summary>
/// Wrapper para el objeto "data" del webhook que contiene "transaction"
/// </summary>
public class WompiWebhookDataDto
{
    [JsonPropertyName("transaction")]
    public WompiWebhookTransactionDto Transaction { get; set; } = new();
}

/// <summary>
/// Datos de la transacción en el webhook (estructura simplificada)
/// </summary>
public class WompiWebhookTransactionDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("reference")]
    public string Reference { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("amount_in_cents")]
    public int AmountInCents { get; set; }

    [JsonPropertyName("currency")]
    public string Currency { get; set; } = "COP";

    [JsonPropertyName("customer_email")]
    public string CustomerEmail { get; set; } = string.Empty;

    [JsonPropertyName("customer_data")]
    public WompiCustomerDataDto? Customer { get; set; }

    [JsonPropertyName("payment_method_type")]
    public string PaymentMethodType { get; set; } = string.Empty;

    [JsonPropertyName("payment_method")]
    public WompiPaymentMethodDataDto? PaymentMethod { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get; set; }

    [JsonPropertyName("finalized_at")]
    public DateTime? FinalizedAt { get; set; }
}

public class WompiCustomerDataDto
{
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("full_name")]
    public string FullName { get; set; } = string.Empty;

    [JsonPropertyName("phone_number")]
    public string PhoneNumber { get; set; } = string.Empty;

    [JsonPropertyName("legal_id")]
    public string? LegalId { get; set; }

    [JsonPropertyName("legal_id_type")]
    public string? LegalIdType { get; set; }
}

public class WompiPaymentMethodDataDto
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("installments")]
    public int? Installments { get; set; }
}

/// <summary>
/// DTO para respuestas de transacciones
/// </summary>
public class WompiTransactionResponseDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("reference")]
    public string Reference { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("amount_in_cents")]
    public int AmountInCents { get; set; }

    [JsonPropertyName("currency")]
    public string Currency { get; set; } = "COP";

    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get; set; }

    [JsonPropertyName("finalized_at")]
    public DateTime? FinalizedAt { get; set; }

    [JsonPropertyName("checkout_url")]
    public string? CheckoutUrl { get; set; }
}
