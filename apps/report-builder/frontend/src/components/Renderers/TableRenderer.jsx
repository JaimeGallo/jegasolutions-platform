import { formatExcelValue } from "../../utils/textAnalysisUtils";

const TableRenderer = ({ component, excelData }) => {
  const {
    dataSource,
    rows: manualRows = 3,
    columns: manualCols = 2,
  } = component;
  const sourceType = dataSource?.sourceType || "manual";

  // --- Caso 1: La fuente de datos es Excel ---
  if (sourceType === "excel") {
    const data = dataSource?.excelData || excelData;

    if (!data?.headers || !data.data) {
      return (
        <div className="p-4 text-gray-500 text-center border-2 border-dashed border-gray-300 rounded">
          No hay datos de Excel disponibles.
        </div>
      );
    }

    const visibleColumns = dataSource?.selectedColumns || data.headers;
    const columnIndexes = visibleColumns.map((col) =>
      data.headers.indexOf(col)
    );

    return (
      <div className="overflow-x-auto">
        <table className="min-w-full border border-gray-300">
          <thead>
            <tr className="bg-gradient-to-r from-blue-600 to-purple-600 text-white">
              {visibleColumns.map((header, index) => (
                <th key={index} className="px-4 py-3 border border-gray-300 text-left font-semibold">
                  {header}
                </th>
              ))}
            </tr>
          </thead>
          <tbody>
            {data.data.map((row, rowIndex) => (
              <tr
                key={rowIndex}
                className={rowIndex % 2 === 0 ? "bg-white hover:bg-gray-50" : "bg-gray-50 hover:bg-gray-100"}
              >
                {columnIndexes.map((colIndex, i) => (
                  <td key={i} className="px-4 py-2 border border-gray-300">
                    {colIndex >= 0 ? formatExcelValue(row[colIndex]) : ""}
                  </td>
                ))}
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    );
  }

  // --- Caso 2: La fuente de datos es Manual ---
  // Renderiza una tabla vacía con las dimensiones especificadas
  return (
    <div className="overflow-x-auto">
      <table className="min-w-full border border-gray-300">
        <tbody>
          {Array.from({ length: manualRows }).map((_, rowIndex) => (
            <tr
              key={rowIndex}
              className={rowIndex % 2 === 0 ? "bg-white" : "bg-gray-50"}
            >
              {Array.from({ length: manualCols }).map((_, colIndex) => (
                <td key={colIndex} className="px-4 py-2 border border-gray-300">
                  &nbsp;
                </td>
              ))}
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default TableRenderer;

