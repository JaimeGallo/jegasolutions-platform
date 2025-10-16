import React, { createContext, useContext, useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { authService } from "../services/authService";

const AuthContext = createContext();

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth must be used within an AuthProvider");
  }
  return context;
};

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();

  useEffect(() => {
    /**
     * 🔐 FLUJO SSO (Single Sign-On):
     * 1. Primero buscar token en URL (desde tenant-dashboard)
     * 2. Si no hay token en URL, buscar en localStorage
     * 3. Validar el token encontrado
     * 4. Si es válido, guardar y autenticar al usuario
     */
    const initializeAuth = async () => {
      try {
        // PASO 1: Buscar token en URL query parameters
        const urlParams = new URLSearchParams(window.location.search);
        const tokenFromUrl = urlParams.get("token");

        let tokenToValidate = null;

        if (tokenFromUrl) {
          console.log("✅ Token recibido desde URL (SSO)");
          tokenToValidate = tokenFromUrl;

          // Guardar el token en localStorage inmediatamente
          localStorage.setItem("token", tokenFromUrl);

          // Limpiar el token de la URL por seguridad
          window.history.replaceState({}, document.title, window.location.pathname);
        } else {
          // PASO 2: Si no hay token en URL, buscar en localStorage
          tokenToValidate = localStorage.getItem("token");
          if (tokenToValidate) {
            console.log("✅ Token encontrado en localStorage");
          }
        }

        // PASO 3: Validar el token si existe
        if (tokenToValidate) {
          try {
            const userData = await authService.verifyToken(tokenToValidate);
            console.log("✅ Token validado exitosamente:", userData);
            setUser(userData);

            // Si estamos en la raíz o login, redirigir al dashboard
            if (window.location.pathname === "/" || window.location.pathname === "/login") {
              navigate("/", { replace: true });
            }
          } catch (error) {
            console.error("❌ Error al validar token:", error);
            // Token inválido, limpiar
            localStorage.removeItem("token");

            // No redirigir automáticamente a login si venimos de URL con token
            // El usuario podría necesitar ver un mensaje de error
            if (!tokenFromUrl) {
              navigate("/login");
            }
          }
        } else {
          console.log("ℹ️ No se encontró token en URL ni localStorage");
        }
      } catch (error) {
        console.error("❌ Error en inicialización de autenticación:", error);
      } finally {
        setLoading(false);
      }
    };

    initializeAuth();
  }, [navigate]);

  const login = async (email, password) => {
    try {
      const response = await authService.login(email, password);
      const { token, user: userData } = response;

      localStorage.setItem("token", token);
      setUser(userData);

      return { success: true };
    } catch (error) {
      return {
        success: false,
        error: error.response?.data?.message || "Login failed",
      };
    }
  };

  const logout = () => {
    localStorage.removeItem("token");
    setUser(null);
    navigate("/login");
  };

  const updateUser = (userData) => {
    setUser((prev) => ({ ...prev, ...userData }));
  };

  const value = {
    user,
    loading,
    login,
    logout,
    updateUser,
    isAuthenticated: !!user,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};




