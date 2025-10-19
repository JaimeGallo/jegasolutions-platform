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
            // Extract tenant ID from JWT token claims
            var tenantIdClaim = context.User.FindFirst("tenant_id");

            // ✅ AGREGAR LOG PARA DEBUGGING
            Console.WriteLine($"[TenantMiddleware] tenant_id claim: {tenantIdClaim?.Value}");

            if (tenantIdClaim != null && int.TryParse(tenantIdClaim.Value, out int tenantId))
            {
                tenantContextService.SetCurrentTenantId(tenantId);
                Console.WriteLine($"[TenantMiddleware] Tenant context set to: {tenantId}");
            }
            else
            {
                // Para backwards compatibility con tokens del Landing
                if (context.Request.Headers.ContainsKey("X-Tenant-Id"))
                {
                    var headerTenantId = context.Request.Headers["X-Tenant-Id"].FirstOrDefault();
                    if (int.TryParse(headerTenantId, out int headerTenantIdInt))
                    {
                        tenantContextService.SetCurrentTenantId(headerTenantIdInt);
                        Console.WriteLine($"[TenantMiddleware] Tenant from header: {headerTenantIdInt}");
                    }
                }
                else
                {
                    // Buscar también "tenantId" (sin guion bajo) por compatibilidad con Landing
                    var alternativeClaim = context.User.FindFirst("tenantId")?.Value;
                    Console.WriteLine($"[TenantMiddleware] tenantId claim: {alternativeClaim}");

                    if (alternativeClaim != null && int.TryParse(alternativeClaim, out int altTenantId))
                    {
                        tenantContextService.SetCurrentTenantId(altTenantId);
                        Console.WriteLine($"[TenantMiddleware] Tenant from 'tenantId' claim: {altTenantId}");
                    }
                    else
                    {
                        // Default tenant ID para compatibilidad
                        tenantContextService.SetCurrentTenantId(1);
                        Console.WriteLine("[TenantMiddleware] Using default tenant: 1");
                    }
                }
            }

            await _next(context);
        }
    }
}
