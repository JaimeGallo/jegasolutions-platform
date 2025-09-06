using JEGASolutions.Landing.Application.DTOs;

namespace JEGASolutions.Landing.Application.Interfaces;

public interface IPaymentService
{
    Task<PaymentResponseDto> CreatePaymentAsync(PaymentRequestDto request);
    Task<PaymentResponseDto?> GetPaymentByReferenceAsync(string reference);
    Task<bool> UpdatePaymentStatusAsync(string reference, string status);
    Task<IEnumerable<PaymentResponseDto>> GetPaymentsByCustomerAsync(string customerEmail);
}