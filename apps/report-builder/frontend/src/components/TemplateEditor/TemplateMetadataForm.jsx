const TemplateMetadataForm = ({ template, updateTemplate }) => {
  return (
    <div className="mt-6">
      <h2 className="font-semibold text-lg mb-2">Configuración General</h2>
      <div className="space-y-3">
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">
            Tipo de Plantilla
          </label>
          <select
            value={template.metadata.templateType}
            onChange={(e) =>
              updateTemplate("metadata.templateType", e.target.value)
            }
            className="w-full p-2 border rounded text-sm focus:ring-2 focus:ring-blue-500"
          >
            <option value="generic">Genérica</option>
            <option value="technical">Técnica</option>
            <option value="financial">Financiera</option>
            <option value="monthly-report">Informe Mensual</option>
            <option value="annual-report">Informe Anual</option>
            <option value="project-report">Informe de Proyecto</option>
          </select>
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">
            Descripción
          </label>
          <textarea
            value={template.metadata.description}
            onChange={(e) =>
              updateTemplate("metadata.description", e.target.value)
            }
            className="w-full p-2 border rounded text-sm focus:ring-2 focus:ring-blue-500"
            rows={3}
            placeholder="Descripción de la plantilla..."
          />
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">
            Versión
          </label>
          <input
            type="text"
            value={template.metadata.version || "1.0.0"}
            onChange={(e) =>
              updateTemplate("metadata.version", e.target.value)
            }
            className="w-full p-2 border rounded text-sm focus:ring-2 focus:ring-blue-500"
            placeholder="1.0.0"
          />
        </div>
      </div>
    </div>
  );
};

export default TemplateMetadataForm;

