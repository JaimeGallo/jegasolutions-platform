import { useState, useEffect } from "react";

const ChartConfig = ({ component, onUpdate, sectionData }) => {
  const [excelColumns, setExcelColumns] = useState([]);
  const [dataMapping, setDataMapping] = useState({
    xAxisField: "",
    yAxisField: "",
    seriesField: "",
  });

  useEffect(() => {
    const excelData =
      component.dataSource?.excelData ||
      (component.dataSource?.sourceType === "excel" && sectionData?.excelData);

    if (excelData?.headers) {
      setExcelColumns(excelData.headers);

      if (!component.dataSource.mappings) {
        const initialMappings = {
          xAxisField: excelData.headers[0] || "",
          yAxisField: excelData.headers[1] || "",
          seriesField: excelData.headers[2] || "",
        };
        onUpdate("dataSource.mappings", initialMappings);
        setDataMapping(initialMappings);
      } else {
        setDataMapping(component.dataSource.mappings);
      }
    }
  }, [component.dataSource, sectionData]);

  const handleMappingChange = (field, value) => {
    const newMapping = { ...dataMapping, [field]: value };
    setDataMapping(newMapping);
    onUpdate("dataSource.mappings", newMapping);
  };

  const handleChartOptionChange = (option, value) => {
    onUpdate(option, value);
  };

  return (
    <div className="space-y-3">
      <div>
        <label className="block text-sm font-medium text-gray-700 mb-1">
          Tipo de gráfico
        </label>
        <select
          value={component.chartType || "bar"}
          onChange={(e) => handleChartOptionChange("chartType", e.target.value)}
          className="w-full p-2 border rounded text-sm focus:ring-2 focus:ring-blue-500"
        >
          <option value="bar">📊 Barras</option>
          <option value="line">📈 Líneas</option>
          <option value="pie">🥧 Circular</option>
          <option value="area">📉 Área</option>
          <option value="scatter">⚪ Dispersión</option>
          <option value="radar">🎯 Radar</option>
        </select>
      </div>

      <div>
        <label className="block text-sm font-medium text-gray-700 mb-1">
          Fuente de datos
        </label>
        <select
          value={component.dataSource?.sourceType || "manual"}
          onChange={(e) =>
            handleChartOptionChange("dataSource.sourceType", e.target.value)
          }
          className="w-full p-2 border rounded text-sm focus:ring-2 focus:ring-blue-500"
        >
          <option value="manual">Manual</option>
          <option value="excel">Excel</option>
          <option value="api">API</option>
          <option value="database">Base de datos</option>
        </select>
      </div>

      {component.dataSource?.sourceType === "excel" && excelColumns.length > 0 && (
        <div className="mt-2 p-3 bg-gray-50 border rounded">
          <h4 className="text-sm font-medium mb-3 text-gray-700">Mapeo de datos</h4>
          <div className="space-y-3">
            <div>
              <label className="block text-xs text-gray-600 mb-1">
                Eje X (Categorías)
              </label>
              <select
                value={dataMapping.xAxisField || ""}
                onChange={(e) => handleMappingChange("xAxisField", e.target.value)}
                className="w-full p-2 border rounded text-sm focus:ring-2 focus:ring-blue-500"
              >
                <option value="">Seleccionar columna</option>
                {excelColumns.map((column, idx) => (
                  <option key={`x-${idx}`} value={column}>
                    {column}
                  </option>
                ))}
              </select>
            </div>

            <div>
              <label className="block text-xs text-gray-600 mb-1">
                Eje Y (Valores)
              </label>
              <select
                value={dataMapping.yAxisField || ""}
                onChange={(e) => handleMappingChange("yAxisField", e.target.value)}
                className="w-full p-2 border rounded text-sm focus:ring-2 focus:ring-blue-500"
              >
                <option value="">Seleccionar columna</option>
                {excelColumns.map((column, idx) => (
                  <option key={`y-${idx}`} value={column}>
                    {column}
                  </option>
                ))}
              </select>
            </div>

            {component.chartType !== "pie" && (
              <div>
                <label className="block text-xs text-gray-600 mb-1">
                  Series (Opcional)
                </label>
                <select
                  value={dataMapping.seriesField || ""}
                  onChange={(e) => handleMappingChange("seriesField", e.target.value)}
                  className="w-full p-2 border rounded text-sm focus:ring-2 focus:ring-blue-500"
                >
                  <option value="">Sin series</option>
                  {excelColumns.map((column, idx) => (
                    <option key={`series-${idx}`} value={column}>
                      {column}
                    </option>
                  ))}
                </select>
              </div>
            )}
          </div>
        </div>
      )}

      {component.dataSource?.sourceType === "excel" && excelColumns.length === 0 && (
        <div className="p-3 bg-yellow-50 border border-yellow-200 rounded">
          <p className="text-sm text-yellow-700">
            ⚠️ No hay datos de Excel disponibles. Por favor, carga un archivo Excel en la sección.
          </p>
        </div>
      )}

      <div className="space-y-2 pt-3 border-t">
        <label className="flex items-center text-sm">
          <input
            type="checkbox"
            checked={component.showLegend !== false}
            onChange={(e) => handleChartOptionChange("showLegend", e.target.checked)}
            className="mr-2"
          />
          Mostrar leyenda
        </label>

        <label className="flex items-center text-sm">
          <input
            type="checkbox"
            checked={component.showTooltip !== false}
            onChange={(e) => handleChartOptionChange("showTooltip", e.target.checked)}
            className="mr-2"
          />
          Mostrar tooltip
        </label>

        {component.chartType === "bar" && (
          <label className="flex items-center text-sm">
            <input
              type="checkbox"
              checked={component.stackBars || false}
              onChange={(e) => handleChartOptionChange("stackBars", e.target.checked)}
              className="mr-2"
            />
            Apilar barras
          </label>
        )}

        {(component.chartType === "line" || component.chartType === "area") && (
          <label className="flex items-center text-sm">
            <input
              type="checkbox"
              checked={component.fillArea || false}
              onChange={(e) => handleChartOptionChange("fillArea", e.target.checked)}
              className="mr-2"
            />
            Rellenar área
          </label>
        )}
      </div>
    </div>
  );
};

export default ChartConfig;

