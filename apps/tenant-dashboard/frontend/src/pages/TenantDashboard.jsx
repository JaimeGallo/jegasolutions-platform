import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { useTenant } from '../contexts/TenantContext';
import { motion } from 'framer-motion';
import {
  Building2,
  Clock,
  FileText,
  BarChart3,
  Settings,
  Users,
  LogOut,
  ExternalLink,
  CheckCircle,
  XCircle,
} from 'lucide-react';
import UserMenu from '../components/layout/UserMenu';

const TenantDashboard = () => {
  const navigate = useNavigate();
  const { user, logout, isAuthenticated, isLoading: authLoading } = useAuth();
  const {
    tenant,
    modules,
    isLoading: tenantLoading,
    tenantName,
    getUserModuleAccess,
    hasModuleAccess,
  } = useTenant();
  const [stats, setStats] = useState({
    totalModules: 0,
    activeModules: 0,
    totalUsers: 0,
    lastActivity: null,
  });
  const [userModules, setUserModules] = useState([]);

  // Redirigir a login si no está autenticado
  useEffect(() => {
    if (!authLoading && !isAuthenticated) {
      navigate('/login');
    }
  }, [authLoading, isAuthenticated, navigate]);

  useEffect(() => {
    if (modules) {
      setStats({
        totalModules: modules.length,
        activeModules: modules.filter(m => m.status === 'ACTIVE').length,
        totalUsers: 0,
        lastActivity: new Date().toLocaleDateString(),
      });
    }
  }, [modules]);

  // ✅ SSO: Cargar permisos del usuario al autenticarse
  useEffect(() => {
    if (user && user.id) {
      getUserModuleAccess(user.id).then(modules => {
        setUserModules(modules);
        console.log('🔐 User modules loaded:', modules);
        console.log(
          '🔐 User modules loaded detailed:',
          JSON.stringify(modules, null, 2)
        );
      });
    }
  }, [user, getUserModuleAccess]);

  // Helper function normalizada
  const getModuleConfig = moduleName => {
    const normalized = moduleName.toLowerCase().replace(/[-\s]/g, '');

    if (normalized === 'extrahours') {
      return {
        displayName: 'GestorHorasExtra',
        icon: Clock,
        color: 'bg-blue-500',
        description: 'Gestión completa de horas extra y compensaciones',
        features: [
          'Control de horas extra',
          'Gestión de colaboradores',
          'Reportes automáticos',
          'Cumplimiento normativo',
        ],
        // URL configurable vía variable de entorno
        url:
          import.meta.env.VITE_EXTRA_HOURS_URL ||
          'https://extrahours.jegasolutions.co',
      };
    }

    if (normalized === 'reportbuilder') {
      return {
        displayName: 'ReportBuilder con IA',
        icon: FileText,
        color: 'bg-purple-500',
        description:
          'Generación inteligente de reportes con análisis automático',
        features: [
          'Análisis con IA',
          'Narrativas ejecutivas',
          'Exportación múltiples formatos',
          'Dashboards interactivos',
        ],
        // URL configurable vía variable de entorno
        url:
          import.meta.env.VITE_REPORT_BUILDER_URL ||
          'https://reportbuilder.jegasolutions.co',
      };
    }

    // Default
    return {
      displayName: moduleName,
      icon: FileText,
      color: 'bg-gray-500',
      description: `Módulo ${moduleName}`,
      features: [],
      url: '#',
    };
  };

  // ✅ SSO: Mapear módulos del backend con permisos del usuario
  const availableModules = modules.map(module => {
    const config = getModuleConfig(module.moduleName);
    const hasAccess = hasModuleAccess(module.moduleName, userModules);
    const isModuleActive = module.status.toUpperCase() === 'ACTIVE';

    console.log(`🔍 Processing module: ${module.moduleName}`);
    console.log(`  - module.status: ${module.status}`);
    console.log(`  - isModuleActive: ${isModuleActive}`);
    console.log(`  - hasAccess: ${hasAccess}`);
    console.log(`  - userModules:`, userModules);
    console.log(`  - Final isActive: ${isModuleActive && hasAccess}`);

    return {
      id: module.id,
      name: config.displayName,
      moduleName: module.moduleName,
      description: config.description,
      icon: config.icon,
      color: config.color,
      features: config.features,
      isActive: isModuleActive && hasAccess, // ✅ Solo activo si el módulo está activo Y el usuario tiene acceso
      hasAccess, // ✅ Nuevo: si el usuario tiene permisos
      url: config.url,
    };
  });

  const handleLogout = () => {
    logout();
    window.location.href = '/login';
  };

  const handleModuleClick = module => {
    if (module.isActive) {
      // ✅ SSO: Pasar token JWT al módulo
      const token = localStorage.getItem('token');
      const urlWithToken = token
        ? `${module.url}?token=${encodeURIComponent(token)}`
        : module.url;

      console.log('🚀 Opening module:', module.name, 'with SSO token');
      window.open(urlWithToken, '_blank');
    } else if (!module.hasAccess) {
      // ✅ SSO: Mostrar mensaje si no tiene acceso
      alert('No tienes acceso a este módulo. Contacta al administrador.');
    } else {
      // Módulo no activo
      alert('Este módulo no está disponible en este momento.');
    }
  };

  // Mostrar loading mientras verifica autenticación
  if (authLoading || tenantLoading) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-jega-blue-600 mx-auto mb-4"></div>
          <p className="text-gray-600">Cargando dashboard...</p>
        </div>
      </div>
    );
  }

  // No renderizar nada si no está autenticado (se redirigirá a login)
  if (!isAuthenticated) {
    return null;
  }

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Header */}
      <header className="bg-white shadow-sm border-b border-gray-200">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between items-center h-16">
            <div className="flex items-center">
              <div className="flex-shrink-0">
                <div className="flex items-center">
                  <img
                    src="/LogoV2.png"
                    alt="JEGASolutions Logo"
                    className="h-8 w-auto"
                  />
                </div>
              </div>
            </div>

            <div className="flex items-center space-x-4">
              <div className="text-right">
                <p className="text-sm font-medium text-gray-900">
                  {tenantName}
                </p>
                <p className="text-xs text-gray-500">{user?.email}</p>
              </div>
              <UserMenu />
            </div>
          </div>
        </div>
      </header>

      {/* Main Content */}
      <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {/* Welcome Section */}
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.5 }}
          className="mb-8"
        >
          <h1 className="text-3xl font-bold text-gray-900 mb-2">
            ¡Bienvenido a tu Dashboard!
          </h1>
          <p className="text-gray-600">
            Gestiona tus módulos y accede a todas las funcionalidades de
            JEGASolutions
          </p>
        </motion.div>

        {/* Stats Grid */}
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.5, delay: 0.1 }}
          className="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8"
        >
          <div className="card">
            <div className="flex items-center">
              <div className="p-3 bg-blue-100 rounded-lg">
                <BarChart3 className="h-6 w-6 text-blue-600" />
              </div>
              <div className="ml-4">
                <p className="text-sm font-medium text-gray-600">
                  Módulos Totales
                </p>
                <p className="text-2xl font-bold text-gray-900">
                  {stats.totalModules}
                </p>
              </div>
            </div>
          </div>

          <div className="card">
            <div className="flex items-center">
              <div className="p-3 bg-green-100 rounded-lg">
                <CheckCircle className="h-6 w-6 text-green-600" />
              </div>
              <div className="ml-4">
                <p className="text-sm font-medium text-gray-600">
                  Módulos Activos
                </p>
                <p className="text-2xl font-bold text-gray-900">
                  {stats.activeModules}
                </p>
              </div>
            </div>
          </div>

          <div className="card">
            <div className="flex items-center">
              <div className="p-3 bg-purple-100 rounded-lg">
                <Users className="h-6 w-6 text-purple-600" />
              </div>
              <div className="ml-4">
                <p className="text-sm font-medium text-gray-600">Usuarios</p>
                <p className="text-2xl font-bold text-gray-900">
                  {stats.totalUsers}
                </p>
              </div>
            </div>
          </div>

          <div className="card">
            <div className="flex items-center">
              <div className="p-3 bg-orange-100 rounded-lg">
                <Clock className="h-6 w-6 text-orange-600" />
              </div>
              <div className="ml-4">
                <p className="text-sm font-medium text-gray-600">
                  Última Actividad
                </p>
                <p className="text-2xl font-bold text-gray-900">
                  {stats.lastActivity}
                </p>
              </div>
            </div>
          </div>
        </motion.div>

        {/* Modules Section */}
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.5, delay: 0.2 }}
        >
          <h2 className="text-2xl font-bold text-gray-900 mb-6">
            Módulos Disponibles
          </h2>

          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            {availableModules.map((module, index) => (
              <motion.div
                key={module.id}
                initial={{ opacity: 0, y: 20 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ duration: 0.5, delay: 0.3 + index * 0.1 }}
                className={`card cursor-pointer transition-all duration-200 ${
                  module.isActive
                    ? 'hover:shadow-lg hover:scale-105 border-jega-blue-200'
                    : module.hasAccess
                    ? 'opacity-60 cursor-not-allowed border-gray-200'
                    : 'opacity-40 cursor-not-allowed border-red-200 bg-red-50'
                }`}
                onClick={() => handleModuleClick(module)}
              >
                <div className="flex items-start justify-between mb-4">
                  <div className="flex items-center">
                    <div className={`p-3 rounded-lg ${module.color}`}>
                      <module.icon className="h-6 w-6 text-white" />
                    </div>
                    <div className="ml-4">
                      <h3 className="text-lg font-semibold text-gray-900">
                        {module.name}
                      </h3>
                      <p className="text-sm text-gray-600">
                        {module.description}
                      </p>
                    </div>
                  </div>

                  <div className="flex items-center space-x-2">
                    {module.isActive ? (
                      <CheckCircle className="h-5 w-5 text-green-500" />
                    ) : !module.hasAccess ? (
                      <XCircle className="h-5 w-5 text-red-500" />
                    ) : (
                      <XCircle className="h-5 w-5 text-orange-500" />
                    )}
                    {module.isActive && (
                      <ExternalLink className="h-4 w-4 text-gray-400" />
                    )}
                    {!module.hasAccess && (
                      <span className="text-xs text-red-600 font-medium">
                        Sin Acceso
                      </span>
                    )}
                  </div>
                </div>

                <div className="mb-4">
                  <h4 className="text-sm font-medium text-gray-700 mb-2">
                    Características:
                  </h4>
                  <ul className="space-y-1">
                    {module.features.map((feature, idx) => (
                      <li
                        key={idx}
                        className="text-sm text-gray-600 flex items-center"
                      >
                        <CheckCircle className="h-3 w-3 text-green-500 mr-2 flex-shrink-0" />
                        {feature}
                      </li>
                    ))}
                  </ul>
                </div>

                <div className="flex items-center justify-between">
                  <span
                    className={`px-3 py-1 rounded-full text-xs font-medium ${
                      module.isActive
                        ? 'bg-green-100 text-green-800'
                        : 'bg-red-100 text-red-800'
                    }`}
                  >
                    {module.isActive ? 'Activo' : 'No disponible'}
                  </span>

                  {module.isActive && (
                    <button className="btn-primary text-sm">Acceder</button>
                  )}
                </div>
              </motion.div>
            ))}
          </div>
        </motion.div>
      </main>
    </div>
  );
};

export default TenantDashboard;
