namespace JEGASolutions.ReportBuilder.Core.Interfaces
{
    /// <summary>
    /// Interfaz base para proveedores de AI (OpenAI, Anthropic, DeepSeek, Groq, Ollama)
    /// </summary>
    public interface IAIProvider
    {
        /// <summary>
        /// Nombre del proveedor: openai, anthropic, deepseek, groq, ollama
        /// </summary>
        string ProviderName { get; }

        /// <summary>
        /// Indica si el proveedor está disponible (API key configurada)
        /// </summary>
        bool IsAvailable { get; }

        /// <summary>
        /// Genera respuesta de AI basada en prompt
        /// </summary>
        Task<AIResponse> GenerateResponseAsync(AIRequest request);

        /// <summary>
        /// Genera análisis de datos estructurados
        /// </summary>
        Task<AIAnalysisResponse> AnalyzeDataAsync(AIAnalysisRequest request);

        /// <summary>
        /// Obtiene modelos disponibles del proveedor
        /// </summary>
        Task<List<string>> GetAvailableModelsAsync();

        /// <summary>
        /// Valida que el proveedor esté correctamente configurado
        /// </summary>
        Task<(bool IsValid, string? ErrorMessage)> ValidateConfigurationAsync();
    }

    /// <summary>
    /// Request genérico para AI
    /// </summary>
    public class AIRequest
    {
        public string Prompt { get; set; } = string.Empty;
        public string? SystemPrompt { get; set; }
        public string? Model { get; set; }
        public double Temperature { get; set; } = 0.7;
        public int MaxTokens { get; set; } = 1000;
        public Dictionary<string, object>? AdditionalParameters { get; set; }
    }

    /// <summary>
    /// Response genérico de AI
    /// </summary>
    public class AIResponse
    {
        public string Content { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string Provider { get; set; } = string.Empty;
        public int TokensUsed { get; set; }
        public decimal? CostEstimate { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
        public Dictionary<string, object>? Metadata { get; set; }
    }

    /// <summary>
    /// Request para análisis de datos
    /// </summary>
    public class AIAnalysisRequest
    {
        public object Data { get; set; } = new();
        public string AnalysisType { get; set; } = "summary"; // summary, trends, anomalies, predictions
        public string? CustomPrompt { get; set; }
        public string? Model { get; set; }
        public Dictionary<string, object>? Options { get; set; }
    }

    /// <summary>
    /// Response para análisis de datos
    /// </summary>
    public class AIAnalysisResponse
    {
        public string Summary { get; set; } = string.Empty;
        public List<string> KeyFindings { get; set; } = new();
        public List<string> Recommendations { get; set; } = new();
        public object? StructuredInsights { get; set; }
        public decimal Confidence { get; set; }
        public string Provider { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }
}

