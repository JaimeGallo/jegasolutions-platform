import { useState, useEffect } from "react";

const TableConfig = ({ component, onUpdate, sectionData }) => {
  const [excelColumns, setExcelColumns] = useState([]);
  const [selectedColumns, setSelectedColumns] = useState([]);

  useEffect(() => {
    const excelData =
      component.dataSource?.excelData ||
      component.excelData ||
      (component.dataSource?.sourceType === "excel" && sectionData?.excelData);

    if (excelData?.headers) {
      setExcelColumns(excelData.headers);

      const currentSelected = component.dataSource?.selectedColumns;
      const newSelected =
        currentSelected && currentSelected.length > 0
          ? currentSelected.filter((col) => excelData.headers.includes(col))
          : excelData.headers;

      if (!currentSelected || currentSelected.length === 0) {
        onUpdate("dataSource.selectedColumns", newSelected);
      }
      setSelectedColumns(newSelected);
    }
  }, [component.dataSource, component.excelData, sectionData]);

  const handleColumnToggle = (column) => {
    let newSelectedColumns;
    if (selectedColumns.includes(column)) {
      newSelectedColumns = selectedColumns.filter((col) => col !== column);
    } else {
      newSelectedColumns = [...selectedColumns, column];
    }

    setSelectedColumns(newSelectedColumns);
    onUpdate("dataSource.selectedColumns", newSelectedColumns);
  };

  const handleRowsChange = (e) => {
    const rows = parseInt(e.target.value) || 1;
    onUpdate("rows", Math.max(1, Math.min(20, rows)));
  };

  const handleColumnsChange = (e) => {
    const columns = parseInt(e.target.value) || 1;
    onUpdate("columns", Math.max(1, Math.min(10, columns)));
  };

  return (
    <div className="space-y-3">
      {(!component.dataSource || component.dataSource?.sourceType === "manual") && (
        <div className="grid grid-cols-2 gap-2">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Filas</label>
            <input
              type="number"
              min="1"
              max="20"
              value={component.rows || 3}
              onChange={handleRowsChange}
              className="w-full p-2 border rounded text-sm focus:ring-2 focus:ring-blue-500"
            />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Columnas</label>
            <input
              type="number"
              min="1"
              max="10"
              value={component.columns || 2}
              onChange={handleColumnsChange}
              className="w-full p-2 border rounded text-sm focus:ring-2 focus:ring-blue-500"
            />
          </div>
        </div>
      )}

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
            placeholder="https://api.example.com/data"
          />
        </div>
      )}

      {component.dataSource?.sourceType === "excel" && excelColumns.length > 0 && (
        <div className="mt-2 p-3 bg-gray-50 border rounded">
          <h4 className="text-sm font-medium mb-2 text-gray-700">
            Seleccionar columnas a mostrar
          </h4>
          <div className="space-y-1 max-h-48 overflow-y-auto">
            {excelColumns.map((column, index) => (
              <label key={index} className="flex items-center p-1 hover:bg-gray-100 rounded cursor-pointer">
                <input
                  type="checkbox"
                  checked={selectedColumns.includes(column)}
                  onChange={() => handleColumnToggle(column)}
                  className="mr-2"
                />
                <span className="text-sm text-gray-700">{column}</span>
              </label>
            ))}
          </div>
          <div className="mt-2 pt-2 border-t">
            <button
              onClick={() => {
                const newSelected = excelColumns;
                setSelectedColumns(newSelected);
                onUpdate("dataSource.selectedColumns", newSelected);
              }}
              className="text-xs text-blue-600 hover:text-blue-700 mr-3"
            >
              Seleccionar todas
            </button>
            <button
              onClick={() => {
                setSelectedColumns([]);
                onUpdate("dataSource.selectedColumns", []);
              }}
              className="text-xs text-gray-600 hover:text-gray-700"
            >
              Deseleccionar todas
            </button>
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
    </div>
  );
};

export default TableConfig;

