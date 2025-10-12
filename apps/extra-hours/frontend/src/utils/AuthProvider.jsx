import { useState, useEffect } from "react";
import PropTypes from "prop-types";
import { useNavigate, useLocation } from "react-router-dom";
import { jwtDecode } from "jwt-decode";
import { AuthContext } from "./AuthContext"; // Importar el contexto desde su archivo

export const AuthProvider = ({ children }) => {
  const navigate = useNavigate();
  const location = useLocation();

  const [auth, setAuth] = useState(() => {
    const token = localStorage.getItem("token");
    const role = localStorage.getItem("role");
    const uniqueName = localStorage.getItem("unique_name");

    if (token && role) {
      const decodedToken = jwtDecode(token); // Decodificar el token
      console.log("Token decodificado:", decodedToken);

      // Almacenar el ID del usuario en localStorage si no está presente
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

  // ✅ SSO: Detectar token en URL al cargar la aplicación
  useEffect(() => {
    const urlParams = new URLSearchParams(location.search);
    const ssoToken = urlParams.get('token');

    if (ssoToken) {
      console.log('🔐 SSO: Token detectado en URL');
      
      try {
        // Validar y decodificar el token
        const decodedToken = jwtDecode(ssoToken);
        console.log('✅ SSO: Token válido, userId:', decodedToken.userId || decodedToken.id);

        // Extraer rol del token (puede venir como 'role' o dentro de un array)
        let userRole = decodedToken.role;
        if (Array.isArray(userRole)) {
          userRole = userRole[0];
        }
        if (!userRole) {
          userRole = 'employee'; // Rol por defecto
        }

        // Usar la función login existente para configurar la autenticación
        const formattedRole = userRole.replace(/[[\]]/g, "");
        
        localStorage.setItem("token", ssoToken);
        localStorage.setItem("role", formattedRole);
        localStorage.setItem("id", decodedToken.userId || decodedToken.id || decodedToken.sub);

        if (decodedToken.unique_name || decodedToken.email) {
          localStorage.setItem("unique_name", decodedToken.unique_name || decodedToken.email);
        }

        setAuth({
          token: ssoToken,
          role: formattedRole,
          uniqueName: decodedToken.unique_name || decodedToken.email,
        });

        // Limpiar el token de la URL y redirigir al menú
        const cleanUrl = window.location.pathname;
        window.history.replaceState({}, document.title, cleanUrl);
        
        console.log('🚀 SSO: Redirigiendo al menú...');
        navigate("/menu");
      } catch (error) {
        console.error('❌ SSO: Token inválido:', error);
        // Si el token es inválido, continuar con el flujo normal de login
      }
    }
  }, [location.search, navigate]);

  const login = ({ token, role }) => {
    const formattedRole = role.replace(/[[\]]/g, "");
    const decodedToken = jwtDecode(token); // Decodificar el token

    // Almacenar el rol y el ID del usuario en localStorage
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
