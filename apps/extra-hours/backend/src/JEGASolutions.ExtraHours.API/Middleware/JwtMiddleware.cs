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
            // Lista de rutas públicas que no requieren token
            var publicPaths = new[]
            {
                "/health",
                "/",
                "/swagger",
                "/api/logout",
                "/api/extra-hour/calculate"
            };

            var isPublicPath = publicPaths.Any(path =>
                context.Request.Path.StartsWithSegments(path, StringComparison.OrdinalIgnoreCase));

            var isOptionsRequest = context.Request.Method == "OPTIONS";

            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            // Solo loguear si NO es una ruta pública y NO es un OPTIONS request
            if (!isPublicPath && !isOptionsRequest)
            {
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

                            _logger.LogDebug("Usuario autenticado - Role: {Role}, UserId: {UserId}", roleClaim, userId);

                            if (string.IsNullOrEmpty(roleClaim))
                            {
                                _logger.LogWarning("No se encontró el rol en las claims del token");
                            }
                        }
                        else
                        {
                            _logger.LogWarning("No se pudo extraer información del token JWT");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error al procesar el token");
                    }
                }
                else
                {
                    // Solo loguear warning si es una ruta protegida
                    _logger.LogDebug("No se recibió token en la petición a: {Path}", context.Request.Path);
                }
            }
            else if (!string.IsNullOrEmpty(token))
            {
                // Si hay token en ruta pública, procesarlo silenciosamente
                try
                {
                    var principal = jwtUtils.ExtractClaims(token);
                    if (principal != null)
                    {
                        context.User = principal;
                    }
                }
                catch
                {
                    // Silenciar errores en rutas públicas
                }
            }

            await _next(context);
        }
    }
}
