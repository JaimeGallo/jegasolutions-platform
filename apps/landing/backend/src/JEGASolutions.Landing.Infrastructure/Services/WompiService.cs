using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using JEGASolutions.Landing.Application.DTOs;
using JEGASolutions.Landing.Application.Interfaces;
using JEGASolutions.Landing.Domain.Entities;
using JEGASolutions.Landing.Domain.Interfaces;
using JEGASolutions.Landing.Infrastructure.Utils;

namespace JEGASolutions.Landing.Infrastructure.Services;

public class WompiService : IWompiService
{
    private readonly HttpClient _httpClient;
    private readonly IRepository<Payment> _paymentRepository;
    private readonly IRepository<Tenant> _tenantRepository;
    private readonly IRepository<TenantModule> _tenantModuleRepository;
    private readonly IRepository<User> _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;
    private readonly ILogger<WompiService> _logger;
    private readonly IPasswordGenerator _passwordGenerator;
    private readonly string _privateKey;
    private readonly string _publicKey;
    private readonly string _eventsSecret;
    private readonly string _integritySecret;

    public WompiService(
        HttpClient httpClient,
        IRepository<Payment> paymentRepository,
        IRepository<Tenant> tenantRepository,
        IRepository<TenantModule> tenantModuleRepository,
        IRepository<User> userRepository,
        IUnitOfWork unitOfWork,
        IConfiguration configuration,
        IEmailService emailService,
        ILogger<WompiService> logger,
        IPasswordGenerator passwordGenerator)
    {
        _httpClient = httpClient;
        _paymentRepository = paymentRepository;
        _tenantRepository = tenantRepository;
        _tenantModuleRepository = tenantModuleRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _configuration = configuration;
        _emailService = emailService;
        _logger = logger;
        _passwordGenerator = passwordGenerator;

        _privateKey = _configuration["Wompi__PrivateKey"] ?? _configuration["Wompi:PrivateKey"]
            ?? throw new ArgumentNullException("Wompi__PrivateKey", "Wompi private key is required.");

        _publicKey = _configuration["Wompi__PublicKey"] ?? _configuration["Wompi:PublicKey"]
            ?? throw new ArgumentNullException("Wompi__PublicKey", "Wompi public key is required.");

            _eventsSecret = _configuration["Wompi__EventsSecret"] ?? _configuration["Wompi:EventsSecret"]
        ?? _privateKey; // Fallback al private key si no existe

        _integritySecret = _configuration["Wompi__IntegritySecret"] ?? _configuration["Wompi:IntegritySecret"]
            ?? throw new ArgumentNullException("Wompi__IntegritySecret", "Wompi integrity secret is required.");

    _logger.LogInformation("Wompi Events Secret configured: {Configured}",
        !string.IsNullOrEmpty(_eventsSecret) ? "Yes" : "No");

        var baseUrl = _configuration["Wompi__BaseUrl"] ?? _configuration["Wompi:BaseUrl"]
            ?? "https://production.wompi.co/v1/";
        _httpClient.BaseAddress = new Uri(baseUrl);

        var environment = baseUrl.Contains("sandbox") ? "SANDBOX" : "PRODUCTION";
        _logger.LogInformation("Wompi configured with Environment: {Environment}, BaseUrl: {BaseUrl}",
            environment, baseUrl);
    }

    private string GenerateIntegritySignature(string reference, int amountInCents, string currency, string? expirationTime = null)
    {
        // Concatenar según la documentación de Wompi
        string concatenatedString;

        if (!string.IsNullOrEmpty(expirationTime))
        {
            // Con expiration-time: referencia + monto + moneda + fecha + secreto
            concatenatedString = $"{reference}{amountInCents}{currency}{expirationTime}{_integritySecret}";
        }
        else
        {
            // Sin expiration-time: referencia + monto + moneda + secreto
            concatenatedString = $"{reference}{amountInCents}{currency}{_integritySecret}";
        }

        _logger.LogInformation("Generating signature for concatenated string (length: {Length})",
            concatenatedString.Length);

        // Generar hash SHA256
        return ComputeSHA256(concatenatedString);
    }

