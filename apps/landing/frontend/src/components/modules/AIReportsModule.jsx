import { useState } from "react";
import { Brain, CheckCircle, Sparkles, BarChart3, Zap, Play } from "lucide-react";
import FlippableModuleCard from "./FlippableModuleCard";
import VideoModal from "../VideoModal";

const AIReportsModule = () => {
  const [isVideoOpen, setIsVideoOpen] = useState(false);
  const features = [
    "Narrativas automáticas con Claude AI",
    "Análisis semántico de datos",
    "Consolidación de informes",
    "Exportación PDF/Word",
    "Insights inteligentes",
    "Personalización de plantillas",
    "Sincronización en tiempo real",
    "Historial de versiones",
  ];

  const aiCapabilities = [
    {
      icon: Sparkles,
      title: "Generación Inteligente",
      description: "Crea narrativas profesionales automáticamente",
    },
    {
      icon: BarChart3,
      title: "Análisis Avanzado",
      description: "Identifica patrones y tendencias ocultas",
    },
    {
      icon: Zap,
      title: "Velocidad",
      description: "Reduce tiempo de creación de reportes en 90%",
    },
  ];

  const frontContent = (
    <>
      <div className="w-24 h-24 bg-gradient-to-br from-purple-500 to-purple-600 rounded-3xl flex items-center justify-center mb-6 transition-transform duration-300 group-hover:scale-110">
        <Brain className="w-12 h-12 text-white" />
      </div>

      <h3 className="text-3xl font-bold text-gray-900 mb-4">Reportes con IA</h3>
      <p className="text-lg text-gray-600 max-w-md mb-6">
        Generación automática de narrativas profesionales y análisis inteligente
        con Claude AI. Transforma datos complejos en insights accionables.
      </p>
      
      <button
        onClick={(e) => {
          e.stopPropagation();
          setIsVideoOpen(true);
        }}
        className="inline-flex items-center space-x-2 px-6 py-3 bg-purple-600 hover:bg-purple-700 text-white rounded-lg transition-all duration-300 hover:scale-105 shadow-md hover:shadow-lg"
      >
        <Play className="w-5 h-5" />
        <span className="font-semibold">Ver Demo</span>
      </button>
    </>
  );

  const backContent = (
    <div className="text-left flex flex-col h-full">
      <h3 className="text-xl font-bold text-gray-900 mb-4 text-center">
        Reportes con IA
      </h3>
      <div className="grid grid-cols-1 md:grid-cols-2 gap-6 mb-6 flex-grow">
        <div>
          <h4 className="font-semibold text-gray-800 mb-2">
            Características Principales
          </h4>
          <ul className="space-y-2">
            {features.slice(0, 5).map((feature, idx) => (
              <li key={idx} className="flex items-start text-sm text-gray-600">
                <CheckCircle className="w-4 h-4 text-green-500 mr-2 mt-0.5 flex-shrink-0" />
                <span>{feature}</span>
              </li>
            ))}
          </ul>
        </div>
        <div>
          <h4 className="font-semibold text-gray-800 mb-2">
            Capacidades de IA
          </h4>
          <div className="space-y-3">
            {aiCapabilities.map((capability, idx) => (
              <div key={idx} className="flex items-start space-x-3">
                <div className="w-8 h-8 bg-purple-100 rounded-lg flex items-center justify-center flex-shrink-0">
                  <capability.icon className="w-4 h-4 text-purple-600" />
                </div>
                <div>
                  <p className="font-medium text-sm text-gray-900">
                    {capability.title}
                  </p>
                  <p className="text-xs text-gray-600">
                    {capability.description}
                  </p>
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
        title="Demo - Reportes con IA"
      />
    </>
  );
};

export default AIReportsModule;
