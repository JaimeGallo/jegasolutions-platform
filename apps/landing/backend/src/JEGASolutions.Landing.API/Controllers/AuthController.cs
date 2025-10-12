using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using JEGASolutions.Landing.Application.Interfaces;
using JEGASolutions.Landing.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JEGASolutions.Landing.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IAuthService authService,
        ApplicationDbContext context,
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Login de usuario - Para Tenant Dashboard y SSO
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            _logger.LogInformation("🔐 Login attempt for: {Email}, TenantId: {TenantId}",
                request.Email, request.TenantId);

            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { message = "Email y contraseña son requeridos" });
            }

            var token = await _authService.AuthenticateAsync(
                request.Email,
                request.Password,
                request.TenantId);

            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("❌ Login failed for: {Email}", request.Email);
                return Unauthorized(new { message = "Credenciales inválidas" });
            }

            var user = await _authService.GetUserByEmailAsync(request.Email, request.TenantId);

            if (user == null)
            {
                return Unauthorized(new { message = "Usuario no encontrado" });
            }

            _logger.LogInformation("✅ Login successful for: {Email}", request.Email);

            return Ok(new
            {
                token = token,
                user = new
                {
                    id = user.Id,
                    email = user.Email,
                    name = $"{user.FirstName} {user.LastName}".Trim(),
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    role = user.Role,
                    tenantId = user.TenantId
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Error during login for: {Email}", request.Email);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Valida un token JWT
    /// </summary>
    [HttpPost("validate")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> ValidateToken([FromBody] ValidateTokenRequest request)
    {
        try
        {
            var isValid = await _authService.ValidateTokenAsync(request.Token);

            if (!isValid)
            {
                return Unauthorized(new { message = "Token inválido o expirado" });
            }

            var user = await _authService.GetUserFromTokenAsync(request.Token);

            if (user == null)
            {
                return Unauthorized(new { message = "Usuario no encontrado" });
            }

            return Ok(new
            {
                valid = true,
                user = new
                {
                    id = user.Id,
                    email = user.Email,
                    name = $"{user.FirstName} {user.LastName}".Trim(),
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    role = user.Role,
                    tenantId = user.TenantId
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating token");
            return Unauthorized(new { message = "Token inválido" });
        }
    }

    /// <summary>
    /// Obtiene los módulos a los que un usuario tiene acceso - Para SSO
    /// </summary>
    [HttpGet("user-modules/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetUserModules(int userId)
    {
        try
        {
            _logger.LogInformation("📦 Getting modules for user: {UserId}", userId);

            var userModules = await _context.UserModuleAccess
                .Where(uma => uma.UserId == userId && uma.IsActive)
                .Select(uma => new
                {
                    moduleName = uma.ModuleName,
                    role = uma.Role,
                    tenantId = uma.TenantId,
                    isActive = uma.IsActive
                })
                .ToListAsync();

            _logger.LogInformation("✅ Found {Count} modules for user {UserId}", userModules.Count, userId);

            return Ok(userModules);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Error getting modules for user: {UserId}", userId);
            return StatusCode(500, new { message = "Error al obtener módulos del usuario" });
        }
    }

    /// <summary>
    /// Verifica si un usuario tiene acceso a un módulo específico - Para SSO
    /// </summary>
    [HttpGet("check-access")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> CheckModuleAccess(
        [FromQuery] int userId,
        [FromQuery] string moduleName)
    {
        try
        {
            _logger.LogInformation("🔍 Checking access for user {UserId} to module {ModuleName}",
                userId, moduleName);

            var access = await _context.UserModuleAccess
                .FirstOrDefaultAsync(uma =>
                    uma.UserId == userId &&
                    uma.ModuleName == moduleName &&
                    uma.IsActive);

            if (access == null)
            {
                _logger.LogWarning("❌ User {UserId} does not have access to {ModuleName}",
                    userId, moduleName);

                return Ok(new
                {
                    hasAccess = false,
                    message = "Usuario no tiene acceso a este módulo"
                });
            }

            _logger.LogInformation("✅ User {UserId} has {Role} access to {ModuleName}",
                userId, access.Role, moduleName);

            return Ok(new
            {
                hasAccess = true,
                role = access.Role,
                tenantId = access.TenantId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Error checking access for user: {UserId}", userId);
            return StatusCode(500, new { message = "Error al verificar acceso" });
        }
    }
}

// DTOs
public record LoginRequest(string Email, string Password, int? TenantId = null);
public record ValidateTokenRequest(string Token);

