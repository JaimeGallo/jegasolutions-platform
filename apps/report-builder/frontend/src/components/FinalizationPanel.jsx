import { ArrowLeft, CheckCircle, Sparkles, Hand, Loader2, AlertCircle } from "lucide-react";
import LivePreview from "./TemplateEditor/LivePreview";

const FinalizationPanel = ({
  template,
  areaAssignments,
  onBack,
  onFinalize,
  loading,
}) => {
  const assignedCount = Object.values(areaAssignments).filter((a) => a.areaId).length;
  const totalSections = template?.sections?.length || 0;
  const totalComponents = template?.sections?.reduce(
    (acc, s) => acc + (s.components?.length || 0),
    0
  ) || 0;

  return (
    <div className="flex h-full bg-gray-50">
      {/* Summary Panel */}
      <div className="flex-1 flex flex-col overflow-hidden">
        {/* Header */}
        <div className="bg-white border-b border-gray-200 p-6">
          <h2 className="text-2xl font-bold text-gray-900 mb-2">
            Revisión y Finalización
          </h2>
          <p className="text-gray-600">
            Revisa la plantilla y las asignaciones antes de finalizar
          </p>
        </div>

        {/* Content - Scrollable */}
        <div className="flex-1 p-6 overflow-y-auto">
          <div className="max-w-4xl space-y-6">
            {/* Template Summary Card */}
            <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
              <div className="flex items-center gap-2 mb-4">
                <div className="p-2 bg-blue-100 rounded-lg">
                  <CheckCircle className="w-6 h-6 text-blue-600" />
                </div>
                <h3 className="text-lg font-semibold text-gray-900">
                  Resumen de Plantilla
                </h3>
              </div>

              <div className="grid grid-cols-2 gap-6">
                <div>
                  <label className="text-sm text-gray-600 font-medium">Nombre:</label>
                  <p className="text-lg font-semibold text-gray-900 mt-1">
                    {template?.metadata?.description || "Sin nombre"}
                  </p>
                </div>

                <div>
                  <label className="text-sm text-gray-600 font-medium">Tipo:</label>
                  <p className="text-lg font-semibold text-gray-900 mt-1">
                    {template?.metadata?.type || "Genérica"}
                  </p>
                </div>

                <div>
                  <label className="text-sm text-gray-600 font-medium">Secciones:</label>
                  <p className="text-3xl font-bold text-blue-600 mt-1">
                    {totalSections}
                  </p>
                </div>

                <div>
                  <label className="text-sm text-gray-600 font-medium">Componentes:</label>
                  <p className="text-3xl font-bold text-purple-600 mt-1">
                    {totalComponents}
                  </p>
                </div>
              </div>
            </div>

            {/* Assignments Summary Card */}
            <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
              <div className="flex items-center gap-2 mb-4">
                <div className="p-2 bg-green-100 rounded-lg">
                  <CheckCircle className="w-6 h-6 text-green-600" />
                </div>
                <h3 className="text-lg font-semibold text-gray-900">
                  Resumen de Asignaciones
                </h3>
              </div>

              {/* Progress */}
              <div className="mb-6">
                <div className="flex justify-between items-center mb-2">
                  <span className="text-sm font-medium text-gray-700">
                    Progreso de asignación
                  </span>
                  <span className="text-sm font-bold text-green-600">
                    {assignedCount} / {totalSections}
                  </span>
                </div>
                <div className="w-full bg-gray-200 rounded-full h-3">
                  <div
                    className="bg-green-600 h-3 rounded-full transition-all"
                    style={{ width: `${(assignedCount / totalSections) * 100}%` }}
                  />
                </div>
              </div>

              {/* Assignments List */}
              <div className="space-y-3 max-h-96 overflow-y-auto">
                {Object.entries(areaAssignments).map(([sectionId, assignment]) => {
                  const section = template?.sections?.find(
                    (s) => s.sectionId === sectionId
                  );

                  return (
                    <div
                      key={sectionId}
                      className="flex items-center justify-between p-3 bg-gray-50 rounded-lg border border-gray-200"
                    >
                      <div className="flex-1">
                        <p className="font-medium text-gray-900">
                          {section?.title || "Sección"}
                        </p>
                        <p className="text-sm text-gray-600 mt-1">
                          → {assignment.areaName || "Sin asignar"}
                        </p>
                      </div>

                      <div className="flex items-center gap-2">
                        {assignment.isAutoAssigned ? (
                          <span className="inline-flex items-center gap-1 px-3 py-1 bg-blue-100 text-blue-800 text-xs font-medium rounded-full">
                            <Sparkles className="w-3 h-3" />
                            IA {assignment.confidence && `(${Math.round(assignment.confidence * 100)}%)`}
                          </span>
                        ) : (
                          <span className="inline-flex items-center gap-1 px-3 py-1 bg-gray-100 text-gray-700 text-xs font-medium rounded-full">
                            <Hand className="w-3 h-3" />
                            Manual
                          </span>
                        )}
                      </div>
                    </div>
                  );
                })}
              </div>
            </div>

            {/* Validation Message */}
            {assignedCount === totalSections ? (
              <div className="bg-green-50 border border-green-200 rounded-lg p-4">
                <div className="flex items-center gap-2">
                  <CheckCircle className="w-5 h-5 text-green-600 flex-shrink-0" />
                  <div>
                    <p className="font-semibold text-green-900">
                      ¡Todo listo para finalizar!
                    </p>
                    <p className="text-sm text-green-700 mt-1">
                      Todas las secciones han sido asignadas correctamente
                    </p>
                  </div>
                </div>
              </div>
            ) : (
              <div className="bg-yellow-50 border border-yellow-200 rounded-lg p-4">
                <div className="flex items-center gap-2">
                  <AlertCircle className="w-5 h-5 text-yellow-600 flex-shrink-0" />
                  <div>
                    <p className="font-semibold text-yellow-900">
                      Atención: Faltan asignaciones
                    </p>
                    <p className="text-sm text-yellow-700 mt-1">
                      {totalSections - assignedCount} sección(es) sin asignar. Vuelve al paso anterior para completar.
                    </p>
                  </div>
                </div>
              </div>
            )}
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
              Volver a Asignaciones
            </button>

            <button
              onClick={onFinalize}
              disabled={loading || assignedCount !== totalSections}
              className="flex items-center gap-2 px-8 py-3 bg-green-600 text-white rounded-lg hover:bg-green-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors shadow-lg font-semibold"
            >
              {loading ? (
                <>
                  <Loader2 className="w-5 h-5 animate-spin" />
                  Finalizando...
                </>
              ) : (
                <>
                  <CheckCircle className="w-5 h-5" />
                  Finalizar Plantilla
                </>
              )}
            </button>
          </div>
        </div>
      </div>

      {/* Live Preview */}
      <div className="w-1/2 min-w-[500px] border-l border-gray-200 bg-white">
        <LivePreview template={template} />
      </div>
    </div>
  );
};

export default FinalizationPanel;

