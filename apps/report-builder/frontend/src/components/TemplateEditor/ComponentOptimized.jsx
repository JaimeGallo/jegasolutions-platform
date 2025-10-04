import { useState } from "react";
import { GripVertical, Trash2, ChevronDown, ChevronUp, Sparkles } from "lucide-react";
import AIAnalysisPanel from "../AI/AIAnalysisPanel";
import AIConfigPanel from "../AI/AIConfigPanel";

const ComponentOptimized = ({
  component,
  sectionIndex,
  componentIndex,
  removeComponent,
  updateTemplate,
  sectionData,
}) => {
  const [isExpanded, setIsExpanded] = useState(false);
  const [showAIPanel, setShowAIPanel] = useState(false);

  const componentIcons = {
    text: "📝",
    table: "📊",
    chart: "📈",
    kpi: "🔢",
    image: "🖼️",
  };

  const componentNames = {
    text: "Texto",
    table: "Tabla",
    chart: "Gráfico",
    kpi: "KPI",
    image: "Imagen",
  };

  const onComponentUpdate = (path, value) => {
    updateTemplate(
      `sections[${sectionIndex}].components[${componentIndex}].${path}`,
      value
    );
  };

  const handleDelete = (e) => {
    e.stopPropagation();
    if (confirm(`¿Eliminar este componente de ${componentNames[component.type]}?`)) {
      removeComponent(sectionIndex, componentIndex);
    }
  };

  const toggleExpand = (e) => {
    e.stopPropagation();
    setIsExpanded(!isExpanded);
  };

  const toggleAIPanel = (e) => {
    e.stopPropagation();
    setShowAIPanel(!showAIPanel);
  };

  const getComponentValue = () => {
    switch (component.type) {
      case "text":
        return component.content || "Texto sin contenido";
      case "kpi":
        return `${component.value || "0"} ${component.unit || ""}`;
      case "chart":
        return component.chartType || "Gráfico";
      case "table":
        return `Tabla (${component.dataSource?.sourceType || "manual"})`;
      case "image":
        return component.imageData ? "Imagen cargada" : "Sin imagen";
      default:
        return "Componente";
    }
  };

  return (
    <div className="border border-gray-300 rounded-lg bg-white hover:border-blue-400 transition-all">
      {/* Component Header */}
      <div className="flex items-center justify-between p-3 bg-gray-50">
        <div className="flex items-center gap-3 flex-1">
          <GripVertical className="w-4 h-4 text-gray-400 cursor-move" />
          <span className="text-2xl">{componentIcons[component.type]}</span>
          <div className="flex-1">
            <div className="font-medium text-gray-900">
              {componentNames[component.type]}
            </div>
            <div className="text-xs text-gray-500 truncate">
              {getComponentValue()}
            </div>
          </div>
        </div>

        <div className="flex items-center gap-2">
          {/* AI Button for Text Components */}
          {component.type === "text" && sectionData?.excelData?.data?.length > 0 && (
            <button
              onClick={toggleAIPanel}
              className={`p-2 rounded-lg transition-colors ${
                showAIPanel
                  ? "bg-blue-600 text-white"
                  : "text-blue-600 hover:bg-blue-50"
              }`}
              title="Generar narrativa con IA"
            >
              <Sparkles className="w-4 h-4" />
            </button>
          )}

          <button
            onClick={toggleExpand}
            className="p-2 text-gray-600 hover:bg-gray-200 rounded-lg transition-colors"
            title={isExpanded ? "Contraer" : "Expandir"}
          >
            {isExpanded ? (
              <ChevronUp className="w-4 h-4" />
            ) : (
              <ChevronDown className="w-4 h-4" />
            )}
          </button>

          <button
            onClick={handleDelete}
            className="p-2 text-red-600 hover:bg-red-50 rounded-lg transition-colors"
            title="Eliminar"
          >
            <Trash2 className="w-4 h-4" />
          </button>
        </div>
      </div>

      {/* AI Analysis Panel (for Text Components) */}
      {showAIPanel && component.type === "text" && (
        <div className="border-t border-gray-200 p-4 bg-blue-50">
          <div className="mb-4">
            <h4 className="font-semibold text-blue-900 mb-2 flex items-center gap-2">
              <Sparkles className="w-5 h-5" />
              Generación Automática con IA
            </h4>
            <p className="text-sm text-blue-700">
              Genera texto automáticamente a partir de los datos de Excel
            </p>
          </div>

          {/* AI Config Panel */}
          <div className="mb-4">
            <AIConfigPanel
              config={component.aiConfig || {}}
              onConfigChange={(newConfig) => onComponentUpdate("aiConfig", newConfig)}
              hasData={sectionData?.excelData?.data?.length > 0}
              showAdvanced={false}
            />
          </div>

          {/* AI Analysis Panel */}
          <AIAnalysisPanel
            component={component}
            onUpdate={onComponentUpdate}
            sectionData={sectionData}
          />
        </div>
      )}

      {/* Component Config (Expanded) */}
      {isExpanded && (
        <div className="border-t border-gray-200 p-4">
          {component.type === "text" && (
            <div className="space-y-3">
              <label className="block text-sm font-medium text-gray-700">
                Contenido
              </label>
              <textarea
                value={component.content || ""}
                onChange={(e) => onComponentUpdate("content", e.target.value)}
                className="w-full p-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent text-sm"
                rows={4}
                placeholder="Escribe el contenido o usa IA para generarlo..."
              />
              
              {!sectionData?.excelData && (
                <p className="text-xs text-gray-500">
                  💡 Carga datos de Excel en esta sección para usar la generación automática con IA
                </p>
              )}
            </div>
          )}

          {component.type === "kpi" && (
            <div className="space-y-3">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Valor
                </label>
                <input
                  type="text"
                  value={component.value || ""}
                  onChange={(e) => onComponentUpdate("value", e.target.value)}
                  className="w-full p-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 text-sm"
                  placeholder="Ej: 1,234,567"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Unidad
                </label>
                <input
                  type="text"
                  value={component.unit || ""}
                  onChange={(e) => onComponentUpdate("unit", e.target.value)}
                  className="w-full p-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 text-sm"
                  placeholder="Ej: USD, %, personas"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Título
                </label>
                <input
                  type="text"
                  value={component.title || ""}
                  onChange={(e) => onComponentUpdate("title", e.target.value)}
                  className="w-full p-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 text-sm"
                  placeholder="Ej: Ventas Totales"
                />
              </div>
            </div>
          )}

          {component.type === "chart" && (
            <div className="space-y-3">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Tipo de gráfico
                </label>
                <select
                  value={component.chartType || "bar"}
                  onChange={(e) => onComponentUpdate("chartType", e.target.value)}
                  className="w-full p-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 text-sm"
                >
                  <option value="bar">Barras</option>
                  <option value="line">Líneas</option>
                  <option value="pie">Pastel</option>
                  <option value="area">Área</option>
                </select>
              </div>
              <p className="text-xs text-gray-500">
                📊 Configura los datos desde el Excel de la sección
              </p>
            </div>
          )}

          {component.type === "table" && (
            <div>
              <p className="text-sm text-gray-600">
                📊 La tabla usa los datos del Excel de la sección automáticamente
              </p>
            </div>
          )}

          {component.type === "image" && (
            <div className="space-y-3">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Cargar imagen
                </label>
                <input
                  type="file"
                  accept="image/*"
                  onChange={(e) => {
                    const file = e.target.files?.[0];
                    if (file) {
                      const reader = new FileReader();
                      reader.onloadend = () => {
                        onComponentUpdate("imageData", reader.result);
                      };
                      reader.readAsDataURL(file);
                    }
                  }}
                  className="w-full text-sm"
                />
              </div>
              {component.imageData && (
                <div className="mt-2">
                  <img
                    src={component.imageData}
                    alt="Preview"
                    className="w-full h-32 object-cover rounded border"
                  />
                </div>
              )}
            </div>
          )}
        </div>
      )}
    </div>
  );
};

export default ComponentOptimized;

