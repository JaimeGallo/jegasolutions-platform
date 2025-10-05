using System.Text.Json.Serialization;

namespace JEGASolutions.Landing.Application.DTOs;

/// <summary>
/// DTO para el webhook completo enviado por Wompi
/// </summary>
public class WompiWebhookDto
{
    /// <summary>
    /// Tipo de evento: "transaction.updated"
    /// </summary>
    [JsonPropertyName("event")]
    public string Event { get; set; } = string.Empty;

    /// <summary>
    /// Datos de la transacción (contiene el objeto transaction)
    /// </summary>
    [JsonPropertyName("data")]
    public WompiWebhookDataDto Data { get; set; } = new();

    /// <summary>
    /// Ambiente: "test" o "production"
    /// </summary>
    [JsonPropertyName("environment")]
    public string Environment { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp en milisegundos desde epoch
    /// </summary>
    [JsonPropertyName("timestamp")]
    public long? Timestamp { get; set; }

    /// <summary>
    /// Fecha de envío en formato ISO
    /// </summary>
    [JsonPropertyName("sent_at")]
    public string SentAt { get; set; } = string.Empty;

    /// <summary>
    /// Firma del webhook (opcional, usado para validación)
    /// </summary>
    [JsonPropertyName("signature")]
    public WompiSignatureDto? Signature { get; set; }
}

/// <summary>
/// Objeto Data que contiene la transacción
/// </summary>
public class WompiWebhookDataDto
{
    /// <summary>
    /// Objeto transaction completo
    /// </summary>
    [JsonPropertyName("transaction")]
    public WompiTransactionDataDto Transaction { get; set; } = new();
}

/// <summary>
/// Datos completos de la transacción de Wompi
/// </summary>
public class WompiTransactionDataDto
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

    [JsonPropertyName("redirect_url")]
    public string? RedirectUrl { get; set; }

    [JsonPropertyName("shipping_address")]
    public object? ShippingAddress { get; set; }

    [JsonPropertyName("payment_link_id")]
    public string? PaymentLinkId { get; set; }

    [JsonPropertyName("payment_source_id")]
    public int? PaymentSourceId { get; set; }

    [JsonPropertyName("taxes")]
    public List<object>? Taxes { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get; set; }

    [JsonPropertyName("finalized_at")]
    public DateTime? FinalizedAt { get; set; }
}

/// <summary>
/// Datos del cliente
/// </summary>
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

/// <summary>
/// Datos del método de pago
/// </summary>
public class WompiPaymentMethodDataDto
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("extra")]
    public WompiPaymentMethodExtraDto? Extra { get; set; }

    [JsonPropertyName("installments")]
    public int? Installments { get; set; }
}

/// <summary>
/// Información adicional del método de pago
/// </summary>
public class WompiPaymentMethodExtraDto
{
    // Para tarjetas
    [JsonPropertyName("bin")]
    public string? Bin { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("brand")]
    public string? Brand { get; set; }

    [JsonPropertyName("exp_year")]
    public string? ExpYear { get; set; }

    [JsonPropertyName("exp_month")]
    public string? ExpMonth { get; set; }

    [JsonPropertyName("last_four")]
    public string? LastFour { get; set; }

    [JsonPropertyName("card_holder")]
    public string? CardHolder { get; set; }

    [JsonPropertyName("is_three_ds")]
    public bool? IsThreeDs { get; set; }

    // Para PSE
    [JsonPropertyName("async_payment_url")]
    public string? AsyncPaymentUrl { get; set; }

    [JsonPropertyName("bank_code")]
    public string? BankCode { get; set; }

    [JsonPropertyName("ticket_id")]
    public string? TicketId { get; set; }

    // Para Nequi
    [JsonPropertyName("phone_number")]
    public string? PhoneNumber { get; set; }
}

/// <summary>
/// Firma del webhook para validación
/// </summary>
public class WompiSignatureDto
{
    [JsonPropertyName("properties")]
    public List<string> Properties { get; set; } = new();

    [JsonPropertyName("checksum")]
    public string Checksum { get; set; } = string.Empty;
}

/// <summary>
/// DTO para respuestas de transacciones (usado en CreateTransaction y GetStatus)
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

    /// <summary>
    /// URL del checkout (solo para respuestas de CreateTransaction)
    /// </summary>
    [JsonPropertyName("checkout_url")]
    public string? CheckoutUrl { get; set; }
}