import React, { createContext, useContext, useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { jwtDecode } from "jwt-decode";
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
    const initAuth = async () => {
      // PASO 1: Verificar SSO token en URL
      const urlParams = new URLSearchParams(window.location.search);
      const ssoToken = urlParams.get('token');

      if (ssoToken) {
        console.log('🔐 SSO token detected in URL');
        try {
          const decoded = jwtDecode(ssoToken);
          console.log('✅ Token decoded successfully:', decoded);

          localStorage.setItem('token', ssoToken);
          localStorage.setItem('ssoValidated', 'true');

          setUser(decoded);
          setLoading(false);

          navigate('/', { replace: true });

          console.log('✅ SSO login successful, redirecting to dashboard');
          return;
        } catch (error) {
          console.error('❌ Error decoding SSO token:', error);
          localStorage.removeItem('ssoValidated');
          setLoading(false);
          return;
        }
      }

      // PASO 2: Verificar token existente
      const token = localStorage.getItem('token');
      const isSSOValidated = localStorage.getItem('ssoValidated') === 'true';

      if (!token) {
        setLoading(false);
        return;
      }

      if (isSSOValidated) {
        console.log('✅ SSO token detected in localStorage, skipping backend verification');
        try {
          const decoded = jwtDecode(token);
          setUser(decoded);
          console.log('✅ User loaded from SSO token:', decoded);
        } catch (error) {
          console.error('❌ Error decoding stored SSO token:', error);
          localStorage.removeItem('token');
          localStorage.removeItem('ssoValidated');
          setUser(null);
        }
        setLoading(false);
        return;
      }

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

    initAuth();
  }, [navigate]);

  const login = async (email, password) => {
    try {
      const response = await authService.login(email, password);
      const { token, user: userData } = response;

      localStorage.setItem("token", token);
      localStorage.removeItem('ssoValidated');
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
    localStorage.removeItem("ssoValidated");
    setUser(null);
    navigate('/login');
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
