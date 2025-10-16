import { useState, useEffect } from "react";
import PropTypes from "prop-types";
import { useNavigate } from "react-router-dom";
import { jwtDecode } from "jwt-decode";
import { AuthContext } from "./AuthContext";

export const AuthProvider = ({ children }) => {
  const navigate = useNavigate();

  const [auth, setAuth] = useState(() => {
    const token = localStorage.getItem("token");
    const role = localStorage.getItem("role");
    const uniqueName = localStorage.getItem("unique_name");

    if (token && role) {
      const decodedToken = jwtDecode(token);
      console.log("Token decodificado:", decodedToken);

      if (!localStorage.getItem("id")) {
        localStorage.setItem("id", decodedToken.id);
      }

      if (decodedToken.unique_name && !uniqueName) {
        localStorage.setItem("unique_name", decodedToken.unique_name);
      }

      return {
        token,
        role,
        uniqueName: uniqueName || decodedToken.unique_name,
      };
    }
    return null;
  });

  /**
   * 🔐 NUEVO: Capturar token desde URL para SSO
   */
  useEffect(() => {
    const initializeAuthFromUrl = () => {
      // Buscar token en URL query parameters
      const urlParams = new URLSearchParams(window.location.search);
      const tokenFromUrl = urlParams.get("token");

      if (tokenFromUrl) {
        console.log("✅ Token recibido desde URL (SSO)");

        try {
          // Decodificar el token
          const decodedToken = jwtDecode(tokenFromUrl);
          console.log("✅ Token decodificado:", decodedToken);

          // Extraer información del token
          const role = decodedToken.role || decodedToken["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
          const uniqueName = decodedToken.unique_name || decodedToken["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"];
          const userId = decodedToken.userId || decodedToken.sub || decodedToken.id;

          // Formatear el rol (eliminar corchetes si los tiene)
          const formattedRole = role ? role.replace(/[[\]]/g, "") : "user";

          // Guardar en localStorage
          localStorage.setItem("token", tokenFromUrl);
          localStorage.setItem("role", formattedRole);
          localStorage.setItem("id", userId);

          if (uniqueName) {
            localStorage.setItem("unique_name", uniqueName);
          }

          // Actualizar estado de autenticación
          setAuth({
            token: tokenFromUrl,
            role: formattedRole,
            uniqueName: uniqueName,
          });

          // Limpiar el token de la URL por seguridad
          window.history.replaceState({}, document.title, window.location.pathname);

          // Redirigir al menu
          console.log("✅ Redirigiendo al menú principal");
          navigate("/menu", { replace: true });
        } catch (error) {
          console.error("❌ Error al procesar token desde URL:", error);
          // Si hay error, limpiar y redirigir a login
          localStorage.clear();
          navigate("/", { replace: true });
        }
      }
    };

    initializeAuthFromUrl();
  }, [navigate]);

  const login = ({ token, role }) => {
    const formattedRole = role.replace(/[[\]]/g, "");
    const decodedToken = jwtDecode(token);

    localStorage.setItem("token", token);
    localStorage.setItem("role", formattedRole);
    localStorage.setItem("id", decodedToken.id);

    if (decodedToken.unique_name) {
      localStorage.setItem("unique_name", decodedToken.unique_name);
    }

    setAuth({
      token,
      role: formattedRole,
      uniqueName: decodedToken.unique_name,
    });
    navigate("/menu");
  };

  const logout = () => {
    setAuth(null);
    localStorage.removeItem("token");
    localStorage.removeItem("role");
    localStorage.removeItem("id");
    localStorage.removeItem("unique_name");
    navigate("/");
  };

  return (
    <AuthContext.Provider value={{ auth, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
};

AuthProvider.propTypes = {
  children: PropTypes.node.isRequired,
};