 public Task<WompiTransactionResponseDto> CreateTransactionAsync(Payment payment)
{
    _logger.LogInformation("Creating Wompi checkout for reference {Reference}", payment.Reference);

    var redirectUrl = _configuration["Wompi:RedirectUrl"]
        ?? "https://jegasolutions-platform-frontend-95l.vercel.app/payment-success";

    var amountInCents = (int)(payment.Amount * 100);
    var currency = "COP";
    var reference = payment.Reference;

     var signature = GenerateIntegritySignature(reference, amountInCents, currency);

        _logger.LogInformation("Generated integrity signature: {Signature}", signature);

    // URL directa del widget (permite TODOS los métodos de pago)
    var checkoutUrl = "https://checkout.wompi.co/p/" +
            $"?public-key={_publicKey}" +
            $"&currency={currency}" +
            $"&amount-in-cents={amountInCents}" +
            $"&reference={reference}" +
            $"&signature:integrity={signature}" + // ⚠️ CRÍTICO: Agregar la firma
            $"&redirect-url={Uri.EscapeDataString(redirectUrl)}";

        _logger.LogInformation("Checkout URL created: {CheckoutUrl}", checkoutUrl);

    return Task.FromResult(new WompiTransactionResponseDto
    {
        Id = $"WMP_{DateTime.Now:yyyyMMdd}_{payment.Id}",
        Reference = reference,
        CheckoutUrl = checkoutUrl,
        Status = "PENDING"
    });
}

   public Task<bool> ValidateWebhookSignature(string payload, string signature)
{
    // Usar Events Secret en lugar de Private Key
    var expectedSignature = ComputeHMAC(payload, _eventsSecret);
    var isValid = signature.Equals(expectedSignature, StringComparison.OrdinalIgnoreCase);

    if (!isValid)
    {
        _logger.LogWarning("Invalid webhook signature. Expected: {Expected}, Received: {Received}",
            expectedSignature, signature);
    }

    return Task.FromResult(isValid);
}

