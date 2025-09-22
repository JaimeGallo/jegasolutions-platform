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
        _privateKey = _configuration["Wompi__PrivateKey"] ?? throw new ArgumentNullException("Wompi__PrivateKey");
        _publicKey = _configuration["Wompi__PublicKey"] ?? throw new ArgumentNullException("Wompi__PublicKey");

        var baseUrl = _configuration["Wompi__BaseUrl"] ?? "https://sandbox.wompi.co/v1/";
        _httpClient.BaseAddress = new Uri(baseUrl);

        // Log para confirmar el ambiente
        _logger.LogInformation("Wompi configured with BaseUrl: {BaseUrl}", baseUrl);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _publicKey);
    }

    public async Task<WompiTransactionResponseDto> CreateTransactionAsync(Payment payment)
    {
        _logger.LogInformation("Creating Wompi transaction for reference {Reference}", payment.Reference);

        var wompiApiUrl = _configuration["Wompi__BaseUrl"] ?? "https://sandbox.wompi.co/v1/";
        var redirectUrl = _configuration["Wompi:RedirectUrl"] ?? $"https://{payment.Reference}.localhost/payment-status"; // Fallback a localhost

        var requestPayload = new
        {
            amount_in_cents = (int)(payment.Amount * 100),
            currency = "COP",
            customer_email = payment.CustomerEmail,
            reference = payment.Reference,
            payment_method = new
            {
                // Permite todos los métodos de pago disponibles en el checkout
                type = "CARD", // Este valor es un placeholder, el checkout de Wompi mostrará todas las opciones
            },
            redirect_url = redirectUrl,
            customer_data = new
            {
                full_name = payment.CustomerName,
                phone_number = payment.CustomerPhone,
                email = payment.CustomerEmail
            }
        };

        var jsonPayload = JsonSerializer.Serialize(requestPayload, JsonUtils.GetJsonSerializerOptions());
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("transactions", content);

        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Error creating Wompi transaction for reference {Reference}. Status: {StatusCode}, Body: {Body}", payment.Reference, response.StatusCode, responseBody);
            throw new ApplicationException($"Error from Wompi API: {responseBody}");
        }

        var wompiResponse = JsonSerializer.Deserialize<WompiApiResponse<WompiTransactionResponseDto>>(responseBody, JsonUtils.GetJsonSerializerOptions());

        var transactionData = wompiResponse?.Data ?? throw new ApplicationException("Failed to deserialize Wompi transaction response.");

        transactionData.CheckoutUrl = $"https://checkout.wompi.co/p/{transactionData.Id}";

        return transactionData;
    }

    public Task<bool> ValidateWebhookSignature(string payload, string signature)
    {
        var expectedSignature = ComputeSignature(payload, _privateKey);
        var isValid = signature.Equals(expectedSignature, StringComparison.OrdinalIgnoreCase);
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
        catch
        {
            return null;
        }
    }

    public async Task<bool> ProcessPaymentWebhook(WompiWebhookDto payload)
    {
        try
        {
            var payment = await _paymentRepository.FirstOrDefaultAsync(p => p.Reference == payload.Data.Reference);

            if (payment == null)
            {
                // Create new payment record if not exists
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
            }
            else
            {
                // Update existing payment
                payment.Status = MapWompiStatus(payload.Data.Status);
                payment.WompiTransactionId = payload.Data.Id;
                payment.UpdatedAt = DateTime.UtcNow;
                await _paymentRepository.UpdateAsync(payment);
            }

            await _unitOfWork.SaveChangesAsync();

            // If payment is approved, trigger tenant creation
            if (payment.Status == "APPROVED")
            {
                await CreateTenantFromPayment(payment);

                // Send payment confirmation email
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

            // Extract module information from reference
            var referenceParts = payment.Reference.Split('-');
            if (referenceParts.Length < 3)
            {
                _logger.LogWarning("Invalid payment reference format: {Reference}", payment.Reference);
                return;
            }

            var modules = referenceParts.Skip(1).TakeWhile(p => p != "saas" && p != "onpremise").ToList();
            var deploymentType = referenceParts.FirstOrDefault(p => p == "saas" || p == "onpremise") ?? "saas";

            // Generate subdomain
            var baseSubdomain = _passwordGenerator.GenerateSubdomain(payment.CustomerName ?? payment.CustomerEmail ?? "cliente");
            var subdomain = baseSubdomain;
            var counter = 1;

            // Check if subdomain already exists
            while (await _tenantRepository.FirstOrDefaultAsync(t => t.Subdomain == subdomain) != null)
            {
                // Generate a new subdomain if it exists
                subdomain = $"{baseSubdomain}{counter}";
                counter++;
            }

            // Create tenant
            var tenant = new Tenant
            {
                CompanyName = payment.CustomerName ?? "Cliente",
                Subdomain = subdomain,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
            };

            await _tenantRepository.AddAsync(tenant);

            _logger.LogInformation("Created tenant {TenantId} with subdomain {Subdomain}", tenant.Id, tenant.Subdomain);

            // Add modules to tenant
            foreach (var module in modules)
            {
                var tenantModule = new TenantModule
                {
                    TenantId = tenant.Id,
                    ModuleName = module,
                    Status = "ACTIVE"
                };
                await _tenantModuleRepository.AddAsync(tenantModule);
            }

            // Create admin user for the tenant
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

            // Save all changes in a single transaction
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Created admin user {UserId} for tenant {TenantId}", adminUser.Id, tenant.Id);

            // Send welcome email with credentials
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