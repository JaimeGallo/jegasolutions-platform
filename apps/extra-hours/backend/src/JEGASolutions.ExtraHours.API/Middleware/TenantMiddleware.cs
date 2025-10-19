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

        public TenantMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ITenantContextService tenantContextService)
        {
            // Debug: Log all available claims
            if (context.User?.Identity?.IsAuthenticated == true)
            {
                var allClaims = context.User.Claims.Select(c => $"{c.Type}={c.Value}").ToList();
                Console.WriteLine($"[TenantMiddleware] 🔍 All available claims: {string.Join(", ", allClaims)}");
            }
            else
            {
                Console.WriteLine("[TenantMiddleware] ⚠️ User not authenticated or no identity");
            }

            // Intenta extraer tenant_id del JWT
            var tenantIdClaim = context.User.FindFirst("tenant_id")?.Value;
            var tenantIdClaimAlt = context.User.FindFirst("tenantId")?.Value;

            Console.WriteLine($"[TenantMiddleware] tenant_id claim: {tenantIdClaim}");
            Console.WriteLine($"[TenantMiddleware] tenantId claim: {tenantIdClaimAlt}");

            if (!string.IsNullOrEmpty(tenantIdClaim) && int.TryParse(tenantIdClaim, out int tenantId))
            {
                tenantContextService.SetCurrentTenantId(tenantId);
                Console.WriteLine($"[TenantMiddleware] ✅ Tenant set from JWT: {tenantId}");
            }
            else if (!string.IsNullOrEmpty(tenantIdClaimAlt) && int.TryParse(tenantIdClaimAlt, out int tenantIdAlt))
            {
                tenantContextService.SetCurrentTenantId(tenantIdAlt);
                Console.WriteLine($"[TenantMiddleware] ✅ Tenant set from JWT (alt): {tenantIdAlt}");
            }
            else if (context.Request.Headers.ContainsKey("X-Tenant-Id"))
            {
                var headerTenantId = context.Request.Headers["X-Tenant-Id"].FirstOrDefault();
                if (int.TryParse(headerTenantId, out int headerTenantIdInt))
                {
                    tenantContextService.SetCurrentTenantId(headerTenantIdInt);
                    Console.WriteLine($"[TenantMiddleware] ✅ Tenant set from header: {headerTenantIdInt}");
                }
            }
            else
            {
                // Default tenant ID para backwards compatibility
                tenantContextService.SetCurrentTenantId(1);
                Console.WriteLine("[TenantMiddleware] ⚠️ Using default tenant: 1");
            }

            await _next(context);
        }
    }
}
