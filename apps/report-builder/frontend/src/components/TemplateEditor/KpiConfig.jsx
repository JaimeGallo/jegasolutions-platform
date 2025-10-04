import { useState, useEffect } from "react";

const KpiConfig = ({ component, onUpdate, sectionData }) => {
  const [excelColumns, setExcelColumns] = useState([]);

  useEffect(() => {
    const excelData =
      component.dataSource?.excelData ||
      (component.dataSource?.sourceType === "excel" && sectionData?.excelData);

    if (excelData?.headers) {
      setExcelColumns(excelData.headers);

      if (!component.dataSource.mappings) {
        onUpdate("dataSource.mappings", {
          dataField: excelData.headers[0] || "",
          rowIndex: 0,
        });
      }
    }
  }, [component.dataSource, sectionData?.excelData]);

  return (
    <div className="space-y-3">
      <div className="grid grid-cols-2 gap-2">
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">Valor</label>
          <input
            type="text"
            value={component.value || ""}
            onChange={(e) => onUpdate("value", e.target.value)}
            className="w-full p-2 border rounded text-sm focus:ring-2 focus:ring-blue-500"
            placeholder="0"
            disabled={component.dataSource?.sourceType === "excel"}
          />
        </div>
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">Unidad</label>
          <input
            type="text"
            value={component.unit || ""}
            onChange={(e) => onUpdate("unit", e.target.value)}
            className="w-full p-2 border rounded text-sm focus:ring-2 focus:ring-blue-500"
            placeholder="%, $, unidades..."
          />
        </div>
      </div>

      <div>
        <label className="block text-sm font-medium text-gray-700 mb-1">Título (opcional)</label>
        <input
          type="text"
          value={component.title || ""}
          onChange={(e) => onUpdate("title", e.target.value)}
          className="w-full p-2 border rounded text-sm focus:ring-2 focus:ring-blue-500"
          placeholder="Ej: Total de ventas"
        />
      </div>

      <div>
        <label className="block text-sm font-medium text-gray-700 mb-1">Fuente de datos</label>
        <select
          value={component.dataSource?.sourceType || "manual"}
          onChange={(e) => onUpdate("dataSource.sourceType", e.target.value)}
          className="w-full p-2 border rounded text-sm focus:ring-2 focus:ring-blue-500"
        >
          <option value="manual">Manual</option>
          <option value="excel">Excel</option>
          <option value="api">API</option>
        </select>
      </div>

      {component.dataSource?.sourceType === "api" && (
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">URL API</label>
          <input
            type="text"
            value={component.dataSource?.apiUrl || ""}
            onChange={(e) => onUpdate("dataSource.apiUrl", e.target.value)}
            className="w-full p-2 border rounded text-sm focus:ring-2 focus:ring-blue-500"
            placeholder="https://api.example.com/kpi"
          />
        </div>
      )}

      {component.dataSource?.sourceType === "excel" && excelColumns.length > 0 && (
        <div className="mt-2 p-3 bg-gray-50 border rounded">
          <h4 className="text-sm font-medium mb-2 text-gray-700">Seleccionar dato</h4>
          <div className="grid grid-cols-2 gap-2">
            <div>
              <label className="block text-xs text-gray-600 mb-1">Columna</label>
              <select
                value={component.dataSource.mappings?.dataField || ""}
                onChange={(e) =>
                  onUpdate("dataSource.mappings.dataField", e.target.value)
                }
                className="w-full p-2 border rounded text-sm focus:ring-2 focus:ring-blue-500"
              >
                <option value="">Seleccionar</option>
                {excelColumns.map((column, idx) => (
                  <option key={idx} value={column}>
                    {column}
                  </option>
                ))}
              </select>
            </div>

            <div>
              <label className="block text-xs text-gray-600 mb-1">Fila</label>
              <input
                type="number"
                min="0"
                value={component.dataSource.mappings?.rowIndex || 0}
                onChange={(e) =>
                  onUpdate(
                    "dataSource.mappings.rowIndex",
                    parseInt(e.target.value) || 0
                  )
                }
                className="w-full p-2 border rounded text-sm focus:ring-2 focus:ring-blue-500"
              />
            </div>
          </div>

          <div className="mt-2">
            <label className="block text-xs text-gray-600 mb-1">Agregación</label>
            <select
              value={component.dataSource.mappings?.aggregation || "value"}
              onChange={(e) =>
                onUpdate("dataSource.mappings.aggregation", e.target.value)
              }
              className="w-full p-2 border rounded text-sm focus:ring-2 focus:ring-blue-500"
            >
              <option value="value">Valor directo</option>
              <option value="sum">Suma</option>
              <option value="avg">Promedio</option>
              <option value="count">Conteo</option>
              <option value="max">Máximo</option>
              <option value="min">Mínimo</option>
            </select>
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

      <div>
        <label className="block text-sm font-medium text-gray-700 mb-1">Formato</label>
        <select
          value={component.format || "number"}
          onChange={(e) => onUpdate("format", e.target.value)}
          className="w-full p-2 border rounded text-sm focus:ring-2 focus:ring-blue-500"
        >
          <option value="number">Número</option>
          <option value="currency">Moneda</option>
          <option value="percent">Porcentaje</option>
        </select>
      </div>
    </div>
  );
};

export default KpiConfig;

