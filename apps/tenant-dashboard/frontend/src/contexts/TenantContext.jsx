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
      const response = await fetch(`/api/tenants/${subdomain}`);
      if (response.ok) {
        const data = await response.json();
        setTenant(data.tenant);
        setModules(data.modules || []);
      }
    } catch (error) {
      console.error("Error loading tenant data:", error);
    } finally {
      setIsLoading(false);
    }
  };

  const getModuleStatus = (moduleName) => {
    return (
      modules.find((m) => m.moduleName === moduleName)?.status === "ACTIVE"
    );
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




