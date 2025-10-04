import { useState, useEffect } from "react";
import { Check, X, Sparkles, Hand } from "lucide-react";

const AreaAssignmentPanel = ({
  sections,
  availableAreas,
  onAssignmentsChange,
  onAutoAssign,
  onManualAssign,
}) => {
  const [assignments, setAssignments] = useState({});
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    // Inicializar asignaciones vacías
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
  }, [sections]);

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

  return (
    <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
      <div className="flex items-center justify-between mb-6">
        <h3 className="text-lg font-semibold text-gray-900">
          Asignación de Áreas
        </h3>
        <div className="flex space-x-2">
          <button
            onClick={handleAutoAssign}
            disabled={loading}
            className="flex items-center px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 disabled:opacity-50 transition-colors"
          >
            <Sparkles className="w-4 h-4 mr-2" />
            {loading ? "Asignando..." : "Asignación Automática"}
          </button>
          <button
            onClick={() => onManualAssign(assignments)}
            className="flex items-center px-4 py-2 bg-gray-600 text-white rounded-md hover:bg-gray-700 transition-colors"
          >
            <Hand className="w-4 h-4 mr-2" />
            Asignación Manual
          </button>
        </div>
      </div>

      <div className="space-y-4">
        {sections.map((section) => (
          <div
            key={section.sectionId}
            className="border border-gray-200 rounded-lg p-4 hover:bg-gray-50 transition-colors"
          >
            <div className="flex items-center justify-between">
              <div className="flex-1">
                <h4 className="font-medium text-gray-900">{section.title}</h4>
                <p className="text-sm text-gray-600 mt-1">
                  {section.content?.substring(0, 100)}...
                </p>
              </div>

              <div className="flex items-center space-x-3">
                <select
                  value={assignments[section.sectionId]?.areaId || ""}
                  onChange={(e) =>
                    handleManualAssign(section.sectionId, e.target.value)
                  }
                  className="px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                >
                  <option value="">Seleccionar área</option>
                  {availableAreas.map((area) => (
                    <option key={area.id} value={area.id}>
                      {area.name}
                    </option>
                  ))}
                </select>

                {assignments[section.sectionId]?.areaId && (
                  <div className="flex items-center space-x-2">
                    {assignments[section.sectionId]?.isAutoAssigned && (
                      <span className="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium bg-blue-100 text-blue-800">
                        <Sparkles className="w-3 h-3 mr-1" />
                        IA
                      </span>
                    )}

                    {assignments[section.sectionId]?.confidence > 0 && (
                      <span className="text-sm text-gray-500">
                        {Math.round(assignments[section.sectionId]?.confidence * 100)}%
                      </span>
                    )}

                    <button
                      onClick={() => clearAssignment(section.sectionId)}
                      className="text-red-600 hover:text-red-800 transition-colors"
                    >
                      <X className="w-4 h-4" />
                    </button>
                  </div>
                )}
              </div>
            </div>

            {assignments[section.sectionId]?.areaName && (
              <div className="mt-2 flex items-center text-sm text-green-600">
                <Check className="w-4 h-4 mr-1" />
                Asignado a:{" "}
                <span className="font-medium ml-1">
                  {assignments[section.sectionId].areaName}
                </span>
              </div>
            )}
          </div>
        ))}
      </div>

      <div className="mt-6 pt-4 border-t border-gray-200">
        <div className="flex justify-between items-center">
          <div className="text-sm text-gray-600">
            {Object.values(assignments).filter((a) => a.areaId).length} de{" "}
            {sections.length} secciones asignadas
          </div>
          <div className="flex space-x-2 text-xs text-gray-500">
            <span className="flex items-center">
              <Sparkles className="w-3 h-3 mr-1" />
              Asignación automática
            </span>
            <span className="flex items-center">
              <Hand className="w-3 h-3 mr-1" />
              Asignación manual
            </span>
          </div>
        </div>
      </div>
    </div>
  );
};

export default AreaAssignmentPanel;

