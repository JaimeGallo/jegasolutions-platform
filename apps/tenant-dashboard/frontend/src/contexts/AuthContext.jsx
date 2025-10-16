import { createContext, useContext, useState, useEffect } from 'react';

const AuthContext = createContext();

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};

export const AuthProvider = ({ children, tenantId }) => {
  const [user, setUser] = useState(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    // Check for existing auth token
    const token = localStorage.getItem('token');
    const userData = localStorage.getItem('userData');

    if (token && userData) {
      try {
        setUser(JSON.parse(userData));
      } catch (error) {
        console.error('Error parsing user data:', error);
        localStorage.removeItem('token');
        localStorage.removeItem('userData');
      }
    }

    setIsLoading(false);
  }, []);

  const login = async (email, password) => {
    try {
      // Get API URL from environment variable
      const apiUrl =
        import.meta.env.VITE_API_URL || 'http://localhost:5014/api';

      console.log('🔐 Attempting login to:', `${apiUrl}/auth/login`);
      console.log('📧 Email:', email);
      console.log('🏢 TenantId:', tenantId);

      const response = await fetch(`${apiUrl}/auth/login`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          email,
          password,
          tenantId: tenantId || null,
        }),
      });

      if (response.ok) {
        const data = await response.json();
        const userData = {
          id: data.user.id,
          email: data.user.email,
          name: data.user.name,
          role: data.user.role,
          tenantId: data.user.tenantId,
        };

        localStorage.setItem('token', data.token);
        localStorage.setItem('userData', JSON.stringify(userData));
        setUser(userData);

        console.log('✅ Login successful');
        return { success: true };
      } else {
        const error = await response.json();
        console.error('❌ Login failed:', error);
        return { success: false, error: error.message };
      }
    } catch (error) {
      console.error('💥 Login error:', error);
      return { success: false, error: 'Error de conexión con el servidor' };
    }
  };

  const logout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('userData');
    setUser(null);
  };

  const value = {
    user,
    isLoading,
    login,
    logout,
    isAuthenticated: !!user,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};
