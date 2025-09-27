import React, { createContext, useContext, useState, useEffect } from "react";
import { useAuth } from "./AuthContext";

const TenantContext = createContext();

export const useTenant = () => {
  const context = useContext(TenantContext);
  if (!context) {
    throw new Error("useTenant must be used within a TenantProvider");
  }
  return context;
};

export const TenantProvider = ({ children }) => {
  const { user } = useAuth();
  const [tenant, setTenant] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (user) {
      // Extract tenant information from user claims or fetch from API
      const tenantId = user.tenantId || 1; // Default tenant for development
      const tenantInfo = {
        id: tenantId,
        name: user.tenantName || "Default Tenant",
        domain: user.tenantDomain || "localhost",
        settings: {
          theme: "light",
          timezone: "America/Bogota",
          currency: "COP",
          language: "es",
        },
      };

      setTenant(tenantInfo);
      setLoading(false);
    } else {
      setTenant(null);
      setLoading(false);
    }
  }, [user]);

  const updateTenantSettings = (newSettings) => {
    setTenant((prev) => ({
      ...prev,
      settings: {
        ...prev.settings,
        ...newSettings,
      },
    }));
  };

  const value = {
    tenant,
    loading,
    updateTenantSettings,
    tenantId: tenant?.id,
    tenantName: tenant?.name,
    tenantSettings: tenant?.settings,
  };

  return (
    <TenantContext.Provider value={value}>{children}</TenantContext.Provider>
  );
};
