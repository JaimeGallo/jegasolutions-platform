using System.ComponentModel.DataAnnotations;

namespace JEGASolutions.ReportBuilder.Core.Dto
{
    /// <summary>
    /// DTO para listar uploads de Excel
    /// </summary>
    public class ExcelUploadListDto
    {
        public int Id { get; set; }
        public int AreaId { get; set; }
        public string AreaName { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string Period { get; set; } = string.Empty;
        public string ProcessingStatus { get; set; } = string.Empty;
        public DateTime UploadDate { get; set; }
        public string UploadedByUserName { get; set; } = string.Empty;
        public bool HasExtractedData { get; set; }
    }

    /// <summary>
    /// DTO con detalles completos del upload de Excel
    /// </summary>
    public class ExcelUploadDetailDto
    {
        public int Id { get; set; }
        public int AreaId { get; set; }
        public string AreaName { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string Period { get; set; } = string.Empty;
        public string ProcessingStatus { get; set; } = string.Empty;
        public DateTime UploadDate { get; set; }
        public int UploadedByUserId { get; set; }
        public string UploadedByUserName { get; set; } = string.Empty;
        public string? ExtractedJsonData { get; set; }
        public object? ParsedData { get; set; } // JSON parsed
        public string? ErrorMessage { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// DTO para crear upload de Excel
    /// </summary>
    public class ExcelUploadCreateDto
    {
        [Required]
        public int AreaId { get; set; }

        [Required]
        [StringLength(50)]
        public string Period { get; set; } = string.Empty;

        /// <summary>
        /// Archivo Excel como Base64 (para API REST)
        /// </summary>
        [Required]
        public string FileBase64 { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string FileName { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO para resultado del procesamiento de Excel
    /// </summary>
    public class ExcelProcessingResultDto
    {
        public int ExcelUploadId { get; set; }
        public bool Success { get; set; }
        public string ProcessingStatus { get; set; } = string.Empty;
        public object? ExtractedData { get; set; }
        public string? ErrorMessage { get; set; }
        public int TotalRows { get; set; }
        public int ProcessedRows { get; set; }
        public List<string> Warnings { get; set; } = new();
    }

    /// <summary>
    /// DTO para solicitar análisis AI de datos de Excel
    /// </summary>
    public class ExcelAIAnalysisRequestDto
    {
        [Required]
        public int ExcelUploadId { get; set; }

        /// <summary>
        /// Proveedor de AI a utilizar: openai, anthropic, deepseek, groq, ollama
        /// </summary>
        [Required]
        [StringLength(20)]
        public string AIProvider { get; set; } = "openai";

        /// <summary>
        /// Prompt específico para el análisis
        /// </summary>
        [StringLength(2000)]
        public string? CustomPrompt { get; set; }

        /// <summary>
        /// Tipo de análisis: summary, trends, anomalies, predictions, custom
        /// </summary>
        [Required]
        [StringLength(50)]
        public string AnalysisType { get; set; } = "summary";
    }

    /// <summary>
    /// DTO para resultado de análisis AI
    /// </summary>
    public class ExcelAIAnalysisResultDto
    {
        public int ExcelUploadId { get; set; }
        public string AIProvider { get; set; } = string.Empty;
        public string AnalysisType { get; set; } = string.Empty;
        public string InsightText { get; set; } = string.Empty;
        public object? StructuredInsights { get; set; } // JSON con insights estructurados
        public DateTime GeneratedAt { get; set; }
        public decimal Confidence { get; set; }
        public List<string> KeyFindings { get; set; } = new();
        public List<string> Recommendations { get; set; } = new();
    }
}

