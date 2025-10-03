namespace JEGASolutions.ReportBuilder.Core.Interfaces
{
    /// <summary>
    /// Servicio que coordina múltiples proveedores de AI
    /// </summary>
    public interface IMultiAIService
    {
        /// <summary>
        /// Genera respuesta usando el proveedor especificado
        /// </summary>
        Task<AIResponse> GenerateResponseAsync(
            string providerName, 
            AIRequest request);

        /// <summary>
        /// Analiza datos usando el proveedor especificado
        /// </summary>
        Task<AIAnalysisResponse> AnalyzeDataAsync(
            string providerName, 
            AIAnalysisRequest request);

        /// <summary>
        /// Obtiene lista de proveedores disponibles
        /// </summary>
        Task<List<AIProviderInfo>> GetAvailableProvidersAsync();

        /// <summary>
        /// Obtiene el mejor proveedor disponible según criterio
        /// </summary>
        Task<string> GetBestProviderAsync(AIProviderCriteria criteria);

        /// <summary>
        /// Compara respuestas de múltiples proveedores
        /// </summary>
        Task<MultiProviderComparisonResult> CompareProvidersAsync(
            List<string> providerNames, 
            AIRequest request);

        /// <summary>
        /// Valida que un proveedor específico esté disponible
        /// </summary>
        Task<bool> IsProviderAvailableAsync(string providerName);
    }

    /// <summary>
    /// Información de proveedor de AI
    /// </summary>
    public class AIProviderInfo
    {
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
        public List<string> AvailableModels { get; set; } = new();
        public string? DefaultModel { get; set; }
        public decimal? CostPerToken { get; set; }
        public int MaxTokens { get; set; }
        public Dictionary<string, object>? Capabilities { get; set; }
    }

    /// <summary>
    /// Criterios para seleccionar el mejor proveedor
    /// </summary>
    public class AIProviderCriteria
    {
        public bool PreferCost { get; set; } = false;
        public bool PreferSpeed { get; set; } = false;
        public bool PreferQuality { get; set; } = true;
        public int? MinTokenLimit { get; set; }
        public List<string>? PreferredProviders { get; set; }
        public List<string>? ExcludedProviders { get; set; }
    }

    /// <summary>
    /// Resultado de comparación entre proveedores
    /// </summary>
    public class MultiProviderComparisonResult
    {
        public AIRequest OriginalRequest { get; set; } = new();
        public List<ProviderComparisonItem> Results { get; set; } = new();
        public DateTime ComparisonDate { get; set; } = DateTime.UtcNow;
        public string? RecommendedProvider { get; set; }
    }

    /// <summary>
    /// Item de comparación por proveedor
    /// </summary>
    public class ProviderComparisonItem
    {
        public string ProviderName { get; set; } = string.Empty;
        public AIResponse Response { get; set; } = new();
        public long ResponseTimeMs { get; set; }
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public decimal? QualityScore { get; set; }
    }
}

