import { useState } from "react";
import { DndProvider } from "react-dnd";
import { HTML5Backend } from "react-dnd-html5-backend";
import ComponentPalette from "./ComponentPalette";
import SectionsArea from "./SectionsArea";
import ConfigurationPanel from "./ConfigurationPanel";
import PreviewPanel from "./PreviewPanel";
import useTemplateManagement from "./useTemplateManagement";
import EventModal from "./EventModal";

const TemplateEditor = ({ initialTemplate, onSave, onCancel }) => {
  const [showPreview, setShowPreview] = useState(false);
  const [showSelectEventsModal, setShowSelectEventsModal] = useState(false);

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

  const togglePreview = () => {
    setShowPreview(!showPreview);
  };

  const handleAddEventToSection = () => {
    if (selectedItem && typeof selectedItem.index === "number") {
      addEventToSection(selectedItem.index);
    }
  };

  const handleSelectEvents = (events) => {
    if (selectedItem && typeof selectedItem.index === "number") {
      const sectionIndex = selectedItem.index;
      const updatedSections = [...template.sections];
      updatedSections[sectionIndex].events = [
        ...(updatedSections[sectionIndex].events || []),
        ...events,
      ];
      updateTemplate("sections", updatedSections);
    }
  };

  const getCurrentSection = () => {
    if (!selectedItem) return null;
    return selectedItem.type === "section"
      ? template.sections[selectedItem.index]
      : template.sections[selectedItem.sectionIndex];
  };

  const handleSave = () => {
    // Validación básica
    if (!template.sections || template.sections.length === 0) {
      alert("La plantilla debe tener al menos una sección");
      return;
    }

    if (!template.metadata?.description) {
      alert("Por favor, agrega una descripción a la plantilla");
      return;
    }

    // Callback al componente padre
    onSave(template);
  };

  return (
    <DndProvider backend={HTML5Backend}>
      <div className="flex h-full bg-gray-50 overflow-hidden">
        {/* Component Palette - Left Sidebar */}
        <ComponentPalette
          addSection={addSection}
          template={template}
          updateTemplate={updateTemplate}
          generateDefaultStructure={generateDefaultStructure}
        />

        {/* Sections Area - Main Content */}
        <SectionsArea
          template={template}
          selectedItem={selectedItem}
          setSelectedItem={setSelectedItem}
          addSection={addSection}
          removeSection={removeSection}
          addComponent={addComponent}
          moveComponent={moveComponent}
          removeComponent={removeComponent}
          handleFileUpload={handleFileUpload}
          updateTemplate={updateTemplate}
          onSave={handleSave}
          onCancel={onCancel}
          initialTemplate={initialTemplate}
          onPreview={togglePreview}
          removeEvent={removeEvent}
        />

        {/* Configuration Panel - Right Sidebar */}
        {selectedItem && (
          <ConfigurationPanel
            selectedItem={selectedItem}
            template={template}
            updateTemplate={updateTemplate}
            handleFileUpload={handleFileUpload}
            currentSection={getCurrentSection()}
            addEventToSection={handleAddEventToSection}
            setShowSelectEventsModal={setShowSelectEventsModal}
            setSelectedItem={setSelectedItem}
          />
        )}

        {/* Preview Modal */}
        {showPreview && (
          <PreviewPanel template={template} onClose={togglePreview} />
        )}

        {/* Event Modal */}
        {isModalOpen && (
          <EventModal
            eventData={eventData}
            setEventData={setEventData}
            setIsModalOpen={setIsModalOpen}
            addEventToSection={handleAddEventToSection}
          />
        )}

        {/* Select Events Modal - Placeholder */}
        {showSelectEventsModal && (
          <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
            <div className="bg-white rounded-lg shadow-xl w-full max-w-2xl p-6">
              <h3 className="text-lg font-medium mb-4">Seleccionar Sucesos Existentes</h3>
              <p className="text-gray-600 mb-4">
                Esta funcionalidad permitirá seleccionar sucesos existentes de una lista.
              </p>
              <div className="flex justify-end space-x-2">
                <button
                  onClick={() => setShowSelectEventsModal(false)}
                  className="px-4 py-2 border border-gray-300 rounded hover:bg-gray-100"
                >
                  Cancelar
                </button>
                <button
                  onClick={() => {
                    // handleSelectEvents([...]);
                    setShowSelectEventsModal(false);
                  }}
                  className="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700"
                >
                  Seleccionar
                </button>
              </div>
            </div>
          </div>
        )}
      </div>
    </DndProvider>
  );
};

export default TemplateEditor;

