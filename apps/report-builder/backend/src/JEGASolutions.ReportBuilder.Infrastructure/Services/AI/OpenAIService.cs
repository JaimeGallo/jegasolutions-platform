using Azure.AI.OpenAI;
using JEGASolutions.ReportBuilder.Core.Interfaces;
using JEGASolutions.ReportBuilder.Core.Dto;
using System.Text.Json;

namespace JEGASolutions.ReportBuilder.Infrastructure.Services.AI
{
    public class OpenAIService : IAIAnalysisService
    {
        private readonly OpenAIClient _openAIClient;
        private readonly ILogger<OpenAIService> _logger;

        public OpenAIService(OpenAIClient openAIClient, ILogger<OpenAIService> logger)
        {
            _openAIClient = openAIClient;
            _logger = logger;
        }

        public async Task<AIAnalysisResultDto> AnalyzeReportAsync(AIAnalysisRequestDto request, int tenantId)
        {
            try
            {
                var insights = new List<AIInsightDto>();

                switch (request.AnalysisType.ToLower())
                {
                    case "trend":
                        insights = await AnalyzeTrendsAsync(request);
                        break;
                    case "anomaly":
                        insights = await DetectAnomaliesAsync(request);
                        break;
                    case "summary":
                        insights = await GenerateSummaryAsync(request);
                        break;
                    case "recommendation":
                        insights = await GenerateRecommendationsAsync(request);
                        break;
                    default:
                        return new AIAnalysisResultDto
                        {
                            Success = false,
                            Message = "Invalid analysis type",
                            ErrorDetails = "Analysis type must be one of: trend, anomaly, summary, recommendation"
                        };
                }

                return new AIAnalysisResultDto
                {
                    Success = true,
                    Message = "Analysis completed successfully",
                    Insights = insights
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error analyzing report for tenant {TenantId}", tenantId);
                return new AIAnalysisResultDto
                {
                    Success = false,
                    Message = "Analysis failed",
                    ErrorDetails = ex.Message
                };
            }
        }

        public async Task<List<AIInsightDto>> GetInsightsForReportAsync(int reportSubmissionId, int tenantId)
        {
            // This would typically query the database for existing insights
            // For now, return empty list as this is handled by the repository layer
            return new List<AIInsightDto>();
        }

        public async Task<List<AIInsightDto>> GetInsightsByTypeAsync(string insightType, int tenantId)
        {
            // This would typically query the database for insights by type
            // For now, return empty list as this is handled by the repository layer
            return new List<AIInsightDto>();
        }

        public async Task<bool> GenerateInsightsAsync(int reportSubmissionId, int tenantId)
        {
            try
            {
                // Implementation would generate insights for the report
                // This is a placeholder for the actual AI processing
                _logger.LogInformation("Generating insights for report {ReportId} in tenant {TenantId}", reportSubmissionId, tenantId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating insights for report {ReportId} in tenant {TenantId}", reportSubmissionId, tenantId);
                return false;
            }
        }

        public async Task<bool> DeleteInsightAsync(int insightId, int tenantId)
        {
            // This would typically delete the insight from the database
            // For now, return true as this is handled by the repository layer
            return true;
        }

        private async Task<List<AIInsightDto>> AnalyzeTrendsAsync(AIAnalysisRequestDto request)
        {
            var prompt = "Analyze the following data for trends and patterns...";
            var response = await CallOpenAIAsync(prompt);
            
            return new List<AIInsightDto>
            {
                new AIInsightDto
                {
                    Id = 0, // Will be set by the repository
                    ReportSubmissionId = request.ReportSubmissionId,
                    InsightType = "trend",
                    Title = "Trend Analysis",
                    Content = response,
                    ConfidenceScore = 0.85m,
                    GeneratedAt = DateTime.UtcNow,
                    AIModel = "gpt-4"
                }
            };
        }

        private async Task<List<AIInsightDto>> DetectAnomaliesAsync(AIAnalysisRequestDto request)
        {
            var prompt = "Detect anomalies in the following data...";
            var response = await CallOpenAIAsync(prompt);
            
            return new List<AIInsightDto>
            {
                new AIInsightDto
                {
                    Id = 0, // Will be set by the repository
                    ReportSubmissionId = request.ReportSubmissionId,
                    InsightType = "anomaly",
                    Title = "Anomaly Detection",
                    Content = response,
                    ConfidenceScore = 0.90m,
                    GeneratedAt = DateTime.UtcNow,
                    AIModel = "gpt-4"
                }
            };
        }

        private async Task<List<AIInsightDto>> GenerateSummaryAsync(AIAnalysisRequestDto request)
        {
            var prompt = "Generate a comprehensive summary of the following data...";
            var response = await CallOpenAIAsync(prompt);
            
            return new List<AIInsightDto>
            {
                new AIInsightDto
                {
                    Id = 0, // Will be set by the repository
                    ReportSubmissionId = request.ReportSubmissionId,
                    InsightType = "summary",
                    Title = "Executive Summary",
                    Content = response,
                    ConfidenceScore = 0.95m,
                    GeneratedAt = DateTime.UtcNow,
                    AIModel = "gpt-4"
                }
            };
        }

        private async Task<List<AIInsightDto>> GenerateRecommendationsAsync(AIAnalysisRequestDto request)
        {
            var prompt = "Generate actionable recommendations based on the following data...";
            var response = await CallOpenAIAsync(prompt);
            
            return new List<AIInsightDto>
            {
                new AIInsightDto
                {
                    Id = 0, // Will be set by the repository
                    ReportSubmissionId = request.ReportSubmissionId,
                    InsightType = "recommendation",
                    Title = "Recommendations",
                    Content = response,
                    ConfidenceScore = 0.80m,
                    GeneratedAt = DateTime.UtcNow,
                    AIModel = "gpt-4"
                }
            };
        }

        private async Task<string> CallOpenAIAsync(string prompt)
        {
            try
            {
                var chatCompletionsOptions = new ChatCompletionsOptions()
                {
                    DeploymentName = "gpt-4", // Use your deployment name
                    Messages =
                    {
                        new ChatRequestSystemMessage("You are an AI assistant specialized in data analysis and business intelligence."),
                        new ChatRequestUserMessage(prompt)
                    },
                    MaxTokens = 2000,
                    Temperature = 0.7f
                };

                var response = await _openAIClient.GetChatCompletionsAsync(chatCompletionsOptions);
                return response.Value.Choices[0].Message.Content;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling OpenAI API");
                return "Error generating AI response. Please try again later.";
            }
        }
    }
}
