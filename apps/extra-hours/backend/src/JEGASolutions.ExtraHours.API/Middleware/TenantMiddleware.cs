using JEGASolutions.ExtraHours.Core.Services;
using System.Security.Claims;

namespace JEGASolutions.ExtraHours.API.Middleware
{
    /// <summary>
    /// Middleware to extract tenant ID from JWT token and set it in the tenant context
    /// </summary>
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TenantMiddleware> _logger;

        // Rutas que NO requieren tenant context (públicas/health checks)
        private static readonly string[] ExcludedPaths = new[]
        {
            "/health",
            "/",
            "/swagger",
            "/api/auth/login",
            "/api/auth/register",
            "/api/auth/refresh"
        };

        public TenantMiddleware(RequestDelegate next, ILogger<TenantMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, ITenantContextService tenantContextService)
        {
            // ✅ SKIP middleware para rutas públicas/health checks
            var path = context.Request.Path.Value?.ToLower() ?? "";
            if (ExcludedPaths.Any(excluded => path.StartsWith(excluded.ToLower())))
            {
                // No procesar tenant para health checks, swagger, etc.
                await _next(context);
                return;
            }

            // Check if user is authenticated
            if (context.User?.Identity?.IsAuthenticated != true)
            {
                _logger.LogWarning("[TenantMiddleware] ⚠️ User not authenticated on path: {Path}", path);
            }
            else
            {
                _logger.LogDebug("[TenantMiddleware] ✅ User authenticated on path: {Path}", path);

                // Log all claims for debugging
                var claims = context.User.Claims.Select(c => $"{c.Type}={c.Value}").ToList();
                _logger.LogDebug("[TenantMiddleware] Claims: {Claims}", string.Join(", ", claims));
            }

            // Try to extract tenant_id from JWT claims (primary)
            var tenantIdClaim = context.User.FindFirst("tenant_id")?.Value;
            var tenantIdClaimAlt = context.User.FindFirst("tenantId")?.Value;

            _logger.LogDebug("[TenantMiddleware] tenant_id claim: {TenantId}", tenantIdClaim ?? "null");
            _logger.LogDebug("[TenantMiddleware] tenantId claim: {TenantIdAlt}", tenantIdClaimAlt ?? "null");

            if (!string.IsNullOrEmpty(tenantIdClaim) && int.TryParse(tenantIdClaim, out int tenantId))
            {
                tenantContextService.SetCurrentTenantId(tenantId);
                _logger.LogInformation("[TenantMiddleware] ✅ Tenant set from JWT (tenant_id): {TenantId}", tenantId);

                // Verify it was set correctly
                if (tenantContextService.HasTenantId())
                {
                    _logger.LogDebug("[TenantMiddleware] ✅ Verified tenant context: {TenantId}", tenantContextService.GetCurrentTenantId());
                }
                else
                {
                    _logger.LogError("[TenantMiddleware] ❌ Failed to verify tenant context after setting!");
                }
            }
            else if (!string.IsNullOrEmpty(tenantIdClaimAlt) && int.TryParse(tenantIdClaimAlt, out int tenantIdAlt))
            {
                tenantContextService.SetCurrentTenantId(tenantIdAlt);
                _logger.LogInformation("[TenantMiddleware] ✅ Tenant set from JWT (tenantId): {TenantId}", tenantIdAlt);

                // Verify it was set correctly
                if (tenantContextService.HasTenantId())
                {
                    _logger.LogDebug("[TenantMiddleware] ✅ Verified tenant context: {TenantId}", tenantContextService.GetCurrentTenantId());
                }
                else
                {
                    _logger.LogError("[TenantMiddleware] ❌ Failed to verify tenant context after setting!");
                }
            }
            // Fallback: Check for tenant in header
            else if (context.Request.Headers.ContainsKey("X-Tenant-Id"))
            {
                var headerTenantId = context.Request.Headers["X-Tenant-Id"].FirstOrDefault();
                if (int.TryParse(headerTenantId, out int headerTenantIdInt))
                {
                    tenantContextService.SetCurrentTenantId(headerTenantIdInt);
                    _logger.LogInformation("[TenantMiddleware] ✅ Tenant set from header: {TenantId}", headerTenantIdInt);
                }
                else
                {
                    tenantContextService.SetCurrentTenantId(1);
                    _logger.LogWarning("[TenantMiddleware] ⚠️ Invalid header tenant, using default: 1");
                }
            }
            else
            {
                // Default tenant ID for backwards compatibility
                tenantContextService.SetCurrentTenantId(1);
                _logger.LogWarning("[TenantMiddleware] ⚠️ Using default tenant: 1 (no claims or header found) on path: {Path}", path);

                // Verify it was set correctly
                if (tenantContextService.HasTenantId())
                {
                    _logger.LogDebug("[TenantMiddleware] ✅ Verified default tenant context: {TenantId}", tenantContextService.GetCurrentTenantId());
                }
                else
                {
                    _logger.LogError("[TenantMiddleware] ❌ Failed to verify default tenant context after setting!");
                }
            }

            await _next(context);
        }
    }
}
