import { useState } from "react";
import { Eye, EyeOff, RefreshCw } from "lucide-react";

const ExcelDataPreview = ({ excelData, sectionIndex, handleFileUpload }) => {
  const [showPreview, setShowPreview] = useState(false);

  if (!excelData) return null;

  const { headers, data: rows } = excelData;
  const displayRows = rows ? rows.slice(0, 5) : [];

  const togglePreview = (e) => {
    e.stopPropagation();
    setShowPreview(!showPreview);
  };

  return (
    <div
      className="mb-4 p-3 bg-blue-50 border border-blue-200 rounded-lg"
      onClick={(e) => e.stopPropagation()}
    >
      <div className="flex justify-between items-center mb-2">
        <h3 className="text-sm font-medium text-blue-700">
          ✅ Datos Excel cargados ({rows ? rows.length : 0} filas, {headers?.length || 0} columnas)
        </h3>
        <div className="flex space-x-2">
          <button
            onClick={togglePreview}
            className="text-xs px-3 py-1 bg-blue-100 hover:bg-blue-200 rounded flex items-center transition-colors"
          >
            {showPreview ? (
              <>
                <EyeOff className="h-3 w-3 mr-1" />
                Ocultar
              </>
            ) : (
              <>
                <Eye className="h-3 w-3 mr-1" />
                Mostrar
              </>
            )}
          </button>
          <button
            onClick={(e) => {
              e.stopPropagation();
              const fileInput = document.createElement("input");
              fileInput.type = "file";
              fileInput.accept = ".xlsx, .xls, .csv";
              fileInput.onchange = (event) =>
                handleFileUpload(sectionIndex, event);
              fileInput.click();
            }}
            className="text-xs px-3 py-1 bg-blue-100 hover:bg-blue-200 rounded flex items-center transition-colors"
          >
            <RefreshCw className="h-3 w-3 mr-1" />
            Cambiar
          </button>
        </div>
      </div>

      {showPreview && (
        <div className="mt-2 overflow-x-auto bg-white rounded border border-blue-200">
          <table className="min-w-full text-xs">
            <thead>
              <tr className="bg-blue-100">
                {headers.map((header, index) => (
                  <th key={index} className="p-2 border border-blue-200 text-left font-semibold">
                    {header}
                  </th>
                ))}
              </tr>
            </thead>
            <tbody>
              {displayRows.map((row, rowIndex) => (
                <tr
                  key={rowIndex}
                  className={rowIndex % 2 === 0 ? "bg-white" : "bg-blue-50"}
                >
                  {row.map((cell, cellIndex) => (
                    <td key={cellIndex} className="p-2 border border-blue-200">
                      {cell !== null && cell !== undefined ? cell : "-"}
                    </td>
                  ))}
                </tr>
              ))}
            </tbody>
          </table>
          {rows && rows.length > 5 && (
            <div className="p-2 bg-blue-50 text-center">
              <p className="text-xs text-blue-600 italic">
                Mostrando 5 de {rows.length} filas
              </p>
            </div>
          )}
        </div>
      )}
    </div>
  );
};

export default ExcelDataPreview;

