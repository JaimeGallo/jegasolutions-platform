// ARCHIVO: apps/extra-hours/backend/src/JEGASolutions.ExtraHours.Infrastructure/Services/JWTUtils.cs
// FIX CRÍTICO: Agregar tenant_id a los claims del JWT para multi-tenant

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using JEGASolutions.ExtraHours.Core.Interfaces;
using JEGASolutions.ExtraHours.Core.Entities.Models;

namespace JEGASolutions.ExtraHours.Infrastructure.Services
{
    public class JWTUtils : IJWTUtils
    {
        private const long ACCESS_TOKEN_EXPIRATION = 3600000; // 1 hora en milisegundos
        private const long REFRESH_TOKEN_EXPIRATION = 604800000; // 7 días en milisegundos
        private readonly SymmetricSecurityKey _key;

        public JWTUtils(IConfiguration configuration)
        {
            var secretString = configuration["JwtSettings:SecretKey"]
                ?? throw new ArgumentNullException("JwtSettings:SecretKey",
                    "JWT Secret Key not found in configuration");
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretString));
        }

        /// <summary>
        /// Genera un token JWT de acceso con todos los claims del usuario, incluyendo TenantId
        /// </summary>
        public string GenerateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.email?.Trim() ?? string.Empty),
                new Claim("role", user.role ?? string.Empty),
                new Claim("id", user.id.ToString()),
                new Claim("name", user.name ?? string.Empty),
                new Claim("tenant_id", user.TenantId.ToString() ?? "0")  //Incluir TenantId
            };

            return CreateToken(claims, ACCESS_TOKEN_EXPIRATION);
        }

        /// <summary>
        /// Genera un refresh token con claims mínimos, incluyendo TenantId
        /// </summary>
        public string GenerateRefreshToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.email?.Trim() ?? string.Empty),
                new Claim("id", user.id.ToString()),
                new Claim("tenant_id", user.TenantId.ToString() ?? "0")  // Incluir TenantId
            };

            return CreateToken(claims, REFRESH_TOKEN_EXPIRATION);
        }

        /// <summary>
        /// Crea el token JWT con los claims y expiración especificados
        /// </summary>
        private string CreateToken(IEnumerable<Claim> claims, long expiration)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMilliseconds(expiration),
                Issuer = "JEGASolutions.Landing.API",        // ✅ CAMBIAR: antes era "JEGASolutions.ExtraHours"
                Audience = "jegasolutions-landing-client",   // ✅ CAMBIAR: antes era "JEGASolutions.ExtraHours.Users"
                SigningCredentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Extrae los claims de un token JWT
        /// </summary>
        public ClaimsPrincipal ExtractClaims(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _key,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = "JEGASolutions.Landing.API",        // ✅ CAMBIAR
                ValidAudience = "jegasolutions-landing-client",   // ✅ CAMBIAR
                ClockSkew = TimeSpan.Zero,
                RoleClaimType = "role",
                NameClaimType = ClaimTypes.Name
            };

            return tokenHandler.ValidateToken(token, validationParameters, out _);
        }

        /// <summary>
        /// Valida que el token sea válido para el usuario especificado
        /// </summary>
        public bool IsTokenValid(string token, User user)
        {
            try
            {
                var principal = ExtractClaims(token);
                var username = principal.Identity?.Name;
                var userId = principal.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
                var tenantId = principal.Claims.FirstOrDefault(c => c.Type == "tenant_id")?.Value;

                // Validar que el token corresponde al usuario correcto
                return username == user.email
                    && userId == user.id.ToString()
                    && tenantId == user.TenantId.ToString();  // ✅ También validar TenantId
            }
            catch
            {
                return false;
            }
        }
    }
}
