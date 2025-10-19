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
            // Check if user is authenticated
            if (context.User?.Identity?.IsAuthenticated != true)
            {
                Console.WriteLine("[TenantMiddleware] ⚠️ User not authenticated or no identity");
            }

            // Try to extract tenant_id from JWT claims (primary)
            var tenantIdClaim = context.User.FindFirst("tenant_id")?.Value;
            var tenantIdClaimAlt = context.User.FindFirst("tenantId")?.Value;

            Console.WriteLine($"[TenantMiddleware] tenant_id claim: {tenantIdClaim ?? "null"}");
            Console.WriteLine($"[TenantMiddleware] tenantId claim: {tenantIdClaimAlt ?? "null"}");

            if (!string.IsNullOrEmpty(tenantIdClaim) && int.TryParse(tenantIdClaim, out int tenantId))
            {
                tenantContextService.SetCurrentTenantId(tenantId);
                Console.WriteLine($"[TenantMiddleware] ✅ Tenant set from JWT (tenant_id): {tenantId}");
            }
            else if (!string.IsNullOrEmpty(tenantIdClaimAlt) && int.TryParse(tenantIdClaimAlt, out int tenantIdAlt))
            {
                tenantContextService.SetCurrentTenantId(tenantIdAlt);
                Console.WriteLine($"[TenantMiddleware] ✅ Tenant set from JWT (tenantId): {tenantIdAlt}");
            }
            // Fallback: Check for tenant in header
            else if (context.Request.Headers.ContainsKey("X-Tenant-Id"))
            {
                var headerTenantId = context.Request.Headers["X-Tenant-Id"].FirstOrDefault();
                if (int.TryParse(headerTenantId, out int headerTenantIdInt))
                {
                    tenantContextService.SetCurrentTenantId(headerTenantIdInt);
                    Console.WriteLine($"[TenantMiddleware] ✅ Tenant set from header: {headerTenantIdInt}");
                }
                else
                {
                    tenantContextService.SetCurrentTenantId(1);
                    Console.WriteLine("[TenantMiddleware] ⚠️ Invalid header tenant, using default: 1");
                }
            }
            else
            {
                // Default tenant ID for backwards compatibility
                tenantContextService.SetCurrentTenantId(1);
                Console.WriteLine("[TenantMiddleware] ⚠️ Using default tenant: 1 (no claims or header found)");
            }

            await _next(context);
        }
    }
}
