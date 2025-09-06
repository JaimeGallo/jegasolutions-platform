using JEGASolutions.Landing.Application.Interfaces;
using JEGASolutions.Landing.Domain.Entities;

namespace JEGASolutions.Landing.API.Middleware;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TenantMiddleware> _logger;

    public TenantMiddleware(RequestDelegate next, ILogger<TenantMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Extract subdomain from Host header
        var host = context.Request.Host.Host;
        var subdomain = ExtractSubdomain(host);

        if (!string.IsNullOrEmpty(subdomain))
        {
            // Get tenant service from DI container
            var tenantService = context.RequestServices.GetRequiredService<ITenantService>();

            try
            {
                var tenant = await tenantService.GetTenantBySubdomainAsync(subdomain);

                if (tenant != null)
                {
                    if (tenant.IsActive)
                    {
                        // Store tenant information in HttpContext for use in controllers
                        context.Items["TenantId"] = tenant.Id;
                        context.Items["TenantName"] = tenant.CompanyName;
                        context.Items["Subdomain"] = tenant.Subdomain;

                        _logger.LogInformation("Tenant resolved: {TenantName} (ID: {TenantId}) for subdomain: {Subdomain}",
                            tenant.CompanyName, tenant.Id, subdomain);
                    }
                    else
                    {
                        _logger.LogWarning("Inactive tenant accessed: {Subdomain}", subdomain);
                        context.Response.StatusCode = 403;
                        await context.Response.WriteAsync("Tenant account is inactive.");
                        return;
                    }
                }
                else
                {
                    _logger.LogWarning("Tenant not found for subdomain: {Subdomain}", subdomain);
                    // Continue without tenant (for main domain access)
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resolving tenant for subdomain: {Subdomain}", subdomain);
                // Continue without tenant to prevent breaking the request
            }
        }

        await _next(context);
    }

    private string? ExtractSubdomain(string host)
    {
        try
        {
            // Remove port if present
            if (host.Contains(':'))
            {
                host = host.Split(':')[0];
            }

            // Skip localhost and IP addresses
            if (host.Equals("localhost", StringComparison.OrdinalIgnoreCase) ||
                System.Net.IPAddress.TryParse(host, out _))
            {
                return null;
            }

            var parts = host.Split('.');

            // Need at least 3 parts for subdomain (subdomain.domain.com)
            if (parts.Length < 3)
            {
                return null;
            }

            // Skip common subdomains that are not tenant subdomains
            var firstPart = parts[0].ToLower();
            if (firstPart == "www" || firstPart == "api" || firstPart == "admin")
            {
                return null;
            }

            return parts[0];
        }
        catch
        {
            return null;
        }
    }
}

// Extension method for easier registration
public static class TenantMiddlewareExtensions
{
    public static IApplicationBuilder UseTenantMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<TenantMiddleware>();
    }
}