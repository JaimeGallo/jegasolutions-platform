import axios from 'axios';

const API_BASE_URL =
  import.meta.env.VITE_API_URL || 'http://localhost:5014/api';

const authService = {
  /**
   * Cambiar contraseña del usuario autenticado
   * @param {string} currentPassword - Contraseña actual
   * @param {string} newPassword - Nueva contraseña
   * @returns {Promise<{message: string}>}
   */
  async changePassword(currentPassword, newPassword) {
    try {
      const token = localStorage.getItem('token');

      if (!token) {
        throw new Error('No hay sesión activa');
      }

      const response = await axios.put(
        `${API_BASE_URL}/auth/change-password`,
        {
          currentPassword,
          newPassword,
        },
        {
          headers: {
            Authorization: `Bearer ${token}`,
            'Content-Type': 'application/json',
          },
        }
      );

      return response.data;
    } catch (error) {
      // Manejar errores de Axios
      if (error.response) {
        throw new Error(
          error.response.data.message || 'Error al cambiar la contraseña'
        );
      } else if (error.request) {
        throw new Error('No se pudo conectar con el servidor');
      } else {
        throw error;
      }
    }
  },

  /**
   * Cambiar contraseña de cualquier usuario (solo admin)
   * @param {number} userId - ID del usuario
   * @param {string} newPassword - Nueva contraseña
   * @returns {Promise<{message: string}>}
   */
  async changePasswordAdmin(userId, newPassword) {
    try {
      const token = localStorage.getItem('token');

      if (!token) {
        throw new Error('No hay sesión activa');
      }

      const response = await axios.put(
        `${API_BASE_URL}/auth/change-password-admin`,
        {
          userId,
          newPassword,
        },
        {
          headers: {
            Authorization: `Bearer ${token}`,
            'Content-Type': 'application/json',
          },
        }
      );

      return response.data;
    } catch (error) {
      if (error.response) {
        throw new Error(
          error.response.data.message || 'Error al cambiar la contraseña'
        );
      } else if (error.request) {
        throw new Error('No se pudo conectar con el servidor');
      } else {
        throw error;
      }
    }
  },
};

export default authService;
