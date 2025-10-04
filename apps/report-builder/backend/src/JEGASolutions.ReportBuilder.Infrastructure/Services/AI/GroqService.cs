using JEGASolutions.ReportBuilder.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace JEGASolutions.ReportBuilder.Infrastructure.Services.AI
{
    public class GroqService : IAIProvider
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<GroqService> _logger;
        private readonly string? _apiKey;
        private readonly string _model;

        public string ProviderName => "groq";

        public bool IsAvailable => !string.IsNullOrEmpty(_apiKey);

        public GroqService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<GroqService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;

            _apiKey = _configuration["AI:Groq:ApiKey"];
            _model = _configuration["AI:Groq:Model"] ?? "llama-3.3-70b-versatile";

            if (!string.IsNullOrEmpty(_apiKey))
            {
                _httpClient.BaseAddress = new Uri("https://api.groq.com/openai/v1/");
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _apiKey);
                _httpClient.Timeout = TimeSpan.FromSeconds(60); // Groq is fast!
            }
        }

        public async Task<AIResponse> GenerateResponseAsync(AIRequest request)
        {
            try
            {
                if (!IsAvailable)
                {
                    throw new InvalidOperationException("Groq API key no configurada");
                }

                _logger.LogInformation("Generando respuesta con Groq (Ultra-fast LLM)");

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
                    _logger.LogError("Error en API Groq: {Status} - {Content}",
                        response.StatusCode, responseContent);
                    throw new HttpRequestException(
                        $"Error en API Groq: {response.StatusCode} - {responseContent}");
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
                        ["totalTokens"] = totalTokens,
                        ["speed"] = "ultra-fast" // Groq's main advantage
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en GroqService.GenerateResponseAsync");
                throw;
            }
        }

        public async Task<AIAnalysisResponse> AnalyzeDataAsync(AIAnalysisRequest request)
        {
            try
            {
                _logger.LogInformation("Analizando datos con Groq: Tipo {AnalysisType}",
                    request.AnalysisType);

                var prompt = BuildAnalysisPrompt(request);
                var aiRequest = new AIRequest
                {
                    Prompt = prompt,
                    SystemPrompt = "You are an expert data analyst specializing in business intelligence. " +
                                   "Provide concise, accurate, and actionable insights.",
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
                    Confidence = 0.88m, // Llama 3.3 70B es muy competente
                    Provider = ProviderName,
                    GeneratedAt = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en GroqService.AnalyzeDataAsync");
                throw;
            }
        }

        public async Task<List<string>> GetAvailableModelsAsync()
        {
            await Task.CompletedTask;
            return new List<string>
            {
                "llama-3.3-70b-versatile",      // Latest Llama 3.3 (best balance)
                "llama-3.1-70b-versatile",      // Llama 3.1 70B
                "llama-3.1-8b-instant",         // Fastest, good for simple tasks
                "mixtral-8x7b-32768",           // Mixtral (long context)
                "gemma2-9b-it"                  // Google's Gemma 2
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
            // Groq pricing (enero 2025) - extremadamente económico
            var costPerMillionTokens = model.ToLower() switch
            {
                var m when m.Contains("llama-3.3-70b") => 0.59m,      // Output pricing
                var m when m.Contains("llama-3.1-70b") => 0.59m,
                var m when m.Contains("llama-3.1-8b") => 0.10m,       // Más barato
                var m when m.Contains("mixtral") => 0.27m,
                var m when m.Contains("gemma2") => 0.10m,
                _ => 0.59m
            };

            return (tokens / 1_000_000m) * costPerMillionTokens;
        }
    }
}

