import React, { createContext, useContext, useState, useEffect } from 'react';
import { useAuth } from './AuthContext';

const TenantContext = createContext();

export const useTenant = () => {
  const context = useContext(TenantContext);
  if (!context) {
    throw new Error('useTenant must be used within a TenantProvider');
  }
  return context;
};

export const TenantProvider = ({ children }) => {
  const { user, loading: authLoading } = useAuth(); // ← 🔥 Obtener authLoading
  const [tenant, setTenant] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    // 🔥 CRÍTICO: Si AuthContext está cargando, esperar
    if (authLoading) {
      return;
    }

    // Si hay user, crear tenant inmediatamente
    if (user) {
      console.log('🏢 Creating tenant from user:', user);

      const tenantId = user.tenantId || user.tenant_id || 1;
      const tenantInfo = {
        id: tenantId,
        name: user.tenantName || user.firstName || 'Default Tenant',
        domain: user.tenantDomain || 'jegasolutions.co',
        settings: {
          theme: 'light',
          timezone: 'America/Bogota',
          currency: 'COP',
          language: 'es',
        },
      };

      // Solo actualizar si el tenant es diferente
      setTenant(prevTenant => {
        if (prevTenant && prevTenant.id === tenantInfo.id) {
          return prevTenant; // No cambiar si es el mismo
        }
        console.log('✅ Tenant created:', tenantInfo);
        return tenantInfo;
      });
      setLoading(false);
    } else {
      // No hay user, limpiar tenant
      setTenant(null);
      setLoading(false);
      console.log('❌ No user, tenant cleared');
    }
  }, [user, authLoading]); // ← Depende de AMBOS

  const updateTenantSettings = newSettings => {
    setTenant(prev => ({
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
