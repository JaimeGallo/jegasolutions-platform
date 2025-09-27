import api from "./api";

export const templateService = {
  async getTemplates() {
    const response = await api.get("/templates");
    return response.data;
  },

  async getTemplate(id) {
    const response = await api.get(`/templates/${id}`);
    return response.data;
  },

  async getTemplatesByType(type) {
    const response = await api.get(`/templates/by-type/${type}`);
    return response.data;
  },

  async createTemplate(templateData) {
    const response = await api.post("/templates", templateData);
    return response.data;
  },

  async updateTemplate(id, templateData) {
    const response = await api.put(`/templates/${id}`, templateData);
    return response.data;
  },

  async deleteTemplate(id) {
    const response = await api.delete(`/templates/${id}`);
    return response.data;
  },
};
