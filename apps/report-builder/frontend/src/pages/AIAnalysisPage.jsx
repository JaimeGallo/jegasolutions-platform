import { useState } from "react";
import { useQuery } from "react-query";
import {
  Brain,
  TrendingUp,
  AlertTriangle,
  Lightbulb,
  FileText,
} from "lucide-react";
import { aiService } from "../services/aiService";
import { reportService } from "../services/reportService";

const AIAnalysisPage = () => {
  const [selectedReport, setSelectedReport] = useState(null);
  const [analysisType, setAnalysisType] = useState("summary");
  const [isAnalyzing, setIsAnalyzing] = useState(false);

  const { data: reports } = useQuery("reports", reportService.getReports);
  const { data: insights, refetch: refetchInsights } = useQuery(
    ["insights", selectedReport],
    () =>
      selectedReport ? aiService.getInsightsForReport(selectedReport) : [],
    { enabled: !!selectedReport }
  );

  const analysisTypes = [
    {
      value: "summary",
      label: "Summary",
      icon: FileText,
      description: "Generate executive summary",
    },
    {
      value: "trend",
      label: "Trends",
      icon: TrendingUp,
      description: "Identify trends and patterns",
    },
    {
      value: "anomaly",
      label: "Anomalies",
      icon: AlertTriangle,
      description: "Detect unusual patterns",
    },
    {
      value: "recommendation",
      label: "Recommendations",
      icon: Lightbulb,
      description: "Get actionable insights",
    },
  ];

  const handleAnalyze = async () => {
    if (!selectedReport) return;

    setIsAnalyzing(true);
    try {
      await aiService.analyzeReport(selectedReport, analysisType);
      await refetchInsights();
    } catch (error) {
      console.error("Analysis failed:", error);
    } finally {
      setIsAnalyzing(false);
    }
  };

  const getInsightIcon = (type) => {
    const insightType = analysisTypes.find((t) => t.value === type);
    return insightType ? insightType.icon : Brain;
  };

  const getInsightColor = (type) => {
    switch (type) {
      case "trend":
        return "bg-blue-100 text-blue-800";
      case "anomaly":
        return "bg-red-100 text-red-800";
      case "recommendation":
        return "bg-green-100 text-green-800";
      default:
        return "bg-gray-100 text-gray-800";
    }
  };

  return (
    <div className="space-y-6">
      {/* Header */}
      <div>
        <h1 className="text-2xl font-bold text-gray-900">AI Analysis</h1>
        <p className="text-gray-600">
          Leverage AI to gain insights from your reports
        </p>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        {/* Analysis Controls */}
        <div className="lg:col-span-1 space-y-6">
          <div className="card">
            <h3 className="text-lg font-semibold text-gray-900 mb-4">
              Select Report
            </h3>
            <select
              className="input"
              value={selectedReport || ""}
              onChange={(e) =>
                setSelectedReport(
                  e.target.value ? parseInt(e.target.value) : null
                )
              }
            >
              <option value="">Choose a report...</option>
              {reports?.map((report) => (
                <option key={report.id} value={report.id}>
                  {report.title}
                </option>
              ))}
            </select>
          </div>

          <div className="card">
            <h3 className="text-lg font-semibold text-gray-900 mb-4">
              Analysis Type
            </h3>
            <div className="space-y-3">
              {analysisTypes.map((type) => (
                <label
                  key={type.value}
                  className="flex items-center space-x-3 cursor-pointer"
                >
                  <input
                    type="radio"
                    name="analysisType"
                    value={type.value}
                    checked={analysisType === type.value}
                    onChange={(e) => setAnalysisType(e.target.value)}
                    className="text-primary-600 focus:ring-primary-500"
                  />
                  <div className="flex items-center space-x-2">
                    <type.icon className="h-4 w-4 text-gray-600" />
                    <div>
                      <p className="font-medium text-gray-900">{type.label}</p>
                      <p className="text-sm text-gray-600">
                        {type.description}
                      </p>
                    </div>
                  </div>
                </label>
              ))}
            </div>
          </div>

          <button
            onClick={handleAnalyze}
            disabled={!selectedReport || isAnalyzing}
            className="w-full btn btn-primary flex items-center justify-center"
          >
            <Brain className="h-4 w-4 mr-2" />
            {isAnalyzing ? "Analyzing..." : "Analyze Report"}
          </button>
        </div>

        {/* Results */}
        <div className="lg:col-span-2">
          <div className="card">
            <h3 className="text-lg font-semibold text-gray-900 mb-4">
              AI Insights
            </h3>

            {!selectedReport ? (
              <div className="text-center py-8">
                <Brain className="h-12 w-12 text-gray-400 mx-auto mb-4" />
                <p className="text-gray-600">
                  Select a report to view AI insights
                </p>
              </div>
            ) : insights && insights.length > 0 ? (
              <div className="space-y-4">
                {insights.map((insight, index) => {
                  const Icon = getInsightIcon(insight.insightType);
                  return (
                    <div
                      key={index}
                      className="border border-gray-200 rounded-lg p-4"
                    >
                      <div className="flex items-center justify-between mb-2">
                        <div className="flex items-center space-x-2">
                          <Icon className="h-5 w-5 text-gray-600" />
                          <h4 className="font-medium text-gray-900">
                            {insight.title}
                          </h4>
                        </div>
                        <span
                          className={`px-2 py-1 text-xs font-medium rounded-full ${getInsightColor(
                            insight.insightType
                          )}`}
                        >
                          {insight.insightType}
                        </span>
                      </div>
                      <p className="text-gray-700 mb-2">{insight.content}</p>
                      <div className="flex items-center justify-between text-sm text-gray-500">
                        <span>
                          Confidence:{" "}
                          {(insight.confidenceScore * 100).toFixed(1)}%
                        </span>
                        <span>
                          {new Date(insight.generatedAt).toLocaleDateString()}
                        </span>
                      </div>
                    </div>
                  );
                })}
              </div>
            ) : (
              <div className="text-center py-8">
                <Brain className="h-12 w-12 text-gray-400 mx-auto mb-4" />
                <p className="text-gray-600">
                  No insights available for this report
                </p>
                <p className="text-sm text-gray-500 mt-1">
                  Run an analysis to generate insights
                </p>
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
};

export default AIAnalysisPage;
