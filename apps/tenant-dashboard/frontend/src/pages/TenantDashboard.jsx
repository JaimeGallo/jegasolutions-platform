import { useState, useEffect } from 'react';
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

const TenantDashboard = () => {
  const { user, logout } = useAuth();
  const {
    tenant,
    modules,
    isLoading,
    getModuleStatus,
    getModuleUrl,
    tenantName,
  } = useTenant();
  const [stats, setStats] = useState({
    totalModules: 0,
    activeModules: 0,
    totalUsers: 0,
    lastActivity: null,
  });

  useEffect(() => {
    if (modules) {
      setStats({
        totalModules: modules.length,
        activeModules: modules.filter(m => m.status === 'ACTIVE').length,
        totalUsers: 0, // This would come from API
        lastActivity: new Date().toLocaleDateString(),
      });
    }
  }, [modules]);

  const getModuleFeatures = moduleName => {
    switch (moduleName) {
      case 'Extra Hours':
        return [
          'Control de horas extra',
          'Gestión de colaboradores',
          'Reportes automáticos',
          'Cumplimiento normativo',
        ];
      case 'Report Builder':
        return [
          'Análisis con IA',
          'Narrativas ejecutivas',
          'Exportación múltiples formatos',
          'Dashboards interactivos',
        ];
      default:
        return [];
    }
  };

  // DESPUÉS: Usar la función (línea ~47-56)
  const availableModules = modules.map(module => ({
    id: module.moduleName.toLowerCase().replace(/ /g, '-'),
    name: module.moduleName,
    description: module.description,
    icon: module.icon === 'clock' ? Clock : FileText,
    color: module.icon === 'clock' ? 'bg-blue-500' : 'bg-purple-500',
    features: getModuleFeatures(module.moduleName),
    isActive: module.status === 'active',
    url: module.url,
  }));

  const handleLogout = () => {
    logout();
    window.location.href = '/login';
  };

  const handleModuleClick = module => {
    if (module.isActive) {
      window.open(module.url, '_blank');
    }
  };

  if (isLoading) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-jega-blue-600 mx-auto mb-4"></div>
          <p className="text-gray-600">Cargando dashboard...</p>
        </div>
      </div>
    );
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
                  <Building2 className="h-8 w-8 text-jega-blue-600" />
                  <span className="ml-2 text-xl font-bold text-gray-900">
                    JEGASolutions
                  </span>
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
              <button
                onClick={handleLogout}
                className="flex items-center space-x-2 text-gray-600 hover:text-gray-900 transition-colors"
              >
                <LogOut className="h-4 w-4" />
                <span className="text-sm">Salir</span>
              </button>
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
                    : 'opacity-60 cursor-not-allowed'
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
                    ) : (
                      <XCircle className="h-5 w-5 text-red-500" />
                    )}
                    {module.isActive && (
                      <ExternalLink className="h-4 w-4 text-gray-400" />
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

        {/* Quick Actions */}
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.5, delay: 0.4 }}
          className="mt-8"
        >
          <h2 className="text-2xl font-bold text-gray-900 mb-6">
            Acciones Rápidas
          </h2>

          <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
            <div className="card text-center">
              <Settings className="h-8 w-8 text-jega-blue-600 mx-auto mb-4" />
              <h3 className="text-lg font-semibold text-gray-900 mb-2">
                Configuración
              </h3>
              <p className="text-gray-600 mb-4">
                Gestiona la configuración de tu cuenta
              </p>
              <button className="btn-secondary w-full">Configurar</button>
            </div>

            <div className="card text-center">
              <Users className="h-8 w-8 text-jega-blue-600 mx-auto mb-4" />
              <h3 className="text-lg font-semibold text-gray-900 mb-2">
                Usuarios
              </h3>
              <p className="text-gray-600 mb-4">
                Administra los usuarios de tu organización
              </p>
              <button className="btn-secondary w-full">Gestionar</button>
            </div>

            <div className="card text-center">
              <BarChart3 className="h-8 w-8 text-jega-blue-600 mx-auto mb-4" />
              <h3 className="text-lg font-semibold text-gray-900 mb-2">
                Reportes
              </h3>
              <p className="text-gray-600 mb-4">
                Visualiza el rendimiento de tus módulos
              </p>
              <button className="btn-secondary w-full">Ver Reportes</button>
            </div>
          </div>
        </motion.div>
      </main>
    </div>
  );
};

export default TenantDashboard;
