using System.Security.Cryptography;
using System.Text;
using System.Net.Http.Headers;
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

        // ===========================================
        // CONFIGURACIÓN WOMPI PRODUCCIÓN
        // ===========================================
        _privateKey = _configuration["Wompi__PrivateKey"] ?? _configuration["Wompi:PrivateKey"] 
            ?? throw new ArgumentNullException("Wompi__PrivateKey", "Wompi private key is required. Configure in appsettings.json or environment variables.");
        
        _publicKey = _configuration["Wompi__PublicKey"] ?? _configuration["Wompi:PublicKey"] 
            ?? throw new ArgumentNullException("Wompi__PublicKey", "Wompi public key is required. Configure in appsettings.json or environment variables.");

        var baseUrl = _configuration["Wompi__BaseUrl"] ?? _configuration["Wompi:BaseUrl"] ?? "https://production.wompi.co/v1/";
        _httpClient.BaseAddress = new Uri(baseUrl);

        var environment = baseUrl.Contains("sandbox") ? "SANDBOX" : "PRODUCTION";
        _logger.LogInformation("Wompi configured with Environment: {Environment}, BaseUrl: {BaseUrl}", environment, baseUrl);
    }

    public async Task<WompiTransactionResponseDto> CreateTransactionAsync(Payment payment)
    {
        _logger.LogInformation("Creating Wompi checkout for reference {Reference}", payment.Reference);

        var redirectUrl = _configuration["Wompi:RedirectUrl"] 
            ?? "https://jegasolutions-platform-frontend-95l.vercel.app/payment-success";

        var amountInCents = (int)(payment.Amount * 100);
        
        // Generar firma de integridad para checkout (SHA256 simple, NO HMAC)
        var integrity = ComputeCheckoutIntegrity(
            payment.Reference, 
            amountInCents, 
            "COP", 
            _privateKey
        );

        // Generar URL de checkout con firma
        var checkoutUrl = "https://checkout.wompi.co/p/" +
            $"?public-key={_publicKey}" +
            $"&currency=COP" +
            $"&amount-in-cents={amountInCents}" +
            $"&reference={payment.Reference}" +
            $"&signature:integrity={integrity}" +
            $"&redirect-url={Uri.EscapeDataString(redirectUrl)}";

        var shortId = $"WMP_{DateTime.Now:yyyyMMdd}_{payment.Id}";

        var result = new WompiTransactionResponseDto
        {
            Id = shortId,
            Reference = payment.Reference,
            CheckoutUrl = checkoutUrl,
            Status = "PENDING"
        };

        _logger.LogInformation("Checkout URL created: {CheckoutUrl}", checkoutUrl);
        return result;
    }

    public Task<bool> ValidateWebhookSignature(string payload, string signature)
    {
        var expectedSignature = ComputeSignature(payload, _privateKey);
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
                var result = JsonSerializer.Deserialize<WompiApiResponse<WompiTransactionResponseDto>>(content, JsonUtils.GetJsonSerializerOptions());
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
            _logger.LogInformation("Processing webhook for reference {Reference}, status {Status}", 
                payload.Data.Reference, payload.Data.Status);

            var payment = await _paymentRepository.FirstOrDefaultAsync(p => p.Reference == payload.Data.Reference);

            if (payment == null)
            {
                payment = new Payment
                {
                    Reference = payload.Data.Reference,
                    Amount = payload.Data.AmountInCents / 100m,
                    CustomerEmail = payload.Data.Customer.Email,
                    CustomerName = payload.Data.Customer.FullName,
                    WompiTransactionId = payload.Data.Id,
                    Status = MapWompiStatus(payload.Data.Status),
                    CreatedAt = payload.Data.CreatedAt,
                    UpdatedAt = DateTime.UtcNow
                };

                await _paymentRepository.AddAsync(payment);
                _logger.LogInformation("Created new payment record for reference {Reference}", payment.Reference);
            }
            else
            {
                payment.Status = MapWompiStatus(payload.Data.Status);
                payment.WompiTransactionId = payload.Data.Id;
                payment.UpdatedAt = DateTime.UtcNow;
                await _paymentRepository.UpdateAsync(payment);
                _logger.LogInformation("Updated payment record for reference {Reference}", payment.Reference);
            }

            await _unitOfWork.SaveChangesAsync();

            if (payment.Status == "APPROVED")
            {
                _logger.LogInformation("Payment APPROVED - triggering tenant creation for {Reference}", payment.Reference);
                await CreateTenantFromPayment(payment);
                await _emailService.SendPaymentConfirmationAsync(payment);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing webhook for reference {Reference}", payload.Data.Reference);
            return false;
        }
    }

    private async Task CreateTenantFromPayment(Payment payment)
    {
        try
        {
            _logger.LogInformation("Creating tenant for payment {Reference}", payment.Reference);

            var referenceParts = payment.Reference.Split('-');
            if (referenceParts.Length < 3)
            {
                _logger.LogWarning("Invalid payment reference format: {Reference}", payment.Reference);
                return;
            }

            var modules = referenceParts.Skip(1).TakeWhile(p => p != "saas" && p != "onpremise").ToList();
            var deploymentType = referenceParts.FirstOrDefault(p => p == "saas" || p == "onpremise") ?? "saas";

            _logger.LogInformation("Extracted modules: {Modules}, deployment type: {DeploymentType}", 
                string.Join(", ", modules), deploymentType);

            var baseSubdomain = _passwordGenerator.GenerateSubdomain(payment.CustomerName ?? payment.CustomerEmail ?? "cliente");
            var subdomain = baseSubdomain;
            var counter = 1;

            while (await _tenantRepository.FirstOrDefaultAsync(t => t.Subdomain == subdomain) != null)
            {
                subdomain = $"{baseSubdomain}{counter}";
                counter++;
            }

            var tenant = new Tenant
            {
                CompanyName = payment.CustomerName ?? "Cliente",
                Subdomain = subdomain,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
            };

            await _tenantRepository.AddAsync(tenant);

            _logger.LogInformation("Created tenant {TenantId} with subdomain {Subdomain}", tenant.Id, tenant.Subdomain);

            foreach (var module in modules)
            {
                var tenantModule = new TenantModule
                {
                    TenantId = tenant.Id,
                    ModuleName = module,
                    Status = "ACTIVE"
                };
                await _tenantModuleRepository.AddAsync(tenantModule);
                _logger.LogInformation("Added module {ModuleName} to tenant {TenantId}", module, tenant.Id);
            }

            var temporaryPassword = _passwordGenerator.GenerateSecurePassword();
            var nameParts = (payment.CustomerName ?? "").Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var firstName = nameParts.FirstOrDefault() ?? "Admin";
            var lastName = nameParts.Length > 1 ? string.Join(" ", nameParts.Skip(1)) : "";

            var adminUser = new User
            {
                TenantId = tenant.Id,
                Email = payment.CustomerEmail ?? "",
                FirstName = firstName,
                LastName = lastName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(temporaryPassword),
                Role = "Admin"
            };

            await _userRepository.AddAsync(adminUser);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Created admin user {UserId} for tenant {TenantId}", adminUser.Id, tenant.Id);

            try
            {
                await _emailService.SendWelcomeEmailAsync(tenant, temporaryPassword);
                _logger.LogInformation("Welcome email sent to {Email}", payment.CustomerEmail);
            }
            catch (Exception emailEx)
            {
                _logger.LogWarning(emailEx, "Failed to send welcome email to {Email}", payment.CustomerEmail);
            }

            _logger.LogInformation("Tenant setup completed for {Subdomain}.jegasolutions.co", subdomain);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tenant for payment {Reference}", payment.Reference);
            throw;
        }
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

    // Firma de integridad para CHECKOUT WIDGET (SHA256 simple)
    private string ComputeCheckoutIntegrity(string reference, int amountInCents, string currency, string integrityKey)
    {
        var concatenated = $"{reference}{amountInCents}{currency}{integrityKey}";
        
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(concatenated));
        return Convert.ToHexString(hash).ToLower();
    }

    // Firma para WEBHOOKS (HMAC-SHA256)
    private string ComputeSignature(string payload, string privateKey)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(privateKey));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        return Convert.ToHexString(hash).ToLower();
    }
}

public class WompiApiResponse<T>
{
    public T? Data { get; set; }
    public bool Success { get; set; }
    public string? Error { get; set; }
}