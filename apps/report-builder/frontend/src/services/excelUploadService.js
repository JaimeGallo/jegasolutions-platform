import api from "./api";

export const ExcelUploadService = {
  // Subir archivo Excel
  uploadExcel: async (file, areaId, period) => {
    try {
      // Convertir archivo a Base64
      const base64 = await new Promise((resolve, reject) => {
        const reader = new FileReader();
        reader.onload = () => {
          const base64String = reader.result.split(",")[1];
          resolve(base64String);
        };
        reader.onerror = reject;
        reader.readAsDataURL(file);
      });

      const payload = {
        areaId: parseInt(areaId),
        period: period,
        fileName: file.name,
        fileBase64: base64,
      };

      const response = await api.post("/ExcelUploads/upload", payload);
      return response.data;
    } catch (error) {
      console.error("Error subiendo Excel:", error);
      throw error;
    }
  },

  // Listar uploads con filtros opcionales
  getExcelUploads: async (areaId = null, period = null) => {
    try {
      const params = {};
      if (areaId) params.areaId = areaId;
      if (period) params.period = period;

      const response = await api.get("/ExcelUploads", { params });
      return response.data;
    } catch (error) {
      console.error("Error obteniendo uploads:", error);
      throw error;
    }
  },

  // Obtener detalle de un upload
  getExcelUpload: async (id) => {
    try {
      const response = await api.get(`/ExcelUploads/${id}`);
      return response.data;
    } catch (error) {
      console.error(`Error obteniendo upload ${id}:`, error);
      throw error;
    }
  },

  // Obtener uploads por área
  getExcelUploadsByArea: async (areaId) => {
    try {
      const response = await api.get(`/ExcelUploads/area/${areaId}`);
      return response.data;
    } catch (error) {
      console.error(`Error obteniendo uploads del área ${areaId}:`, error);
      throw error;
    }
  },

  // Eliminar upload
  deleteExcelUpload: async (id) => {
    try {
      await api.delete(`/ExcelUploads/${id}`);
      return true;
    } catch (error) {
      console.error(`Error eliminando upload ${id}:`, error);
      throw error;
    }
  },

  // Re-procesar archivo
  reprocessExcelUpload: async (id) => {
    try {
      const response = await api.post(`/ExcelUploads/${id}/reprocess`);
      return response.data;
    } catch (error) {
      console.error(`Error re-procesando upload ${id}:`, error);
      throw error;
    }
  },

  // Solicitar análisis AI
  analyzeWithAI: async (excelUploadId, aiProvider, analysisType, customPrompt = null) => {
    try {
      const payload = {
        excelUploadId: parseInt(excelUploadId),
        aiProvider: aiProvider, // "openai", "anthropic", "deepseek", "groq"
        analysisType: analysisType, // "summary", "trends", "insights", "recommendations"
        customPrompt: customPrompt,
      };

      const response = await api.post("/ExcelUploads/analyze-ai", payload);
      return response.data;
    } catch (error) {
      console.error("Error solicitando análisis AI:", error);
      throw error;
    }
  },

  // Utilidades
  getStatusColor: (status) => {
    const statusColors = {
      uploaded: "bg-blue-100 text-blue-800",
      processing: "bg-yellow-100 text-yellow-800",
      completed: "bg-green-100 text-green-800",
      error: "bg-red-100 text-red-800",
    };
    return statusColors[status] || "bg-gray-100 text-gray-800";
  },

  getStatusText: (status) => {
    const statusTexts = {
      uploaded: "Cargado",
      processing: "Procesando",
      completed: "Completado",
      error: "Error",
    };
    return statusTexts[status] || status;
  },

  formatFileSize: (bytes) => {
    if (bytes === 0) return "0 Bytes";
    const k = 1024;
    const sizes = ["Bytes", "KB", "MB", "GB"];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return Math.round(bytes / Math.pow(k, i) * 100) / 100 + " " + sizes[i];
  },
};

export default ExcelUploadService;

