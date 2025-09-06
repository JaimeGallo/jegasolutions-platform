using System.ComponentModel.DataAnnotations;

namespace JEGASolutions.Landing.Application.DTOs;

public class UpdatePaymentStatusDto
{
    [Required]
    [StringLength(20)]
    public string Status { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Reason { get; set; } // Razón del cambio de estado (opcional)
}