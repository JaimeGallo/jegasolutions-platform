import { useState } from "react";
import { Sparkles, Hand, Check, X, ArrowLeft, ArrowRight, Eye } from "lucide-react";
import LivePreview from "./TemplateEditor/LivePreview";

const AreaAssignmentOptimized = ({
  sections,
  availableAreas,
  assignments: initialAssignments,
  onAssignmentsChange,
  onAutoAssign,
  onBack,
  onNext,
  canProceed,
}) => {
  const [assignments, setAssignments] = useState(initialAssignments || {});
  const [loading, setLoading] = useState(false);
  const [showPreview, setShowPreview] = useState(true);

  // Initialize assignments if empty
  useState(() => {
    if (Object.keys(assignments).length === 0 && sections.length > 0) {
      const initialAssignments = {};
      sections.forEach((section) => {
        initialAssignments[section.sectionId] = {
          areaId: null,
          areaName: "",
          confidence: 0,
          isAutoAssigned: false,
        };
      });
      setAssignments(initialAssignments);
      onAssignmentsChange(initialAssignments);
    }
  });

  const handleAutoAssign = async () => {
    setLoading(true);
    try {
      const autoAssignments = await onAutoAssign(sections);
      setAssignments(autoAssignments);
      onAssignmentsChange(autoAssignments);
    } catch (error) {
      console.error("Error en asignación automática:", error);
    } finally {
      setLoading(false);
    }
  };

  const handleManualAssign = (sectionId, areaId) => {
    const area = availableAreas.find((a) => a.id === parseInt(areaId));
    const newAssignments = {
      ...assignments,
      [sectionId]: {
        areaId: parseInt(areaId),
        areaName: area ? area.name : "",
        confidence: 1.0,
        isAutoAssigned: false,
      },
    };
    setAssignments(newAssignments);
    onAssignmentsChange(newAssignments);
  };

  const clearAssignment = (sectionId) => {
    const newAssignments = {
      ...assignments,
      [sectionId]: {
        areaId: null,
        areaName: "",
        confidence: 0,
        isAutoAssigned: false,
      },
    };
    setAssignments(newAssignments);
    onAssignmentsChange(newAssignments);
  };

  const assignedCount = Object.values(assignments).filter((a) => a.areaId).length;
  const template = { sections, metadata: { description: "Vista Previa" } };

  return (
    <div className="flex h-full bg-gray-50">
      {/* Assignment Area */}
      <div className="w-1/2 flex flex-col overflow-hidden">
        {/* Header with Actions */}
        <div className="bg-white border-b border-gray-200 p-4">
          <div className="flex items-center justify-between mb-4">
            <div>
              <h2 className="text-xl font-bold text-gray-900">
                Asignar Áreas de Responsabilidad
              </h2>
              <p className="text-sm text-gray-600 mt-1">
                Asigna cada sección a un área específica con IA o manualmente
              </p>
            </div>

            <div className="flex items-center gap-2">
              <button
                onClick={handleAutoAssign}
                disabled={loading}
                className="flex items-center gap-2 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50 transition-colors shadow"
              >
                <Sparkles className="w-4 h-4" />
                {loading ? "Asignando..." : "Asignación Automática (IA)"}
              </button>

              <button
                onClick={() => setShowPreview(!showPreview)}
                className={`flex items-center gap-2 px-4 py-2 rounded-lg transition-colors ${
                  showPreview
                    ? "bg-blue-600 text-white"
                    : "border border-gray-300 hover:bg-gray-100"
                }`}
              >
                <Eye className="w-4 h-4" />
                Vista Previa
              </button>
            </div>
          </div>

          {/* Progress */}
          <div className="flex items-center gap-3">
            <div className="flex-1 bg-gray-200 rounded-full h-2">
              <div
                className="bg-green-600 h-2 rounded-full transition-all"
                style={{ width: `${(assignedCount / sections.length) * 100}%` }}
              />
            </div>
            <span className="text-sm font-medium text-gray-700 whitespace-nowrap">
              {assignedCount} de {sections.length} secciones asignadas
            </span>
          </div>
        </div>

        {/* Sections List - Scrollable */}
        <div className="flex-1 p-6 overflow-y-auto">
          <div className="space-y-3 max-w-4xl">
            {sections.map((section) => {
              const assignment = assignments[section.sectionId];
              const hasAssignment = assignment?.areaId;

              return (
                <div
                  key={section.sectionId}
                  className={`border rounded-lg p-4 transition-all ${
                    hasAssignment
                      ? "border-green-300 bg-green-50"
                      : "border-gray-300 bg-white hover:border-blue-300"
                  }`}
                >
                  <div className="flex items-start justify-between gap-4">
                    {/* Section Info */}
                    <div className="flex-1 min-w-0">
                      <h3 className="font-semibold text-gray-900 mb-1">
                        {section.title || "Sección sin título"}
                      </h3>
                      {section.components && section.components.length > 0 && (
                        <p className="text-sm text-gray-600">
                          {section.components.length} componente(s)
                        </p>
                      )}
                    </div>

                    {/* Area Selector */}
                    <div className="flex items-center gap-3">
                      <select
                        value={assignment?.areaId || ""}
                        onChange={(e) =>
                          handleManualAssign(section.sectionId, e.target.value)
                        }
                        className={`px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 min-w-[200px] ${
                          hasAssignment
                            ? "border-green-500 bg-white"
                            : "border-gray-300"
                        }`}
                      >
                        <option value="">Seleccionar área...</option>
                        {availableAreas.map((area) => (
                          <option key={area.id} value={area.id}>
                            {area.name}
                          </option>
                        ))}
                      </select>

                      {hasAssignment && (
                        <button
                          onClick={() => clearAssignment(section.sectionId)}
                          className="p-2 text-red-600 hover:bg-red-50 rounded-lg transition-colors"
                          title="Limpiar asignación"
                        >
                          <X className="w-4 h-4" />
                        </button>
                      )}
                    </div>
                  </div>

                  {/* Assignment Info */}
                  {hasAssignment && (
                    <div className="mt-3 flex items-center gap-3 pt-3 border-t border-green-200">
                      <div className="flex items-center gap-2 text-sm text-green-700">
                        <Check className="w-4 h-4" />
                        <span className="font-medium">
                          Asignado a: {assignment.areaName}
                        </span>
                      </div>

                      {assignment.isAutoAssigned && (
                        <span className="inline-flex items-center gap-1 px-2 py-1 bg-blue-100 text-blue-800 text-xs rounded-full">
                          <Sparkles className="w-3 h-3" />
                          Asignación IA
                        </span>
                      )}

                      {assignment.confidence > 0 && assignment.isAutoAssigned && (
                        <span className="text-xs text-gray-600">
                          Confianza: {Math.round(assignment.confidence * 100)}%
                        </span>
                      )}

                      {!assignment.isAutoAssigned && (
                        <span className="inline-flex items-center gap-1 px-2 py-1 bg-gray-100 text-gray-700 text-xs rounded-full">
                          <Hand className="w-3 h-3" />
                          Manual
                        </span>
                      )}
                    </div>
                  )}
                </div>
              );
            })}
          </div>
        </div>

        {/* Navigation Footer */}
        <div className="bg-white border-t border-gray-200 p-4">
          <div className="flex items-center justify-between max-w-4xl">
            <button
              onClick={onBack}
              className="flex items-center gap-2 px-4 py-2 border border-gray-300 rounded-lg hover:bg-gray-100 transition-colors"
            >
              <ArrowLeft className="w-4 h-4" />
              Volver a Plantilla
            </button>

            <button
              onClick={onNext}
              disabled={!canProceed}
              className="flex items-center gap-2 px-6 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors shadow font-medium"
            >
              Continuar a Revisión
              <ArrowRight className="w-4 h-4" />
            </button>
          </div>
        </div>
      </div>

      {/* Live Preview */}
      {showPreview && (
        <div className="w-1/2 border-l border-gray-200 bg-white">
          <LivePreview template={template} />
        </div>
      )}
    </div>
  );
};

export default AreaAssignmentOptimized;

