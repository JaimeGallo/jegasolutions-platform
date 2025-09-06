using JEGASolutions.Landing.Domain.Entities;

namespace JEGASolutions.Landing.Application.Interfaces;

public interface IAuthService
{
    Task<string?> AuthenticateAsync(string email, string password, int? tenantId = null);
    Task<User?> GetUserByEmailAsync(string email, int? tenantId = null);
    Task<bool> ValidateTokenAsync(string token);
    Task<User?> GetUserFromTokenAsync(string token);
    string GenerateJwtToken(User user);
    bool VerifyPassword(string password, string hash);
    string HashPassword(string password);
}