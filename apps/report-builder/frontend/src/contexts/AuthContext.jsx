import React, { createContext, useContext, useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { jwtDecode } from 'jwt-decode';
import { authService } from '../services/authService';

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

  // Effect 1: SSO Token Handler - Ejecuta PRIMERO
  useEffect(() => {
    const urlParams = new URLSearchParams(window.location.search);
    const ssoToken = urlParams.get('token');

    if (ssoToken) {
      console.log('🔐 SSO token detected in URL');
      try {
        // Decodificar el JWT para obtener los datos del usuario
        const decoded = jwtDecode(ssoToken);
        console.log('✅ Token decoded successfully:', decoded);

        // Guardar token en localStorage
        localStorage.setItem('token', ssoToken);

        // CRÍTICO: Marcar que este token viene de SSO y ya está validado
        localStorage.setItem('ssoValidated', 'true');

        // Setear el usuario desde el token decodificado
        setUser(decoded);
        setLoading(false);

        // Limpiar la URL para quitar el token
        navigate('/', { replace: true });

        console.log('✅ SSO login successful, redirecting to dashboard');
      } catch (error) {
        console.error('❌ Error decoding SSO token:', error);
        localStorage.removeItem('ssoValidated');
        setLoading(false);
      }
      return;
    }
  }, [navigate]);

  // Effect 2: Token Verification - Solo para tokens NO-SSO
  useEffect(() => {
    const checkAuth = async () => {
      const token = localStorage.getItem('token');
      const isSSOValidated = localStorage.getItem('ssoValidated') === 'true';

      // Si no hay token, terminar loading
      if (!token) {
        setLoading(false);
        return;
      }

      // CRÍTICO: Si el token viene de SSO, solo decodificarlo, NO llamar al backend
      if (isSSOValidated) {
        console.log(
          '✅ SSO token detected in localStorage, skipping backend verification'
        );
        try {
          const decoded = jwtDecode(token);
          setUser(decoded);
          console.log('✅ User loaded from SSO token:', decoded);
        } catch (error) {
          console.error('❌ Error decoding stored SSO token:', error);
          // Si el token es inválido, limpiar todo
          localStorage.removeItem('token');
          localStorage.removeItem('ssoValidated');
          setUser(null);
        }
        setLoading(false);
        return;
      }

      // Solo para tokens NO-SSO: Verificar contra el backend
      console.log('🔍 Non-SSO token detected, verifying against backend...');
      try {
        const response = await authService.verifyToken(token);
        setUser(response.user);
        console.log('✅ Token verified successfully');
      } catch (error) {
        console.error('❌ Token verification failed:', error);
        localStorage.removeItem('token');
        localStorage.removeItem('ssoValidated');
        setUser(null);
      } finally {
        setLoading(false);
      }
    };

    // Solo ejecutar si loading es true
    if (loading) {
      checkAuth();
    }
  }, [loading]);

  const login = async (email, password) => {
    try {
      const response = await authService.login(email, password);
      const { token, user: userData } = response;

      localStorage.setItem('token', token);
      // Marcar como NO-SSO para que se verifique en el futuro
      localStorage.removeItem('ssoValidated');
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
    localStorage.removeItem('ssoValidated');
    setUser(null);
    navigate('/login');
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
