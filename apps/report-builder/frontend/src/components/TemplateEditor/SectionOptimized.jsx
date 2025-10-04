import { useDrop } from "react-dnd";
import { Upload, Trash2, Plus, Type, BarChart3, Table as TableIcon, Hash, Image as ImageIcon, Calendar } from "lucide-react";
import ComponentOptimized from "./ComponentOptimized";

const SectionOptimized = ({
  section,
  index,
  selectedItem,
  setSelectedItem,
  removeSection,
  addComponent,
  moveComponent,
  removeComponent,
  handleFileUpload,
  updateTemplate,
  removeEvent,
}) => {
  const isSelected =
    selectedItem &&
    selectedItem.type === "section" &&
    selectedItem.index === index;

  const [{ isOver }, drop] = useDrop(() => ({
    accept: "COMPONENT_TYPE",
    drop: (item) => {
      addComponent(index, item.type);
      return { dropped: true };
    },
    collect: (monitor) => ({
      isOver: !!monitor.isOver(),
    }),
  }));

  const handleTitleChange = (e) => {
    updateTemplate(`sections[${index}].title`, e.target.value);
  };

  // Component buttons for inline adding
  const componentButtons = [
    { type: "text", icon: Type, label: "Texto", color: "blue" },
    { type: "table", icon: TableIcon, label: "Tabla", color: "green" },
    { type: "chart", icon: BarChart3, label: "Gráfico", color: "purple" },
    { type: "kpi", icon: Hash, label: "KPI", color: "orange" },
    { type: "image", icon: ImageIcon, label: "Imagen", color: "pink" },
  ];

  const getSectionStyle = () => {
    let style = "border rounded-lg transition-all ";

    if (isSelected) {
      style += "ring-2 ring-blue-500 border-blue-500 ";
    } else {
      style += "border-gray-300 ";
    }

    if (isOver) {
      style += "bg-blue-50";
    } else {
      style += "bg-white";
    }

    return style;
  };

  return (
    <div
      ref={drop}
      className={getSectionStyle()}
      onClick={(e) => {
        e.stopPropagation();
        setSelectedItem({ type: "section", index });
      }}
    >
      {/* Section Header - Inline Controls */}
      <div className="p-4 border-b border-gray-200 bg-gray-50">
        <div className="flex items-center justify-between gap-4 mb-3">
          {/* Title (Editable) */}
          <input
            type="text"
            value={section.title}
            onChange={handleTitleChange}
            placeholder="Título de la sección..."
            className="flex-1 text-lg font-semibold text-gray-800 bg-transparent border-none outline-none focus:ring-2 focus:ring-blue-500 rounded px-2 py-1"
            onClick={(e) => e.stopPropagation()}
          />

          {/* Excel Upload Button */}
          <div>
            <input
              type="file"
              id={`excel-upload-${index}`}
              accept=".xlsx,.xls,.csv"
              style={{ display: "none" }}
              onChange={(e) => handleFileUpload(index, e)}
            />
            <button
              onClick={(e) => {
                e.stopPropagation();
                document.getElementById(`excel-upload-${index}`).click();
              }}
              className="flex items-center gap-2 px-3 py-2 text-sm bg-green-600 text-white rounded-lg hover:bg-green-700 transition-colors"
              title="Cargar datos Excel"
            >
              <Upload className="w-4 h-4" />
              {section.excelData ? "Cambiar Excel" : "Cargar Excel"}
            </button>
          </div>

          {/* Delete Section */}
          <button
            onClick={(e) => {
              e.stopPropagation();
              if (confirm("¿Eliminar esta sección?")) {
                removeSection(index);
              }
            }}
            className="p-2 text-red-600 hover:bg-red-50 rounded-lg transition-colors"
            title="Eliminar sección"
          >
            <Trash2 className="w-4 h-4" />
          </button>
        </div>

        {/* Excel Data Info */}
        {section.excelData && (
          <div className="mb-3 p-2 bg-green-50 border border-green-200 rounded text-sm text-green-700">
            ✅ {section.excelData.headers.length} columnas, {section.excelData.data.length} filas
            {section.excelData.fileName && <span className="ml-2">• {section.excelData.fileName}</span>}
          </div>
        )}

        {/* Component Selector + Event Management - Same Row */}
        <div className="flex items-center justify-between gap-2">
          {/* Add Components Inline */}
          <div className="flex items-center gap-1">
            <span className="text-xs text-gray-500 font-medium mr-2">Agregar:</span>
            {componentButtons.map(({ type, icon: Icon, label, color }) => (
              <button
                key={type}
                onClick={(e) => {
                  e.stopPropagation();
                  addComponent(index, type);
                }}
                className={`flex items-center gap-1 px-2 py-1 text-xs rounded transition-colors
                  ${color === "blue" ? "text-blue-700 bg-blue-50 hover:bg-blue-100" : ""}
                  ${color === "green" ? "text-green-700 bg-green-50 hover:bg-green-100" : ""}
                  ${color === "purple" ? "text-purple-700 bg-purple-50 hover:bg-purple-100" : ""}
                  ${color === "orange" ? "text-orange-700 bg-orange-50 hover:bg-orange-100" : ""}
                  ${color === "pink" ? "text-pink-700 bg-pink-50 hover:bg-pink-100" : ""}
                `}
                title={`Agregar ${label}`}
              >
                <Icon className="w-3 h-3" />
                <span className="hidden sm:inline">{label}</span>
              </button>
            ))}
          </div>

          {/* Event Management */}
          <button
            onClick={(e) => {
              e.stopPropagation();
              // Aquí iría la lógica de gestión de eventos
              alert("Gestión de sucesos - En desarrollo");
            }}
            className="flex items-center gap-1 px-3 py-1 text-xs text-yellow-700 bg-yellow-50 hover:bg-yellow-100 rounded transition-colors"
            title="Gestionar sucesos"
          >
            <Calendar className="w-3 h-3" />
            <span>Sucesos ({section.events?.length || 0})</span>
          </button>
        </div>
      </div>

      {/* Components Area */}
      <div className="p-4">
        {section.components && section.components.length > 0 ? (
          <div className="space-y-3">
            {section.components.map((component, componentIndex) => (
              <ComponentOptimized
                key={component.componentId}
                component={component}
                sectionIndex={index}
                componentIndex={componentIndex}
                removeComponent={removeComponent}
                updateTemplate={updateTemplate}
                sectionData={section}
              />
            ))}
          </div>
        ) : (
          <div className="py-8 text-center border-2 border-dashed border-gray-200 rounded-lg">
            <Plus className="w-8 h-8 mx-auto text-gray-400 mb-2" />
            <p className="text-gray-500 text-sm">
              Arrastra componentes aquí o usa los botones de arriba
            </p>
          </div>
        )}
      </div>
    </div>
  );
};

export default SectionOptimized;

