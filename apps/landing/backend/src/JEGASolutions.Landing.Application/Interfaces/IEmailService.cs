using JEGASolutions.Landing.Domain.Entities;

namespace JEGASolutions.Landing.Application.Interfaces;

public interface IEmailService
{
    Task<bool> SendPaymentConfirmationAsync(Payment payment);
    Task<bool> SendWelcomeEmailAsync(Tenant tenant, string temporaryPassword);
    Task<bool> SendWelcomeEmailAsync(string toEmail, string subject, string htmlBody);
}