import DraggableItem from "./DraggableItem";
import TemplateMetadataForm from "./TemplateMetadataForm";

const ComponentPalette = ({
  addSection,
  template,
  updateTemplate,
  generateDefaultStructure,
}) => {
  const componentTypes = [
    { type: "text", name: "Texto", icon: "📝" },
    { type: "table", name: "Tabla", icon: "📊" },
    { type: "chart", name: "Gráfico", icon: "📈" },
    { type: "kpi", name: "KPI", icon: "🔢" },
    { type: "image", name: "Imagen", icon: "🖼️" },
  ];

  return (
    <div className="w-64 bg-white p-4 border-r border-gray-200 overflow-y-auto">
      <h2 className="font-semibold text-lg mb-4">🎨 Componentes</h2>
      
      <div className="mb-4 p-3 bg-blue-50 border border-blue-200 rounded">
        <p className="text-sm text-blue-700">
          💡 Arrastra los componentes a las secciones o haz clic en "+" dentro de cada sección
        </p>
      </div>

      <div className="space-y-2 mb-4">
        {componentTypes.map((item) => (
          <DraggableItem
            key={item.type}
            type={item.type}
            icon={item.icon}
            name={item.name}
          />
        ))}
      </div>

      <div className="space-y-2 mb-4">
        <button
          onClick={addSection}
          className="w-full px-3 py-2 bg-blue-50 text-blue-600 rounded-md hover:bg-blue-100 border border-blue-200 transition-colors font-medium"
        >
          ➕ Añadir Sección
        </button>

        <button
          onClick={generateDefaultStructure}
          className="w-full px-3 py-2 bg-green-50 text-green-600 rounded-md hover:bg-green-100 border border-green-200 transition-colors font-medium"
        >
          📄 Usar plantilla base
        </button>
      </div>

      <TemplateMetadataForm
        template={template}
        updateTemplate={updateTemplate}
      />
    </div>
  );
};

export default ComponentPalette;

