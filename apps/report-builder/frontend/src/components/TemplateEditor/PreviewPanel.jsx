import { useState } from "react";
import { X, ZoomIn, ZoomOut, Download } from "lucide-react";
import ComponentRenderer from "../Renderers/ComponentRenderer";

const PreviewPanel = ({ template, onClose }) => {
  const [zoom, setZoom] = useState(100);

  const handleExportPDF = () => {
    // Por ahora solo mostramos un mensaje
    alert("Función de exportación a PDF en desarrollo. Se implementará próximamente.");
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 overflow-auto p-4">
      <div className="bg-white rounded-lg shadow-2xl flex flex-col w-full max-w-6xl" style={{ height: "90vh" }}>
        {/* Header */}
        <div className="border-b border-gray-200 p-4 flex justify-between items-center bg-gray-50">
          <div>
            <h2 className="text-xl font-bold text-gray-800">📄 Vista Previa del Informe</h2>
            <p className="text-sm text-gray-500">
              {template.metadata?.description || "Sin descripción"}
            </p>
          </div>
          <div className="flex items-center space-x-4">
            {/* Zoom controls */}
            <div className="flex items-center bg-white border border-gray-300 rounded">
              <button
                onClick={() => setZoom(Math.max(50, zoom - 10))}
                className="px-3 py-2 hover:bg-gray-100 transition-colors"
                title="Reducir zoom"
              >
                <ZoomOut className="h-4 w-4" />
              </button>
              <span className="px-3 py-2 border-x border-gray-300 text-sm font-medium min-w-[60px] text-center">
                {zoom}%
              </span>
              <button
                onClick={() => setZoom(Math.min(150, zoom + 10))}
                className="px-3 py-2 hover:bg-gray-100 transition-colors"
                title="Aumentar zoom"
              >
                <ZoomIn className="h-4 w-4" />
              </button>
            </div>

            {/* Export PDF button */}
            <button
              onClick={handleExportPDF}
              className="px-4 py-2 bg-indigo-600 hover:bg-indigo-700 text-white rounded flex items-center transition-colors shadow"
            >
              <Download className="h-4 w-4 mr-2" />
              Exportar PDF
            </button>

            {/* Close button */}
            <button
              onClick={onClose}
              className="px-4 py-2 bg-red-600 text-white rounded hover:bg-red-700 flex items-center transition-colors shadow"
            >
              <X className="h-4 w-4 mr-2" />
              Cerrar
            </button>
          </div>
        </div>

        {/* Content */}
        <div className="flex-1 overflow-auto p-6 bg-gray-100">
          <div
            id="report-preview"
            className="bg-white shadow-lg mx-auto transition-all"
            style={{
              transform: `scale(${zoom / 100})`,
              transformOrigin: "top center",
              width: "210mm",
              minHeight: "297mm",
              padding: "20mm",
            }}
          >
            {/* Report Header */}
            <div className="mb-6 pb-4 border-b-2 border-gray-200">
              <h1 className="text-3xl font-bold text-gray-800 mb-2">
                {template.metadata?.templateType === "generic" ? "Informe General" :
                 template.metadata?.templateType === "monthly-report" ? "Informe Mensual" :
                 template.metadata?.templateType === "annual-report" ? "Informe Anual" :
                 template.metadata?.templateType === "financial" ? "Informe Financiero" :
                 template.metadata?.templateType === "technical" ? "Informe Técnico" :
                 "Informe"}
              </h1>
              {template.metadata?.description && (
                <p className="text-gray-600">{template.metadata.description}</p>
              )}
              <p className="text-sm text-gray-500 mt-2">
                Generado el {new Date().toLocaleDateString('es-ES', { 
                  year: 'numeric', 
                  month: 'long', 
                  day: 'numeric' 
                })}
              </p>
            </div>

            {/* Sections and Components */}
            {template.sections.map((section, sectionIndex) => (
              <div key={section.sectionId || sectionIndex} className="mb-8">
                {/* Section Title */}
                <h2 className="text-2xl font-bold text-gray-800 mb-4 pb-2 border-b border-gray-300">
                  {section.title}
                </h2>

                {/* Section Components */}
                <div className="space-y-4">
                  {section.components.map((component, componentIndex) => {
                    // Skip full-page images in main flow
                    if (component.type === "image" && component.position === "full-page") {
                      return null;
                    }

                    return (
                      <div key={component.componentId || componentIndex} className="component-wrapper">
                        <ComponentRenderer
                          component={component}
                          excelData={section.excelData}
                        />
                      </div>
                    );
                  })}
                </div>

                {/* Events Timeline */}
                {section.events && section.events.length > 0 && (
                  <div className="mt-6 p-4 border-t border-gray-200">
                    <h3 className="text-lg font-semibold mb-3 text-gray-700">
                      📅 Línea de Tiempo
                    </h3>
                    <div className="space-y-3">
                      {section.events.map((event, eventIndex) => (
                        <div key={event.id || eventIndex} className="flex items-start">
                          <div className="flex flex-col items-center mr-4">
                            <div className="w-3 h-3 bg-blue-500 rounded-full"></div>
                            {eventIndex < section.events.length - 1 && (
                              <div className="w-px h-full bg-blue-300 min-h-[40px]"></div>
                            )}
                          </div>
                          <div className="bg-blue-50 p-3 rounded shadow-sm flex-1 border border-blue-200">
                            <div className="font-bold text-gray-800">{event.title}</div>
                            <div className="text-sm text-gray-600">{event.date}</div>
                            {event.description && (
                              <div className="text-sm mt-1 text-gray-700">{event.description}</div>
                            )}
                          </div>
                        </div>
                      ))}
                    </div>
                  </div>
                )}
              </div>
            ))}

            {/* Empty state */}
            {template.sections.length === 0 && (
              <div className="text-center py-12 text-gray-500">
                <p className="text-lg">No hay secciones en esta plantilla</p>
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
};

export default PreviewPanel;

