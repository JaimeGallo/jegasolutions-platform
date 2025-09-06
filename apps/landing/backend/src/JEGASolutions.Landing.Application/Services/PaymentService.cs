using JEGASolutions.Landing.Application.DTOs;
using JEGASolutions.Landing.Application.Interfaces;
using JEGASolutions.Landing.Domain.Entities;
using JEGASolutions.Landing.Domain.Interfaces;

namespace JEGASolutions.Landing.Application.Services;

public class PaymentService : IPaymentService
{
    private readonly IRepository<Payment> _paymentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PaymentService(IRepository<Payment> paymentRepository, IUnitOfWork unitOfWork)
    {
        _paymentRepository = paymentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PaymentResponseDto> CreatePaymentAsync(PaymentRequestDto request)
    {
        var payment = new Payment
        {
            Reference = request.Reference,
            Amount = request.Amount,
            CustomerEmail = request.CustomerEmail,
            CustomerName = request.CustomerName,
            Status = "PENDING",
            CreatedAt = DateTime.UtcNow,
            Metadata = request.Metadata
        };

        await _paymentRepository.AddAsync(payment);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(payment);
    }

    public async Task<PaymentResponseDto?> GetPaymentByReferenceAsync(string reference)
    {
        var payment = await _paymentRepository.FirstOrDefaultAsync(p => p.Reference == reference);
        return payment != null ? MapToDto(payment) : null;
    }

    public async Task<bool> UpdatePaymentStatusAsync(string reference, string status)
    {
        var payment = await _paymentRepository.FirstOrDefaultAsync(p => p.Reference == reference);

        if (payment == null)
            return false;

        payment.Status = status;
        payment.UpdatedAt = DateTime.UtcNow;

        await _paymentRepository.UpdateAsync(payment);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<PaymentResponseDto>> GetPaymentsByCustomerAsync(string customerEmail)
    {
        var payments = await _paymentRepository.FindAsync(p => p.CustomerEmail == customerEmail);
        return payments.Select(MapToDto).OrderByDescending(p => p.CreatedAt);
    }

    private static PaymentResponseDto MapToDto(Payment payment)
    {
        return new PaymentResponseDto
        {
            Id = payment.Id,
            Reference = payment.Reference,
            Status = payment.Status,
            Amount = payment.Amount,
            CustomerEmail = payment.CustomerEmail,
            CustomerName = payment.CustomerName,
            WompiTransactionId = payment.WompiTransactionId,
            CreatedAt = payment.CreatedAt,
            UpdatedAt = payment.UpdatedAt,
            Metadata = payment.Metadata
        };
    }
}