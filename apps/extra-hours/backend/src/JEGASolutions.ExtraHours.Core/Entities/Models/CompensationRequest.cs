using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JEGASolutions.ExtraHours.Core.Entities;

namespace JEGASolutions.ExtraHours.Core.Entities.Models
{
    [Table("compensation_requests")]
    public class CompensationRequest : TenantEntity
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Column("employee_id")]
        public long EmployeeId { get; set; }

        [Column("work_date")]
        public DateTime WorkDate { get; set; } // Día trabajado (domingo/festivo)

        [Column("requested_compensation_date")]
        public DateTime RequestedCompensationDate { get; set; } // Día solicitado como compensación

        [Column("status")]
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected - valor por defecto

        [Column("justification")]
        public string? Justification { get; set; } // Motivo de rechazo o comentario de aprobación

        [Column("approved_by_id")]
        public long? ApprovedById { get; set; } // Manager o Superusuario que decide

        [Column("requested_at")]
        public DateTime RequestedAt { get; set; }

        [Column("decided_at")]
        public DateTime? DecidedAt { get; set; }

        // Relaciones de navegación (opcional, para EF Core)
        [ForeignKey("EmployeeId")]
        public Employee? Employee { get; set; }

        [ForeignKey("ApprovedById")]
        public User? ApprovedBy { get; set; }
    }
}
