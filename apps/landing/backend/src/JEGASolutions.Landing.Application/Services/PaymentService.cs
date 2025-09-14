using JEGASolutions.Landing.Application.DTOs;
using JEGASolutions.Landing.Application.Interfaces;
using JEGASolutions.Landing.Domain.Entities;
using JEGASolutions.Landing.Domain.Interfaces;

namespace JEGASolutions.Landing.Application.Services;

public class PaymentService : IPaymentService
{
    private readonly IRepository<Payment> _paymentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWompiService _wompiService;

    public PaymentService(IRepository<Payment> paymentRepository, IUnitOfWork unitOfWork, IWompiService wompiService)
    {
        _paymentRepository = paymentRepository;
        _unitOfWork = unitOfWork;
        _wompiService = wompiService;
    }

    public async Task<PaymentResponseDto> CreatePaymentAsync(PaymentRequestDto request)
    {
        // 1. Crear la entidad de pago en estado PENDING
        // Usamos una referencia única para la transacción interna y con Wompi.
        var reference = $"JEGA-{Guid.NewGuid().ToString().Substring(0, 8)}";
        var payment = new Payment()
        {
            Reference = request.Reference,
            Amount = request.Amount,
            CustomerEmail = request.CustomerEmail,
            CustomerName = request.CustomerName,
            CustomerPhone = request.CustomerPhone,
            Status = "PENDING",
            CreatedAt = DateTime.UtcNow,
            Metadata = request.Metadata
        };

        await _paymentRepository.AddAsync(payment);
        await _unitOfWork.SaveChangesAsync();

        // 2. Crear la transacción en Wompi
        var wompiTransaction = await _wompiService.CreateTransactionAsync(payment);

        // 3. Actualizar nuestra entidad de pago con el ID de Wompi
        payment.WompiTransactionId = wompiTransaction.Id;
        await _paymentRepository.UpdateAsync(payment);
        await _unitOfWork.SaveChangesAsync();

        // 4. Devolver el DTO con la URL de checkout para el frontend
        var responseDto = MapToDto(payment);
        responseDto.CheckoutUrl = wompiTransaction.CheckoutUrl;

        return responseDto;
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
            CustomerPhone = payment.CustomerPhone,
            WompiTransactionId = payment.WompiTransactionId,
            // CheckoutUrl no se mapea aquí porque solo es relevante al crear el pago.
            CreatedAt = payment.CreatedAt,
            UpdatedAt = payment.UpdatedAt,
            Metadata = payment.Metadata
        };
    }
}