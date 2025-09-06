using JEGASolutions.Landing.Application.DTOs;

namespace JEGASolutions.Landing.Application.Interfaces;

public interface IWompiService
{
    Task<bool> ValidateWebhookSignature(string payload, string signature);
    Task<WompiTransactionResponseDto?> GetTransactionStatus(string transactionId);
    Task<bool> ProcessPaymentWebhook(WompiWebhookDto payload);
}