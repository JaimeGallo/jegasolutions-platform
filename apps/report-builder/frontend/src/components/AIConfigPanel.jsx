import { useState } from "react";

const AIConfigPanel = ({ onAnalyze, isLoading }) => {
  const [config, setConfig] = useState({
    aiProvider: "openai",
    analysisType: "summary",
    customPrompt: "",
  });

  const aiProviders = [
    {
      id: "openai",
      name: "OpenAI GPT-4o",
      description: "Alta calidad, versátil",
      cost: "$30/1M tokens",
      speed: "Rápido",
      icon: "🤖",
    },
    {
      id: "anthropic",
      name: "Anthropic Claude 3.5",
      description: "Excelente razonamiento, contexto largo",
      cost: "$15/1M tokens",
      speed: "Rápido",
      icon: "🧠",
    },
    {
      id: "groq",
      name: "Groq Llama 3.3 70B",
      description: "Ultra-rápido, buena calidad",
      cost: "$0.59/1M tokens",
      speed: "Ultra-rápido",
      icon: "⚡",
    },
    {
      id: "deepseek",
      name: "DeepSeek Chat",
      description: "Muy económico, buena calidad",
      cost: "$0.14/1M tokens",
      speed: "Rápido",
      icon: "💎",
    },
  ];

  const analysisTypes = [
    {
      id: "summary",
      name: "Resumen Ejecutivo",
      description: "Resumen conciso de los datos principales",
      icon: "📋",
    },
    {
      id: "trends",
      name: "Análisis de Tendencias",
      description: "Identificar patrones y tendencias en los datos",
      icon: "📈",
    },
    {
      id: "insights",
      name: "Insights Profundos",
      description: "Análisis detallado con hallazgos clave",
      icon: "💡",
    },
    {
      id: "recommendations",
      name: "Recomendaciones",
      description: "Sugerencias accionables basadas en los datos",
      icon: "🎯",
    },
  ];

  const handleChange = (field, value) => {
    setConfig((prev) => ({
      ...prev,
      [field]: value,
    }));
  };

  const handleAnalyze = () => {
    if (onAnalyze) {
      onAnalyze(config);
    }
  };

  return (
    <div className="ai-config-panel bg-white rounded-lg shadow-lg p-6">
      <div className="flex items-center gap-3 mb-6">
        <div className="text-3xl">🤖</div>
        <div>
          <h2 className="text-2xl font-bold text-gray-900">
            Configuración de Análisis AI
          </h2>
          <p className="text-sm text-gray-600">
            Selecciona el proveedor de IA y el tipo de análisis
          </p>
        </div>
      </div>

      {/* Selección de Proveedor de IA */}
      <div className="mb-6">
        <h3 className="text-lg font-semibold text-gray-800 mb-3">
          Proveedor de IA
        </h3>
        <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
          {aiProviders.map((provider) => (
            <div
              key={provider.id}
              className={`
                border-2 rounded-lg p-4 cursor-pointer transition-all
                ${
                  config.aiProvider === provider.id
                    ? "border-blue-500 bg-blue-50"
                    : "border-gray-200 hover:border-gray-300"
                }
              `}
              onClick={() => handleChange("aiProvider", provider.id)}
            >
              <div className="flex items-start gap-3">
                <div className="text-3xl">{provider.icon}</div>
                <div className="flex-1">
                  <h4 className="font-semibold text-gray-900">
                    {provider.name}
                  </h4>
                  <p className="text-xs text-gray-600 mb-2">
                    {provider.description}
                  </p>
                  <div className="flex items-center gap-2 text-xs">
                    <span className="text-green-600 font-medium">
                      {provider.cost}
                    </span>
                    <span className="text-gray-400">•</span>
                    <span className="text-purple-600 font-medium">
                      {provider.speed}
                    </span>
                  </div>
                </div>
                {config.aiProvider === provider.id && (
                  <div className="text-blue-500 text-xl">✓</div>
                )}
              </div>
            </div>
          ))}
        </div>
      </div>

      {/* Tipo de Análisis */}
      <div className="mb-6">
        <h3 className="text-lg font-semibold text-gray-800 mb-3">
          Tipo de Análisis
        </h3>
        <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
          {analysisTypes.map((type) => (
            <div
              key={type.id}
              className={`
                border-2 rounded-lg p-4 cursor-pointer transition-all
                ${
                  config.analysisType === type.id
                    ? "border-purple-500 bg-purple-50"
                    : "border-gray-200 hover:border-gray-300"
                }
              `}
              onClick={() => handleChange("analysisType", type.id)}
            >
              <div className="flex items-start gap-3">
                <div className="text-2xl">{type.icon}</div>
                <div className="flex-1">
                  <h4 className="font-semibold text-gray-900">{type.name}</h4>
                  <p className="text-xs text-gray-600">{type.description}</p>
                </div>
                {config.analysisType === type.id && (
                  <div className="text-purple-500 text-xl">✓</div>
                )}
              </div>
            </div>
          ))}
        </div>
      </div>

      {/* Prompt Personalizado (Opcional) */}
      <div className="mb-6">
        <label className="block text-sm font-semibold text-gray-800 mb-2">
          Prompt Personalizado (Opcional)
        </label>
        <textarea
          value={config.customPrompt}
          onChange={(e) => handleChange("customPrompt", e.target.value)}
          placeholder="Ej: Enfócate en las ventas del primer trimestre y compara con el año anterior..."
          className="w-full p-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
          rows="3"
        />
        <p className="text-xs text-gray-500 mt-1">
          Si lo dejas vacío, se usará un prompt predeterminado según el tipo de
          análisis
        </p>
      </div>

      {/* Botón de Análisis */}
      <button
        onClick={handleAnalyze}
        disabled={isLoading}
        className={`
          w-full py-3 px-6 rounded-lg font-semibold text-white transition-all
          ${
            isLoading
              ? "bg-gray-400 cursor-not-allowed"
              : "bg-gradient-to-r from-blue-600 to-purple-600 hover:from-blue-700 hover:to-purple-700"
          }
        `}
      >
        {isLoading ? (
          <div className="flex items-center justify-center gap-2">
            <div className="animate-spin rounded-full h-5 w-5 border-b-2 border-white"></div>
            <span>Analizando con IA...</span>
          </div>
        ) : (
          <div className="flex items-center justify-center gap-2">
            <span>✨</span>
            <span>Analizar con IA</span>
          </div>
        )}
      </button>

      {/* Estimación de costo */}
      <div className="mt-4 p-3 bg-gray-50 border border-gray-200 rounded-lg">
        <p className="text-xs text-gray-600">
          💰 <strong>Estimación de costo:</strong> El análisis usará
          aproximadamente 1,000-5,000 tokens dependiendo del tamaño de los
          datos.
        </p>
      </div>
    </div>
  );
};

export default AIConfigPanel;

