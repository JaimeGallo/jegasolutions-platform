using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using JEGASolutions.Landing.Application.Interfaces;
using JEGASolutions.Landing.Domain.Entities;
using JEGASolutions.Landing.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace JEGASolutions.Landing.Application.Services;

public class AuthService : IAuthService
{
    private readonly IRepository<User> _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IRepository<User> userRepository,
        IUnitOfWork unitOfWork,
        IConfiguration configuration,
        ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<string?> AuthenticateAsync(string email, string password, int? tenantId = null)
    {
        try
        {
            _logger.LogInformation("Authentication attempt for email: {Email}, TenantId: {TenantId}",
                email, tenantId);

            var user = await GetUserByEmailAsync(email, tenantId);

            if (user == null)
            {
                _logger.LogWarning("User not found: {Email}", email);
                return null;
            }

            if (!user.IsActive)
            {
                _logger.LogWarning("Inactive user attempted login: {Email}", email);
                return null;
            }

            if (!VerifyPassword(password, user.PasswordHash))
            {
                _logger.LogWarning("Invalid password for user: {Email}", email);
                return null;
            }

            // Update last login - Crear propiedad LastLoginAt en User si no existe
            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveAsync();

            var token = GenerateJwtToken(user);
            _logger.LogInformation("Successful authentication for user: {Email}", email);

            return token;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during authentication for email: {Email}", email);
            return null;
        }
    }

    public async Task<User?> GetUserByEmailAsync(string email, int? tenantId = null)
    {
        try
        {
            var users = await _userRepository.FindAsync(u =>
                u.Email.ToLower() == email.ToLower() &&
                (tenantId == null || u.TenantId == tenantId));

            return users.FirstOrDefault();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by email: {Email}", email);
            return null;
        }
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        try
        {
            return await GetUserFromTokenAsync(token) is { IsActive: true };
        }
        catch
        {
            return await Task.FromResult(false);
        }
    }

    public async Task<User?> GetUserFromTokenAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]!);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["JWT:Issuer"],
                ValidAudience = _configuration["JWT:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

            var userIdClaim = principal.FindFirst("userId")?.Value;
            if (int.TryParse(userIdClaim, out var userId))
            {
                return await _userRepository.GetByIdAsync(userId);
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Invalid token validation");
            return null;
        }
    }

    public string GenerateJwtToken(User user)
    {
        // ⚡ CRÍTICO: Limpiar mapeo ANTES de crear tokens
        System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();

        var key = Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]!);
        var fullName = $"{user.FirstName} {user.LastName}".Trim();

        // ✅ SOLO usar nombres CORTOS - NUNCA ClaimTypes.*
        var claims = new List<Claim>
        {
            new Claim("userId", user.Id.ToString()),
            new Claim("email", user.Email),                  // ✅ "email" NO ClaimTypes.Email
            new Claim("tenantId", user.TenantId.ToString()),
            new Claim("role", user.Role),                    // ✅ "role" NO ClaimTypes.Role
            new Claim("name", fullName),                     // ✅ "name" NO ClaimTypes.Name
            new Claim("firstName", user.FirstName),
            new Claim("lastName", user.LastName)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(
                int.TryParse(_configuration["JWT:ExpirationMinutes"], out var minutes) ? minutes : 60),
            Issuer = _configuration["JWT:Issuer"],
            Audience = _configuration["JWT:Audience"],
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        var tokenString = tokenHandler.WriteToken(token);

        // 🔍 LOGGING PARA VERIFICAR
        _logger.LogInformation("✅ Token generado con claims cortos para usuario: {Email}", user.Email);

        return tokenString;
    }

    public bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }

    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public async Task ChangePasswordAsync(int userId, string currentPassword, string newPassword)
    {
        try
        {
            _logger.LogInformation("🔄 Password change attempt for user: {UserId}", userId);

            // 1. Obtener usuario por ID
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("❌ User not found: {UserId}", userId);
                throw new KeyNotFoundException("Usuario no encontrado");
            }

            // 2. Verificar contraseña actual con BCrypt
            if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
            {
                _logger.LogWarning("❌ Invalid current password for user: {UserId}", userId);
                throw new UnauthorizedAccessException("Contraseña actual incorrecta");
            }

            // 3. Validar nueva contraseña (mínimo 8 caracteres)
            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 8)
            {
                _logger.LogWarning("❌ Invalid new password for user: {UserId}", userId);
                throw new ArgumentException("La nueva contraseña debe tener al menos 8 caracteres");
            }

            // 4. Verificar que la nueva contraseña sea diferente a la actual
            if (BCrypt.Net.BCrypt.Verify(newPassword, user.PasswordHash))
            {
                _logger.LogWarning("❌ New password same as current for user: {UserId}", userId);
                throw new ArgumentException("La nueva contraseña debe ser diferente a la actual");
            }

            // 5. Hashear nueva contraseña con BCrypt (work factor 11)
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword, workFactor: 11);
            user.UpdatedAt = DateTime.UtcNow;

            // 6. Actualizar en base de datos
            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveAsync();

            _logger.LogInformation("✅ Password updated successfully for user: {UserId}", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Error changing password for user: {UserId}", userId);
            throw;
        }
    }

    public async Task ChangePasswordAdminAsync(int userId, string newPassword)
    {
        try
        {
            _logger.LogInformation("🔄 Admin password change for user: {UserId}", userId);

            // 1. Obtener usuario por ID
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("❌ User not found: {UserId}", userId);
                throw new KeyNotFoundException("Usuario no encontrado");
            }

            // 2. Validar nueva contraseña (mínimo 8 caracteres)
            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 8)
            {
                _logger.LogWarning("❌ Invalid new password for user: {UserId}", userId);
                throw new ArgumentException("La nueva contraseña debe tener al menos 8 caracteres");
            }

            // 3. Hashear y actualizar directamente (sin validar contraseña actual)
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword, workFactor: 11);
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveAsync();

            _logger.LogInformation("✅ Password updated by admin for user: {UserId}", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Error changing password (admin) for user: {UserId}", userId);
            throw;
        }
    }
}
