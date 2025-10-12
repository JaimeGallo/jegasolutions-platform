using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using JEGASolutions.Landing.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace JEGASolutions.Landing.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IAuthService authService,
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Login de usuario - Para Tenant Dashboard
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
}

// DTOs
public record LoginRequest(string Email, string Password, int? TenantId = null);
public record ValidateTokenRequest(string Token);

