using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JEGASolutions.ExtraHours.Core.Entities;

namespace JEGASolutions.ExtraHours.Core.Entities.Models
{
    [Table("employees")]
    public class Employee : TenantEntity
    {
        [Key]
        [Column("id")]
        public long id { get; set; }

        [ForeignKey("id")]
        public User? User { get; set; }

        [Column("position")]
        [MaxLength(255)]
        public string? position { get; set; }

        [Column("manager_id")]
        public long? manager_id { get; set; }

        [ForeignKey("manager_id")]
        public Manager? manager { get; set; }

        // Propiedades de conveniencia que vienen de User (no están en la tabla employees)
        [NotMapped]
        public string name => User?.name ?? string.Empty;

        [NotMapped]
        public double? salary => 0; // El salario no existe en el esquema actual

    }
}
