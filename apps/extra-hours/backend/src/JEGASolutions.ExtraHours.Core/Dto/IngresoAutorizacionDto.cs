using System.ComponentModel.DataAnnotations;

namespace JEGASolutions.ExtraHours.API.Dto
{
    public class IngresoAutorizacionDto
    {
        [Required]
        public string EmployeeName { get; set; } = string.Empty;
        [Required]
        public string Date { get; set; } = string.Empty;
        [Required]
        public string EstimatedEntryTime { get; set; } = string.Empty;
        [Required]
        public string EstimatedExitTime { get; set; } = string.Empty;
        [Required]
        public string TaskDescription { get; set; } = string.Empty;
        [Required]
        public string ManagerName { get; set; } = string.Empty;
        [Required]
        public string ManagerEmail { get; set; } = string.Empty;
    }
}
