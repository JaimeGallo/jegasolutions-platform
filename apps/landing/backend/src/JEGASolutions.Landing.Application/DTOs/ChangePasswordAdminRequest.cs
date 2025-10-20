namespace JEGASolutions.Landing.Application.DTOs;

public class ChangePasswordAdminRequest
{
    public required int UserId { get; set; }
    public required string NewPassword { get; set; }
}
