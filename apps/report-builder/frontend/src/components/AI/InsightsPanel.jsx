import { useState, useEffect } from "react";
import { Lightbulb, TrendingUp, AlertCircle } from "lucide-react";
import { getInsights } from "../../services/analysisService";

const InsightsPanel = ({ reportId }) => {
  const [insights, setInsights] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  useEffect(() => {
    if (reportId) {
      loadInsights();
    }
  }, [reportId]);

  const loadInsights = async () => {
    setLoading(true);
    setError(null);

    try {
      const data = await getInsights(reportId);
      setInsights(data);
    } catch (err) {
      console.error("Error cargando insights:", err);
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center p-6">
        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
        <span className="ml-3 text-gray-600">Cargando insights...</span>
      </div>
    );
  }

  if (error) {
    return (
      <div className="p-4 bg-red-50 border border-red-200 rounded flex items-start gap-2">
        <AlertCircle className="h-5 w-5 text-red-600 flex-shrink-0 mt-0.5" />
        <p className="text-red-600">{error}</p>
      </div>
    );
  }

  if (!insights) {
    return (
      <div className="p-4 bg-gray-50 border border-gray-200 rounded">
        <p className="text-gray-600">No hay insights disponibles</p>
      </div>
    );
  }

  return (
    <div className="space-y-4">
      {/* Insights principales */}
      {insights.mainInsights?.length > 0 && (
        <div className="bg-gradient-to-r from-blue-50 to-purple-50 p-4 rounded-lg border border-blue-200">
          <div className="flex items-center gap-2 mb-3">
            <Lightbulb className="h-5 w-5 text-blue-600" />
            <h3 className="font-semibold text-gray-900">Insights Principales</h3>
          </div>
          <ul className="space-y-2">
            {insights.mainInsights.map((insight, index) => (
              <li key={index} className="flex items-start gap-2 text-gray-700">
                <span className="text-blue-500 font-bold">•</span>
                <span>{insight}</span>
              </li>
            ))}
          </ul>
        </div>
      )}

      {/* Tendencias */}
      {insights.trends?.length > 0 && (
        <div className="bg-green-50 p-4 rounded-lg border border-green-200">
          <div className="flex items-center gap-2 mb-3">
            <TrendingUp className="h-5 w-5 text-green-600" />
            <h3 className="font-semibold text-gray-900">Tendencias Detectadas</h3>
          </div>
          <ul className="space-y-2">
            {insights.trends.map((trend, index) => (
              <li key={index} className="text-gray-700">
                <span className="font-medium">{trend.name}:</span>{" "}
                <span>{trend.description}</span>
                {trend.confidence && (
                  <span className="ml-2 text-xs bg-green-100 text-green-800 px-2 py-1 rounded">
                    {Math.round(trend.confidence * 100)}% confianza
                  </span>
                )}
              </li>
            ))}
          </ul>
        </div>
      )}

      {/* Alertas */}
      {insights.alerts?.length > 0 && (
        <div className="bg-yellow-50 p-4 rounded-lg border border-yellow-200">
          <div className="flex items-center gap-2 mb-3">
            <AlertCircle className="h-5 w-5 text-yellow-600" />
            <h3 className="font-semibold text-gray-900">Alertas</h3>
          </div>
          <ul className="space-y-2">
            {insights.alerts.map((alert, index) => (
              <li key={index} className="text-gray-700">
                <span className="font-medium text-yellow-800">{alert.title}:</span>{" "}
                <span>{alert.message}</span>
              </li>
            ))}
          </ul>
        </div>
      )}

      {/* Recomendaciones */}
      {insights.recommendations?.length > 0 && (
        <div className="bg-purple-50 p-4 rounded-lg border border-purple-200">
          <h3 className="font-semibold text-gray-900 mb-3">Recomendaciones</h3>
          <ul className="space-y-2">
            {insights.recommendations.map((rec, index) => (
              <li key={index} className="flex items-start gap-2 text-gray-700">
                <span className="text-purple-500">→</span>
                <span>{rec}</span>
              </li>
            ))}
          </ul>
        </div>
      )}
    </div>
  );
};

export default InsightsPanel;

