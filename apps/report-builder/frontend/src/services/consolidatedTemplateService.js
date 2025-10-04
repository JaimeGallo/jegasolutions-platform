import api from "./api";

export const ConsolidatedTemplateService = {
  // Obtener todas las plantillas consolidadas
  getConsolidatedTemplates: async () => {
    try {
      const response = await api.get("/ConsolidatedTemplates");
      return response.data;
    } catch (error) {
      console.error("Error al obtener plantillas consolidadas:", error);
      throw error;
    }
  },

  // Obtener una plantilla consolidada específica
  getConsolidatedTemplate: async (id) => {
    try {
      const response = await api.get(`/ConsolidatedTemplates/${id}`);
      return response.data;
    } catch (error) {
      console.error(`Error al obtener plantilla consolidada ${id}:`, error);
      throw error;
    }
  },

  // Crear una nueva plantilla consolidada desde informes anteriores
  createFromReports: async (templateData) => {
    try {
      const response = await api.post(
        "/ConsolidatedTemplates/from-reports",
        templateData
      );
      return response.data;
    } catch (error) {
      console.error("Error al crear plantilla consolidada:", error);
      throw error;
    }
  },

  // Actualizar una plantilla consolidada
  updateConsolidatedTemplate: async (id, updateData) => {
    try {
      await api.put(`/ConsolidatedTemplates/${id}`, updateData);
      return true;
    } catch (error) {
      console.error(`Error al actualizar plantilla consolidada ${id}:`, error);
      throw error;
    }
  },

  // Asignar una sección a un área
  assignSection: async (templateId, sectionId, assignData) => {
    try {
      await api.post(
        `/ConsolidatedTemplates/${templateId}/sections/${sectionId}/assign`,
        assignData
      );
      return true;
    } catch (error) {
      console.error(
        `Error al asignar sección ${sectionId} de plantilla ${templateId}:`,
        error
      );
      throw error;
    }
  },

  // Actualizar el estado de una sección
  updateSectionStatus: async (templateId, sectionId, statusData) => {
    try {
      await api.put(
        `/ConsolidatedTemplates/${templateId}/sections/${sectionId}/status`,
        statusData
      );
      return true;
    } catch (error) {
      console.error(
        `Error al actualizar estado de sección ${sectionId} de plantilla ${templateId}:`,
        error
      );
      throw error;
    }
  },

  // Obtener el estado detallado de una plantilla consolidada
  getTemplateStatus: async (id) => {
    try {
      const response = await api.get(`/ConsolidatedTemplates/${id}/status`);
      return response.data;
    } catch (error) {
      console.error(
        `Error al obtener estado de plantilla consolidada ${id}:`,
        error
      );
      throw error;
    }
  },

  // Eliminar una plantilla consolidada
  deleteConsolidatedTemplate: async (id) => {
    try {
      await api.delete(`/ConsolidatedTemplates/${id}`);
      return true;
    } catch (error) {
      console.error(`Error al eliminar plantilla consolidada ${id}:`, error);
      throw error;
    }
  },

  // Obtener secciones asignadas al usuario actual
  getMyAssignedSections: async () => {
    try {
      const response = await api.get(
        "/ConsolidatedTemplates/sections/my-assigned"
      );
      return response.data;
    } catch (error) {
      console.error("Error al obtener secciones asignadas:", error);
      throw error;
    }
  },

  // Utilidades para el manejo de estados
  getStatusColor: (status) => {
    const statusColors = {
      draft: "bg-gray-100 text-gray-800",
      in_progress: "bg-blue-100 text-blue-800",
      completed: "bg-green-100 text-green-800",
      archived: "bg-purple-100 text-purple-800",
      pending: "bg-yellow-100 text-yellow-800",
      assigned: "bg-indigo-100 text-indigo-800",
      reviewed: "bg-emerald-100 text-emerald-800",
    };
    return statusColors[status] || "bg-gray-100 text-gray-800";
  },

  getStatusText: (status) => {
    const statusTexts = {
      draft: "Borrador",
      in_progress: "En Progreso",
      completed: "Completado",
      archived: "Archivado",
      pending: "Pendiente",
      assigned: "Asignado",
      reviewed: "Revisado",
    };
    return statusTexts[status] || status;
  },

  getProgressColor: (percentage) => {
    if (percentage >= 80) return "bg-green-500";
    if (percentage >= 60) return "bg-yellow-500";
    if (percentage >= 40) return "bg-orange-500";
    return "bg-red-500";
  },
};

export default ConsolidatedTemplateService;

