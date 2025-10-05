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

        var baseUrl = _configuration["Wompi__BaseUrl"] ?? _configuration["Wompi:BaseUrl"] 
            ?? "https://production.wompi.co/v1/";
        _httpClient.BaseAddress = new Uri(baseUrl);

        var environment = baseUrl.Contains("sandbox") ? "SANDBOX" : "PRODUCTION";
        _logger.LogInformation("Wompi configured with Environment: {Environment}, BaseUrl: {BaseUrl}", 
            environment, baseUrl);
    }

    public async Task<WompiTransactionResponseDto> CreateTransactionAsync(Payment payment)
    {
        _logger.LogInformation("Creating Wompi transaction for reference {Reference}", payment.Reference);

        try
        {
            var redirectUrl = _configuration["Wompi:RedirectUrl"] 
                ?? "https://jegasolutions-platform-frontend-95l.vercel.app/payment-success";

            var amountInCents = (int)(payment.Amount * 100);
            var currency = "COP";
            var reference = payment.Reference;

            // Obtener el integrity secret
            var integritySecret = _configuration["Wompi:IntegritySecret"] 
                ?? _configuration["Wompi__IntegritySecret"];

            // Si no hay integrity secret, usar fallback a URL directa
            if (string.IsNullOrEmpty(integritySecret))
            {
                _logger.LogWarning("Wompi IntegritySecret not configured, using fallback checkout URL");
                
                var checkoutUrl = "https://checkout.wompi.co/p/" +
                    $"?public-key={_publicKey}" +
                    $"&currency={currency}" +
                    $"&amount-in-cents={amountInCents}" +
                    $"&reference={reference}" +
                    $"&redirect-url={Uri.EscapeDataString(redirectUrl)}";

                return new WompiTransactionResponseDto
                {
                    Id = $"WMP_{DateTime.Now:yyyyMMdd}_{payment.Id}",
                    Reference = reference,
                    CheckoutUrl = checkoutUrl,
                    Status = "PENDING"
                };
            }

            // Calcular la firma de integridad
            var signatureString = $"{reference}{amountInCents}{currency}{integritySecret}";
            var signature = ComputeSHA256(signatureString);

            _logger.LogInformation("Creating transaction with signature for reference {Reference}", reference);

            // Obtener acceptance token
            var acceptanceToken = await GetAcceptanceTokenAsync();

            // Crear el objeto de la transacción
            var transactionRequest = new
            {
                acceptance_token = acceptanceToken,
                amount_in_cents = amountInCents,
                currency = currency,
                customer_email = payment.CustomerEmail,
                payment_method = new { },
                reference = reference,
                redirect_url = redirectUrl,
                customer_data = new
                {
                    phone_number = payment.CustomerPhone ?? "",
                    full_name = payment.CustomerName ?? "",
                    legal_id = "",
                    legal_id_type = "CC"
                }
            };

            var content = new StringContent(
                JsonSerializer.Serialize(transactionRequest, JsonUtils.GetJsonSerializerOptions()),
                Encoding.UTF8,
                "application/json"
            );

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_privateKey}");

            var response = await _httpClient.PostAsync("transactions", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Wompi transaction creation failed: {StatusCode} - {Response}", 
                    response.StatusCode, responseContent);
                
                // Fallback a URL directa
                var checkoutUrl = "https://checkout.wompi.co/p/" +
                    $"?public-key={_publicKey}" +
                    $"&currency={currency}" +
                    $"&amount-in-cents={amountInCents}" +
                    $"&reference={reference}" +
                    $"&redirect-url={Uri.EscapeDataString(redirectUrl)}";

                _logger.LogWarning("Using fallback checkout URL due to API error");

                return new WompiTransactionResponseDto
                {
                    Id = $"WMP_{DateTime.Now:yyyyMMdd}_{payment.Id}",
                    Reference = reference,
                    CheckoutUrl = checkoutUrl,
                    Status = "PENDING"
                };
            }

            var wompiResponse = JsonSerializer.Deserialize<WompiApiResponseDto>(
                responseContent, 
                JsonUtils.GetJsonSerializerOptions()
            );

            if (wompiResponse?.Data == null)
            {
                throw new Exception("Invalid response from Wompi API");
            }

            var widgetUrl = $"https://checkout.wompi.co/l/{wompiResponse.Data.Id}";

            _logger.LogInformation("Transaction created successfully: {TransactionId}, Widget URL: {Url}", 
                wompiResponse.Data.Id, widgetUrl);

            return new WompiTransactionResponseDto
            {
                Id = wompiResponse.Data.Id ?? "",
                Reference = wompiResponse.Data.Reference ?? "",
                CheckoutUrl = widgetUrl,
                Status = wompiResponse.Data.Status?.ToUpper() ?? "PENDING"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating Wompi transaction for reference {Reference}", 
                payment.Reference);
            throw;
        }
    }

    public Task<bool> ValidateWebhookSignature(string payload, string signature)
    {
        var expectedSignature = ComputeHMAC(payload, _privateKey);
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
            var existingTenant = await _tenantRepository.FirstOrDefaultAsync(
                t => t.Email == payment.CustomerEmail
            );

            if (existingTenant != null)
            {
                _logger.LogInformation("Tenant already exists for email {Email}", payment.CustomerEmail);
                return;
            }

            var subdomain = GenerateSubdomain(payment.CustomerName ?? payment.CustomerEmail ?? "");
            var temporaryPassword = _passwordGenerator.GeneratePassword();

            var tenant = new Tenant
            {
                Name = payment.CustomerName ?? "",
                Email = payment.CustomerEmail ?? "",
                Subdomain = subdomain,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _tenantRepository.AddAsync(tenant);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Created tenant {TenantId} with subdomain {Subdomain}", 
                tenant.Id, subdomain);

            var moduleId = ExtractModuleIdFromReference(payment.Reference);
            var tenantModule = new TenantModule
            {
                TenantId = tenant.Id,
                ModuleId = moduleId,
                IsActive = true,
                ActivatedAt = DateTime.UtcNow
            };

            await _tenantModuleRepository.AddAsync(tenantModule);

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
                Role = "Admin"
            };

            await _userRepository.AddAsync(adminUser);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Created admin user {UserId} for tenant {TenantId}", 
                adminUser.Id, tenant.Id);

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

    private string GenerateSubdomain(string name)
    {
        var subdomain = name.ToLower()
            .Replace(" ", "-")
            .Replace("á", "a").Replace("é", "e").Replace("í", "i")
            .Replace("ó", "o").Replace("ú", "u").Replace("ñ", "n");

        subdomain = new string(subdomain.Where(c => char.IsLetterOrDigit(c) || c == '-').ToArray());

        if (string.IsNullOrWhiteSpace(subdomain))
        {
            subdomain = "tenant-" + Guid.NewGuid().ToString().Substring(0, 8);
        }

        return subdomain;
    }

    private int ExtractModuleIdFromReference(string reference)
    {
        if (reference.Contains("EXTRAHOURS", StringComparison.OrdinalIgnoreCase))
            return 1;
        if (reference.Contains("REPORTBUILDER", StringComparison.OrdinalIgnoreCase))
            return 2;

        return 1;
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