using JEGASolutions.Landing.Application.Interfaces;
using JEGASolutions.Landing.Domain.Entities;
using JEGASolutions.Landing.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using BCrypt.Net;

namespace JEGASolutions.Landing.Application.Services;

public class TenantService : ITenantService
{
    private readonly IRepository<Tenant> _tenantRepository;
    private readonly IRepository<User> _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordGenerator _passwordGenerator;
    private readonly IEmailService _emailService;
    private readonly ILogger<TenantService> _logger;

    public TenantService(
        IRepository<Tenant> tenantRepository,
        IRepository<User> userRepository,
        IUnitOfWork unitOfWork,
        IPasswordGenerator passwordGenerator,
        IEmailService emailService,
        ILogger<TenantService> logger)
    {
        _tenantRepository = tenantRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _passwordGenerator = passwordGenerator;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<Tenant?> GetTenantBySubdomainAsync(string subdomain)
    {
        try
        {
            var tenants = await _tenantRepository.FindAsync(t =>
                t.Subdomain.ToLower() == subdomain.ToLower());
            return tenants.FirstOrDefault();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tenant by subdomain: {Subdomain}", subdomain);
            return null;
        }
    }

    public async Task<Tenant?> GetTenantByIdAsync(int tenantId)
    {
        try
        {
            return await _tenantRepository.GetByIdAsync(tenantId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tenant by ID: {TenantId}", tenantId);
            return null;
        }
    }

    public async Task<Tenant> CreateTenantAsync(string companyName, string email, string contactName)
    {
        try
        {
            _logger.LogInformation("Creating new tenant: {CompanyName}", companyName);

            // Generate unique subdomain
            var baseSubdomain = _passwordGenerator.GenerateSubdomain(companyName);
            var subdomain = await GenerateUniqueSubdomainAsync(baseSubdomain);

            // Create tenant
            var tenant = new Tenant
            {
                CompanyName = companyName,
                Subdomain = subdomain,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _tenantRepository.AddAsync(tenant);
            await _unitOfWork.SaveAsync();

            _logger.LogInformation("Tenant created with ID: {TenantId}, Subdomain: {Subdomain}", tenant.Id, subdomain);

            // Create admin user for tenant
            var adminPassword = _passwordGenerator.GenerateSecurePassword();

            // Parse contact name into first and last name
            var nameParts = contactName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var firstName = nameParts.FirstOrDefault() ?? contactName;
            var lastName = nameParts.Length > 1 ? string.Join(" ", nameParts.Skip(1)) : "";

            var adminUser = new User
            {
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword),
                TenantId = tenant.Id,
                Role = "Admin",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _userRepository.AddAsync(adminUser);
            await _unitOfWork.SaveAsync();

            _logger.LogInformation("Admin user created for tenant: {TenantId}", tenant.Id);

            // Send welcome email
            try
            {
                await _emailService.SendWelcomeEmailAsync(tenant, adminPassword);
                _logger.LogInformation("Welcome email sent to: {Email}", email);
            }
            catch (Exception emailEx)
            {
                _logger.LogWarning(emailEx, "Failed to send welcome email to: {Email}", email);
                // No fallar la creación del tenant por un error de email
            }

            return tenant;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tenant: {CompanyName}", companyName);
            throw;
        }
    }

    public async Task<IEnumerable<Tenant>> GetAllTenantsAsync()
    {
        try
        {
            return await _tenantRepository.GetAllAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all tenants");
            return Enumerable.Empty<Tenant>();
        }
    }

    public async Task<bool> UpdateTenantAsync(Tenant tenant)
    {
        try
        {
            tenant.UpdatedAt = DateTime.UtcNow;
            await _tenantRepository.UpdateAsync(tenant);
            await _unitOfWork.SaveAsync();

            _logger.LogInformation("Tenant updated successfully: {TenantId}", tenant.Id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tenant: {TenantId}", tenant.Id);
            return false;
        }
    }

    public async Task<bool> DeleteTenantAsync(int tenantId)
    {
        try
        {
            var tenant = await _tenantRepository.GetByIdAsync(tenantId);
            if (tenant == null)
            {
                _logger.LogWarning("Tenant not found for deletion: {TenantId}", tenantId);
                return false;
            }

            // Soft delete - deactivate instead of hard delete
            tenant.IsActive = false;
            tenant.UpdatedAt = DateTime.UtcNow;

            await _tenantRepository.UpdateAsync(tenant);
            await _unitOfWork.SaveAsync();

            _logger.LogInformation("Tenant deactivated successfully: {TenantId}", tenantId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tenant: {TenantId}", tenantId);
            return false;
        }
    }

    public async Task<bool> TenantExistsAsync(string subdomain)
    {
        try
        {
            var tenant = await GetTenantBySubdomainAsync(subdomain);
            return tenant != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if tenant exists: {Subdomain}", subdomain);
            return false;
        }
    }

    // Métodos adicionales para compatibilidad completa
    public async Task<bool> IsTenantActiveAsync(int tenantId)
    {
        try
        {
            var tenant = await _tenantRepository.GetByIdAsync(tenantId);
            return tenant?.IsActive ?? false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking tenant status: {TenantId}", tenantId);
            return false;
        }
    }

    public async Task<bool> IsSubdomainAvailableAsync(string subdomain)
    {
        return !await TenantExistsAsync(subdomain);
    }

    private async Task<string> GenerateUniqueSubdomainAsync(string baseSubdomain)
    {
        var subdomain = baseSubdomain;
        var counter = 1;

        while (await TenantExistsAsync(subdomain))
        {
            subdomain = $"{baseSubdomain}{counter}";
            counter++;
        }

        return subdomain;
    }
}