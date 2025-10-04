import { useState } from "react";
import { DndProvider } from "react-dnd";
import { HTML5Backend } from "react-dnd-html5-backend";
import { Plus, Save, X, Eye, FileText, BarChart3, Table as TableIcon, Type, Image as ImageIcon, Hash } from "lucide-react";
import useTemplateManagement from "./useTemplateManagement";
import SectionOptimized from "./SectionOptimized";
import LivePreview from "./LivePreview";

const TemplateEditorOptimized = ({ initialTemplate, onSave, onCancel }) => {
  const [livePreview, setLivePreview] = useState(true);

  const {
    template,
    selectedItem,
    isModalOpen,
    eventData,
    updateTemplate,
    addSection,
    removeSection,
    addComponent,
    moveComponent,
    removeComponent,
    setSelectedItem,
    setIsModalOpen,
    setEventData,
    handleFileUpload,
    addEventToSection,
    removeEvent,
    generateDefaultStructure,
  } = useTemplateManagement(initialTemplate);

  const handleSave = () => {
    if (!template.sections || template.sections.length === 0) {
      alert("La plantilla debe tener al menos una sección");
      return;
    }
    if (!template.metadata?.description) {
      alert("Por favor, agrega una descripción a la plantilla");
      return;
    }
    onSave(template);
  };

  const handleTitleChange = (e) => {
    updateTemplate("metadata.description", e.target.value);
  };

  const handleTypeChange = (e) => {
    updateTemplate("metadata.type", e.target.value);
  };

  return (
    <DndProvider backend={HTML5Backend}>
      <div className="flex h-full bg-gray-50">
        {/* Editor Area - Main Content - 75% WIDTH */}
        <div className="w-[75%] flex flex-col overflow-hidden">
          {/* Header with Template Metadata */}
          <div className="bg-white border-b border-gray-200 p-4 shadow-sm">
            <div className="flex items-center justify-between gap-4">
              {/* Left: Title + Type + Description */}
              <div className="flex-1 flex items-center gap-4">
                <div className="flex flex-col gap-2 flex-1 max-w-2xl">
                  <div className="flex items-center gap-3">
                    <input
                      type="text"
                      value={template.metadata?.description || ""}
                      onChange={handleTitleChange}
                      placeholder="Nombre de la plantilla..."
                      className="flex-1 text-2xl font-bold text-gray-800 border-none outline-none focus:ring-2 focus:ring-blue-500 rounded px-2 py-1"
                    />
                    <select
                      value={template.metadata?.type || "Genérica"}
                      onChange={handleTypeChange}
                      className="px-3 py-2 border border-gray-300 rounded-lg text-sm focus:ring-2 focus:ring-blue-500"
                    >
                      <option value="Genérica">Genérica</option>
                      <option value="Financiera">Financiera</option>
                      <option value="Operativa">Operativa</option>
                      <option value="Ejecutiva">Ejecutiva</option>
                    </select>
                  </div>
                </div>
              </div>

              {/* Right: Actions */}
              <div className="flex items-center gap-2">
                <button
                  onClick={() => generateDefaultStructure()}
                  className="flex items-center gap-2 px-3 py-2 text-sm text-green-700 bg-green-50 border border-green-200 rounded-lg hover:bg-green-100 transition-colors"
                >
                  <FileText className="w-4 h-4" />
                  Usar plantilla base
                </button>
                <button
                  onClick={onCancel}
                  className="flex items-center gap-2 px-4 py-2 text-sm border border-gray-300 rounded-lg hover:bg-gray-100 transition-colors"
                >
                  <X className="w-4 h-4" />
                  Cancelar
                </button>
                <button
                  onClick={() => setLivePreview(!livePreview)}
                  className={`flex items-center gap-2 px-4 py-2 text-sm rounded-lg transition-colors ${
                    livePreview
                      ? "bg-blue-600 text-white hover:bg-blue-700"
                      : "border border-gray-300 hover:bg-gray-100"
                  }`}
                >
                  <Eye className="w-4 h-4" />
                  Vista Previa
                </button>
                <button
                  onClick={handleSave}
                  className="flex items-center gap-2 px-6 py-2 text-sm bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors shadow font-medium"
                >
                  <Save className="w-4 h-4" />
                  Guardar
                </button>
              </div>
            </div>
          </div>

          {/* Sections Area - Scrollable */}
          <div className="flex-1 p-6 overflow-y-auto">
            {template.sections.length === 0 ? (
              <div className="flex flex-col items-center justify-center h-full border-2 border-dashed border-gray-300 rounded-lg bg-white">
                <div className="text-6xl mb-4">📄</div>
                <p className="text-gray-500 mb-4 text-lg">
                  No hay secciones en esta plantilla
                </p>
                <p className="text-gray-400 mb-6 text-sm max-w-md text-center">
                  Las secciones organizan tu plantilla. Puedes agregar componentes inline.
                </p>
                <button
                  onClick={addSection}
                  className="flex items-center gap-2 px-6 py-3 bg-blue-600 text-white rounded-lg hover:bg-blue-700 font-medium shadow-lg transition-all hover:shadow-xl"
                >
                  <Plus className="w-5 h-5" />
                  Crear primera sección
                </button>
              </div>
            ) : (
              <div className="space-y-4">
                {template.sections.map((section, index) => (
                  <SectionOptimized
                    key={section.sectionId}
                    section={section}
                    index={index}
                    selectedItem={selectedItem}
                    setSelectedItem={setSelectedItem}
                    removeSection={removeSection}
                    addComponent={addComponent}
                    moveComponent={moveComponent}
                    removeComponent={removeComponent}
                    handleFileUpload={handleFileUpload}
                    updateTemplate={updateTemplate}
                    removeEvent={removeEvent}
                  />
                ))}

                {/* Add Section Button */}
                <button
                  onClick={addSection}
                  className="w-full py-4 border-2 border-dashed border-gray-300 rounded-lg hover:border-blue-500 hover:bg-blue-50 transition-all text-gray-500 hover:text-blue-600 font-medium"
                >
                  <Plus className="w-5 h-5 inline mr-2" />
                  Añadir Sección
                </button>
              </div>
            )}
          </div>
        </div>

        {/* Live Preview - Right Side - 25% WIDTH */}
        {livePreview && (
          <div className="w-[25%] border-l border-gray-200 bg-white overflow-hidden">
            <LivePreview template={template} />
          </div>
        )}
      </div>
    </DndProvider>
  );
};

export default TemplateEditorOptimized;