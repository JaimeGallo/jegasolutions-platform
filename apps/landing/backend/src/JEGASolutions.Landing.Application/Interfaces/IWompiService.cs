using JEGASolutions.Landing.Application.DTOs;
using JEGASolutions.Landing.Domain.Entities;

namespace JEGASolutions.Landing.Application.Interfaces;

public interface IWompiService
{
    Task<WompiTransactionResponseDto> CreateTransactionAsync(Payment payment);
    Task<bool> ValidateWebhookSignature(string payload, string signature);
    Task<WompiTransactionResponseDto?> GetTransactionStatus(string transactionId);
    Task<bool> ProcessPaymentWebhook(WompiWebhookDto payload);
}