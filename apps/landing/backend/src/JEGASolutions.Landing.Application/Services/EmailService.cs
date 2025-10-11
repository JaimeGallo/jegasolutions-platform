using System.Net;
using SendGrid;
using SendGrid.Helpers.Mail;
using JEGASolutions.Landing.Application.Interfaces;
using JEGASolutions.Landing.Domain.Entities;
using JEGASolutions.Landing.Application.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;

namespace JEGASolutions.Landing.Application.Services;

public class EmailService : IEmailService
{
    private readonly SmtpSettings _smtpSettings;
    private readonly ILogger<EmailService> _logger;
    private readonly IConfiguration _configuration;
    private readonly string? _sendGridApiKey;

    public EmailService(
        IOptions<SmtpSettings> smtpSettings,
        ILogger<EmailService> logger,
        IConfiguration configuration)
    {
        _smtpSettings = smtpSettings.Value;
        _logger = logger;
        _configuration = configuration;
        _sendGridApiKey = _configuration["SendGrid__ApiKey"]
            ?? _configuration["SendGrid:ApiKey"];

        if (string.IsNullOrEmpty(_sendGridApiKey))
        {
            _logger.LogWarning("⚠️ SendGrid API Key not configured. Emails will fail.");
        }
        else
        {
            _logger.LogInformation("✅ SendGrid configured successfully");
        }
    }

    /// <summary>
    /// SOBRECARGA 1: Envía email de bienvenida usando Tenant (método original)
    /// </summary>
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

