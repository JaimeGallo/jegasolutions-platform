using System.ComponentModel.DataAnnotations;

namespace JEGASolutions.ReportBuilder.Core.Dto
{
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;
    }

    public class AuthResponse
    {
        public string Token { get; set; } = string.Empty;
        public UserDto User { get; set; } = null!;
    }

    public class UserDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int TenantId { get; set; }
    }

    public class TokenRequest
    {
        [Required]
        public string Token { get; set; } = string.Empty;
    }
}

