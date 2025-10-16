import { useState, useEffect } from "react";
import { useAuth } from "../contexts/AuthContext";
import { useTenant } from "../contexts/TenantContext";
import { motion } from "framer-motion";
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
} from "lucide-react";

const TenantDashboard = () => {
  const { user, logout } = useAuth();
  const {
    tenant,
    modules,
    stats,
    isLoading,
    getModuleStatus,
    getModuleUrl,
    tenantName,
  } = useTenant();

  const availableModules = [
    {
      id: "extra-hours",
      name: "GestorHorasExtra",
      description: "Gestión completa de horas extra y compensaciones",
      icon: Clock,
      color: "bg-blue-500",
      features: [
        "Control de horas extra",
        "Gestión de colaboradores",
        "Reportes automáticos",
        "Cumplimiento normativo",
      ],
      isActive: getModuleStatus("extra-hours"),
      url: getModuleUrl("extra-hours"),
    },
    {
      id: "report-builder",
      name: "ReportBuilder con IA",
      description: "Generación inteligente de reportes con análisis automático",
      icon: FileText,
      color: "bg-purple-500",
      features: [
        "Análisis con IA",
        "Narrativas ejecutivas",
        "Exportación múltiples formatos",
        "Dashboards interactivos",
      ],
      isActive: getModuleStatus("report-builder"),
      url: getModuleUrl("report-builder"),
    },
  ];

  const handleLogout = () => {
    logout();
    window.location.href = "/login";
  };

  /**
   * Esta función obtiene el token JWT actual del usuario y lo pasa al módulo
   */
  const handleModuleClick = (module) => {
    if (!module.isActive) {
      return;
    }

    // Obtener el token JWT actual del usuario
    const token = localStorage.getItem("authToken");

    if (!token) {
      console.error("No auth token found");
      window.location.href = "/login";
      return;
    }

    // Construir la URL con el token como query parameter
    const moduleUrlWithToken = `${module.url}?token=${token}`;

    // Abrir el módulo en nueva pestaña con el token
    window.open(moduleUrlWithToken, "_blank");
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
                <Building2 className="h-8 w-8 text-jega-blue-600" />
              </div>
              <div className="ml-4">
                <h1 className="text-xl font-bold text-gray-900">{tenantName}</h1>
                <p className="text-sm text-gray-500">Dashboard Central</p>
              </div>
            </div>

            <div className="flex items-center space-x-4">
              <div className="text-right">
                <p className="text-sm font-medium text-gray-900">
                  {user?.name || "Usuario"}
                </p>
                <p className="text-xs text-gray-500">{user?.email || ""}</p>
              </div>
              <button
                onClick={handleLogout}
                className="btn-secondary flex items-center space-x-2"
              >
                <LogOut className="h-4 w-4" />
                <span>Salir</span>
              </button>
            </div>
          </div>
        </div>
      </header>

      {/* Main Content */}
      <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {/* Stats Grid */}
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.5 }}
          className="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8"
        >
          <div className="card">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-gray-600">Total Módulos</p>
                <p className="text-2xl font-bold text-gray-900">
                  {stats?.totalModules || 0}
                </p>
              </div>
              <BarChart3 className="h-8 w-8 text-jega-blue-600" />
            </div>
          </div>

          <div className="card">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-gray-600">Módulos Activos</p>
                <p className="text-2xl font-bold text-green-600">
                  {stats?.activeModules || 0}
                </p>
              </div>
              <CheckCircle className="h-8 w-8 text-green-600" />
            </div>
          </div>

          <div className="card">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-gray-600">Usuarios</p>
                <p className="text-2xl font-bold text-gray-900">
                  {stats?.totalUsers || 0}
                </p>
              </div>
              <Users className="h-8 w-8 text-jega-blue-600" />
            </div>
          </div>

          <div className="card">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-gray-600">Última Actividad</p>
                <p className="text-sm font-medium text-gray-900">
                  {stats?.lastActivity ? new Date(stats.lastActivity).toLocaleDateString() : new Date().toLocaleDateString()}
                </p>
              </div>
              <Clock className="h-8 w-8 text-jega-blue-600" />
            </div>
          </div>
        </motion.div>

        {/* Modules Grid */}
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.5, delay: 0.2 }}
        >
          <h2 className="text-2xl font-bold text-gray-900 mb-6">
            Mis Módulos
          </h2>

          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            {availableModules.map((module) => (
              <motion.div
                key={module.id}
                initial={{ opacity: 0, scale: 0.95 }}
                animate={{ opacity: 1, scale: 1 }}
                transition={{ duration: 0.3 }}
                className={`card hover:shadow-xl transition-all duration-300 ${
                  module.isActive ? "cursor-pointer" : "opacity-60"
                }`}
                onClick={() => module.isActive && handleModuleClick(module)}
              >
                <div className="flex items-start justify-between mb-4">
                  <div className="flex items-center space-x-4">
                    <div
                      className={`${module.color} p-3 rounded-lg text-white`}
                    >
                      <module.icon className="h-6 w-6" />
                    </div>
                    <div>
                      <h3 className="text-xl font-bold text-gray-900">
                        {module.name}
                      </h3>
                      <p className="text-sm text-gray-600">
                        {module.description}
                      </p>
                    </div>
                  </div>
                  {module.isActive ? (
                    <CheckCircle className="h-6 w-6 text-green-600" />
                  ) : (
                    <XCircle className="h-6 w-6 text-red-600" />
                  )}
                </div>

                <div className="mb-4">
                  <h4 className="text-sm font-semibold text-gray-700 mb-2">
                    Características:
                  </h4>
                  <ul className="space-y-1">
                    {module.features.map((feature, index) => (
                      <li
                        key={index}
                        className="text-sm text-gray-600 flex items-center"
                      >
                        <span className="mr-2">•</span>
                        {feature}
                      </li>
                    ))}
                  </ul>
                </div>

                <div className="flex items-center justify-between pt-4 border-t border-gray-200">
                  <span
                    className={`text-xs font-medium px-3 py-1 rounded-full ${
                      module.isActive
                        ? "bg-green-100 text-green-800"
                        : "bg-red-100 text-red-800"
                    }`}
                  >
                    {module.isActive ? "Activo" : "No disponible"}
                  </span>

                  {module.isActive && (
                    <button
                      className="btn-primary text-sm flex items-center space-x-2"
                      onClick={(e) => {
                        e.stopPropagation();
                        handleModuleClick(module);
                      }}
                    >
                      <span>Acceder</span>
                      <ExternalLink className="h-4 w-4" />
                    </button>
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
