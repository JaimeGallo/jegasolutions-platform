import { Upload, Calendar } from "lucide-react";
import TextConfig from "./TextConfig";
import TableConfig from "./TableConfig";
import ChartConfig from "./ChartConfig";
import KpiConfig from "./KpiConfig";
import ImageConfig from "./ImageConfig";
import AIConfigPanel from "../AI/AIConfigPanel";
import AIAnalysisPanel from "../AI/AIAnalysisPanel";

const ConfigurationPanel = ({
  selectedItem,
  updateTemplate,
  handleFileUpload,
  currentSection,
  addEventToSection,
  setShowSelectEventsModal,
  setSelectedItem,
}) => {
  // Early return si no hay nada seleccionado
  if (!selectedItem) {
    return (
      <div className="w-96 bg-white p-4 border-l border-gray-200 overflow-y-auto flex flex-col items-center justify-center text-gray-500">
        <div className="text-6xl mb-4">⚙️</div>
        <p className="text-center">Selecciona una sección o componente para configurar</p>
      </div>
    );
  }

  // Función para renderizar el panel de configuración del componente específico
  const renderComponentConfig = () => {
    // Aseguramos que el componente exista antes de intentar acceder a él
    if (!currentSection?.components?.[selectedItem.componentIndex]) {
      return <div className="text-gray-500">Selecciona un componente para configurar.</div>;
    }
    const component = currentSection.components[selectedItem.componentIndex];

    const onComponentUpdate = (path, value) => {
      updateTemplate(
        `sections[${selectedItem.sectionIndex}].components[${selectedItem.componentIndex}].${path}`,
        value
      );
    };

    switch (component.type) {
      case "text":
        return (
          <div className="space-y-4">
            <TextConfig component={component} onUpdate={onComponentUpdate} />
            
            {/* AI Analysis Panel para componentes de texto */}
            <div className="border-t pt-4">
              <AIAnalysisPanel
                component={component}
                onUpdate={onComponentUpdate}
                sectionData={currentSection}
              />
            </div>
            
            {/* AI Config Panel para configurar opciones AI */}
            <div className="border-t pt-4">
              <AIConfigPanel
                config={component.aiConfig || {}}
                onConfigChange={(newConfig) => onComponentUpdate("aiConfig", newConfig)}
                hasData={currentSection?.excelData?.data?.length > 0}
                showAdvanced={false}
              />
            </div>
          </div>
        );
      case "table":
        return (
          <TableConfig
            component={component}
            onUpdate={onComponentUpdate}
            sectionData={currentSection}
          />
        );
      case "chart":
        return (
          <ChartConfig
            component={component}
            onUpdate={onComponentUpdate}
            sectionData={currentSection}
          />
        );
      case "kpi":
        return (
          <KpiConfig
            component={component}
            onUpdate={onComponentUpdate}
            sectionData={currentSection}
          />
        );
      case "image":
        return (
          <ImageConfig
            component={component}
            onUpdate={onComponentUpdate}
          />
        );
      default:
        return <div className="text-gray-500">Tipo de componente no soportado</div>;
    }
  };

  return (
    <div className="w-96 bg-white p-4 border-l border-gray-200 overflow-y-auto flex flex-col">
      <h2 className="font-semibold text-lg mb-4">⚙️ Configuración</h2>

      <div className="flex-grow overflow-y-auto">
        {selectedItem.type === "section" && currentSection ? (
          // Panel de configuración para una SECCIÓN
          <div className="space-y-4">
            <div className="bg-blue-50 border border-blue-200 rounded p-3">
              <h3 className="font-medium text-blue-700">
                📑 Sección: {currentSection.title}
              </h3>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Título de la sección
              </label>
              <input
                type="text"
                value={currentSection.title}
                onChange={(e) =>
                  updateTemplate(
                    `sections[${selectedItem.index}].title`,
                    e.target.value
                  )
                }
                className="w-full p-2 border rounded text-sm focus:ring-2 focus:ring-blue-500"
                placeholder="Ej: Introducción, Análisis, Conclusiones..."
              />
            </div>

            {/* Excel Upload Button */}
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Datos de Excel
              </label>
              <input
                type="file"
                id={`config-excel-upload-${selectedItem.index}`}
                accept=".xlsx,.xls,.csv"
                style={{ display: "none" }}
                onChange={(e) => handleFileUpload(selectedItem.index, e)}
              />
              <button
                onClick={() =>
                  document.getElementById(`config-excel-upload-${selectedItem.index}`).click()
                }
                className="w-full px-4 py-2 bg-green-50 text-green-700 rounded hover:bg-green-100 border border-green-200 flex items-center justify-center transition-colors"
              >
                <Upload className="h-4 w-4 mr-2" />
                {currentSection.excelData ? "Cambiar archivo Excel" : "Cargar archivo Excel"}
              </button>
            </div>

            {currentSection.excelData && (
              <div className="p-3 bg-green-50 border border-green-200 rounded">
                <p className="text-sm font-medium text-green-700 mb-1">
                  ✅ Datos cargados exitosamente
                </p>
                <p className="text-xs text-green-600">
                  📊 {currentSection.excelData.headers.length} columnas, {currentSection.excelData.data.length} filas
                </p>
                {currentSection.excelData.fileName && (
                  <p className="text-xs text-green-600 mt-1">
                    📄 {currentSection.excelData.fileName}
                  </p>
                )}
              </div>
            )}

            {/* Mostrar componentes de la sección para selección rápida */}
            {currentSection.components && currentSection.components.length > 0 && (
              <div className="mt-4">
                <h4 className="text-sm font-medium text-gray-700 mb-2">
                  📦 Componentes en esta sección:
                </h4>
                <div className="space-y-2">
                  {currentSection.components.map((component, idx) => (
                    <button
                      key={component.componentId || idx}
                      onClick={() => {
                        // Cambiar selección al componente
                        if (setSelectedItem) {
                          setSelectedItem({
                            type: "component",
                            sectionIndex: selectedItem.index,
                            componentIndex: idx,
                          });
                        }
                      }}
                      className="w-full text-left p-2 border rounded hover:bg-blue-50 text-sm transition-colors"
                    >
                      <span className="font-medium">
                        {component.type === "text" && "📝"}
                        {component.type === "table" && "📊"}
                        {component.type === "chart" && "📈"}
                        {component.type === "kpi" && "🔢"}
                        {component.type === "image" && "🖼️"}
                        {" "}{component.type}
                      </span>
                      {component.content && (
                        <span className="text-gray-600 ml-2">
                          - {component.content.substring(0, 30)}...
                        </span>
                      )}
                    </button>
                  ))}
                </div>
              </div>
            )}
          </div>
        ) : (
          // Panel de configuración para un COMPONENTE
          currentSection && (
            <div className="space-y-4">
              <div className="bg-purple-50 border border-purple-200 rounded p-3">
                <h3 className="font-medium text-purple-700">
                  {currentSection.components?.[selectedItem.componentIndex]?.type === "text" && "📝 Texto"}
                  {currentSection.components?.[selectedItem.componentIndex]?.type === "table" && "📊 Tabla"}
                  {currentSection.components?.[selectedItem.componentIndex]?.type === "chart" && "📈 Gráfico"}
                  {currentSection.components?.[selectedItem.componentIndex]?.type === "kpi" && "🔢 KPI"}
                  {currentSection.components?.[selectedItem.componentIndex]?.type === "image" && "🖼️ Imagen"}
                </h3>
              </div>
              {renderComponentConfig()}
            </div>
          )
        )}
      </div>

      {/* Botones de Sucesos - Solo visible para secciones */}
      {selectedItem.type === "section" && (
        <div className="border-t pt-4 mt-4">
          <h3 className="font-medium text-gray-900 mb-3 flex items-center">
            <Calendar className="h-4 w-4 mr-2" />
            Gestión de Sucesos
          </h3>
          <div className="space-y-2">
            <button
              onClick={addEventToSection}
              className="w-full px-3 py-2 bg-yellow-50 text-yellow-700 rounded hover:bg-yellow-100 border border-yellow-200 text-sm transition-colors"
            >
              ➕ Agregar Suceso Manual
            </button>
            {setShowSelectEventsModal && (
              <button
                onClick={() => setShowSelectEventsModal(true)}
                className="w-full px-3 py-2 bg-purple-50 text-purple-700 rounded hover:bg-purple-100 border border-purple-200 text-sm transition-colors"
              >
                📅 Seleccionar Sucesos Existentes
              </button>
            )}
          </div>
        </div>
      )}
    </div>
  );
};

export default ConfigurationPanel;

