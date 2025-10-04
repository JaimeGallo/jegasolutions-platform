using JEGASolutions.ReportBuilder.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace JEGASolutions.ReportBuilder.Infrastructure.Services.AI
{
    /// <summary>
    /// OpenAI Provider Service que implementa IAIProvider para el sistema Multi-AI
    /// Usa la API directa de OpenAI (no Azure)
    /// </summary>
    public class OpenAIProviderService : IAIProvider
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<OpenAIProviderService> _logger;
        private readonly string? _apiKey;
        private readonly string _model;

        public string ProviderName => "openai";

        public bool IsAvailable => !string.IsNullOrEmpty(_apiKey);

        public OpenAIProviderService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<OpenAIProviderService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;

            // Priorizar variable de entorno OPENAI_API_KEY
            _apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") 
                      ?? _configuration["AI:OpenAI:ApiKey"]
                      ?? _configuration["OpenAI:ApiKey"];
            
            _model = _configuration["AI:OpenAI:Model"] 
                     ?? _configuration["OpenAI:Model"] 
                     ?? "gpt-4o-mini";

            if (!string.IsNullOrEmpty(_apiKey))
            {
                _httpClient.BaseAddress = new Uri("https://api.openai.com/v1/");
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _apiKey);
                _httpClient.Timeout = TimeSpan.FromSeconds(120);
            }
        }

        public async Task<AIResponse> GenerateResponseAsync(AIRequest request)
        {
            try
            {
                if (!IsAvailable)
                {
                    throw new InvalidOperationException("OpenAI API key no configurada");
                }

                _logger.LogInformation("Generando respuesta con OpenAI (GPT-4)");

                var model = request.Model ?? _model;
                var messages = new List<object>();

                if (!string.IsNullOrEmpty(request.SystemPrompt))
                {
                    messages.Add(new { role = "system", content = request.SystemPrompt });
                }

                messages.Add(new { role = "user", content = request.Prompt });

                var requestBody = new
                {
                    model = model,
                    messages = messages,
                    max_tokens = request.MaxTokens,
                    temperature = request.Temperature,
                    stream = false
                };

                var jsonContent = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("chat/completions", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Error en API OpenAI: {Status} - {Content}",
                        response.StatusCode, responseContent);
                    throw new HttpRequestException(
                        $"Error en API OpenAI: {response.StatusCode} - {responseContent}");
                }

                var jsonResponse = JsonDocument.Parse(responseContent);
                var messageContent = jsonResponse.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString() ?? string.Empty;

                var usage = jsonResponse.RootElement.GetProperty("usage");
                var totalTokens = usage.GetProperty("total_tokens").GetInt32();
                var completionTokens = usage.GetProperty("completion_tokens").GetInt32();

                return new AIResponse
                {
                    Content = messageContent,
                    Model = model,
                    Provider = ProviderName,
                    TokensUsed = totalTokens,
                    CostEstimate = CalculateCost(model, completionTokens),
                    GeneratedAt = DateTime.UtcNow,
                    Metadata = new Dictionary<string, object>
                    {
                        ["promptTokens"] = usage.GetProperty("prompt_tokens").GetInt32(),
                        ["completionTokens"] = completionTokens,
                        ["totalTokens"] = totalTokens
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en OpenAIProviderService.GenerateResponseAsync");
                throw;
            }
        }

        public async Task<AIAnalysisResponse> AnalyzeDataAsync(AIAnalysisRequest request)
        {
            try
            {
                _logger.LogInformation("Analizando datos con OpenAI: Tipo {AnalysisType}",
                    request.AnalysisType);

                var prompt = BuildAnalysisPrompt(request);
                var aiRequest = new AIRequest
                {
                    Prompt = prompt,
                    SystemPrompt = "You are an expert business intelligence analyst. " +
                                   "Provide detailed, accurate, and actionable analysis.",
                    Model = request.Model,
                    Temperature = 0.7,
                    MaxTokens = 2000
                };

                var response = await GenerateResponseAsync(aiRequest);

                // Parsear respuesta
                var analysis = ParseAnalysisResponse(response.Content, request.AnalysisType);

                return new AIAnalysisResponse
                {
                    Summary = analysis.Summary,
                    KeyFindings = analysis.KeyFindings,
                    Recommendations = analysis.Recommendations,
                    StructuredInsights = analysis.StructuredInsights,
                    Confidence = 0.92m, // GPT-4 tiene alta confianza
                    Provider = ProviderName,
                    GeneratedAt = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en OpenAIProviderService.AnalyzeDataAsync");
                throw;
            }
        }

        public async Task<List<string>> GetAvailableModelsAsync()
        {
            await Task.CompletedTask;
            return new List<string>
            {
                "gpt-4o",              // Latest GPT-4 Optimized (best overall)
                "gpt-4o-mini",         // Faster and cheaper GPT-4
                "gpt-4-turbo",         // GPT-4 Turbo (128K context)
                "gpt-4",               // Original GPT-4
                "gpt-3.5-turbo"        // Faster, cheaper, good for simple tasks
            };
        }

        public async Task<(bool IsValid, string? ErrorMessage)> ValidateConfigurationAsync()
        {
            try
            {
                if (!IsAvailable)
                {
                    return (false, "API key no configurada. Configure OPENAI_API_KEY como variable de entorno.");
                }

                // Intentar una llamada simple de validación
                var testRequest = new AIRequest
                {
                    Prompt = "Test",
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
                "summary" => "Generate an executive summary of the following data:",
                "trends" => "Analyze trends and patterns in the following data:",
                "anomalies" => "Detect anomalies or outliers in the following data:",
                "predictions" => "Generate predictions based on the following historical data:",
                _ => "Analyze the following data:"
            };

            return $@"{basePrompt}

DATA:
{dataJson}

{(string.IsNullOrEmpty(request.CustomPrompt) ? "" : $"\nADDITIONAL INSTRUCTIONS:\n{request.CustomPrompt}")}

RESPONSE FORMAT:
Please structure your response with the following sections:
1. SUMMARY: A brief executive summary
2. KEY FINDINGS: List of 3-5 main findings
3. RECOMMENDATIONS: List of 3-5 actionable recommendations
4. ADDITIONAL INSIGHTS: Any other relevant insights";
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

                if (trimmedLine.Contains("SUMMARY:", StringComparison.OrdinalIgnoreCase))
                {
                    currentSection = "summary";
                    continue;
                }
                else if (trimmedLine.Contains("FINDING", StringComparison.OrdinalIgnoreCase) ||
                         trimmedLine.Contains("KEY", StringComparison.OrdinalIgnoreCase))
                {
                    currentSection = "findings";
                    continue;
                }
                else if (trimmedLine.Contains("RECOMMENDATION", StringComparison.OrdinalIgnoreCase))
                {
                    currentSection = "recommendations";
                    continue;
                }

                if (currentSection == "summary" && !string.IsNullOrWhiteSpace(trimmedLine))
                {
                    summary += trimmedLine + " ";
                }
                else if (currentSection == "findings" && (trimmedLine.StartsWith("-") ||
                         trimmedLine.Any(char.IsDigit)))
                {
                    keyFindings.Add(trimmedLine.TrimStart('-', ' ', '1', '2', '3', '4', '5', '.'));
                }
                else if (currentSection == "recommendations" && (trimmedLine.StartsWith("-") ||
                         trimmedLine.Any(char.IsDigit)))
                {
                    recommendations.Add(trimmedLine.TrimStart('-', ' ', '1', '2', '3', '4', '5', '.'));
                }
            }

            if (string.IsNullOrEmpty(summary))
            {
                summary = content.Length > 500 ? content.Substring(0, 500) + "..." : content;
            }

            return new AIAnalysisResponse
            {
                Summary = summary.Trim(),
                KeyFindings = keyFindings.Count > 0 ? keyFindings : new List<string> { "See full content" },
                Recommendations = recommendations.Count > 0 ? recommendations : new List<string> { "See full content" },
                StructuredInsights = new { fullContent = content, analysisType = analysisType }
            };
        }

        private decimal CalculateCost(string model, int tokens)
        {
            // OpenAI pricing (enero 2025) - costo por 1M tokens output
            var costPerMillionTokens = model.ToLower() switch
            {
                var m when m.Contains("gpt-4o-mini") => 0.60m,    // GPT-4o mini output
                var m when m.Contains("gpt-4o") => 10.00m,        // GPT-4o output
                var m when m.Contains("gpt-4-turbo") => 30.00m,   // GPT-4 Turbo output
                var m when m.Contains("gpt-4") => 60.00m,         // GPT-4 output
                var m when m.Contains("gpt-3.5-turbo") => 2.00m,  // GPT-3.5 Turbo output
                _ => 30.00m
            };

            return (tokens / 1_000_000m) * costPerMillionTokens;
        }
    }
}

