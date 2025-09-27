import api from "./api";

export const aiService = {
  async analyzeReport(reportId, analysisType, parameters = {}) {
    const response = await api.post("/aianalysis/analyze", {
      reportSubmissionId: reportId,
      analysisType,
      parameters,
    });
    return response.data;
  },

  async getInsightsForReport(reportId) {
    const response = await api.get(`/aianalysis/insights/${reportId}`);
    return response.data;
  },

  async getInsightsByType(insightType) {
    const response = await api.get(
      `/aianalysis/insights/by-type/${insightType}`
    );
    return response.data;
  },

  async generateInsights(reportId) {
    const response = await api.post(
      `/aianalysis/generate-insights/${reportId}`
    );
    return response.data;
  },

  async deleteInsight(insightId) {
    const response = await api.delete(`/aianalysis/insights/${insightId}`);
    return response.data;
  },
};
