// Elimina una solicitud de compensación por ID
export const deleteCompensationRequest = async (id) => {
  try {
    const response = await fetch(`${BASE_URL}/${id}`, {
      method: "DELETE",
      headers: getAuthHeaders(),
    });
    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      throw new Error(
        errorData.error || "Error al eliminar la solicitud de compensación"
      );
    }
    return true;
  } catch (error) {
    console.error(
      "Error al eliminar solicitud de compensación:",
      error.message
    );
    throw error;
  }
};

// Edita una solicitud de compensación (PUT)
export const editCompensationRequest = async (id, data) => {
  try {
    const response = await fetch(`${BASE_URL}/${id}`, {
      method: "PUT",
      headers: getAuthHeaders(),
      body: JSON.stringify(data),
    });
    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      throw new Error(
        errorData.error || "Error al editar la solicitud de compensación"
      );
    }
    return await response.json();
  } catch (error) {
    console.error("Error al editar solicitud de compensación:", error.message);
    throw error;
  }
};
import { API_CONFIG } from "../environments/api.config";
import { getAuthHeaders } from "../environments/http-headers";

const BASE_URL = `${API_CONFIG.BASE_URL}/api/CompensationRequest`;

export const createCompensationRequest = async (compensationRequest) => {
  try {
    // Verificar que estos datos se estén enviando correctamente
    console.log("Datos enviados:", {
      employeeId: compensationRequest.employeeId,
      workDate: compensationRequest.workDate, // Formato ISO date
      requestedCompensationDate: compensationRequest.requestedCompensationDate,
      justification: compensationRequest.justification,
    });

    const response = await fetch(BASE_URL, {
      method: "POST",
      headers: getAuthHeaders(),
      body: JSON.stringify(compensationRequest),
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      throw new Error(
        errorData.error || "Error al crear la solicitud de compensación"
      );
    }

    return await response.json();
  } catch (error) {
    console.error("Error al crear solicitud de compensación:", error.message);
    throw error;
  }
};

export const getCompensationRequests = async () => {
  try {
    const response = await fetch(BASE_URL, {
      method: "GET",
      headers: getAuthHeaders(),
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      throw new Error(
        errorData.error || "Error al obtener las solicitudes de compensación"
      );
    }

    return await response.json();
  } catch (error) {
    console.error(
      "Error al obtener solicitudes de compensación:",
      error.message
    );
    throw error;
  }
};

export const getCompensationRequestsByEmployee = async (employeeId) => {
  try {
    const response = await fetch(`${BASE_URL}/employee/${employeeId}`, {
      method: "GET",
      headers: getAuthHeaders(),
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      throw new Error(
        errorData.error || "Error al obtener las solicitudes del empleado"
      );
    }

    return await response.json();
  } catch (error) {
    console.error("Error al obtener solicitudes del empleado:", error.message);
    throw error;
  }
};

export const updateCompensationRequestStatus = async (
  requestId,
  status,
  justification
) => {
  try {
    const body = {
      status,
      justification: justification || "",
    };
    const response = await fetch(`${BASE_URL}/${requestId}/status`, {
      method: "PUT", // El controlador en C# usa [HttpPut]
      headers: getAuthHeaders(),
      body: JSON.stringify(body),
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      throw new Error(
        errorData.error || "Error al actualizar el estado de la solicitud"
      );
    }

    return await response.json();
  } catch (error) {
    console.error("Error al actualizar estado de solicitud:", error.message);
    throw error;
  }
};

/**
 * Obtiene todas las solicitudes de compensación (para superusuarios).
 * @param {string|null} startDate - Fecha de inicio (YYYY-MM-DD)
 * @param {string|null} endDate - Fecha de fin (YYYY-MM-DD)
 */
export const getAllCompensationRequests = async (
  startDate = null,
  endDate = null
) => {
  try {
    const url = new URL(`${BASE_URL}/all`);
    if (startDate && endDate) {
      url.searchParams.append("startDate", startDate);
      url.searchParams.append("endDate", endDate);
    }

    const response = await fetch(url.toString(), {
      method: "GET",
      headers: getAuthHeaders(),
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      throw new Error(
        errorData.error ||
          "Error al obtener todas las solicitudes de compensación"
      );
    }
    return await response.json();
  } catch (error) {
    console.error(
      "Error al obtener todas las solicitudes de compensación:",
      error.message
    );
    throw error;
  }
};

/**
 * Obtiene las solicitudes de compensación para el manager autenticado.
 * @param {string|null} startDate - Fecha de inicio (YYYY-MM-DD)
 * @param {string|null} endDate - Fecha de fin (YYYY-MM-DD)
 */
export const getCompensationRequestsByManager = async (
  startDate = null,
  endDate = null
) => {
  try {
    const url = new URL(`${BASE_URL}/manager`);
    if (startDate && endDate) {
      url.searchParams.append("startDate", startDate);
      url.searchParams.append("endDate", endDate);
    }
    const response = await fetch(url.toString(), {
      method: "GET",
      headers: getAuthHeaders(),
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      throw new Error(
        errorData.error || "Error al obtener las solicitudes de compensación"
      );
    }
    return await response.json();
  } catch (error) {
    console.error(
      "Error al obtener las solicitudes de compensación del manager:",
      error.message
    );
    throw error;
  }
};
