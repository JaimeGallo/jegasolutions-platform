using System;
using System.ComponentModel.DataAnnotations;

namespace JEGASolutions.ExtraHours.API.Dto
{
    public class CompensationRequestDto
    {
        [Required(ErrorMessage = "El ID del empleado es requerido")]
        [Range(1, long.MaxValue, ErrorMessage = "El ID del empleado debe ser mayor a 0")]
        public long EmployeeId { get; set; }

        [Required(ErrorMessage = "La fecha de trabajo es requerida")]
        [MinLength(1, ErrorMessage = "La fecha de trabajo no puede estar vacía")]
        public string WorkDate { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de compensación solicitada es requerida")]
        [MinLength(1, ErrorMessage = "La fecha de compensación no puede estar vacía")]
        public string RequestedCompensationDate { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "La justificación no puede exceder 500 caracteres")]
        public string? Justification { get; set; }
    }
}