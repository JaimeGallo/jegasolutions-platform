using System.Text.Json.Serialization;

namespace JEGASolutions.Landing.Application.DTOs;

public class WompiApiResponseDto
{
    [JsonPropertyName("data")]
    public WompiTransactionDataDto? Data { get; set; }
}

public class WompiTransactionDataDto
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("reference")]
    public string? Reference { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }
}

public class WompiMerchantResponseDto
{
    [JsonPropertyName("data")]
    public WompiMerchantDataDto? Data { get; set; }
}

public class WompiMerchantDataDto
{
    [JsonPropertyName("presigned_acceptance")]
    public PresignedAcceptanceDto? PresignedAcceptance { get; set; }
}

public class PresignedAcceptanceDto
{
    [JsonPropertyName("acceptance_token")]
    public string AcceptanceToken { get; set; } = "";
}