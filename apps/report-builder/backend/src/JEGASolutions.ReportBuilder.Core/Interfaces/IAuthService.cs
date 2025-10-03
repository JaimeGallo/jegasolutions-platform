using JEGASolutions.ReportBuilder.Core.Dto;
using JEGASolutions.ReportBuilder.Core.Entities.Models;

namespace JEGASolutions.ReportBuilder.Core.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(string email, string password);
        Task<bool> VerifyTokenAsync(string token);
        Task<User?> GetUserByEmailAsync(string email);
        string GenerateJwtToken(User user);
    }
}

