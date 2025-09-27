import api from "./api";

export const reportService = {
  async getReports() {
    const response = await api.get("/reportsubmissions");
    return response.data;
  },

  async getReport(id) {
    const response = await api.get(`/reportsubmissions/${id}`);
    return response.data;
  },

  async createReport(reportData) {
    const response = await api.post("/reportsubmissions", reportData);
    return response.data;
  },

  async updateReport(id, reportData) {
    const response = await api.put(`/reportsubmissions/${id}`, reportData);
    return response.data;
  },

  async deleteReport(id) {
    const response = await api.delete(`/reportsubmissions/${id}`);
    return response.data;
  },

  async submitReport(id) {
    const response = await api.post(`/reportsubmissions/${id}/submit`);
    return response.data;
  },

  async approveReport(id) {
    const response = await api.post(`/reportsubmissions/${id}/approve`);
    return response.data;
  },

  async rejectReport(id, reason) {
    const response = await api.post(`/reportsubmissions/${id}/reject`, {
      reason,
    });
    return response.data;
  },
};
