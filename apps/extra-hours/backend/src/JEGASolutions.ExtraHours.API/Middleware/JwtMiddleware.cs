using JEGASolutions.ExtraHours.Core.Interfaces;
using System.Security.Claims;

namespace JEGASolutions.ExtraHours.API.Middleware
{
    /// <summary>
    /// Middleware to handle JWT token processing and claim mapping
    /// </summary>
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<JwtMiddleware> _logger;

        public JwtMiddleware(RequestDelegate next, ILogger<JwtMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IJWTUtils jwtUtils)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            Console.WriteLine($"🔍 Token recibido en JwtMiddleware");

            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    var principal = jwtUtils.ExtractClaims(token);
                    if (principal != null)
                    {
                        context.User = principal;

                        var roleClaim = principal.FindFirst("role")?.Value;
                        var userId = principal.FindFirst("userId")?.Value
                                  ?? principal.FindFirst("id")?.Value;

                        Console.WriteLine($"✅ Usuario autenticado - Role: {roleClaim}, UserId: {userId}");

                        if (string.IsNullOrEmpty(roleClaim))
                        {
                            Console.WriteLine("⚠️ No se encontró el rol en las claims del token");
                        }
                    }
                    else
                    {
                        Console.WriteLine("❌ No se pudo extraer información del token JWT");
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"❌ Error al procesar el token: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("⚠️ No se recibió token en la petición");
            }

            await _next(context);
        }
    }
}
