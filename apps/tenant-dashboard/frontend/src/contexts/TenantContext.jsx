import { createContext, useContext, useState, useEffect } from 'react';
import axios from 'axios';

const TenantContext = createContext();

export const useTenant = () => {
  const context = useContext(TenantContext);
  if (!context) {
    throw new Error('useTenant must be used within a TenantProvider');
  }
  return context;
};

export const TenantProvider = ({ children }) => {
  const [tenant, setTenant] = useState(null);
  const [modules, setModules] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    detectAndLoadTenant();
  }, []);

  const detectAndLoadTenant = async () => {
    try {
      // Detectar subdomain de la URL
      const hostname = window.location.hostname;
      let subdomain = null;

      console.log('🌐 Hostname:', hostname);

      // Detectar subdomain en producción
      if (hostname.includes('jegasolutions.co')) {
        const parts = hostname.split('.');
        console.log('📍 Parts:', parts);

        if (parts.length >= 3 && parts[0] !== 'www') {
          subdomain = parts[0];
        }
      }

      // En desarrollo, usar variable de entorno
      if (!subdomain && import.meta.env.DEV) {
        subdomain = import.meta.env.VITE_DEV_TENANT || 'test-tenant';
        console.log('🔧 Modo desarrollo - tenant:', subdomain);
      }

      if (!subdomain) {
        throw new Error('No se pudo detectar el tenant desde la URL');
      }

      console.log('✅ Subdomain detectado:', subdomain);

      // Llamar a API del Landing Backend
      const apiUrl =
        import.meta.env.VITE_API_URL || 'http://localhost:5014/api';

      console.log(
        '🔌 Conectando a:',
        `${apiUrl}/tenants/by-subdomain/${subdomain}`
      );

      const response = await axios.get(
        `${apiUrl}/tenants/by-subdomain/${subdomain}`
      );

      console.log('📦 Tenant data:', response.data);
      setTenant(response.data);

      // Cargar módulos del tenant
      const modulesResponse = await axios.get(
        `${apiUrl}/tenants/${response.data.id}/modules`
      );

      console.log('📦 Modules data:', modulesResponse.data);
      setModules(modulesResponse.data);

      setIsLoading(false);
    } catch (err) {
      console.error('❌ Error al cargar tenant:', err);
      setError(err.response?.data?.message || err.message);
      setIsLoading(false);
    }
  };

  const getModuleStatus = moduleName => {
    return modules.find(m => m.moduleName === moduleName)?.status === 'active';
  };

  const getModuleUrl = moduleName => {
    const module = modules.find(m => m.moduleName === moduleName);
    return module?.url || '#';
  };

  if (isLoading) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gray-50">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto mb-4"></div>
          <p className="text-gray-600">Cargando dashboard...</p>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gray-50">
        <div className="text-center max-w-md p-8 bg-white rounded-lg shadow-lg">
          <div className="text-red-500 text-6xl mb-4">⚠️</div>
          <h1 className="text-2xl font-bold text-gray-900 mb-2">
            Error al Cargar Tenant
          </h1>
          <p className="text-gray-600 mb-4">{error}</p>
          <a
            href="https://www.jegasolutions.co"
            className="inline-block px-6 py-3 bg-blue-600 text-white rounded-lg hover:bg-blue-700"
          >
            Volver al Sitio Principal
          </a>
        </div>
      </div>
    );
  }

  const value = {
    tenant,
    modules,
    isLoading,
    getModuleStatus,
    getModuleUrl,
    tenantName: tenant?.companyName || 'Tenant',
    subdomain: tenant?.subdomain,
  };

  return (
    <TenantContext.Provider value={value}>{children}</TenantContext.Provider>
  );
};
