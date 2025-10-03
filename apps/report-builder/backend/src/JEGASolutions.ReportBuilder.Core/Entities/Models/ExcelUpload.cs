using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JEGASolutions.ReportBuilder.Core.Entities;

namespace JEGASolutions.ReportBuilder.Core.Entities.Models
{
    /// <summary>
    /// Representa un archivo Excel subido por un usuario de un área específica.
    /// Los datos extraídos se almacenan en JSON y pueden usarse para:
    /// - Generar narrativas automáticas con IA
    /// - Análisis de datos
    /// - Integración en reportes
    /// MULTI-TENANT: Cada tenant tiene sus propios archivos Excel aislados.
    /// </summary>
    [Table("excel_uploads")]
    public class ExcelUpload : TenantEntity
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("area_id")]
        public int AreaId { get; set; }

        [ForeignKey("AreaId")]
        public virtual Area? Area { get; set; }

        [Required]
        [StringLength(255)]
        [Column("file_name")]
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// Ruta del archivo en el sistema de almacenamiento
        /// Formato: uploads/{tenantId}/{areaId}/{fileName}
        /// </summary>
        [Required]
        [StringLength(500)]
        [Column("file_path")]
        public string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// Tamaño del archivo en bytes
        /// </summary>
        [Column("file_size")]
        public long FileSize { get; set; }

        [Required]
        [StringLength(50)]
        [Column("period")]
        public string Period { get; set; } = string.Empty; // Ej: "Abril 2025"

        [Column("upload_date")]
        public DateTime UploadDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Usuario que subió el archivo
        /// </summary>
        [Required]
        [Column("uploaded_by_user_id")]
        public int UploadedByUserId { get; set; }

        [ForeignKey("UploadedByUserId")]
        public virtual User? UploadedByUser { get; set; }

        /// <summary>
        /// Datos extraídos del Excel en formato JSON
        /// Incluye tablas, KPIs y datos estructurados
        /// </summary>
        [Column("extracted_json_data", TypeName = "text")]
        public string? ExtractedJsonData { get; set; }

        /// <summary>
        /// Estado del procesamiento
        /// </summary>
        [Required]
        [StringLength(20)]
        [Column("processing_status")]
        public string ProcessingStatus { get; set; } = "pending"; // pending, processing, completed, failed

        /// <summary>
        /// Mensaje de error si el procesamiento falló
        /// </summary>
        [Column("error_message", TypeName = "text")]
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Fecha de procesamiento
        /// </summary>
        [Column("processed_at")]
        public DateTime? ProcessedAt { get; set; }

        /// <summary>
        /// Metadatos adicionales del archivo
        /// Puede incluir: número de hojas, número de filas, columnas detectadas, etc.
        /// </summary>
        [Column("metadata_json", TypeName = "text")]
        public string? MetadataJson { get; set; }
    }
}

