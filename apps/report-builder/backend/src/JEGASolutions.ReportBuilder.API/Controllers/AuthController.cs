using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using JEGASolutions.ReportBuilder.Core.Dto;
using JEGASolutions.ReportBuilder.Core.Interfaces;
using System.Security.Claims;

namespace JEGASolutions.ReportBuilder.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IAuthService authService,
            IUserRepository userRepository,
            ILogger<AuthController> logger)
        {
            _authService = authService;
            _userRepository = userRepository;
            _logger = logger;
        }

        /// <summary>
        /// Login with email and password
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await _authService.LoginAsync(request.Email, request.Password);
                
                _logger.LogInformation("User {Email} logged in successfully", request.Email);
                
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Login failed for {Email}: {Message}", request.Email, ex.Message);
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for {Email}", request.Email);
                return StatusCode(500, new { message = "An error occurred during login" });
            }
        }

        /// <summary>
        /// Verify current token and get user info
        /// </summary>
        [HttpGet("verify")]
        [Authorize]
        public async Task<ActionResult<UserDto>> VerifyToken()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? 
                             User.FindFirst("sub")?.Value;
                
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "Invalid token" });
                }

                var user = await _userRepository.GetByIdAsync(int.Parse(userId));
                
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    Role = user.Role,
                    TenantId = user.TenantId
                };

                return Ok(userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying token");
                return StatusCode(500, new { message = "An error occurred during token verification" });
            }
        }

        /// <summary>
        /// Refresh token (optional endpoint for future implementation)
        /// </summary>
        [HttpPost("refresh")]
        [Authorize]
        public async Task<ActionResult<AuthResponse>> RefreshToken()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? 
                             User.FindFirst("sub")?.Value;
                
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "Invalid token" });
                }

                var user = await _userRepository.GetByIdAsync(int.Parse(userId));
                
                if (user == null || !user.IsActive)
                {
                    return Unauthorized(new { message = "User not found or inactive" });
                }

                // Generate new token
                var token = _authService.GenerateJwtToken(user);

                var response = new AuthResponse
                {
                    Token = token,
                    User = new UserDto
                    {
                        Id = user.Id,
                        Email = user.Email,
                        FullName = user.FullName,
                        Role = user.Role,
                        TenantId = user.TenantId
                    }
                };

                _logger.LogInformation("Token refreshed for user {UserId}", userId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing token");
                return StatusCode(500, new { message = "An error occurred during token refresh" });
            }
        }
    }
}

