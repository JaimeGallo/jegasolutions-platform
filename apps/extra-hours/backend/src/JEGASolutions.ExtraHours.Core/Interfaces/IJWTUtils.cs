using JEGASolutions.ExtraHours.API.Model;
using System.Security.Claims;

namespace JEGASolutions.ExtraHours.API.Utils
{
    public interface IJWTUtils
    {
        string GenerateToken(User user);
        string GenerateRefreshToken(User user);
        ClaimsPrincipal ExtractClaims(string token);
        bool IsTokenValid(string token, User user);
    }
}