    public async Task<WompiTransactionResponseDto?> GetTransactionStatus(string transactionId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"transactions/{transactionId}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<WompiApiResponse<WompiTransactionResponseDto>>(
                    content,
                    JsonUtils.GetJsonSerializerOptions()
                );
                return result?.Data;
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting transaction status for {TransactionId}", transactionId);
            return null;
        }
    }

    public async Task<bool> ProcessPaymentWebhook(WompiWebhookDto payload)
{
    try
    {
        // Acceder a payload.Data.Transaction
        var transaction = payload.Data.Transaction;

        _logger.LogInformation("Processing webhook for reference {Reference}, status {Status}",
            transaction.Reference, transaction.Status);

        var payment = await _paymentRepository.FirstOrDefaultAsync(
            p => p.Reference == transaction.Reference
        );

        if (payment == null)
        {
            payment = new Payment
            {
                Reference = transaction.Reference,
                Amount = transaction.AmountInCents / 100m,
                CustomerEmail = transaction.CustomerEmail,
                CustomerName = transaction.Customer?.FullName ?? "Cliente",
                CustomerPhone = transaction.Customer?.PhoneNumber,
                WompiTransactionId = transaction.Id,
                Status = MapWompiStatus(transaction.Status),
                CreatedAt = transaction.CreatedAt ?? DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _paymentRepository.AddAsync(payment);
            _logger.LogInformation("Created new payment record for reference {Reference}",
                payment.Reference);
        }
        else
        {
            payment.Status = MapWompiStatus(transaction.Status);
            payment.WompiTransactionId = transaction.Id;
            payment.UpdatedAt = DateTime.UtcNow;

            if (transaction.Customer != null)
            {
                payment.CustomerEmail = transaction.CustomerEmail;
                payment.CustomerName = transaction.Customer.FullName;
                payment.CustomerPhone = transaction.Customer.PhoneNumber;
            }

            await _paymentRepository.UpdateAsync(payment);
            _logger.LogInformation("Updated payment record for reference {Reference}",
                payment.Reference);
        }

        await _unitOfWork.SaveChangesAsync();

        if (transaction.Status.ToUpper() == "APPROVED")
        {
            _logger.LogInformation("Payment approved, creating tenant for reference {Reference}",
                transaction.Reference);
            await CreateTenantFromPayment(payment);
        }

        return true;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error processing webhook");
        return false;
    }
}

    private async Task<string> GetAcceptanceTokenAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("merchants/" + _publicKey);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var merchantData = JsonSerializer.Deserialize<WompiMerchantResponseDto>(
                    content,
                    JsonUtils.GetJsonSerializerOptions()
                );

                return merchantData?.Data?.PresignedAcceptance?.AcceptanceToken ?? "";
            }

            _logger.LogWarning("Could not retrieve acceptance token, using empty string");
            return "";
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error getting acceptance token");
            return "";
        }
    }

    private async Task CreateTenantFromPayment(Payment payment)
    {
        try
        {
            // Verificar si ya existe un usuario con este email
            var existingUser = await _userRepository.FirstOrDefaultAsync(
                u => u.Email == payment.CustomerEmail
            );

            if (existingUser != null)
            {
                _logger.LogInformation("User already exists for email {Email}. Adding module to existing tenant.", payment.CustomerEmail);

                // Buscar tenant del usuario existente
                var existingTenant = await _tenantRepository.FirstOrDefaultAsync(
                    t => t.Id == existingUser.TenantId
                );

                if (existingTenant != null)
                {
                    // Determinar el nombre del módulo
                    var moduleName = ExtractModuleNameFromReference(payment.Reference);

                    // Verificar si el módulo ya existe para este tenant
                    var existingModule = await _tenantModuleRepository.FirstOrDefaultAsync(
                        tm => tm.TenantId == existingTenant.Id && tm.ModuleName == moduleName
                    );

                    if (existingModule == null)
                    {
                        // Agregar el nuevo módulo
                        var tenantModule = new TenantModule
                        {
                            TenantId = existingTenant.Id,
                            ModuleName = moduleName,
                            Status = "ACTIVE",
                            PurchasedAt = DateTime.UtcNow
                        };

                        await _tenantModuleRepository.AddAsync(tenantModule);
                        await _unitOfWork.SaveChangesAsync();

                        _logger.LogInformation("Added module {ModuleName} to existing tenant {TenantId}",
                            moduleName, existingTenant.Id);

                        // Enviar email de confirmación de compra de módulo adicional
                        await SendModulePurchaseEmailAsync(payment, existingTenant, moduleName);
                    }
                    else
                    {
                        _logger.LogInformation("Module {ModuleName} already exists for tenant {TenantId}",
                            moduleName, existingTenant.Id);
                    }
                }

                return;
            }

            // Generar subdomain usando el servicio
            var baseName = payment.CustomerName ?? payment.CustomerEmail ?? "cliente";
            var subdomain = _passwordGenerator.GenerateSubdomain(baseName, 20);

            // Generar contraseña temporal
            var temporaryPassword = _passwordGenerator.GenerateSecurePassword(12);

            // Crear el tenant
            var tenant = new Tenant
            {
                CompanyName = payment.CustomerName ?? "Empresa",
                Subdomain = subdomain,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _tenantRepository.AddAsync(tenant);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Created tenant {TenantId} with subdomain {Subdomain}",
                tenant.Id, subdomain);

            // Determinar el nombre del módulo según la referencia
            var moduleName = ExtractModuleNameFromReference(payment.Reference);

            // Determinar URL del módulo (NO usar subdomain del tenant)
        string moduleUrl;
        switch (moduleName.ToLower())
        {
            case "extra hours":
                moduleUrl = "https://extrahours.jegasolutions.co";
                break;
            case "report builder":
                moduleUrl = "https://reportbuilder.jegasolutions.co"; // Cuando esté desplegado
                break;
            default:
                moduleUrl = "https://extrahours.jegasolutions.co";
                break;
        }

            // Crear el módulo para el tenant
            var tenantModule = new TenantModule
            {
                TenantId = tenant.Id,
                ModuleName = moduleName,
                Status = "ACTIVE",
                PurchasedAt = DateTime.UtcNow
            };

            await _tenantModuleRepository.AddAsync(tenantModule);

            // Crear usuario admin
            var nameParts = (payment.CustomerName ?? "")
                .Trim()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var firstName = nameParts.FirstOrDefault() ?? "Admin";
            var lastName = nameParts.Length > 1 ? string.Join(" ", nameParts.Skip(1)) : "";

            var adminUser = new User
            {
                TenantId = tenant.Id,
                Email = payment.CustomerEmail ?? "",
                FirstName = firstName,
                LastName = lastName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(temporaryPassword),
                Role = "Admin",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(adminUser);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Created admin user {UserId} for tenant {TenantId}",
                adminUser.Id, tenant.Id);

            /*
            // Enviar email de bienvenida
            try
            {
                await _emailService.SendWelcomeEmailAsync(tenant, temporaryPassword);
                _logger.LogInformation("Welcome email sent to {Email}", payment.CustomerEmail);
            }
            catch (Exception emailEx)
            {
                _logger.LogWarning(emailEx, "Failed to send welcome email to {Email}",
                    payment.CustomerEmail);
            }

            _logger.LogInformation("Tenant setup completed for {Subdomain}.jegasolutions.co", subdomain);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tenant for payment {Reference}", payment.Reference);
            throw;
        }
    }
    */

     // ============================================
        // NUEVO EMAIL DE BIENVENIDA (Con URL correcta)
        // ============================================
        try
        {
            // Construir email HTML profesional
            var emailSubject = "🎉 ¡Bienvenido a JEGASolutions!";
            var emailBody = $@"
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
        .credentials-box {{
            background: #f9fafb;
            border-left: 4px solid #667eea;
            padding: 20px;
            margin: 20px 0;
            border-radius: 4px;
        }}
        .credential-item {{ margin: 15px 0; }}
        .credential-label {{
            font-weight: bold;
            color: #4b5563;
            display: block;
            margin-bottom: 5px;
        }}
        .credential-value {{
            background: white;
            padding: 10px;
            border-radius: 4px;
            border: 1px solid #d1d5db;
            font-family: 'Courier New', monospace;
            color: #1f2937;
        }}
        .button {{
            display: inline-block;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white !important;
            padding: 15px 40px;
            text-decoration: none;
            border-radius: 6px;
            font-weight: bold;
            margin: 20px 0;
        }}
        .warning-box {{
            background: #fef2f2;
            border-left: 4px solid #ef4444;
            padding: 15px;
            margin: 20px 0;
            border-radius: 4px;
        }}
        .info-box {{
            background: #eff6ff;
            border-left: 4px solid #3b82f6;
            padding: 15px;
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
        .highlight {{
            color: #667eea;
            font-weight: bold;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1 style='margin: 0;'>¡Bienvenido a JEGASolutions! 🚀</h1>
            <p style='margin: 10px 0 0 0; opacity: 0.9;'>Tu módulo {moduleName} está listo</p>
        </div>

        <div class='content'>
            <h2 style='color: #1f2937;'>Hola {firstName},</h2>

            <p style='font-size: 16px;'>
                ¡Gracias por confiar en nosotros! Tu cuenta ha sido creada exitosamente y ya puedes
                empezar a usar <strong class='highlight'>{moduleName}</strong>.
            </p>

            <div class='info-box'>
                <strong>📋 Información de tu Empresa</strong>
                <p style='margin: 10px 0 0 0;'>
                    <strong>Empresa:</strong> {tenant.CompanyName}<br/>
                    <strong>Módulo Adquirido:</strong> {moduleName}<br/>
                    <strong>Fecha de Activación:</strong> {DateTime.UtcNow:dd/MM/yyyy HH:mm}
                </p>
            </div>

            <div class='credentials-box'>
                <h3 style='margin-top: 0; color: #667eea;'>🔑 Tus Credenciales de Acceso</h3>

                <div class='credential-item'>
                    <span class='credential-label'>URL de Acceso:</span>
                    <div class='credential-value'>{moduleUrl}</div>
                </div>

                <div class='credential-item'>
                    <span class='credential-label'>Usuario (Email):</span>
                    <div class='credential-value'>{payment.CustomerEmail}</div>
                </div>

                <div class='credential-item'>
                    <span class='credential-label'>Contraseña Temporal:</span>
                    <div class='credential-value'>{temporaryPassword}</div>
                </div>
            </div>

            <div class='warning-box'>
                <strong>⚠️ Importante - Seguridad</strong>
                <p style='margin: 10px 0 0 0;'>
                    Por tu seguridad, te recomendamos <strong>cambiar tu contraseña</strong>
                    después de iniciar sesión por primera vez.
                </p>
            </div>

            <div style='text-align: center; margin: 30px 0;'>
                <a href='{moduleUrl}' class='button'>
                    Acceder Ahora →
                </a>
            </div>

            <div class='info-box'>
                <strong>💡 Próximos Pasos</strong>
                <ol style='margin: 10px 0 0 0; padding-left: 20px;'>
                    <li>Haz click en el botón 'Acceder Ahora'</li>
                    <li>Inicia sesión con tus credenciales</li>
                    <li>Cambia tu contraseña temporal</li>
                    <li>Configura tu perfil y empresa</li>
                    <li>¡Empieza a usar el sistema!</li>
                </ol>
            </div>

            <p style='margin-top: 30px;'>
                Si tienes alguna pregunta o necesitas ayuda, no dudes en contactarnos.
                Estamos aquí para ayudarte a sacar el máximo provecho de tu inversión.
            </p>

            <p style='color: #6b7280;'>
                ¡Gracias por elegirnos!<br/>
                <strong>El Equipo de JEGASolutions</strong>
            </p>
        </div>

        <div class='footer'>
            <p style='margin: 0 0 10px 0;'>
                © 2025 JEGASolutions. Todos los derechos reservados.
            </p>
            <p style='margin: 0;'>
                📧 soporte@jegasolutions.co | 🌐 www.jegasolutions.co
            </p>
        </div>
    </div>
</body>
</html>";

            await _emailService.SendWelcomeEmailAsync(
                payment.CustomerEmail ?? "",
                emailSubject,
                emailBody
            );

            _logger.LogInformation("Welcome email sent to {Email} with module URL {ModuleUrl}",
                payment.CustomerEmail, moduleUrl);
        }
        catch (Exception emailEx)
        {
            _logger.LogWarning(emailEx, "Failed to send welcome email to {Email}",
                payment.CustomerEmail);
        }

        _logger.LogInformation(
            "Tenant setup completed for {CompanyName}. Module: {ModuleName}, URL: {ModuleUrl}",
            tenant.CompanyName, moduleName, moduleUrl);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error creating tenant for payment {Reference}", payment.Reference);
        throw;
    }
}

    private async Task SendModulePurchaseEmailAsync(Payment payment, Tenant tenant, string moduleName)
    {
        try
        {
            // Determinar URL del módulo
            string moduleUrl = moduleName.ToLower() switch
            {
                "extra hours" => "https://extrahours.jegasolutions.co",
                "report builder" => "https://reportbuilder.jegasolutions.co",
                _ => "https://extrahours.jegasolutions.co"
            };

            var emailSubject = $"✅ Módulo {moduleName} Activado - JEGASolutions";
            var emailBody = $@"
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
            border-radius: 0 0 10px 10px;
            box-shadow: 0 4px 6px rgba(0,0,0,0.1);
        }}
        .button {{
            display: inline-block;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 15px 30px;
            text-decoration: none;
            border-radius: 5px;
            margin: 20px 0;
            font-weight: bold;
        }}
        .info-box {{
            background: #f8f9fa;
            padding: 20px;
            border-left: 4px solid #667eea;
            margin: 20px 0;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1 style='margin: 0;'>¡Nuevo Módulo Activado!</h1>
        </div>

        <div class='content'>
            <h2>¡Hola {payment.CustomerName}!</h2>

            <p>Tu pago ha sido procesado exitosamente y el módulo <strong>{moduleName}</strong> ha sido agregado a tu cuenta.</p>

            <div class='info-box'>
                <p style='margin: 5px 0;'><strong>📦 Módulo:</strong> {moduleName}</p>
                <p style='margin: 5px 0;'><strong>🏢 Empresa:</strong> {tenant.CompanyName}</p>
                <p style='margin: 5px 0;'><strong>💰 Monto:</strong> ${payment.Amount:N0} COP</p>
                <p style='margin: 5px 0;'><strong>📅 Fecha:</strong> {DateTime.Now:dd/MM/yyyy HH:mm}</p>
            </div>

            <p>Puedes acceder al módulo ahora mismo haciendo clic en el siguiente botón:</p>

            <center>
                <a href='{moduleUrl}' class='button'>
                    🚀 Acceder a {moduleName}
                </a>
            </center>

            <p style='color: #666; font-size: 14px; margin-top: 30px;'>
                Si tienes alguna pregunta o necesitas ayuda, no dudes en contactarnos.
            </p>

            <hr style='border: none; border-top: 1px solid #eee; margin: 30px 0;'>

            <p style='text-align: center; color: #999; font-size: 12px;'>
                © 2025 JEGASolutions. Todos los derechos reservados.<br>
                📧 soporte@jegasolutions.co | 🌐 www.jegasolutions.co
            </p>
        </div>
    </div>
</body>
</html>";

            await _emailService.SendWelcomeEmailAsync(
                payment.CustomerEmail ?? "",
                emailSubject,
                emailBody
            );

            _logger.LogInformation("Module purchase email sent to {Email} for module {ModuleName}",
                payment.CustomerEmail, moduleName);
        }
        catch (Exception emailEx)
        {
            _logger.LogWarning(emailEx, "Failed to send module purchase email to {Email}",
                payment.CustomerEmail);
        }
    }

    private string ExtractModuleNameFromReference(string reference)
    {
        if (reference.Contains("EXTRAHOURS", StringComparison.OrdinalIgnoreCase))
            return "Extra Hours";
        if (reference.Contains("REPORTBUILDER", StringComparison.OrdinalIgnoreCase))
            return "Report Builder";

        return "Extra Hours"; // Default
    }

    private string MapWompiStatus(string wompiStatus)
    {
        return wompiStatus.ToUpper() switch
        {
            "APPROVED" => "APPROVED",
            "DECLINED" => "DECLINED",
            "VOIDED" => "CANCELLED",
            "PENDING" => "PENDING",
            _ => "FAILED"
        };
    }

    private string ComputeSHA256(string data)
    {
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
        return Convert.ToHexString(hash).ToLower();
    }

    private string ComputeHMAC(string data, string key)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        return Convert.ToHexString(hash).ToLower();
    }
}

public class WompiApiResponse<T>
{
    public T? Data { get; set; }
    public bool Success { get; set; }
    public string? Error { get; set; }
}
