import { API_CONFIG } from "../environments/api.config";
import { getAuthHeaders } from "../environments/http-headers";

export const updateConfig = async (configData) => {
  try {
    const configToSend = { ...configData };

    const timeFields = [
      "diurnalStart",
      "diurnalEnd",
      "nocturnalStart",
      "nocturnalEnd",
    ];

    timeFields.forEach((field) => {
      // Asegurarse de que el campo exista y tenga el formato HH:mm
      if (configToSend[field] && /^\d{2}:\d{2}$/.test(configToSend[field])) {
        configToSend[field] = `${configToSend[field]}:00`;
      }
    });

    const response = await fetch(`${API_CONFIG.BASE_URL}/api/config`, {
      method: "PUT",
      headers: getAuthHeaders(),
      body: JSON.stringify(configToSend),
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      // Usamos el `title` del error de validación si está disponible
      const errorMessage =
        errorData.title ||
        errorData.message ||
        "Error actualizando la configuración";
      console.error("Detalles del error:", errorData.errors || errorData);
      throw new Error(errorMessage);
    }

    // Un PUT exitoso puede no devolver contenido (204 No Content)
    if (response.status === 204) {
      return { success: true };
    }

    return await response.json();
  } catch (error) {
    console.error("Error en updateConfig:", error);
    throw error;
  }
};
