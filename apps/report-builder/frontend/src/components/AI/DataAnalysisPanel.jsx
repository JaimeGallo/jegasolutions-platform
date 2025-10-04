import { useState, useEffect } from "react";
import { analyzeData } from "../../services/analysisService";
import { generateNarrativeFromAnalysis } from "../../services/narrativeService";

const DataAnalysisPanel = ({ data, config, onAnalysisComplete }) => {
  const [analysis, setAnalysis] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  useEffect(() => {
    if (data && data.length > 0) {
      performAnalysis();
    }
  }, [data, config]);

  const prepareDataForAnalysis = (rawData) => {
    // Convertir datos a formato adecuado para análisis
    if (!rawData || !Array.isArray(rawData)) return [];
    
    return rawData.map(row => {
      if (Array.isArray(row)) {
        return row;
      } else if (typeof row === 'object') {
        return Object.values(row);
      }
      return [row];
    });
  };

  const performAnalysis = async () => {
    setLoading(true);
    setError(null);

    try {
      const analysisData = prepareDataForAnalysis(data);
      const analysisResult = await analyzeData(analysisData, config);

      // Generar narrativa si está habilitado
      if (config?.includeNarrative !== false) {
        try {
          const narrativeResult = await generateNarrativeFromAnalysis(
            analysisResult,
            config,
            data
          );
          setAnalysis({
            ...analysisResult,
            narrative: narrativeResult,
          });
        } catch (narrativeError) {
          console.error("Error generando narrativa:", narrativeError);
          setAnalysis({
            ...analysisResult,
            narrative: {
              title: "Error generando narrativa",
              content: `No se pudo generar la narrativa automáticamente. Error: ${narrativeError.message}`,
              keyPoints: [],
            },
          });
        }
      } else {
        setAnalysis(analysisResult);
      }

      if (onAnalysisComplete) {
        onAnalysisComplete(analysisResult);
      }
    } catch (err) {
      console.error("Error en el análisis:", err);
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center p-6">
        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
        <span className="ml-3 text-gray-600">Analizando datos...</span>
      </div>
    );
  }

  if (error) {
    return (
      <div className="p-4 bg-red-50 border border-red-200 rounded">
        <p className="text-red-600">{error}</p>
      </div>
    );
  }

  if (!analysis) {
    return (
      <div className="p-4 bg-gray-50 border border-gray-200 rounded">
        <p className="text-gray-600">No hay análisis disponible</p>
      </div>
    );
  }

  return (
    <div className="space-y-4">
      {analysis.narrative && (
        <div className="bg-white p-4 rounded-lg border shadow-sm">
          <h3 className="font-semibold text-gray-900 mb-2">
            {analysis.narrative.title}
          </h3>
          <p className="text-gray-700 whitespace-pre-wrap">
            {analysis.narrative.content}
          </p>
          {analysis.narrative.keyPoints?.length > 0 && (
            <div className="mt-4">
              <h4 className="font-medium text-gray-900 mb-2">Puntos Clave:</h4>
              <ul className="list-disc list-inside space-y-1">
                {analysis.narrative.keyPoints.map((point, index) => (
                  <li key={index} className="text-gray-700">{point}</li>
                ))}
              </ul>
            </div>
          )}
        </div>
      )}

      {analysis.suggestions?.length > 0 && (
        <div className="bg-blue-50 p-4 rounded-lg border border-blue-200">
          <h4 className="font-medium text-blue-900 mb-2">Sugerencias:</h4>
          <ul className="space-y-2">
            {analysis.suggestions.map((suggestion, index) => (
              <li key={index} className="text-blue-800 text-sm">
                • {suggestion}
              </li>
            ))}
          </ul>
        </div>
      )}
    </div>
  );
};

export default DataAnalysisPanel;

