using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JEGASolutions.ExtraHours.Core.Entities;

namespace JEGASolutions.ExtraHours.Core.Entities.Models
{
    [Table("compensationrequests")]
    public class CompensationRequest : TenantEntity
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Column("employeeid")]
        public long EmployeeId { get; set; }

        [Column("workdate")]
        public DateTime WorkDate { get; set; } // Día trabajado (domingo/festivo)

        [Column("requestedcompensationdate")]
        public DateTime RequestedCompensationDate { get; set; } // Día solicitado como compensación

        [Column("status")]
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected - valor por defecto

        [Column("justification")]
        public string? Justification { get; set; } // Motivo de rechazo o comentario de aprobación

        [Column("approvedbyid")]
        public long? ApprovedById { get; set; } // Manager o Superusuario que decide

        [Column("requestedat")]
        public DateTime RequestedAt { get; set; }

        [Column("decidedat")]
        public DateTime? DecidedAt { get; set; }

        // Relaciones de navegación (opcional, para EF Core)
        [ForeignKey("EmployeeId")]
        public Employee? Employee { get; set; }

        [ForeignKey("ApprovedById")]
        public User? ApprovedBy { get; set; }
    }
}
