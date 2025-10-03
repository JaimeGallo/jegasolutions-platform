using System.ComponentModel.DataAnnotations;
using JEGASolutions.ReportBuilder.Core.Entities.Models;

namespace JEGASolutions.ReportBuilder.Core.Dto
{
    /// <summary>
    /// DTO para listar plantillas consolidadas
    /// </summary>
    public class ConsolidatedTemplateListDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Period { get; set; } = string.Empty;
        public DateTime? Deadline { get; set; }
        public int TotalSections { get; set; }
        public int CompletedSections { get; set; }
        public int ProgressPercentage { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedByUserName { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO con detalles completos de plantilla consolidada
    /// </summary>
    public class ConsolidatedTemplateDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Period { get; set; } = string.Empty;
        public DateTime? Deadline { get; set; }
        public ConsolidatedTemplateConfiguration Configuration { get; set; } = new();
        public int CreatedByUserId { get; set; }
        public string CreatedByUserName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<ConsolidatedTemplateSectionDto> Sections { get; set; } = new();
        public int TotalSections => Sections.Count;
        public int CompletedSections => Sections.Count(s => s.Status == "completed");
        public int ProgressPercentage => TotalSections > 0 ? (CompletedSections * 100 / TotalSections) : 0;
    }

    /// <summary>
    /// DTO para crear nueva plantilla consolidada (Admin only)
    /// </summary>
    public class ConsolidatedTemplateCreateDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Period { get; set; } = string.Empty;

        public DateTime? Deadline { get; set; }

        public ConsolidatedTemplateConfiguration Configuration { get; set; } = new();

        /// <summary>
        /// Secciones a crear con sus asignaciones a áreas
        /// </summary>
        [Required]
        [MinLength(1, ErrorMessage = "Debe incluir al menos una sección")]
        public List<ConsolidatedTemplateSectionCreateDto> Sections { get; set; } = new();
    }

    /// <summary>
    /// DTO para actualizar plantilla consolidada
    /// </summary>
    public class ConsolidatedTemplateUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Period { get; set; } = string.Empty;

        public DateTime? Deadline { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = string.Empty; // draft, in_progress, completed, archived

        public ConsolidatedTemplateConfiguration Configuration { get; set; } = new();
    }

    /// <summary>
    /// DTO para sección de plantilla consolidada
    /// </summary>
    public class ConsolidatedTemplateSectionDto
    {
        public int Id { get; set; }
        public int ConsolidatedTemplateId { get; set; }
        public int AreaId { get; set; }
        public string AreaName { get; set; } = string.Empty;
        public string SectionTitle { get; set; } = string.Empty;
        public string SectionDescription { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int Order { get; set; }
        public DateTime? SectionDeadline { get; set; }
        public DateTime? AssignedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int? CompletedByUserId { get; set; }
        public string? CompletedByUserName { get; set; }
        public SectionConfiguration SectionConfiguration { get; set; } = new();
        public object? SectionData { get; set; } // JSON data
        public bool IsOverdue => SectionDeadline.HasValue && SectionDeadline.Value < DateTime.UtcNow && Status != "completed";
    }

    /// <summary>
    /// DTO para crear sección (usado al crear plantilla consolidada)
    /// </summary>
    public class ConsolidatedTemplateSectionCreateDto
    {
        [Required]
        public int AreaId { get; set; }

        [Required]
        [StringLength(200)]
        public string SectionTitle { get; set; } = string.Empty;

        [StringLength(500)]
        public string SectionDescription { get; set; } = string.Empty;

        [Required]
        public int Order { get; set; }

        public DateTime? SectionDeadline { get; set; }

        public SectionConfiguration SectionConfiguration { get; set; } = new();
    }

    /// <summary>
    /// DTO para actualizar el contenido de una sección (Usuario de área)
    /// </summary>
    public class ConsolidatedTemplateSectionUpdateContentDto
    {
        [Required]
        public int SectionId { get; set; }

        [Required]
        public object SectionData { get; set; } = new();

        /// <summary>
        /// Si true, marca la sección como completada
        /// </summary>
        public bool MarkAsCompleted { get; set; } = false;
    }

    /// <summary>
    /// DTO para cambiar estado de sección (Admin)
    /// </summary>
    public class ConsolidatedTemplateSectionUpdateStatusDto
    {
        [Required]
        public int SectionId { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = string.Empty; // pending, assigned, in_progress, completed, reviewed
    }

    /// <summary>
    /// DTO para "Mis Tareas" - secciones asignadas al área del usuario
    /// </summary>
    public class MyTaskDto
    {
        public int SectionId { get; set; }
        public int ConsolidatedTemplateId { get; set; }
        public string ConsolidatedTemplateName { get; set; } = string.Empty;
        public string Period { get; set; } = string.Empty;
        public string SectionTitle { get; set; } = string.Empty;
        public string SectionDescription { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime? SectionDeadline { get; set; }
        public DateTime? AssignedAt { get; set; }
        public bool IsOverdue { get; set; }
        public int DaysUntilDeadline { get; set; }
        public string Priority { get; set; } = string.Empty; // high, medium, low
    }

    /// <summary>
    /// DTO para consolidación final del informe (Admin)
    /// </summary>
    public class ConsolidateReportRequestDto
    {
        [Required]
        public int ConsolidatedTemplateId { get; set; }

        /// <summary>
        /// Formato de exportación: pdf, docx, etc.
        /// </summary>
        [Required]
        public string ExportFormat { get; set; } = "pdf";

        /// <summary>
        /// Opciones adicionales de exportación
        /// </summary>
        public Dictionary<string, object> ExportOptions { get; set; } = new();
    }

    /// <summary>
    /// DTO con estadísticas de progreso (Dashboard Admin)
    /// </summary>
    public class ConsolidatedTemplateStatsDto
    {
        public int TotalTemplates { get; set; }
        public int DraftTemplates { get; set; }
        public int InProgressTemplates { get; set; }
        public int CompletedTemplates { get; set; }
        public int OverdueSections { get; set; }
        public int PendingSections { get; set; }
        public List<AreaProgressDto> AreaProgress { get; set; } = new();
    }

    /// <summary>
    /// DTO de progreso por área
    /// </summary>
    public class AreaProgressDto
    {
        public int AreaId { get; set; }
        public string AreaName { get; set; } = string.Empty;
        public int TotalAssignedSections { get; set; }
        public int CompletedSections { get; set; }
        public int ProgressPercentage { get; set; }
        public int OverdueSections { get; set; }
    }
}

