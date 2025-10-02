import React, { useState } from "react";
import { Clock, CheckCircle, TrendingUp, Shield, FileText, Play } from "lucide-react";
import FlippableModuleCard from "../modules/FlippableModuleCard";
import VideoModal from "../VideoModal";

const ExtraHoursModule = () => {
  const [isVideoOpen, setIsVideoOpen] = useState(false);
  const features = [
    "Registro automático por empleados",
    "Cálculo categorizado (diurna/nocturna/festiva)",
    "Sistema de aprobación por managers",
    "Reportes y exportación a Excel",
    "Cumplimiento normativo 100%",
    "Integración con sistemas de nómina",
    "Notificaciones automáticas",
    "Auditoría completa de cambios",
  ];

  const benefits = [
    {
      icon: TrendingUp,
      title: "Eficiencia",
      description: "Reduce tiempo de procesamiento en un 80%",
    },
    {
      icon: Shield,
      title: "Cumplimiento",
      description: "Garantiza conformidad con normativas laborales",
    },
    {
      icon: FileText,
      title: "Reportes",
      description: "Genera informes detallados automáticamente",
    },
  ];

  const frontContent = (
    <>
      <div className="w-24 h-24 bg-gradient-to-br from-jega-blue-500 to-jega-blue-600 rounded-3xl flex items-center justify-center mb-6 transition-transform duration-300 group-hover:scale-110">
        <Clock className="w-12 h-12 text-white" />
      </div>

      <h3 className="text-3xl font-bold text-gray-900 mb-4">
        Gestión de Horas Extra
      </h3>
      <p className="text-lg text-gray-600 max-w-xs mb-6">
        Automatiza el registro, cálculo y aprobación de horas extra con
        cumplimiento normativo garantizado. Sistema inteligente que se adapta a
        las políticas de tu empresa.
      </p>
      
      <button
        onClick={() => setIsVideoOpen(true)}
        className="inline-flex items-center space-x-2 px-6 py-3 bg-jega-blue-600 hover:bg-jega-blue-700 text-white rounded-lg transition-all duration-300 hover:scale-105 shadow-md hover:shadow-lg"
      >
        <Play className="w-5 h-5" />
        <span className="font-semibold">Ver Demo</span>
      </button>
    </>
  );
  const backContent = (
    <div className="text-left flex flex-col h-full">
      <h3 className="text-xl font-bold text-gray-900 mb-4 text-center">
        Gestión de Horas Extra
      </h3>
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mb-4 flex-grow">
        <div>
          <h4 className="font-semibold text-gray-800 mb-2">
            Características Principales
          </h4>
          <ul className="space-y-1.5">
            {features.slice(0, 4).map((feature, idx) => (
              <li key={idx} className="flex items-start text-sm text-gray-600">
                <CheckCircle className="w-4 h-4 text-green-500 mr-2 mt-0.5 flex-shrink-0" />
                <span>{feature}</span>
              </li>
            ))}
          </ul>
        </div>
        <div>
          <h4 className="font-semibold text-gray-800 mb-2">Beneficios Clave</h4>
          <div className="space-y-2">
            {benefits.map((benefit, idx) => (
              <div key={idx} className="flex items-start space-x-3">
                <div className="w-8 h-8 bg-jega-blue-100 rounded-lg flex items-center justify-center flex-shrink-0">
                  <benefit.icon className="w-4 h-4 text-jega-blue-600" />
                </div>
                <div>
                  <p className="font-medium text-sm text-gray-900">
                    {benefit.title}
                  </p>
                  <p className="text-xs text-gray-600">{benefit.description}</p>
                </div>
              </div>
            ))}
          </div>
        </div>
      </div>
    </div>
  );

  return (
    <>
      <FlippableModuleCard
        frontContent={frontContent}
        backContent={backContent}
      />
      <VideoModal
        isOpen={isVideoOpen}
        onClose={() => setIsVideoOpen(false)}
        videoUrl="https://www.youtube.com/watch?v=dQw4w9WgXcQ"
        title="Demo - Gestión de Horas Extra"
      />
    </>
  );
};

export default ExtraHoursModule;
