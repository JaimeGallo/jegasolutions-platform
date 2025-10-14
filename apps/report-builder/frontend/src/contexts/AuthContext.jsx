import React, { createContext, useContext, useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { authService } from '../services/authService';
import { jwtDecode } from 'jwt-decode';

const AuthContext = createContext();

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();
  const location = useLocation();

  // ✅ SSO: Detectar token en URL al cargar la aplicación
  useEffect(() => {
    const urlParams = new URLSearchParams(location.search);
    const ssoToken = urlParams.get('token');

    if (ssoToken) {
      console.log('🔐 SSO: Token detectado en URL');

      try {
        // Validar y decodificar el token
        const decodedToken = jwtDecode(ssoToken);
        console.log(
          '✅ SSO: Token válido, userId:',
          decodedToken.userId || decodedToken.id
        );

        // Guardar token en localStorage
        localStorage.setItem('token', ssoToken);

        // Mapear rol del SSO a roles internos
        let userRole = Array.isArray(decodedToken.role)
          ? decodedToken.role[0]
          : decodedToken.role || 'employee';

        // Mapear "superusuario" a "superusuario" para mantener consistencia
        if (userRole.toLowerCase() === 'admin') {
          userRole = 'superusuario';
        }

        // Crear objeto de usuario desde el token
        const userData = {
          id: decodedToken.userId || decodedToken.id || decodedToken.sub,
          email: decodedToken.email || decodedToken.unique_name,
          name: decodedToken.name || decodedToken.unique_name,
          role: userRole,
        };

        setUser(userData);
        setLoading(false);

        // Limpiar el token de la URL
        const cleanUrl = window.location.pathname;
        window.history.replaceState({}, document.title, cleanUrl);

        console.log('🚀 SSO: Redirigiendo al dashboard...');
        navigate('/dashboard');
        return;
      } catch (error) {
        console.error('❌ SSO: Token inválido:', error);
        // Si el token es inválido, continuar con el flujo normal
      }
    }

    // Flujo normal: verificar token existente
    const token = localStorage.getItem('token');
    if (token) {
      authService
        .verifyToken(token)
        .then(userData => {
          setUser(userData);
        })
        .catch(() => {
          localStorage.removeItem('token');
        })
        .finally(() => {
          setLoading(false);
        });
    } else {
      setLoading(false);
    }
  }, [location.search, navigate]);

  const login = async (email, password) => {
    try {
      const response = await authService.login(email, password);
      const { token, user: userData } = response;

      localStorage.setItem('token', token);
      setUser(userData);

      return { success: true };
    } catch (error) {
      return {
        success: false,
        error: error.response?.data?.message || 'Login failed',
      };
    }
  };

  const logout = () => {
    localStorage.removeItem('token');
    setUser(null);
  };

  const updateUser = userData => {
    setUser(prev => ({ ...prev, ...userData }));
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
