using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using JEGASolutions.ReportBuilder.Core.Entities;

namespace JEGASolutions.ReportBuilder.Core.Entities.Models
{
    /// <summary>
    /// Representa una plantilla consolidada generada por el administrador
    /// que combina secciones de diferentes áreas para crear un informe completo.
    /// MULTI-TENANT: Cada tenant tiene sus propias plantillas consolidadas.
    /// </summary>
    [Table("consolidated_templates")]
    public class ConsolidatedTemplate : TenantEntity
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        [Column("description")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Estado de la plantilla consolidada
        /// </summary>
        [Required]
        [StringLength(20)]
        [Column("status")]
        public string Status { get; set; } = "draft"; // draft, in_progress, completed, archived

        /// <summary>
        /// Período al que corresponde el informe consolidado
        /// </summary>
        [Required]
        [StringLength(50)]
        [Column("period")]
        public string Period { get; set; } = string.Empty; // Ej: "Abril 2025"

        /// <summary>
        /// Fecha límite para completar todas las secciones
        /// </summary>
        [Column("deadline")]
        public DateTime? Deadline { get; set; }

        /// <summary>
        /// Configuración JSON de la plantilla consolidada
        /// </summary>
        [Column("configuration_json", TypeName = "text")]
        public string ConfigurationJson { get; set; } = string.Empty;

        /// <summary>
        /// ID del usuario administrador que creó la plantilla
        /// </summary>
        [Required]
        [Column("created_by_user_id")]
        public int CreatedByUserId { get; set; }

        // Relación con el usuario creador
        [ForeignKey("CreatedByUserId")]
        public virtual User? CreatedByUser { get; set; }

        // Navegación a las secciones
        public virtual ICollection<ConsolidatedTemplateSection> Sections { get; set; } = new List<ConsolidatedTemplateSection>();

        // Propiedades no mapeadas para trabajar con objetos
        [NotMapped]
        public ConsolidatedTemplateConfiguration Configuration
        {
            get => string.IsNullOrEmpty(ConfigurationJson)
                   ? new ConsolidatedTemplateConfiguration()
                   : JsonSerializer.Deserialize<ConsolidatedTemplateConfiguration>(ConfigurationJson) ?? new ConsolidatedTemplateConfiguration();
            set => ConfigurationJson = JsonSerializer.Serialize(value);
        }
    }

    /// <summary>
    /// Representa una sección específica de una plantilla consolidada
    /// que será asignada a un área particular.
    /// MULTI-TENANT: Aislamiento automático por TenantId.
    /// </summary>
    [Table("consolidated_template_sections")]
    public class ConsolidatedTemplateSection : TenantEntity
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("consolidated_template_id")]
        public int ConsolidatedTemplateId { get; set; }

        [ForeignKey("ConsolidatedTemplateId")]
        public virtual ConsolidatedTemplate? ConsolidatedTemplate { get; set; }

        [Required]
        [Column("area_id")]
        public int AreaId { get; set; }

        [ForeignKey("AreaId")]
        public virtual Area? Area { get; set; }

        [Required]
        [StringLength(200)]
        [Column("section_title")]
        public string SectionTitle { get; set; } = string.Empty;

        [StringLength(500)]
        [Column("section_description")]
        public string SectionDescription { get; set; } = string.Empty;

        /// <summary>
        /// Estado de la sección
        /// </summary>
        [Required]
        [StringLength(20)]
        [Column("status")]
        public string Status { get; set; } = "pending"; // pending, assigned, in_progress, completed, reviewed

        /// <summary>
        /// Orden de la sección en el informe final
        /// </summary>
        [Column("order")]
        public int Order { get; set; } = 1;

        /// <summary>
        /// Fecha límite específica para esta sección
        /// </summary>
        [Column("section_deadline")]
        public DateTime? SectionDeadline { get; set; }

        /// <summary>
        /// Fecha cuando se asignó la sección al área
        /// </summary>
        [Column("assigned_at")]
        public DateTime? AssignedAt { get; set; }

        /// <summary>
        /// Fecha cuando se completó la sección
        /// </summary>
        [Column("completed_at")]
        public DateTime? CompletedAt { get; set; }

        /// <summary>
        /// ID del usuario que completó la sección
        /// </summary>
        [Column("completed_by_user_id")]
        public int? CompletedByUserId { get; set; }

        [ForeignKey("CompletedByUserId")]
        public virtual User? CompletedByUser { get; set; }

        /// <summary>
        /// Configuración específica de la sección
        /// </summary>
        [Column("section_configuration_json", TypeName = "text")]
        public string SectionConfigurationJson { get; set; } = string.Empty;

        /// <summary>
        /// Contenido de la sección completado por el usuario, en formato JSON.
        /// </summary>
        [Column("section_data_json", TypeName = "text")]
        public string? SectionDataJson { get; set; }

        // Propiedades no mapeadas
        [NotMapped]
        public SectionConfiguration SectionConfiguration
        {
            get => string.IsNullOrEmpty(SectionConfigurationJson)
                   ? new SectionConfiguration()
                   : JsonSerializer.Deserialize<SectionConfiguration>(SectionConfigurationJson) ?? new SectionConfiguration();
            set => SectionConfigurationJson = JsonSerializer.Serialize(value);
        }

        [NotMapped]
        public JsonDocument? SectionData
        {
            get => string.IsNullOrEmpty(SectionDataJson)
                ? null
                : JsonDocument.Parse(SectionDataJson);

            set => SectionDataJson = value?.RootElement.ToString();
        }
    }

    /// <summary>
    /// Configuración de la plantilla consolidada
    /// </summary>
    public class ConsolidatedTemplateConfiguration
    {
        public string TemplateType { get; set; } = "consolidated";
        public string Version { get; set; } = "1.0.0";
        public string SourceType { get; set; } = "manual"; // manual, pdf, reports
        public string SourceFileName { get; set; } = string.Empty;
        public Dictionary<string, object> AnalysisMetadata { get; set; } = new();
        public List<string> RequiredAreas { get; set; } = new();
        public Dictionary<string, object> GlobalSettings { get; set; } = new();
        public ReportFormatSettings FormatSettings { get; set; } = new();
    }

    /// <summary>
    /// Configuración específica de una sección
    /// </summary>
    public class SectionConfiguration
    {
        public string SectionType { get; set; } = "standard";
        public List<string> RequiredComponents { get; set; } = new();
        public List<ComponentConfiguration> Components { get; set; } = new();
        public Dictionary<string, object> DisplayOptions { get; set; } = new();
        public bool IsRequired { get; set; } = true;
        public int MaxLength { get; set; } = 0; // 0 = sin límite
        public string Instructions { get; set; } = string.Empty;
    }

    /// <summary>
    /// Configuración de un componente específico
    /// </summary>
    public class ComponentConfiguration
    {
        public string Type { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool Required { get; set; } = true;
        public int Order { get; set; }
        public Dictionary<string, object> Configuration { get; set; } = new();
    }

    /// <summary>
    /// Configuración del formato del reporte
    /// </summary>
    public class ReportFormatSettings
    {
        public List<string> ExportFormats { get; set; } = new() { "pdf", "docx" };
        public string ColorScheme { get; set; } = "corporate";
        public bool IncludeTableOfContents { get; set; } = true;
        public bool IncludeExecutiveSummary { get; set; } = true;
    }
}

