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
    
    _logger.LogInformation("Wompi Events Secret configured: {Configured}", 
        !string.IsNullOrEmpty(_eventsSecret) ? "Yes" : "No");

        var baseUrl = _configuration["Wompi__BaseUrl"] ?? _configuration["Wompi:BaseUrl"] 
            ?? "https://production.wompi.co/v1/";
        _httpClient.BaseAddress = new Uri(baseUrl);

        var environment = baseUrl.Contains("sandbox") ? "SANDBOX" : "PRODUCTION";
        _logger.LogInformation("Wompi configured with Environment: {Environment}, BaseUrl: {BaseUrl}", 
            environment, baseUrl);
    }

 public async Task<WompiTransactionResponseDto> CreateTransactionAsync(Payment payment)
{
    _logger.LogInformation("Creating Wompi checkout for reference {Reference}", payment.Reference);

    var redirectUrl = _configuration["Wompi:RedirectUrl"] 
        ?? "https://jegasolutions-platform-frontend-95l.vercel.app/payment-success";

    var amountInCents = (int)(payment.Amount * 100);
    var currency = "COP";
    var reference = payment.Reference;

    // URL directa del widget (permite TODOS los métodos de pago)
    var checkoutUrl = "https://checkout.wompi.co/p/" +
        $"?public-key={_publicKey}" +
        $"&currency={currency}" +
        $"&amount-in-cents={amountInCents}" +
        $"&reference={reference}" +
        $"&redirect-url={Uri.EscapeDataString(redirectUrl)}";

    _logger.LogInformation("Checkout URL created: {CheckoutUrl}", checkoutUrl);

    return new WompiTransactionResponseDto
    {
        Id = $"WMP_{DateTime.Now:yyyyMMdd}_{payment.Id}",
        Reference = reference,
        CheckoutUrl = checkoutUrl,
        Status = "PENDING"
    };
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
        // Usar estructura plana - payload.Data directamente
        var transaction = payload.Data;
        
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
                _logger.LogInformation("User already exists for email {Email}", payment.CustomerEmail);
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