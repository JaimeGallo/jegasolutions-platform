import { formatExcelValue } from "../../utils/textAnalysisUtils";

const KpiRenderer = ({ component, excelData }) => {
  let value = component.value || 0;
  let unit = component.unit || "";
  let displayValue = value;

  // Si la fuente es Excel, buscar valor según configuración
  if (component.dataSource?.sourceType === "excel" && excelData) {
    const { dataField, rowIndex = 0 } = component.dataSource.mappings || {};

    if (dataField) {
      const colIndex = excelData.headers.indexOf(dataField);
      if (colIndex >= 0 && excelData.data[rowIndex]) {
        const excelValue = excelData.data[rowIndex][colIndex];
        // No intentar parsear a float aquí, dejar que el formateador decida.
        if (excelValue !== undefined && excelValue !== null) {
          displayValue = formatExcelValue(excelValue);
        }
      }
    }
  }

  // Formatear el valor según el tipo
  if (component.format === "currency" && typeof displayValue === "number") {
    displayValue = new Intl.NumberFormat("es-ES", {
      style: "currency",
      currency: "EUR",
    }).format(displayValue);
  } else if (
    component.format === "percent" &&
    typeof displayValue === "number"
  ) {
    displayValue = `${displayValue}%`;
  }

  return (
    <div className="p-6 text-center bg-gradient-to-br from-blue-50 to-white rounded-lg border border-blue-200">
      <div className="text-5xl font-bold text-blue-600 mb-2">{displayValue}</div>
      <div className="text-lg text-gray-700 font-medium">
        {component.title || ""} {unit}
      </div>
      {component.dataSource?.sourceType === "excel" && (
        <div className="text-xs text-gray-400 mt-2">
          Fuente: {component.dataSource.mappings?.dataField || "Datos Excel"}
        </div>
      )}
    </div>
  );
};

export default KpiRenderer;

