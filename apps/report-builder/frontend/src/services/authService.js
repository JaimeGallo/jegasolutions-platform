import api from "./api";

export const authService = {
  async login(email, password) {
    const response = await api.post("/auth/login", { email, password });
    return response.data;
  },

  async logout() {
    // Clear local storage
    localStorage.removeItem("token");
  },

  async verifyToken(token) {
    const response = await api.get("/auth/verify", {
      headers: { Authorization: `Bearer ${token}` },
    });
    return response.data;
  },

  async refreshToken() {
    const response = await api.post("/auth/refresh");
    return response.data;
  },
};
