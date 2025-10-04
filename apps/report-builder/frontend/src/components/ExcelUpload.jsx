import { useState, useCallback } from "react";
import { toast } from "react-toastify";
import { ExcelUploadService } from "../services/excelUploadService";

const ExcelUpload = ({ areaId, period, onUploadSuccess }) => {
  const [isDragging, setIsDragging] = useState(false);
  const [isUploading, setIsUploading] = useState(false);
  const [uploadProgress, setUploadProgress] = useState(0);

  const handleDragEnter = useCallback((e) => {
    e.preventDefault();
    e.stopPropagation();
    setIsDragging(true);
  }, []);

  const handleDragLeave = useCallback((e) => {
    e.preventDefault();
    e.stopPropagation();
    setIsDragging(false);
  }, []);

  const handleDragOver = useCallback((e) => {
    e.preventDefault();
    e.stopPropagation();
  }, []);

  const handleDrop = useCallback(
    (e) => {
      e.preventDefault();
      e.stopPropagation();
      setIsDragging(false);

      const files = Array.from(e.dataTransfer.files);
      const excelFile = files.find((file) =>
        file.name.match(/\.(xlsx|xls|csv)$/i)
      );

      if (excelFile) {
        handleFileUpload(excelFile);
      } else {
        toast.error("Por favor, selecciona un archivo Excel válido (.xlsx, .xls, .csv)");
      }
    },
    [areaId, period]
  );

  const handleFileSelect = (e) => {
    const file = e.target.files[0];
    if (file) {
      handleFileUpload(file);
    }
  };

  const handleFileUpload = async (file) => {
    if (!areaId || !period) {
      toast.error("Área y período son requeridos");
      return;
    }

    // Validar tamaño del archivo (máximo 50MB)
    const maxSize = 50 * 1024 * 1024; // 50MB
    if (file.size > maxSize) {
      toast.error("El archivo es demasiado grande. Máximo 50MB.");
      return;
    }

    // Validar tipo de archivo
    const validExtensions = [".xlsx", ".xls", ".csv"];
    const fileExtension = file.name.substring(file.name.lastIndexOf("."));
    if (!validExtensions.includes(fileExtension.toLowerCase())) {
      toast.error("Tipo de archivo no válido. Use .xlsx, .xls o .csv");
      return;
    }

    try {
      setIsUploading(true);
      setUploadProgress(0);

      // Simular progreso mientras se carga
      const progressInterval = setInterval(() => {
        setUploadProgress((prev) => {
          if (prev >= 90) {
            clearInterval(progressInterval);
            return 90;
          }
          return prev + 10;
        });
      }, 200);

      const result = await ExcelUploadService.uploadExcel(file, areaId, period);

      clearInterval(progressInterval);
      setUploadProgress(100);

      toast.success(`Archivo ${file.name} cargado exitosamente`);

      if (onUploadSuccess) {
        onUploadSuccess(result);
      }

      // Resetear después de 1 segundo
      setTimeout(() => {
        setUploadProgress(0);
        setIsUploading(false);
      }, 1000);
    } catch (error) {
      console.error("Error al subir archivo:", error);
      toast.error(
        error.response?.data?.message ||
          "Error al subir el archivo. Por favor, intenta nuevamente."
      );
      setIsUploading(false);
      setUploadProgress(0);
    }
  };

  return (
    <div className="excel-upload-container">
      <div
        className={`
          border-2 border-dashed rounded-lg p-8 text-center transition-all
          ${
            isDragging
              ? "border-blue-500 bg-blue-50"
              : "border-gray-300 bg-white hover:border-gray-400"
          }
          ${isUploading ? "opacity-50 cursor-not-allowed" : "cursor-pointer"}
        `}
        onDragEnter={handleDragEnter}
        onDragLeave={handleDragLeave}
        onDragOver={handleDragOver}
        onDrop={handleDrop}
      >
        <input
          type="file"
          id="file-upload"
          className="hidden"
          accept=".xlsx,.xls,.csv"
          onChange={handleFileSelect}
          disabled={isUploading}
        />

        <label htmlFor="file-upload" className="cursor-pointer">
          <div className="flex flex-col items-center gap-4">
            {isUploading ? (
              <>
                <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
                <p className="text-gray-600 font-medium">
                  Subiendo archivo... {uploadProgress}%
                </p>
                <div className="w-64 bg-gray-200 rounded-full h-2.5">
                  <div
                    className="bg-blue-600 h-2.5 rounded-full transition-all duration-300"
                    style={{ width: `${uploadProgress}%` }}
                  ></div>
                </div>
              </>
            ) : (
              <>
                <div className="text-6xl">📊</div>
                <div>
                  <p className="text-lg font-semibold text-gray-700 mb-2">
                    {isDragging
                      ? "Suelta el archivo aquí"
                      : "Arrastra y suelta tu archivo Excel"}
                  </p>
                  <p className="text-sm text-gray-500">
                    o haz clic para seleccionar
                  </p>
                  <p className="text-xs text-gray-400 mt-2">
                    Formatos soportados: .xlsx, .xls, .csv (máx. 50MB)
                  </p>
                </div>
              </>
            )}
          </div>
        </label>
      </div>

      {/* Información adicional */}
      <div className="mt-4 p-4 bg-blue-50 border border-blue-200 rounded-lg">
        <h4 className="font-semibold text-blue-900 mb-2">💡 Consejos:</h4>
        <ul className="text-sm text-blue-800 space-y-1">
          <li>• Asegúrate de que tu Excel tenga una fila de encabezados</li>
          <li>• Los datos se extraerán automáticamente</li>
          <li>• Puedes solicitar análisis AI después de subir</li>
          <li>
            • Si el archivo no se procesa correctamente, puedes re-procesarlo
          </li>
        </ul>
      </div>
    </div>
  );
};

export default ExcelUpload;

