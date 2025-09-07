import React from "react";
import { motion } from "framer-motion";
import { Zap, Shield, TrendingUp, Star, Calendar } from "lucide-react";

const UpcomingModules = () => {
  const upcomingModules = [
    {
      icon: Zap,
      title: "Gestión de Nómina",
      description:
        "Sistema completo de nómina con cálculo automático de prestaciones y beneficios.",
      status: "Q1 2025",
      priority: "high",
    },
    {
      icon: Shield,
      title: "Control de Asistencia",
      description:
        "Control biométrico y digital de asistencia con reportes de productividad.",
      status: "Q2 2025",
      priority: "medium",
    },
    {
      icon: TrendingUp,
      title: "Business Intelligence",
      description:
        "Dashboard ejecutivo con métricas clave y análisis predictivo.",
      status: "Q3 2025",
      priority: "medium",
    },
  ];

  const getPriorityColor = (priority) => {
    const colors = {
      high: "bg-red-100 text-red-800 border-red-200",
      medium: "bg-yellow-100 text-yellow-800 border-yellow-200",
      low: "bg-green-100 text-green-800 border-green-200",
    };
    return colors[priority] || colors.medium;
  };

  return (
    <div className="space-y-6">
      <motion.div
        initial={{ opacity: 0, y: 30 }}
        whileInView={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.8 }}
        viewport={{ once: true }}
        className="text-center"
      >
        <div className="flex items-center justify-center space-x-2 mb-4">
          <Star className="w-6 h-6 text-jega-gold-500" />
          <h3 className="text-3xl font-bold text-gray-900">Próximos Módulos</h3>
          <Star className="w-6 h-6 text-jega-gold-500" />
        </div>
        <p className="text-lg text-gray-600 max-w-2xl mx-auto">
          Continuamos expandiendo la suite con nuevas funcionalidades basadas en
          las necesidades reales de nuestros clientes. ¡Mantente atento a las
          novedades!
        </p>
      </motion.div>

      <div className="grid md:grid-cols-3 gap-6">
        {upcomingModules.map((module, index) => (
          <motion.div
            key={module.title}
            initial={{ opacity: 0, y: 30 }}
            whileInView={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.8, delay: index * 0.1 }}
            viewport={{ once: true }}
            className="bg-gradient-to-br from-jega-gold-50 to-yellow-100 rounded-2xl p-6 border border-jega-gold-200 hover:shadow-lg transition-all duration-300 group"
          >
            <div className="w-12 h-12 bg-jega-gold-200 rounded-xl flex items-center justify-center mb-4 group-hover:scale-110 transition-transform">
              <module.icon className="w-6 h-6 text-jega-gold-600" />
            </div>

            <h4 className="text-xl font-bold text-gray-900 mb-3">
              {module.title}
            </h4>
            <p className="text-gray-600 mb-4 text-sm leading-relaxed">
              {module.description}
            </p>

            <div className="flex items-center justify-between mb-4">
              <div className="flex items-center space-x-2 text-sm text-gray-600">
                <Calendar className="w-4 h-4" />
                <span>{module.status}</span>
              </div>
              <div
                className={`px-2 py-1 rounded-full text-xs font-medium border ${getPriorityColor(
                  module.priority
                )}`}
              >
                {module.priority === "high"
                  ? "Alta"
                  : module.priority === "medium"
                  ? "Media"
                  : "Baja"}
              </div>
            </div>

            <div className="bg-jega-gold-200 rounded-lg p-3 text-center">
              <span className="text-sm font-medium text-jega-gold-800">
                Muy Pronto
              </span>
            </div>
          </motion.div>
        ))}
      </div>

      <motion.div
        initial={{ opacity: 0, y: 30 }}
        whileInView={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.8, delay: 0.4 }}
        viewport={{ once: true }}
        className="text-center"
      >
        <div className="bg-white rounded-2xl shadow-xl p-8 max-w-2xl mx-auto border border-gray-100">
          <h4 className="text-2xl font-bold text-gray-900 mb-4">
            ¿Necesitas un módulo específico?
          </h4>
          <p className="text-gray-600 mb-6">
            Desarrollamos soluciones personalizadas para necesidades
            empresariales únicas. ¡Cuéntanos qué necesitas y lo creamos para ti!
          </p>
          <button className="bg-jega-gold-400 hover:bg-jega-gold-500 text-gray-900 px-6 py-3 rounded-xl font-semibold transition-all duration-300 transform hover:scale-105 hover:shadow-lg">
            Solicitar Desarrollo Personalizado
          </button>
        </div>
      </motion.div>
    </div>
  );
};

export default UpcomingModules;


