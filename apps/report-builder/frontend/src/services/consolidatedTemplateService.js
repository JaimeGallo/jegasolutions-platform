// Servicio mejorado y sincronizado con el backend

import api from "./api";

export const ConsolidatedTemplateService = {
  // ==================== SUPERUSUARIO OPERATIONS ====================
  
  /**
   * Obtiene todas las plantillas consolidadas del tenant
   * @param {string} status - Filtro opcional por estado
   * @returns {Promise<Array>}
   */
  getConsolidatedTemplates: async (status = null) => {
    try {
      const url = status 
        ? `/ConsolidatedTemplates?status=${status}`
        : "/ConsolidatedTemplates";
      const response = await api.get(url);
      return response.data;
    } catch (error) {
      console.error("Error al obtener plantillas consolidadas:", error);
      throw error;
    }
  },

  /**
   * Obtiene una plantilla consolidada específica
   * @param {number} id - ID de la plantilla
   * @returns {Promise<Object>}
   */
  getConsolidatedTemplate: async (id) => {
    try {
      const response = await api.get(`/ConsolidatedTemplates/${id}`);
      return response.data;
    } catch (error) {
      console.error(`Error al obtener plantilla consolidada ${id}:`, error);
      throw error;
    }
  },

  /**
   * Crea una nueva plantilla consolidada
   * @param {Object} templateData - Datos de la plantilla
   * @returns {Promise<Object>}
   */
  createConsolidatedTemplate: async (templateData) => {
    try {
      const response = await api.post(
        "/ConsolidatedTemplates",
        templateData
      );
      return response.data;
    } catch (error) {
      console.error("Error al crear plantilla consolidada:", error);
      throw error;
    }
  },

  /**
   * Actualiza una plantilla consolidada
   * @param {number} id - ID de la plantilla
   * @param {Object} updateData - Datos a actualizar
   * @returns {Promise<Object>}
   */
  updateConsolidatedTemplate: async (id, updateData) => {
    try {
      const response = await api.put(`/ConsolidatedTemplates/${id}`, {
        ...updateData,
        id, // Asegurar que el ID está en el body
      });
      return response.data;
    } catch (error) {
      console.error(`Error al actualizar plantilla consolidada ${id}:`, error);
      throw error;
    }
  },

  /**
   * Elimina una plantilla consolidada (soft delete)
   * @param {number} id - ID de la plantilla
   * @returns {Promise<boolean>}
   */
  deleteConsolidatedTemplate: async (id) => {
    try {
      await api.delete(`/ConsolidatedTemplates/${id}`);
      return true;
    } catch (error) {
      console.error(`Error al eliminar plantilla consolidada ${id}:`, error);
      throw error;
    }
  },

  /**
   * Agrega una sección a una plantilla existente
   * @param {number} templateId - ID de la plantilla
   * @param {Object} sectionData - Datos de la sección
   * @returns {Promise<Object>}
   */
  addSectionToTemplate: async (templateId, sectionData) => {
    try {
      const response = await api.post(
        `/ConsolidatedTemplates/${templateId}/sections`,
        sectionData
      );
      return response.data;
    } catch (error) {
      console.error(`Error al agregar sección a plantilla ${templateId}:`, error);
      throw error;
    }
  },

  /**
   * Asigna una sección a un área
   * @param {number} templateId - ID de la plantilla
   * @param {number} sectionId - ID de la sección
   * @param {Object} assignData - Datos de asignación (areaId, userId, deadline)
   * @returns {Promise<boolean>}
   */
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

  /**
   * Actualiza el estado de una sección (Superusuario)
   * @param {number} sectionId - ID de la sección
   * @param {string} status - Nuevo estado
   * @returns {Promise<boolean>}
   */
  updateSectionStatus: async (sectionId, status) => {
    try {
      await api.put(
        `/ConsolidatedTemplates/sections/${sectionId}/status`,
        { sectionId, status }
      );
      return true;
    } catch (error) {
      console.error(
        `Error al actualizar estado de sección ${sectionId}:`,
        error
      );
      throw error;
    }
  },

  /**
   * Obtiene estadísticas de una plantilla consolidada
   * @param {number} tenantId - ID del tenant (opcional si está en JWT)
   * @returns {Promise<Object>}
   */
  getConsolidatedTemplateStats: async () => {
    try {
      const response = await api.get("/ConsolidatedTemplates/stats");
      return response.data;
    } catch (error) {
      console.error("Error al obtener estadísticas:", error);
      throw error;
    }
  },

  /**
   * Consolida el informe final
   * @param {Object} consolidateData - Datos para consolidar
   * @returns {Promise<Blob>}
   */
  consolidateReport: async (consolidateData) => {
    try {
      const response = await api.post(
        "/ConsolidatedTemplates/consolidate",
        consolidateData,
        { responseType: "blob" } // Para recibir archivo
      );
      return response.data;
    } catch (error) {
      console.error("Error al consolidar informe:", error);
      throw error;
    }
  },

  // ==================== USER OPERATIONS ====================

  /**
   * Obtiene "Mis Tareas" - secciones asignadas al usuario actual
   * @returns {Promise<Array>}
   */
  getMyTasks: async () => {
    try {
      const response = await api.get("/ConsolidatedTemplates/my-tasks");
      return response.data;
    } catch (error) {
      console.error("Error al obtener mis tareas:", error);
      throw error;
    }
  },

  /**
   * Obtiene detalle de una tarea específica del usuario
   * @param {number} sectionId - ID de la sección
   * @returns {Promise<Object>}
   */
  getMyTaskDetail: async (sectionId) => {
    try {
      const response = await api.get(
        `/ConsolidatedTemplates/my-tasks/${sectionId}`
      );
      return response.data;
    } catch (error) {
      console.error(`Error al obtener detalle de tarea ${sectionId}:`, error);
      throw error;
    }
  },

  /**
   * Actualiza el contenido de una sección (Usuario del área)
   * @param {number} sectionId - ID de la sección
   * @param {Object} content - Contenido de la sección
   * @param {boolean} markAsCompleted - Si marcar como completada
   * @returns {Promise<Object>}
   */
  updateSectionContent: async (sectionId, content, markAsCompleted = false) => {
    try {
      const response = await api.put(
        `/ConsolidatedTemplates/sections/${sectionId}/content`,
        {
          sectionId,
          sectionData: content,
          markAsCompleted,
        }
      );
      return response.data;
    } catch (error) {
      console.error(
        `Error al actualizar contenido de sección ${sectionId}:`,
        error
      );
      throw error;
    }
  },

  /**
   * Marca una sección como en progreso
   * @param {number} sectionId - ID de la sección
   * @returns {Promise<boolean>}
   */
  startWorkingOnSection: async (sectionId) => {
    try {
      await api.post(
        `/ConsolidatedTemplates/sections/${sectionId}/start-working`
      );
      return true;
    } catch (error) {
      console.error(
        `Error al iniciar trabajo en sección ${sectionId}:`,
        error
      );
      throw error;
    }
  },

  /**
   * Completa una sección
   * @param {number} sectionId - ID de la sección
   * @returns {Promise<boolean>}
   */
  completeSection: async (sectionId) => {
    try {
      await api.post(`/ConsolidatedTemplates/sections/${sectionId}/complete`);
      return true;
    } catch (error) {
      console.error(`Error al completar sección ${sectionId}:`, error);
      throw error;
    }
  },

  // ==================== NOTIFICATIONS ====================

  /**
   * Obtiene secciones próximas a vencer
   * @param {number} daysAhead - Días hacia adelante (default: 7)
   * @returns {Promise<Array>}
   */
  getUpcomingDeadlines: async (daysAhead = 7) => {
    try {
      const response = await api.get(
        `/ConsolidatedTemplates/upcoming-deadlines?daysAhead=${daysAhead}`
      );
      return response.data;
    } catch (error) {
      console.error("Error al obtener deadlines próximos:", error);
      throw error;
    }
  },

  /**
   * Obtiene secciones vencidas
   * @returns {Promise<Array>}
   */
  getOverdueSections: async () => {
    try {
      const response = await api.get("/ConsolidatedTemplates/overdue-sections");
      return response.data;
    } catch (error) {
      console.error("Error al obtener secciones vencidas:", error);
      throw error;
    }
  },

  // ==================== UTILITIES ====================

  /**
   * Obtiene el color de Tailwind para un estado
   * @param {string} status - Estado de la plantilla/sección
   * @returns {string}
   */
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

  /**
   * Obtiene el texto en español para un estado
   * @param {string} status - Estado de la plantilla/sección
   * @returns {string}
   */
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

  /**
   * Obtiene el color de Tailwind para un porcentaje de progreso
   * @param {number} percentage - Porcentaje de progreso (0-100)
   * @returns {string}
   */
  getProgressColor: (percentage) => {
    if (percentage >= 80) return "bg-green-500";
    if (percentage >= 60) return "bg-yellow-500";
    if (percentage >= 40) return "bg-orange-500";
    return "bg-red-500";
  },

  /**
   * Formatea una fecha para mostrar
   * @param {string|Date} date - Fecha a formatear
   * @returns {string}
   */
  formatDate: (date) => {
    if (!date) return "N/A";
    return new Date(date).toLocaleDateString("es-ES", {
      year: "numeric",
      month: "long",
      day: "numeric",
    });
  },

  /**
   * Calcula si una fecha está vencida
   * @param {string|Date} deadline - Fecha límite
   * @param {string} status - Estado actual
   * @returns {boolean}
   */
  isOverdue: (deadline, status) => {
    if (!deadline || status === "completed") return false;
    return new Date(deadline) < new Date();
  },

  /**
   * Calcula días restantes hasta una fecha
   * @param {string|Date} deadline - Fecha límite
   * @returns {number}
   */
  getDaysRemaining: (deadline) => {
    if (!deadline) return null;
    const days = Math.ceil(
      (new Date(deadline) - new Date()) / (1000 * 60 * 60 * 24)
    );
    return days;
  },
};

export default ConsolidatedTemplateService;