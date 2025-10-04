import { Eye, EyeOff, ZoomIn, ZoomOut } from "lucide-react";
import { useState } from "react";
import ComponentRenderer from "../Renderers/ComponentRenderer";

const LivePreview = ({ template }) => {
  const [zoom, setZoom] = useState(85); // Increased from 75 to 85
  const [showGrid, setShowGrid] = useState(false);

  const zoomIn = () => setZoom(Math.min(zoom + 10, 150));
  const zoomOut = () => setZoom(Math.max(zoom - 10, 40));

  return (
    <div className="flex flex-col h-full">
      {/* Preview Header - More compact */}
      <div className="p-2 border-b border-gray-200 bg-gray-50">
        <div className="flex items-center justify-between mb-1">
          <div className="flex items-center gap-2">
            <Eye className="w-4 h-4 text-blue-600" />
            <h3 className="font-semibold text-sm text-gray-900">Vista Previa</h3>
          </div>

          {/* Controls - More compact */}
          <div className="flex items-center gap-1">
            <button
              onClick={zoomOut}
              className="p-1.5 hover:bg-gray-200 rounded transition-colors"
              title="Zoom Out"
            >
              <ZoomOut className="w-3.5 h-3.5" />
            </button>
            <span className="text-xs font-medium text-gray-600 w-10 text-center">
              {zoom}%
            </span>
            <button
              onClick={zoomIn}
              className="p-1.5 hover:bg-gray-200 rounded transition-colors"
              title="Zoom In"
            >
              <ZoomIn className="w-3.5 h-3.5" />
            </button>
            <button
              onClick={() => setShowGrid(!showGrid)}
              className={`p-1.5 rounded transition-colors ${
                showGrid ? "bg-blue-100 text-blue-600" : "hover:bg-gray-200"
              }`}
              title="Toggle Grid"
            >
              {showGrid ? <EyeOff className="w-3.5 h-3.5" /> : <Eye className="w-3.5 h-3.5" />}
            </button>
          </div>
        </div>

        {/* Template Info - More compact */}
        <div className="text-xs text-gray-500">
          {template.sections?.length || 0} secciones • 
          {template.sections?.reduce((acc, s) => acc + (s.components?.length || 0), 0) || 0} componentes
        </div>
      </div>

      {/* Preview Content */}
      <div className="flex-1 overflow-auto bg-gray-100 p-2">
        <div
          className={`bg-white shadow-lg mx-auto transition-transform ${
            showGrid ? "bg-grid" : ""
          }`}
          style={{
            transform: `scale(${zoom / 100})`,
            transformOrigin: "top center",
            width: "100%",
            maxWidth: "210mm",
            minHeight: "297mm",
            padding: "10mm", // Reduced from 15mm to 10mm
          }}
        >
          {/* Template Title - More compact */}
          {template.metadata?.description && (
            <div className="mb-4 pb-2 border-b-2 border-gray-200">
              <h1 className="text-xl font-bold text-gray-900 mb-1">
                {template.metadata.description}
              </h1>
              {template.metadata?.type && (
                <span className="inline-block px-2 py-0.5 bg-blue-100 text-blue-800 text-xs rounded-full">
                  {template.metadata.type}
                </span>
              )}
            </div>
          )}

          {/* Sections */}
          {template.sections && template.sections.length > 0 ? (
            <div className="space-y-4">
              {template.sections.map((section, index) => (
                <div key={section.sectionId} className="section-preview">
                  {/* Section Title - More compact */}
                  <h2 className="text-lg font-bold text-gray-800 mb-2 pb-1 border-b border-gray-300">
                    {section.title || `Sección ${index + 1}`}
                  </h2>

                  {/* Section Components */}
                  {section.components && section.components.length > 0 ? (
                    <div className="space-y-2">
                      {section.components.map((component) => (
                        <div key={component.componentId} className="component-preview">
                          <ComponentRenderer
                            component={component}
                            sectionData={section}
                          />
                        </div>
                      ))}
                    </div>
                  ) : (
                    <div className="py-3 text-center text-gray-400 bg-gray-50 rounded border border-dashed border-gray-300">
                      <p className="text-xs">Sección vacía</p>
                    </div>
                  )}
                </div>
              ))}
            </div>
          ) : (
            <div className="flex flex-col items-center justify-center h-full text-gray-400">
              <Eye className="w-12 h-12 mb-3" />
              <p className="text-base">No hay contenido</p>
              <p className="text-xs mt-1">Agrega secciones al editor</p>
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default LivePreview;