import { useState, useEffect } from "react";
import { toast } from "react-toastify";
import ExcelUpload from "../components/ExcelUpload";
import AIConfigPanel from "../components/AIConfigPanel";
import { ExcelUploadService } from "../services/excelUploadService";

const ExcelUploadsPage = () => {
  const [uploads, setUploads] = useState([]);
  const [loading, setLoading] = useState(false);
  const [selectedUpload, setSelectedUpload] = useState(null);
  const [showAIPanel, setShowAIPanel] = useState(false);
  const [isAnalyzing, setIsAnalyzing] = useState(false);
  const [analysisResult, setAnalysisResult] = useState(null);

  // Estados para el formulario de upload
  const [uploadForm, setUploadForm] = useState({
    areaId: "",
    period: "",
  });

  useEffect(() => {
    loadUploads();
  }, []);

  const loadUploads = async () => {
    try {
      setLoading(true);
      const data = await ExcelUploadService.getExcelUploads();
      setUploads(data);
    } catch (error) {
      console.error("Error cargando uploads:", error);
      toast.error("Error al cargar los archivos");
    } finally {
      setLoading(false);
    }
  };

  const handleUploadSuccess = (result) => {
    toast.success("Archivo cargado y procesado exitosamente");
    loadUploads();
    // Seleccionar automáticamente el upload recién creado para análisis AI
    setSelectedUpload(result);
    setShowAIPanel(true);
  };

  const handleAnalyze = async (config) => {
    if (!selectedUpload) {
      toast.error("Selecciona un archivo para analizar");
      return;
    }

    try {
      setIsAnalyzing(true);
      const result = await ExcelUploadService.analyzeWithAI(
        selectedUpload.id,
        config.aiProvider,
        config.analysisType,
        config.customPrompt || null
      );
      setAnalysisResult(result);
      toast.success("Análisis completado exitosamente");
    } catch (error) {
      console.error("Error analizando con IA:", error);
      toast.error("Error al realizar el análisis con IA");
    } finally {
      setIsAnalyzing(false);
    }
  };

  const handleDeleteUpload = async (uploadId) => {
    if (!window.confirm("¿Estás seguro de que quieres eliminar este archivo?")) {
      return;
    }

    try {
      await ExcelUploadService.deleteExcelUpload(uploadId);
      toast.success("Archivo eliminado exitosamente");
      loadUploads();
      if (selectedUpload?.id === uploadId) {
        setSelectedUpload(null);
        setShowAIPanel(false);
      }
    } catch (error) {
      console.error("Error eliminando archivo:", error);
      toast.error("Error al eliminar el archivo");
    }
  };

  const handleReprocess = async (uploadId) => {
    try {
      const result = await ExcelUploadService.reprocessExcelUpload(uploadId);
      toast.success("Archivo re-procesado exitosamente");
      loadUploads();
    } catch (error) {
      console.error("Error re-procesando archivo:", error);
      toast.error("Error al re-procesar el archivo");
    }
  };

  const handleSelectUpload = (upload) => {
    setSelectedUpload(upload);
    setShowAIPanel(true);
    setAnalysisResult(null);
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-50 to-blue-50 p-6">
      <div className="max-w-7xl mx-auto">
        <div className="mb-8">
          <h1 className="text-4xl font-bold text-gray-900 mb-2">
            Carga y Análisis de Archivos Excel
          </h1>
          <p className="text-gray-600 text-lg">
            Sube archivos Excel y analízalos con IA para obtener insights
            valiosos
          </p>
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
          {/* Panel izquierdo: Upload y lista */}
          <div className="space-y-6">
            {/* Formulario de upload */}
            <div className="bg-white rounded-lg shadow-lg p-6">
              <h2 className="text-2xl font-bold text-gray-900 mb-4">
                Subir Nuevo Archivo
              </h2>

              <div className="space-y-4 mb-6">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Área ID *
                  </label>
                  <input
                    type="number"
                    value={uploadForm.areaId}
                    onChange={(e) =>
                      setUploadForm({ ...uploadForm, areaId: e.target.value })
                    }
                    placeholder="Ej: 1"
                    className="w-full p-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Período *
                  </label>
                  <input
                    type="text"
                    value={uploadForm.period}
                    onChange={(e) =>
                      setUploadForm({ ...uploadForm, period: e.target.value })
                    }
                    placeholder="Ej: Abril 2025"
                    className="w-full p-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                  />
                </div>
              </div>

              <ExcelUpload
                areaId={uploadForm.areaId}
                period={uploadForm.period}
                onUploadSuccess={handleUploadSuccess}
              />
            </div>

            {/* Lista de archivos cargados */}
            <div className="bg-white rounded-lg shadow-lg p-6">
              <h2 className="text-2xl font-bold text-gray-900 mb-4">
                Archivos Cargados ({uploads.length})
              </h2>

              {loading ? (
                <div className="text-center py-8">
                  <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600 mx-auto"></div>
                  <p className="mt-2 text-gray-600">Cargando...</p>
                </div>
              ) : uploads.length === 0 ? (
                <div className="text-center py-8">
                  <div className="text-4xl mb-2">📁</div>
                  <p className="text-gray-600">No hay archivos cargados</p>
                </div>
              ) : (
                <div className="space-y-3 max-h-96 overflow-y-auto">
                  {uploads.map((upload) => (
                    <div
                      key={upload.id}
                      className={`
                        border rounded-lg p-4 cursor-pointer transition-all
                        ${
                          selectedUpload?.id === upload.id
                            ? "border-blue-500 bg-blue-50"
                            : "border-gray-200 hover:border-gray-300"
                        }
                      `}
                      onClick={() => handleSelectUpload(upload)}
                    >
                      <div className="flex justify-between items-start mb-2">
                        <div className="flex-1">
                          <h3 className="font-semibold text-gray-900">
                            {upload.fileName}
                          </h3>
                          <p className="text-sm text-gray-600">
                            {upload.areaName} • {upload.period}
                          </p>
                        </div>
                        <span
                          className={`px-2 py-1 text-xs font-medium rounded-full ${ExcelUploadService.getStatusColor(
                            upload.processingStatus
                          )}`}
                        >
                          {ExcelUploadService.getStatusText(
                            upload.processingStatus
                          )}
                        </span>
                      </div>

                      <div className="flex items-center gap-2 text-xs text-gray-500">
                        <span>
                          📅 {new Date(upload.uploadDate).toLocaleDateString()}
                        </span>
                        {upload.fileSizeBytes && (
                          <span>
                            • {ExcelUploadService.formatFileSize(upload.fileSizeBytes)}
                          </span>
                        )}
                      </div>

                      <div className="flex gap-2 mt-3">
                        <button
                          onClick={(e) => {
                            e.stopPropagation();
                            handleReprocess(upload.id);
                          }}
                          className="text-xs px-3 py-1 bg-yellow-100 text-yellow-700 rounded hover:bg-yellow-200"
                        >
                          🔄 Re-procesar
                        </button>
                        <button
                          onClick={(e) => {
                            e.stopPropagation();
                            handleDeleteUpload(upload.id);
                          }}
                          className="text-xs px-3 py-1 bg-red-100 text-red-700 rounded hover:bg-red-200"
                        >
                          🗑️ Eliminar
                        </button>
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </div>
          </div>

          {/* Panel derecho: AI Analysis */}
          <div className="space-y-6">
            {showAIPanel && selectedUpload ? (
              <>
                <AIConfigPanel
                  onAnalyze={handleAnalyze}
                  isLoading={isAnalyzing}
                />

                {/* Resultados del análisis */}
                {analysisResult && (
                  <div className="bg-white rounded-lg shadow-lg p-6">
                    <h2 className="text-2xl font-bold text-gray-900 mb-4">
                      Resultados del Análisis
                    </h2>

                    <div className="space-y-4">
                      <div>
                        <h3 className="font-semibold text-gray-800 mb-2">
                          Proveedor de IA:
                        </h3>
                        <p className="text-gray-600">{analysisResult.aiProvider}</p>
                      </div>

                      <div>
                        <h3 className="font-semibold text-gray-800 mb-2">
                          Tipo de Análisis:
                        </h3>
                        <p className="text-gray-600">{analysisResult.analysisType}</p>
                      </div>

                      <div>
                        <h3 className="font-semibold text-gray-800 mb-2">
                          Insights:
                        </h3>
                        <div className="bg-gray-50 p-4 rounded-lg">
                          <p className="text-gray-700 whitespace-pre-wrap">
                            {analysisResult.insightText}
                          </p>
                        </div>
                      </div>

                      {analysisResult.keyFindings &&
                        analysisResult.keyFindings.length > 0 && (
                          <div>
                            <h3 className="font-semibold text-gray-800 mb-2">
                              Hallazgos Clave:
                            </h3>
                            <ul className="list-disc list-inside space-y-1">
                              {analysisResult.keyFindings.map((finding, idx) => (
                                <li key={idx} className="text-gray-700">
                                  {finding}
                                </li>
                              ))}
                            </ul>
                          </div>
                        )}

                      {analysisResult.recommendations &&
                        analysisResult.recommendations.length > 0 && (
                          <div>
                            <h3 className="font-semibold text-gray-800 mb-2">
                              Recomendaciones:
                            </h3>
                            <ul className="list-disc list-inside space-y-1">
                              {analysisResult.recommendations.map(
                                (recommendation, idx) => (
                                  <li key={idx} className="text-gray-700">
                                    {recommendation}
                                  </li>
                                )
                              )}
                            </ul>
                          </div>
                        )}

                      {analysisResult.confidence && (
                        <div>
                          <h3 className="font-semibold text-gray-800 mb-2">
                            Nivel de Confianza:
                          </h3>
                          <div className="flex items-center gap-3">
                            <div className="flex-1 bg-gray-200 rounded-full h-3">
                              <div
                                className="bg-green-500 h-3 rounded-full"
                                style={{
                                  width: `${analysisResult.confidence * 100}%`,
                                }}
                              ></div>
                            </div>
                            <span className="text-gray-700 font-medium">
                              {Math.round(analysisResult.confidence * 100)}%
                            </span>
                          </div>
                        </div>
                      )}
                    </div>
                  </div>
                )}
              </>
            ) : (
              <div className="bg-white rounded-lg shadow-lg p-12 text-center">
                <div className="text-6xl mb-4">🤖</div>
                <h3 className="text-xl font-semibold text-gray-800 mb-2">
                  Selecciona un archivo
                </h3>
                <p className="text-gray-600">
                  Haz clic en un archivo de la lista para analizarlo con IA
                </p>
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
};

export default ExcelUploadsPage;

