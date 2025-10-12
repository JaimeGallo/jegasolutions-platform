using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace JEGASolutions.ExtraHours.API.Middleware
{
    /// <summary>
    /// Middleware que verifica que el usuario tenga acceso al módulo Extra Hours
    /// Valida contra el Landing API usando el SSO
    /// </summary>
    public class ModuleAccessMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ModuleAccessMiddleware> _logger;
        private readonly string _landingApiBaseUrl;
        private readonly string _moduleName;

        public ModuleAccessMiddleware(
            RequestDelegate next,
            ILogger<ModuleAccessMiddleware> logger,
            IConfiguration configuration)
        {
            _next = next;
            _logger = logger;
            _landingApiBaseUrl = configuration["LandingApi:BaseUrl"] 
                ?? "https://jegasolutions-platform.onrender.com";
            _moduleName = configuration["LandingApi:ModuleName"] 
                ?? "extra-hours";
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Rutas públicas que no requieren validación de módulo
            var path = context.Request.Path.Value?.ToLower() ?? "";
            
            if (path.Contains("/swagger") || 
                path.Contains("/health") ||
                path == "/" ||
                !context.User.Identity?.IsAuthenticated == true)
            {
                await _next(context);
                return;
            }

            // Extraer userId del token JWT
            var userIdClaim = context.User.FindFirst("userId") ?? 
                             context.User.FindFirst(ClaimTypes.NameIdentifier) ??
                             context.User.FindFirst("sub");

            if (userIdClaim == null)
            {
                _logger.LogWarning("⚠️ No se encontró userId en el token JWT");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new 
                { 
                    message = "Token inválido: falta userId" 
                });
                return;
            }

            var userId = userIdClaim.Value;

            try
            {
                // Verificar acceso al módulo llamando al Landing API
                using var httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(_landingApiBaseUrl);
                
                var checkAccessUrl = $"/api/auth/check-access?userId={userId}&moduleName={_moduleName}";
                
                _logger.LogInformation("🔍 Verificando acceso para userId={UserId} a módulo={ModuleName}", 
                    userId, _moduleName);

                var response = await httpClient.GetAsync(checkAccessUrl);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("❌ Error al verificar acceso: {StatusCode}", response.StatusCode);
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsJsonAsync(new 
                    { 
                        message = "No se pudo verificar acceso al módulo" 
                    });
                    return;
                }

                var accessResult = await response.Content.ReadFromJsonAsync<ModuleAccessResult>();

                if (accessResult == null || !accessResult.HasAccess)
                {
                    _logger.LogWarning("❌ Usuario {UserId} no tiene acceso a {ModuleName}", 
                        userId, _moduleName);
                    
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsJsonAsync(new 
                    { 
                        message = "No tienes acceso a este módulo. Contacta al administrador." 
                    });
                    return;
                }

                _logger.LogInformation("✅ Usuario {UserId} tiene acceso como {Role} a {ModuleName}", 
                    userId, accessResult.Role, _moduleName);

                // Agregar información de acceso a los claims del contexto
                var claims = new List<Claim>
                {
                    new Claim("module_role", accessResult.Role ?? "employee"),
                    new Claim("module_name", _moduleName)
                };

                var appIdentity = new ClaimsIdentity(claims);
                context.User.AddIdentity(appIdentity);

                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Error al verificar acceso al módulo");
                
                // En caso de error, permitir acceso pero loggear (fail-open para evitar bloqueos en producción)
                // TODO: Cambiar a fail-closed cuando el SSO esté estable
                _logger.LogWarning("⚠️ Permitiendo acceso por error en verificación (fail-open)");
                await _next(context);
            }
        }

        private class ModuleAccessResult
        {
            public bool HasAccess { get; set; }
            public string? Role { get; set; }
            public int TenantId { get; set; }
            public string? Message { get; set; }
        }
    }
}

