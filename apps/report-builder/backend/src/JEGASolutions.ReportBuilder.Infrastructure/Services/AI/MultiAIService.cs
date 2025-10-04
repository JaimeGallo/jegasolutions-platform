using JEGASolutions.ReportBuilder.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace JEGASolutions.ReportBuilder.Infrastructure.Services.AI
{
    public class MultiAIService : IMultiAIService
    {
        private readonly IEnumerable<IAIProvider> _providers;
        private readonly ILogger<MultiAIService> _logger;
        private readonly Dictionary<string, IAIProvider> _providerMap;

        public MultiAIService(
            IEnumerable<IAIProvider> providers,
            ILogger<MultiAIService> logger)
        {
            _providers = providers;
            _logger = logger;
            _providerMap = providers.ToDictionary(p => p.ProviderName, p => p);

            _logger.LogInformation("MultiAIService iniciado con {Count} proveedores: {Providers}",
                _providerMap.Count,
                string.Join(", ", _providerMap.Keys));
        }

        public async Task<AIResponse> GenerateResponseAsync(string providerName, AIRequest request)
        {
            try
            {
                _logger.LogInformation("Generando respuesta con proveedor: {Provider}", providerName);

                var provider = GetProvider(providerName);
                if (provider == null)
                {
                    throw new ArgumentException($"Proveedor '{providerName}' no disponible");
                }

                if (!provider.IsAvailable)
                {
                    throw new InvalidOperationException(
                        $"Proveedor '{providerName}' no está configurado correctamente");
                }

                var response = await provider.GenerateResponseAsync(request);
                _logger.LogInformation("Respuesta generada exitosamente por {Provider}", providerName);
                
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando respuesta con proveedor {Provider}", providerName);
                throw;
            }
        }

        public async Task<AIAnalysisResponse> AnalyzeDataAsync(string providerName, AIAnalysisRequest request)
        {
            try
            {
                _logger.LogInformation("Analizando datos con proveedor: {Provider}, Tipo: {AnalysisType}",
                    providerName, request.AnalysisType);

                var provider = GetProvider(providerName);
                if (provider == null)
                {
                    throw new ArgumentException($"Proveedor '{providerName}' no disponible");
                }

                if (!provider.IsAvailable)
                {
                    throw new InvalidOperationException(
                        $"Proveedor '{providerName}' no está configurado correctamente");
                }

                var response = await provider.AnalyzeDataAsync(request);
                _logger.LogInformation("Análisis completado exitosamente por {Provider}", providerName);
                
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error analizando datos con proveedor {Provider}", providerName);
                throw;
            }
        }

        public async Task<List<AIProviderInfo>> GetAvailableProvidersAsync()
        {
            var providerInfos = new List<AIProviderInfo>();

            foreach (var provider in _providers)
            {
                try
                {
                    var models = await provider.GetAvailableModelsAsync();
                    var (isValid, errorMessage) = await provider.ValidateConfigurationAsync();

                    var info = new AIProviderInfo
                    {
                        Name = provider.ProviderName,
                        DisplayName = GetDisplayName(provider.ProviderName),
                        IsAvailable = provider.IsAvailable && isValid,
                        AvailableModels = models,
                        DefaultModel = models.FirstOrDefault(),
                        MaxTokens = GetMaxTokens(provider.ProviderName),
                        CostPerToken = GetCostPerToken(provider.ProviderName),
                        Capabilities = GetCapabilities(provider.ProviderName)
                    };

                    if (!isValid)
                    {
                        info.Capabilities ??= new Dictionary<string, object>();
                        info.Capabilities["configurationError"] = errorMessage ?? "Unknown error";
                    }

                    providerInfos.Add(info);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error obteniendo información del proveedor {Provider}",
                        provider.ProviderName);
                    
                    providerInfos.Add(new AIProviderInfo
                    {
                        Name = provider.ProviderName,
                        DisplayName = GetDisplayName(provider.ProviderName),
                        IsAvailable = false,
                        Capabilities = new Dictionary<string, object>
                        {
                            ["error"] = ex.Message
                        }
                    });
                }
            }

            return providerInfos;
        }

        public async Task<string> GetBestProviderAsync(AIProviderCriteria criteria)
        {
            try
            {
                _logger.LogInformation("Seleccionando mejor proveedor según criterios: Cost={PreferCost}, Speed={PreferSpeed}, Quality={PreferQuality}",
                    criteria.PreferCost, criteria.PreferSpeed, criteria.PreferQuality);

                var availableProviders = await GetAvailableProvidersAsync();
                var validProviders = availableProviders.Where(p => p.IsAvailable).ToList();

                if (!validProviders.Any())
                {
                    throw new InvalidOperationException("No hay proveedores disponibles");
                }

                // Filtrar por preferencias
                if (criteria.PreferredProviders?.Any() == true)
                {
                    validProviders = validProviders
                        .Where(p => criteria.PreferredProviders.Contains(p.Name))
                        .ToList();
                }

                if (criteria.ExcludedProviders?.Any() == true)
                {
                    validProviders = validProviders
                        .Where(p => !criteria.ExcludedProviders.Contains(p.Name))
                        .ToList();
                }

                if (criteria.MinTokenLimit.HasValue)
                {
                    validProviders = validProviders
                        .Where(p => p.MaxTokens >= criteria.MinTokenLimit.Value)
                        .ToList();
                }

                if (!validProviders.Any())
                {
                    throw new InvalidOperationException(
                        "No hay proveedores que cumplan los criterios especificados");
                }

                // Seleccionar basado en prioridades
                var bestProvider = criteria switch
                {
                    _ when criteria.PreferCost => SelectByCost(validProviders),
                    _ when criteria.PreferSpeed => SelectBySpeed(validProviders),
                    _ when criteria.PreferQuality => SelectByQuality(validProviders),
                    _ => validProviders.First().Name // Default
                };

                _logger.LogInformation("Proveedor seleccionado: {Provider}", bestProvider);
                return bestProvider;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error seleccionando mejor proveedor");
                throw;
            }
        }

        public async Task<MultiProviderComparisonResult> CompareProvidersAsync(
            List<string> providerNames,
            AIRequest request)
        {
            try
            {
                _logger.LogInformation("Comparando {Count} proveedores: {Providers}",
                    providerNames.Count, string.Join(", ", providerNames));

                var results = new List<ProviderComparisonItem>();

                foreach (var providerName in providerNames)
                {
                    var stopwatch = Stopwatch.StartNew();
                    
                    try
                    {
                        var response = await GenerateResponseAsync(providerName, request);
                        stopwatch.Stop();

                        results.Add(new ProviderComparisonItem
                        {
                            ProviderName = providerName,
                            Response = response,
                            ResponseTimeMs = stopwatch.ElapsedMilliseconds,
                            Success = true,
                            QualityScore = CalculateQualityScore(response)
                        });
                    }
                    catch (Exception ex)
                    {
                        stopwatch.Stop();
                        _logger.LogWarning(ex, "Error comparando proveedor {Provider}", providerName);

                        results.Add(new ProviderComparisonItem
                        {
                            ProviderName = providerName,
                            ResponseTimeMs = stopwatch.ElapsedMilliseconds,
                            Success = false,
                            ErrorMessage = ex.Message
                        });
                    }
                }

                // Determinar proveedor recomendado
                var successfulResults = results.Where(r => r.Success).ToList();
                string? recommendedProvider = null;

                if (successfulResults.Any())
                {
                    // Balancear calidad vs velocidad vs costo
                    recommendedProvider = successfulResults
                        .OrderByDescending(r => r.QualityScore ?? 0)
                        .ThenBy(r => r.Response?.CostEstimate ?? decimal.MaxValue)
                        .ThenBy(r => r.ResponseTimeMs)
                        .First()
                        .ProviderName;
                }

                return new MultiProviderComparisonResult
                {
                    OriginalRequest = request,
                    Results = results,
                    ComparisonDate = DateTime.UtcNow,
                    RecommendedProvider = recommendedProvider
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en CompareProvidersAsync");
                throw;
            }
        }

        public async Task<bool> IsProviderAvailableAsync(string providerName)
        {
            var provider = GetProvider(providerName);
            if (provider == null) return false;

            try
            {
                var (isValid, _) = await provider.ValidateConfigurationAsync();
                return provider.IsAvailable && isValid;
            }
            catch
            {
                return false;
            }
        }

        // Helper methods
        private IAIProvider? GetProvider(string providerName)
        {
            _providerMap.TryGetValue(providerName.ToLower(), out var provider);
            return provider;
        }

        private string GetDisplayName(string providerName)
        {
            return providerName.ToLower() switch
            {
                "openai" => "OpenAI (GPT-4)",
                "anthropic" => "Anthropic (Claude)",
                "deepseek" => "DeepSeek",
                "groq" => "Groq (Ultra-fast)",
                "ollama" => "Ollama (Local)",
                _ => providerName
            };
        }

        private int GetMaxTokens(string providerName)
        {
            return providerName.ToLower() switch
            {
                "openai" => 128000,      // GPT-4 Turbo
                "anthropic" => 200000,   // Claude 3 (largest)
                "deepseek" => 64000,
                "groq" => 32768,
                "ollama" => 8192,
                _ => 4096
            };
        }

        private decimal? GetCostPerToken(string providerName)
        {
            // Costo por 1M tokens (output) - valores aproximados
            return providerName.ToLower() switch
            {
                "openai" => 30.00m,
                "anthropic" => 15.00m,
                "deepseek" => 0.14m,
                "groq" => 0.59m,
                "ollama" => 0.00m, // Local, gratis
                _ => null
            };
        }

        private Dictionary<string, object> GetCapabilities(string providerName)
        {
            return providerName.ToLower() switch
            {
                "openai" => new Dictionary<string, object>
                {
                    ["reasoning"] = "excellent",
                    ["speed"] = "medium",
                    ["cost"] = "high",
                    ["contextWindow"] = 128000
                },
                "anthropic" => new Dictionary<string, object>
                {
                    ["reasoning"] = "excellent",
                    ["speed"] = "medium",
                    ["cost"] = "medium",
                    ["contextWindow"] = 200000
                },
                "deepseek" => new Dictionary<string, object>
                {
                    ["reasoning"] = "good",
                    ["speed"] = "fast",
                    ["cost"] = "very_low",
                    ["contextWindow"] = 64000
                },
                "groq" => new Dictionary<string, object>
                {
                    ["reasoning"] = "good",
                    ["speed"] = "ultra_fast",
                    ["cost"] = "very_low",
                    ["contextWindow"] = 32768
                },
                "ollama" => new Dictionary<string, object>
                {
                    ["reasoning"] = "medium",
                    ["speed"] = "depends_on_hardware",
                    ["cost"] = "free",
                    ["privacy"] = "complete"
                },
                _ => new Dictionary<string, object>()
            };
        }

        private string SelectByCost(List<AIProviderInfo> providers)
        {
            return providers
                .OrderBy(p => p.CostPerToken ?? decimal.MaxValue)
                .First()
                .Name;
        }

        private string SelectBySpeed(List<AIProviderInfo> providers)
        {
            // Groq es el más rápido, seguido de DeepSeek
            var speedOrder = new[] { "groq", "deepseek", "ollama", "openai", "anthropic" };
            
            foreach (var provider in speedOrder)
            {
                if (providers.Any(p => p.Name == provider))
                    return provider;
            }
            
            return providers.First().Name;
        }

        private string SelectByQuality(List<AIProviderInfo> providers)
        {
            // Claude y GPT-4 son los de mayor calidad
            var qualityOrder = new[] { "anthropic", "openai", "deepseek", "groq", "ollama" };
            
            foreach (var provider in qualityOrder)
            {
                if (providers.Any(p => p.Name == provider))
                    return provider;
            }
            
            return providers.First().Name;
        }

        private decimal? CalculateQualityScore(AIResponse response)
        {
            // Scoring simple basado en longitud de respuesta y confianza
            if (string.IsNullOrEmpty(response.Content)) return 0;

            var lengthScore = Math.Min(response.Content.Length / 1000m, 10); // Max 10
            var providerScore = response.Provider switch
            {
                "anthropic" => 10m,
                "openai" => 9.5m,
                "deepseek" => 8.5m,
                "groq" => 8.8m,
                "ollama" => 7.0m,
                _ => 7.5m
            };

            return (lengthScore + providerScore) / 2;
        }
    }
}

