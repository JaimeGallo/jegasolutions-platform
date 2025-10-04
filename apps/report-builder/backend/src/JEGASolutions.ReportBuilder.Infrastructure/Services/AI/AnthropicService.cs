using JEGASolutions.ReportBuilder.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace JEGASolutions.ReportBuilder.Infrastructure.Services.AI
{
    public class AnthropicService : IAIProvider
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AnthropicService> _logger;
        private readonly string? _apiKey;
        private readonly string _model;

        public string ProviderName => "anthropic";

        public bool IsAvailable => !string.IsNullOrEmpty(_apiKey);

        public AnthropicService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<AnthropicService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;

            _apiKey = _configuration["AI:Anthropic:ApiKey"];
            _model = _configuration["AI:Anthropic:Model"] ?? "claude-3-5-sonnet-20241022";

            if (!string.IsNullOrEmpty(_apiKey))
            {
                _httpClient.BaseAddress = new Uri("https://api.anthropic.com/");
                _httpClient.DefaultRequestHeaders.Add("x-api-key", _apiKey);
                _httpClient.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");
                _httpClient.Timeout = TimeSpan.FromSeconds(120);
            }
        }

        public async Task<AIResponse> GenerateResponseAsync(AIRequest request)
        {
            try
            {
                if (!IsAvailable)
                {
                    throw new InvalidOperationException("Anthropic API key no configurada");
                }

                _logger.LogInformation("Generando respuesta con Anthropic (Claude)");

                var model = request.Model ?? _model;
                var requestBody = new
                {
                    model = model,
                    max_tokens = request.MaxTokens,
                    temperature = request.Temperature,
                    messages = new[]
                    {
                        new
                        {
                            role = "user",
                            content = string.IsNullOrEmpty(request.SystemPrompt)
                                ? request.Prompt
                                : $"{request.SystemPrompt}\n\n{request.Prompt}"
                        }
                    }
                };

                var jsonContent = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("v1/messages", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Error en API Anthropic: {Status} - {Content}", 
                        response.StatusCode, responseContent);
                    throw new HttpRequestException(
                        $"Error en API Anthropic: {response.StatusCode} - {responseContent}");
                }

                var jsonResponse = JsonDocument.Parse(responseContent);
                var contentArray = jsonResponse.RootElement.GetProperty("content");
                var textContent = contentArray[0].GetProperty("text").GetString() ?? string.Empty;
                var usage = jsonResponse.RootElement.GetProperty("usage");
                var outputTokens = usage.GetProperty("output_tokens").GetInt32();

                return new AIResponse
                {
                    Content = textContent,
                    Model = model,
                    Provider = ProviderName,
                    TokensUsed = outputTokens,
                    CostEstimate = CalculateCost(model, outputTokens),
                    GeneratedAt = DateTime.UtcNow,
                    Metadata = new Dictionary<string, object>
                    {
                        ["inputTokens"] = usage.GetProperty("input_tokens").GetInt32(),
                        ["outputTokens"] = outputTokens
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en AnthropicService.GenerateResponseAsync");
                throw;
            }
        }

        public async Task<AIAnalysisResponse> AnalyzeDataAsync(AIAnalysisRequest request)
        {
            try
            {
                _logger.LogInformation("Analizando datos con Anthropic: Tipo {AnalysisType}", 
                    request.AnalysisType);

                var prompt = BuildAnalysisPrompt(request);
                var aiRequest = new AIRequest
                {
                    Prompt = prompt,
                    SystemPrompt = "Eres un analista experto en inteligencia de negocios y análisis de datos. " +
                                   "Proporciona análisis detallados, precisos y accionables.",
                    Model = request.Model,
                    Temperature = 0.7,
                    MaxTokens = 2000
                };

                var response = await GenerateResponseAsync(aiRequest);

                // Parsear respuesta para extraer insights estructurados
                var analysis = ParseAnalysisResponse(response.Content, request.AnalysisType);

                return new AIAnalysisResponse
                {
                    Summary = analysis.Summary,
                    KeyFindings = analysis.KeyFindings,
                    Recommendations = analysis.Recommendations,
                    StructuredInsights = analysis.StructuredInsights,
                    Confidence = 0.90m, // Claude típicamente tiene alta confianza
                    Provider = ProviderName,
                    GeneratedAt = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en AnthropicService.AnalyzeDataAsync");
                throw;
            }
        }

        public async Task<List<string>> GetAvailableModelsAsync()
        {
            await Task.CompletedTask;
            return new List<string>
            {
                "claude-3-5-sonnet-20241022", // Latest and best
                "claude-3-opus-20240229",     // Most powerful
                "claude-3-sonnet-20240229",   // Balanced
                "claude-3-haiku-20240307"     // Fastest and cheapest
            };
        }

        public async Task<(bool IsValid, string? ErrorMessage)> ValidateConfigurationAsync()
        {
            try
            {
                if (!IsAvailable)
                {
                    return (false, "API key no configurada");
                }

                // Intentar hacer una llamada simple para validar
                var testRequest = new AIRequest
                {
                    Prompt = "Hi",
                    MaxTokens = 10,
                    Temperature = 0.5
                };

                await GenerateResponseAsync(testRequest);
                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        // Helper methods
        private string BuildAnalysisPrompt(AIAnalysisRequest request)
        {
            var dataJson = JsonSerializer.Serialize(request.Data, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });

            var basePrompt = request.AnalysisType.ToLower() switch
            {
                "summary" => "Genera un resumen ejecutivo de los siguientes datos:",
                "trends" => "Analiza las tendencias y patrones en los siguientes datos:",
                "anomalies" => "Detecta anomalías o valores atípicos en los siguientes datos:",
                "predictions" => "Genera predicciones basadas en los siguientes datos históricos:",
                _ => "Analiza los siguientes datos:"
            };

            return $@"{basePrompt}

DATOS:
{dataJson}

{(string.IsNullOrEmpty(request.CustomPrompt) ? "" : $"\nINSTRUCCIONES ADICIONALES:\n{request.CustomPrompt}")}

FORMATO DE RESPUESTA:
Por favor estructura tu respuesta con las siguientes secciones:
1. RESUMEN: Un resumen ejecutivo breve
2. HALLAZGOS CLAVE: Lista de 3-5 hallazgos principales
3. RECOMENDACIONES: Lista de 3-5 recomendaciones accionables
4. INSIGHTS ADICIONALES: Cualquier otro insight relevante";
        }

        private AIAnalysisResponse ParseAnalysisResponse(string content, string analysisType)
        {
            var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var summary = "";
            var keyFindings = new List<string>();
            var recommendations = new List<string>();
            
            var currentSection = "";
            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                
                if (trimmedLine.Contains("RESUMEN:", StringComparison.OrdinalIgnoreCase))
                {
                    currentSection = "summary";
                    continue;
                }
                else if (trimmedLine.Contains("HALLAZGOS", StringComparison.OrdinalIgnoreCase))
                {
                    currentSection = "findings";
                    continue;
                }
                else if (trimmedLine.Contains("RECOMENDACIONES", StringComparison.OrdinalIgnoreCase))
                {
                    currentSection = "recommendations";
                    continue;
                }

                if (currentSection == "summary" && !string.IsNullOrWhiteSpace(trimmedLine))
                {
                    summary += trimmedLine + " ";
                }
                else if (currentSection == "findings" && trimmedLine.StartsWith("-") || 
                         trimmedLine.Any(char.IsDigit))
                {
                    keyFindings.Add(trimmedLine.TrimStart('-', ' ', '1', '2', '3', '4', '5', '.'));
                }
                else if (currentSection == "recommendations" && trimmedLine.StartsWith("-") || 
                         trimmedLine.Any(char.IsDigit))
                {
                    recommendations.Add(trimmedLine.TrimStart('-', ' ', '1', '2', '3', '4', '5', '.'));
                }
            }

            // Si el parsing falló, usar el contenido completo como resumen
            if (string.IsNullOrEmpty(summary))
            {
                summary = content.Length > 500 ? content.Substring(0, 500) + "..." : content;
            }

            return new AIAnalysisResponse
            {
                Summary = summary.Trim(),
                KeyFindings = keyFindings.Count > 0 ? keyFindings : new List<string> { "Ver contenido completo" },
                Recommendations = recommendations.Count > 0 ? recommendations : new List<string> { "Ver contenido completo" },
                StructuredInsights = new { fullContent = content, analysisType = analysisType }
            };
        }

        private decimal CalculateCost(string model, int tokens)
        {
            // Costos aproximados por 1M tokens (output) - Abril 2024
            var costPerMillionTokens = model.ToLower() switch
            {
                var m when m.Contains("opus") => 75.00m,      // Claude 3 Opus
                var m when m.Contains("sonnet") => 15.00m,    // Claude 3.5 Sonnet
                var m when m.Contains("haiku") => 1.25m,      // Claude 3 Haiku
                _ => 15.00m
            };

            return (tokens / 1_000_000m) * costPerMillionTokens;
        }
    }
}

