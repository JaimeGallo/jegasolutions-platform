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
    private readonly string _webhookSecret;

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

         _webhookSecret = _configuration["Wompi__WebhookSecret"] ?? _configuration["Wompi:WebhookSecret"]
        ?? _privateKey; // Fallback a private key
    
    _logger.LogInformation("Wompi WebhookSecret configured: {Configured}", 
        !string.IsNullOrEmpty(_webhookSecret) ? "Yes" : "No");

        var environment = baseUrl.Contains("sandbox") ? "SANDBOX" : "PRODUCTION";
        _logger.LogInformation("Wompi configured with Environment: {Environment}, BaseUrl: {BaseUrl}", environment, baseUrl);
    }

   public async Task<WompiTransactionResponseDto> CreateTransactionAsync(Payment payment)
{
    _logger.LogInformation("Creating Wompi transaction for reference {Reference}", payment.Reference);

    try
    {
        // 1. URL de redirección
        var redirectUrl = _configuration["Wompi:RedirectUrl"] 
            ?? "https://jegasolutions-platform-frontend-95l.vercel.app/payment-success";

        // 2. Preparar el cuerpo de la petición
        var amountInCents = (int)(payment.Amount * 100);
        var currency = "COP";
        var reference = payment.Reference;

        // 3. Calcular la firma de integridad
        // Formato: reference + amountInCents + currency + integrity_secret
        var integritySecret = _configuration["Wompi:IntegritySecret"] 
            ?? _configuration["Wompi__IntegritySecret"]
            ?? throw new ArgumentNullException("Wompi:IntegritySecret", 
                "Wompi Integrity Secret is required for transaction creation");

        var signatureString = $"{reference}{amountInCents}{currency}{integritySecret}";
        var signature = ComputeSignature(signatureString, ""); // Usar SHA256 directo

        _logger.LogInformation("Creating transaction with signature for reference {Reference}", reference);

        // 4. Crear el objeto de la transacción
        var transactionRequest = new
        {
            acceptance_token = await GetAcceptanceTokenAsync(), // Necesario para Wompi
            amount_in_cents = amountInCents,
            currency = currency,
            customer_email = payment.CustomerEmail,
            payment_method = new { }, // Vacío para permitir todos los métodos
            reference = reference,
            redirect_url = redirectUrl,
            customer_data = new
            {
                phone_number = payment.CustomerPhone ?? "",
                full_name = payment.CustomerName ?? "",
                legal_id = "", // Opcional
                legal_id_type = "CC" // CC, NIT, etc.
            }
        };

        // 5. Hacer la petición a Wompi
        var content = new StringContent(
            JsonSerializer.Serialize(transactionRequest, JsonUtils.GetJsonSerializerOptions()),
            Encoding.UTF8,
            "application/json"
        );

        // Agregar headers necesarios
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_privateKey}");
        _httpClient.DefaultRequestHeaders.Add("X-Integrity", signature);

        var response = await _httpClient.PostAsync("transactions", content);
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Wompi transaction creation failed: {StatusCode} - {Response}", 
                response.StatusCode, responseContent);
            
            // Fallback a URL directa si falla la API (solo para testing)
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

        // 6. Parsear la respuesta
        var wompiResponse = JsonSerializer.Deserialize<WompiApiResponse<WompiTransactionData>>(
            responseContent, 
            JsonUtils.GetJsonSerializerOptions()
        );

        if (wompiResponse?.Data == null)
        {
            throw new Exception("Invalid response from Wompi API");
        }

        // 7. Construir URL del widget con el ID de la transacción
        var widgetUrl = $"https://checkout.wompi.co/l/{wompiResponse.Data.Id}";

        _logger.LogInformation("Transaction created successfully: {TransactionId}, Widget URL: {Url}", 
            wompiResponse.Data.Id, widgetUrl);

        return new WompiTransactionResponseDto
        {
            Id = wompiResponse.Data.Id,
            Reference = wompiResponse.Data.Reference,
            CheckoutUrl = widgetUrl,
            Status = wompiResponse.Data.Status?.ToUpper() ?? "PENDING"
        };
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error creating Wompi transaction for reference {Reference}", payment.Reference);
        throw;
    }
}

// Nuevo método helper para obtener el acceptance token
private async Task<string> GetAcceptanceTokenAsync()
{
    try
    {
        var response = await _httpClient.GetAsync("merchants/" + _publicKey);
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var merchantData = JsonSerializer.Deserialize<WompiMerchantResponse>(
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

// Método actualizado para calcular la firma SHA256
private string ComputeSignature(string data, string key)
{
    if (string.IsNullOrEmpty(key))
    {
        // Para integrity signature, usar SHA256 directo (sin HMAC)
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
        return Convert.ToHexString(hash).ToLower();
    }
    else
    {
        // Para webhook signature, usar HMAC-SHA256
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        return Convert.ToHexString(hash).ToLower();
    }
}

// Clases auxiliares
private class WompiTransactionData
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = "";

    [JsonPropertyName("reference")]
    public string Reference { get; set; } = "";

    [JsonPropertyName("status")]
    public string? Status { get; set; }
}

private class WompiMerchantResponse
{
    [JsonPropertyName("data")]
    public WompiMerchantData? Data { get; set; }
}

private class WompiMerchantData
{
    [JsonPropertyName("presigned_acceptance")]
    public PresignedAcceptance? PresignedAcceptance { get; set; }
}

private class PresignedAcceptance
{
    [JsonPropertyName("acceptance_token")]
    public string AcceptanceToken { get; set; } = "";
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
        // IMPORTANTE: Ahora accedemos a través de Data.Transaction
        var transaction = payload.Data.Transaction;
        
        _logger.LogInformation("Processing webhook for reference {Reference}, status {Status}", 
            transaction.Reference, transaction.Status);

        var payment = await _paymentRepository.FirstOrDefaultAsync(
            p => p.Reference == transaction.Reference
        );

        if (payment == null)
        {
            // Crear nuevo registro de pago
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
            // Actualizar pago existente
            payment.Status = MapWompiStatus(transaction.Status);
            payment.WompiTransactionId = transaction.Id;
            payment.UpdatedAt = DateTime.UtcNow;
            
            // Actualizar datos del cliente si están disponibles
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

        // Si el pago fue aprobado, crear el tenant
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
   private string ComputeCheckoutIntegrity(string reference, int amountInCents, string currency)
{
    var concatenated = $"{reference}{amountInCents}{currency}{_webhookSecret}";
    
    _logger.LogInformation("Computing integrity for: ref={Reference}, amount={Amount}, currency={Currency}", 
        reference, amountInCents, currency);
    
    using var sha256 = SHA256.Create();
    var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(concatenated));
    var result = Convert.ToHexString(hash).ToLower();
    
    _logger.LogInformation("Integrity hash: {Hash}", result);
    return result;
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