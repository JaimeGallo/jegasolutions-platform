using System.Text.Json.Serialization;

namespace JEGASolutions.Landing.Application.DTOs;

/// <summary>
/// Respuesta de la API de Wompi al crear una transacción
/// </summary>
public class WompiApiResponseDto
{
    [JsonPropertyName("data")]
    public WompiApiTransactionDto? Data { get; set; }
}

/// <summary>
/// Datos de la transacción devueltos por la API de Wompi
/// </summary>
public class WompiApiTransactionDto
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("reference")]
    public string? Reference { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }
}

/// <summary>
/// Respuesta del endpoint de merchants de Wompi
/// </summary>
public class WompiMerchantResponseDto
{
    [JsonPropertyName("data")]
    public WompiMerchantDataDto? Data { get; set; }
}

/// <summary>
/// Datos del merchant
/// </summary>
public class WompiMerchantDataDto
{
    [JsonPropertyName("presigned_acceptance")]
    public PresignedAcceptanceDto? PresignedAcceptance { get; set; }
}

/// <summary>
/// Token de aceptación pre-firmado
/// </summary>
public class PresignedAcceptanceDto
{
    [JsonPropertyName("acceptance_token")]
    public string AcceptanceToken { get; set; } = "";
}