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
      let subdomain = null;
      const hostname = window.location.hostname;
      const pathname = window.location.pathname;
      const searchParams = new URLSearchParams(window.location.search);

      console.log('🌐 Hostname:', hostname);
      console.log('📍 Pathname:', pathname);

      // MÉTODO 1: Detectar desde subdomain (requiere DNS wildcard)
      if (hostname.includes('jegasolutions.co')) {
        const parts = hostname.split('.');
        if (parts.length >= 3 && parts[0] !== 'www') {
          subdomain = parts[0];
          console.log('✅ Tenant detectado desde subdomain:', subdomain);
        }
      }

      // MÉTODO 2: Detectar desde path (/t/tenant-name)
      if (!subdomain) {
        const pathMatch = pathname.match(/^\/t\/([^\/]+)/);
        if (pathMatch) {
          subdomain = pathMatch[1];
          console.log('✅ Tenant detectado desde path:', subdomain);
        }
      }

      // MÉTODO 3: Detectar desde query param (?tenant=tenant-name)
      if (!subdomain) {
        const tenantParam = searchParams.get('tenant');
        if (tenantParam) {
          subdomain = tenantParam;
          console.log('✅ Tenant detectado desde query:', subdomain);
        }
      }

      // MÉTODO 4: En desarrollo, usar variable de entorno
      if (!subdomain && import.meta.env.DEV) {
        subdomain = import.meta.env.VITE_DEV_TENANT || 'test-tenant';
        console.log('🔧 Modo desarrollo - tenant:', subdomain);
      }

      if (!subdomain) {
        throw new Error(
          'No se pudo detectar el tenant. Usa: subdomain, /t/tenant-name, o ?tenant=tenant-name'
        );
      }

      console.log('✅ Tenant final:', subdomain);

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
    // Normalizar para comparación insensible a mayúsculas y guiones/espacios
    const normalizedSearch = moduleName.toLowerCase().replace(/-/g, ' ');

    const isActive = modules.some(m => {
      const normalizedModule = m.moduleName.toLowerCase();
      return normalizedModule === normalizedSearch && m.status === 'ACTIVE';
    });

    console.log(
      `🔍 Checking module: ${moduleName}, normalized: ${normalizedSearch}, found: ${isActive}`
    );
    console.log(
      `📦 Available modules:`,
      modules.map(m => ({ name: m.moduleName, status: m.status }))
    );

    return isActive;
  };

  // ✅ SSO: Verificar permisos del usuario para módulos específicos
  const getUserModuleAccess = async userId => {
    try {
      const apiUrl =
        import.meta.env.VITE_API_URL || 'http://localhost:5014/api';
      const response = await fetch(`${apiUrl}/auth/user-modules/${userId}`, {
        headers: {
          Authorization: `Bearer ${localStorage.getItem('authToken')}`,
        },
      });

      if (response.ok) {
        const userModules = await response.json();
        console.log('🔐 User module access:', userModules);
        return userModules;
      }
    } catch (error) {
      console.error('❌ Error getting user modules:', error);
    }
    return [];
  };

  // ✅ SSO: Verificar si usuario tiene acceso a un módulo específico
  const hasModuleAccess = (moduleName, userModules) => {
    if (!userModules || userModules.length === 0) return false;

    const normalizedModuleName = moduleName.toLowerCase().replace(/\s+/g, '-');
    return userModules.some(
      module =>
        module.moduleName.toLowerCase() === normalizedModuleName &&
        module.isActive
    );
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
    getUserModuleAccess,
    hasModuleAccess,
    tenantName: tenant?.companyName || 'Tenant',
    subdomain: tenant?.subdomain,
  };

  return (
    <TenantContext.Provider value={value}>{children}</TenantContext.Provider>
  );
};