            var subject = "🎉 ¡Bienvenido a JEGASolutions!";
            var htmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{ font-family: 'Segoe UI', Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 30px;
            text-align: center;
            border-radius: 10px 10px 0 0;
        }}
        .content {{
            background: white;
            padding: 30px;
            border: 1px solid #e5e7eb;
            border-top: none;
        }}
        .credentials {{
            background: #f9fafb;
            border-left: 4px solid #667eea;
            padding: 20px;
            margin: 20px 0;
            border-radius: 5px;
        }}
        .button {{
            display: inline-block;
            padding: 12px 30px;
            background: #667eea;
            color: white;
            text-decoration: none;
            border-radius: 5px;
            margin-top: 20px;
        }}
        .footer {{
            background: #1f2937;
            color: #9ca3af;
            padding: 20px;
            text-align: center;
            border-radius: 0 0 10px 10px;
            font-size: 12px;
        }}
        code {{
            background: #e8e8e8;
            padding: 2px 6px;
            border-radius: 3px;
            font-family: monospace;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1 style='margin: 0;'>🎉 ¡Bienvenido a JEGASolutions!</h1>
        </div>
        <div class='content'>
            <h2 style='color: #1f2937;'>¡Hola {user.FirstName}!</h2>
            <p style='font-size: 16px;'>
                Tu cuenta ha sido creada exitosamente. ¡Estamos emocionados de tenerte con nosotros!
            </p>

            <div class='credentials'>
                <h3 style='margin-top: 0; color: #667eea;'>🔑 Tus Credenciales de Acceso</h3>
                <p><strong>URL:</strong> <a href='https://{tenant.Subdomain}.jegasolutions.co/login'>https://{tenant.Subdomain}.jegasolutions.co/login</a></p>
                <p><strong>Usuario:</strong> <code>{user.Email}</code></p>
                <p><strong>Contraseña Temporal:</strong> <code>{temporaryPassword}</code></p>
            </div>

            <p>⚠️ <strong>Importante:</strong> Por seguridad, te recomendamos cambiar tu contraseña después del primer inicio de sesión.</p>

            <center>
                <a href='https://{tenant.Subdomain}.jegasolutions.co/login' class='button'>
                    Acceder Ahora →
                </a>
            </center>

            <p style='margin-top: 30px; color: #6b7280;'>
                Si tienes alguna pregunta, no dudes en contactarnos.<br/>
                ¡Gracias por confiar en nosotros!
            </p>
        </div>
        <div class='footer'>
            <p style='margin: 0 0 10px 0;'>© 2025 JEGASolutions. Todos los derechos reservados.</p>
            <p style='margin: 0;'>📧 soporte@jegasolutions.co | 🌐 www.jegasolutions.co</p>
        </div>
    </div>
</body>
</html>";

            return await SendEmailViaSendGridAsync(user.Email, subject, htmlBody);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send welcome email");
            return false;
        }
    }

    /// <summary>
    /// SOBRECARGA 2: Envía email de bienvenida con HTML personalizado (método nuevo)
    /// </summary>
    public async Task<bool> SendWelcomeEmailAsync(string toEmail, string subject, string htmlBody)
    {
        try
        {
            if (string.IsNullOrEmpty(_sendGridApiKey))
            {
                _logger.LogError("❌ Cannot send email: SendGrid API Key not configured");
                return false;
            }

            var result = await SendEmailViaSendGridAsync(toEmail, subject, htmlBody);

            if (result)
            {
                _logger.LogInformation("✅ Welcome email sent successfully to {Email}", toEmail);
            }
            else
            {
                _logger.LogError("❌ Failed to send welcome email to {Email}", toEmail);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Exception sending welcome email to {Email}", toEmail);
            return false;
        }
    }

    /// <summary>
    /// Envía email de confirmación de pago
    /// </summary>
    public async Task<bool> SendPaymentConfirmationAsync(Payment payment)
    {
        try
        {
            var subject = "✅ Confirmación de Pago - JEGASolutions";
            var htmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{ font-family: 'Segoe UI', Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{
            background: linear-gradient(135deg, #10b981 0%, #059669 100%);
            color: white;
            padding: 30px;
            text-align: center;
            border-radius: 10px 10px 0 0;
        }}
        .content {{
            background: white;
            padding: 30px;
            border: 1px solid #e5e7eb;
            border-top: none;
        }}
        .payment-details {{
            background: #f0fdf4;
            border-left: 4px solid #10b981;
            padding: 20px;
            margin: 20px 0;
            border-radius: 4px;
        }}
        .footer {{
            background: #1f2937;
            color: #9ca3af;
            padding: 20px;
            text-align: center;
            border-radius: 0 0 10px 10px;
            font-size: 12px;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1 style='margin: 0;'>✅ ¡Pago Confirmado!</h1>
        </div>
        <div class='content'>
            <h2 style='color: #1f2937;'>¡Hola {payment.CustomerName}!</h2>
            <p style='font-size: 16px;'>
                Hemos recibido tu pago exitosamente. ¡Gracias por confiar en nosotros!
            </p>

            <div class='payment-details'>
                <h3 style='margin-top: 0; color: #10b981;'>💳 Detalles de la Transacción</h3>
                <p><strong>Monto:</strong> ${payment.Amount:N0} COP</p>
                <p><strong>Referencia:</strong> {payment.Reference}</p>
                <p><strong>Estado:</strong> <span style='color: #10b981;'>✓ APROBADO</span></p>
                <p><strong>Fecha:</strong> {DateTime.UtcNow:dd/MM/yyyy HH:mm}</p>
            </div>

            <p>
                En breve recibirás otro correo con las credenciales de acceso a tu nueva plataforma.
            </p>

            <p style='margin-top: 30px; color: #6b7280;'>
                ¡Gracias por confiar en JEGASolutions!<br/>
                <strong>El Equipo de JEGASolutions</strong>
            </p>
        </div>
        <div class='footer'>
            <p style='margin: 0 0 10px 0;'>© 2025 JEGASolutions. Todos los derechos reservados.</p>
            <p style='margin: 0;'>📧 soporte@jegasolutions.co | 🌐 www.jegasolutions.co</p>
        </div>
    </div>
</body>
</html>";

            return await SendEmailViaSendGridAsync(
                payment.CustomerEmail ?? "",
                subject,
                htmlBody
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send payment confirmation to {Email}", payment.CustomerEmail);
            return false;
        }
    }

    /// <summary>
    /// Método privado central que envía emails usando SendGrid
    /// </summary>
    private async Task<bool> SendEmailViaSendGridAsync(
        string toEmail,
        string subject,
        string htmlContent)
    {
        try
        {
            if (string.IsNullOrEmpty(toEmail))
            {
                _logger.LogWarning("⚠️ Cannot send email: recipient email is empty");
                return false;
            }

            if (string.IsNullOrEmpty(_sendGridApiKey))
            {
                _logger.LogError("❌ Cannot send email: SendGrid API Key not configured");
                return false;
            }

            var client = new SendGridClient(_sendGridApiKey);

            // Usa tu email verificado en SendGrid (Single Sender)
            var fromEmail = _configuration["SendGrid__FromEmail"]
                ?? _configuration["SendGrid:FromEmail"]
                ?? "jaialgallo@gmail.com";

            var from = new EmailAddress(fromEmail, "JEGASolutions");
            var to = new EmailAddress(toEmail);

            var msg = MailHelper.CreateSingleEmail(
                from,
                to,
                subject,
                "", // Plain text vacío (usamos HTML)
                htmlContent
            );

            _logger.LogInformation("📧 Sending email to {Email} with subject '{Subject}'",
                toEmail, subject);

            var response = await client.SendEmailAsync(msg);

            if (response.StatusCode == HttpStatusCode.Accepted ||
                response.StatusCode == HttpStatusCode.OK)
            {
                _logger.LogInformation("✅ Email sent successfully via SendGrid to {Email}", toEmail);
                return true;
            }
            else
            {
                var body = await response.Body.ReadAsStringAsync();
                _logger.LogError("❌ SendGrid error: Status={StatusCode}, Body={Body}",
                    response.StatusCode, body);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Exception sending email via SendGrid to {Email}", toEmail);
            return false;
        }
    }
}
