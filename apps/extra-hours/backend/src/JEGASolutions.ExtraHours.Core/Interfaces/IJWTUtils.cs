using JEGASolutions.ExtraHours.Core.Entities.Models;
using System.Security.Claims;

namespace JEGASolutions.ExtraHours.Core.Interfaces
{
    public interface IJWTUtils
    {
        string GenerateToken(User user);
        string GenerateRefreshToken(User user);
        ClaimsPrincipal ExtractClaims(string token);
        bool IsTokenValid(string token, User user);
    }
}
