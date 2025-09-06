using System.Net;
using System.Net.Mail;
using JEGASolutions.Landing.Application.Interfaces;
using JEGASolutions.Landing.Domain.Entities;
using JEGASolutions.Landing.Application.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JEGASolutions.Landing.Application.Services;

public class EmailService : IEmailService
{
    private readonly SmtpSettings _smtpSettings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<SmtpSettings> smtpSettings, ILogger<EmailService> logger)
    {
        _smtpSettings = smtpSettings.Value;
        _logger = logger;
    }

    public async Task<bool> SendWelcomeEmailAsync(Tenant tenant, string temporaryPassword)
    {
        try
        {
            var user = tenant.Users.FirstOrDefault();
            if (user == null)
            {
                _logger.LogWarning("No user found for tenant {TenantId} to send welcome email.", tenant.Id);
                return false;
            }

            var subject = "¡Bienvenido a JEGASolutions!";
            var body = $@"
                <h1>¡Hola {user.FirstName}!</h1>
                <p>Tu cuenta ha sido creada exitosamente.</p>
                <p>Puedes acceder a tu panel de control en: <a href='https://{tenant.Subdomain}.jegasolutions.co'>https://{tenant.Subdomain}.jegasolutions.co</a></p>
                <p>Tu nombre de usuario es: <strong>{user.Email}</strong></p>
                <p>Tu contraseña temporal es: <strong>{temporaryPassword}</strong></p>
                <p>Te recomendamos cambiar tu contraseña después de iniciar sesión por primera vez.</p>
                <p>¡Gracias por unirte a nosotros!</p>";

            await SendEmailAsync(user.Email, subject, body);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send welcome email to {Email}", tenant.Users.FirstOrDefault()?.Email);
            return false;
        }
    }

    public async Task<bool> SendPaymentConfirmationAsync(Payment payment)
    {
        try
        {
            var subject = "Confirmación de Pago - JEGASolutions";
            var body = $@"
                <h1>¡Gracias por tu pago!</h1>
                <p>Hola {payment.CustomerName},</p>
                <p>Hemos recibido tu pago de ${payment.Amount:N2} COP.</p>
                <p>Referencia de la transacción: {payment.Reference}</p>
                <p>En breve, recibirás otro correo con los detalles de acceso a tu nuevo servicio.</p>
                <p>Gracias por confiar en JEGASolutions.</p>";

            await SendEmailAsync(payment.CustomerEmail ?? "", subject, body);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send payment confirmation to {Email}", payment.CustomerEmail);
            return false;
        }
    }

    private async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        try
        {
            using var client = new SmtpClient(_smtpSettings.Server, _smtpSettings.Port)
            {
                Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                EnableSsl = true
            };

            using var mailMessage = new MailMessage(_smtpSettings.From, toEmail, subject, body) { IsBodyHtml = true };

            await client.SendMailAsync(mailMessage);
            _logger.LogInformation("Email sent to {ToEmail} with subject '{Subject}'", toEmail, subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email to {ToEmail} with subject '{Subject}'", toEmail, subject);
            throw; // Re-throw to allow the calling method to handle it
        }
    }
}
