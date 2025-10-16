import { createContext, useContext, useState, useEffect } from "react";

const TenantContext = createContext();

export const useTenant = () => {
  const context = useContext(TenantContext);
  if (!context) {
    throw new Error("useTenant must be used within a TenantProvider");
  }
  return context;
};

export const TenantProvider = ({ children }) => {
  const [tenant, setTenant] = useState(null);
  const [modules, setModules] = useState([]);
  const [stats, setStats] = useState(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    // Extract tenant info from subdomain
    const hostname = window.location.hostname;
    const subdomain = hostname.split(".")[0];

    if (subdomain && subdomain !== "www" && subdomain !== "jegasolutions") {
      loadTenantData(subdomain);
    } else {
      setIsLoading(false);
    }
  }, []);

  const loadTenantData = async (subdomain) => {
    try {
      // Primero obtener el tenant por subdomain
      const tenantResponse = await fetch(`/api/tenants/by-subdomain/${subdomain}`);
      if (tenantResponse.ok) {
        const tenantData = await tenantResponse.json();
        setTenant(tenantData);
        
        // Luego obtener los módulos del tenant
        const modulesResponse = await fetch(`/api/tenants/${tenantData.id}/modules`);
        if (modulesResponse.ok) {
          const modulesData = await modulesResponse.json();
          setModules(modulesData || []);
        }
        
        // También obtener las estadísticas
        const statsResponse = await fetch(`/api/tenants/${tenantData.id}/stats`);
        if (statsResponse.ok) {
          const statsData = await statsResponse.json();
          setStats(statsData);
        }
      } else {
        console.error("Error loading tenant:", await tenantResponse.text());
      }
    } catch (error) {
      console.error("Error loading tenant data:", error);
    } finally {
      setIsLoading(false);
    }
  };

  const getModuleStatus = (moduleName) => {
    const module = modules.find((m) => m.moduleName === moduleName);
    return module?.status?.toUpperCase() === "ACTIVE";
  };

  /**
   * 🔧 FUNCIÓN CORREGIDA: Retorna URLs correctas de los módulos
   * Usa subdominios separados en producción o rutas en desarrollo
   */
  const getModuleUrl = (moduleName) => {
    const isProduction = window.location.hostname.includes("jegasolutions.co");

    if (isProduction) {
      // En producción, cada módulo tiene su propio subdominio
      switch (moduleName) {
        case "extra-hours":
          return "https://extrahours.jegasolutions.co";
        case "report-builder":
          return "https://reportbuilder.jegasolutions.co";
        default:
          return window.location.origin;
      }
    } else {
      // En desarrollo local, usar puertos diferentes o rutas
      switch (moduleName) {
        case "extra-hours":
          return "http://localhost:5174"; // Puerto de extra-hours frontend
        case "report-builder":
          return "http://localhost:5173"; // Puerto de report-builder frontend
        default:
          return window.location.origin;
      }
    }
  };

  const value = {
    tenant,
    modules,
    stats,
    isLoading,
    getModuleStatus,
    getModuleUrl,
    tenantName: tenant?.companyName || "Tenant",
    subdomain: tenant?.subdomain,
  };

  return (
    <TenantContext.Provider value={value}>{children}</TenantContext.Provider>
  );
};




