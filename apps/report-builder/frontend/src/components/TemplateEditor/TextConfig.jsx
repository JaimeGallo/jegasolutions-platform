const TextConfig = ({ component = {}, onUpdate }) => {
  return (
    <div className="space-y-4">
      <div>
        <label className="block text-sm font-medium text-gray-700 mb-1">
          Contenido del texto
        </label>
        <textarea
          value={component.content || ""}
          onChange={(e) => onUpdate("content", e.target.value)}
          className="w-full p-2 border rounded text-sm focus:ring-2 focus:ring-blue-500"
          rows={6}
          placeholder="Escribe el contenido del texto..."
        />
      </div>

      <div className="border-t pt-4">
        <div className="flex items-start">
          <input
            type="checkbox"
            id="autoGenerate"
            checked={component.autoGenerate || false}
            onChange={(e) => onUpdate("autoGenerate", e.target.checked)}
            className="mt-1 mr-2"
          />
          <div>
            <label htmlFor="autoGenerate" className="block text-sm font-medium text-gray-700 cursor-pointer">
              Generar narrativa automática
            </label>
            <p className="text-xs text-gray-500 mt-1">
              Cuando está activado, se generará un texto analítico básico basado en los datos.
            </p>
          </div>
        </div>

        {component.autoGenerate && (
          <div className="space-y-3 mt-3">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Tipo de análisis
              </label>
              <select
                value={component.analysisConfig?.templateType || "default"}
                onChange={(e) =>
                  onUpdate("analysisConfig.templateType", e.target.value)
                }
                className="w-full p-2 border rounded text-sm focus:ring-2 focus:ring-blue-500"
              >
                <option value="default">General</option>
                <option value="sales">Ventas</option>
                <option value="financial">Financiero</option>
                <option value="marketing">Marketing</option>
                <option value="technical">Técnico</option>
                <option value="custom">Personalizado</option>
              </select>
            </div>
          </div>
        )}
      </div>

      <div className="p-3 bg-blue-50 border border-blue-200 rounded">
        <p className="text-sm text-blue-700">
          💡 <strong>Tip:</strong> Puedes combinar texto manual con análisis automático para obtener mejores resultados.
        </p>
      </div>
    </div>
  );
};

export default TextConfig;

