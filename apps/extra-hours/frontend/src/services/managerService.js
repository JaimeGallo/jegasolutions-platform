import axios from 'axios';

const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:8080';

const getAuthToken = () => {
  return localStorage.getItem('token');
};

export const managerService = {
  /**
   * Obtiene la lista de todos los managers
   */
  getAllManagers: async () => {
    try {
      const token = getAuthToken();
      const response = await axios.get(`${API_URL}/api/managers`, {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });
      return response.data;
    } catch (error) {
      console.error('Error fetching managers:', error);
      throw new Error(
        error.response?.data?.error || 'Error al obtener la lista de managers'
      );
    }
  },

  /**
   * Obtiene un manager específico por ID
   */
  getManagerById: async (id) => {
    try {
      const token = getAuthToken();
      const response = await axios.get(`${API_URL}/api/managers/${id}`, {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });
      return response.data;
    } catch (error) {
      console.error('Error fetching manager:', error);
      throw new Error(
        error.response?.data?.error || 'Error al obtener el manager'
      );
    }
  },
};

export default managerService;

