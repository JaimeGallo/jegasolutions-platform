import Section from "./Section";
import PreviewButton from "./PreviewButton";

const SectionsArea = ({
  template,
  selectedItem,
  setSelectedItem,
  addSection,
  removeSection,
  addComponent,
  moveComponent,
  removeComponent,
  handleFileUpload,
  updateTemplate,
  removeEvent,
  onSave,
  onCancel,
  initialTemplate,
  onPreview,
}) => {
  return (
    <div className="flex-1 p-6 overflow-y-auto bg-gray-50">
      <div className="flex justify-between items-center mb-6">
        <div>
          <h1 className="text-2xl font-bold text-gray-800">
            {initialTemplate?.id ? "Editar Plantilla" : "Nueva Plantilla"}
          </h1>
          <p className="text-sm text-gray-600 mt-1">
            {template.metadata?.description || "Plantilla sin descripción"}
          </p>
        </div>
        <div className="flex space-x-2">
          <button
            onClick={onCancel}
            className="px-4 py-2 border border-gray-300 rounded-md hover:bg-gray-100 text-sm transition-colors"
          >
            Cancelar
          </button>
          <PreviewButton
            onClick={onPreview}
            disabled={template.sections.length === 0}
          />
          <button
            onClick={() => onSave(template)}
            className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 text-sm transition-colors shadow"
          >
            💾 Guardar Plantilla
          </button>
        </div>
      </div>

      {template.sections.length === 0 ? (
        <div className="flex flex-col items-center justify-center h-96 border-2 border-dashed border-gray-300 rounded-lg bg-white">
          <div className="text-6xl mb-4">📄</div>
          <p className="text-gray-500 mb-4 text-lg">
            No hay secciones en esta plantilla
          </p>
          <p className="text-gray-400 mb-6 text-sm max-w-md text-center">
            Las secciones organizan tu plantilla. Puedes agregar componentes como textos,
            tablas, gráficos y más a cada sección.
          </p>
          <button
            onClick={addSection}
            className="px-6 py-3 bg-blue-600 text-white rounded-md hover:bg-blue-700 text-sm font-medium shadow-lg transition-all hover:shadow-xl"
          >
            ➕ Crear primera sección
          </button>
        </div>
      ) : (
        <div className="space-y-4">
          {template.sections.map((section, index) => (
            <Section
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
              updateTemplate={(path, value) => {
                if (path.includes("${")) {
                  const realPath = path
                    .replace("${index}", index)
                    .replace(/\${([^}]*)}/g, (_, variable) => variable);
                  return updateTemplate(realPath, value);
                } else {
                  return updateTemplate(`sections[${index}].${path}`, value);
                }
              }}
              removeEvent={(eventIndex) => removeEvent(index, eventIndex)}
            />
          ))}
        </div>
      )}
    </div>
  );
};

export default SectionsArea;

