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
            
            if (tenantIdClaim != null && int.TryParse(tenantIdClaim.Value, out int tenantId))
            {
                tenantContextService.SetCurrentTenantId(tenantId);
            }
            else
            {
                // For development/testing purposes, you might want to set a default tenant
                // In production, this should throw an exception
                if (context.Request.Headers.ContainsKey("X-Tenant-Id"))
                {
                    var headerTenantId = context.Request.Headers["X-Tenant-Id"].FirstOrDefault();
                    if (int.TryParse(headerTenantId, out int headerTenantIdInt))
                    {
                        tenantContextService.SetCurrentTenantId(headerTenantIdInt);
                    }
                }
            }

            await _next(context);
        }
    }
}
