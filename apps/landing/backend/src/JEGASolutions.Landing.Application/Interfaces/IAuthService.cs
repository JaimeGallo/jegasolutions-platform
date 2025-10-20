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

    /// <summary>
    /// Cambiar contraseña del usuario autenticado
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <param name="currentPassword">Contraseña actual</param>
    /// <param name="newPassword">Nueva contraseña</param>
    Task ChangePasswordAsync(int userId, string currentPassword, string newPassword);

    /// <summary>
    /// Cambiar contraseña de cualquier usuario (solo admin)
    /// </summary>
    /// <param name="userId">ID del usuario a modificar</param>
    /// <param name="newPassword">Nueva contraseña</param>
    Task ChangePasswordAdminAsync(int userId, string newPassword);
}
