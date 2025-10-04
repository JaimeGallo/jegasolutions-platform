import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { FileText, Users, CheckCircle, Home, ArrowLeft } from "lucide-react";
import { toast } from "react-toastify";
import TemplateEditor from "../components/TemplateEditor/TemplateEditor";
import AreaAssignmentPanel from "../components/AreaAssignmentPanel";
import AreaAssignmentService from "../services/areaAssignmentService";

const HybridTemplateBuilderPage = () => {
  const navigate = useNavigate();
  const [currentStep, setCurrentStep] = useState(1);
  const [template, setTemplate] = useState(null);
  const [availableAreas, setAvailableAreas] = useState([]);
  const [areaAssignments, setAreaAssignments] = useState({});
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const steps = [
    { id: 1, name: "Crear Plantilla", icon: FileText },
    { id: 2, name: "Asignar Áreas", icon: Users },
    { id: 3, name: "Finalizar", icon: CheckCircle },
  ];

  useEffect(() => {
    loadAvailableAreas();
  }, []);

  const loadAvailableAreas = async () => {
    try {
      const areas = await AreaAssignmentService.getAvailableAreas();
      setAvailableAreas(areas);
    } catch (error) {
      console.error("Error cargando áreas:", error);
      setError("Error cargando áreas disponibles");
    }
  };

  const handleTemplateSave = (savedTemplate) => {
    setTemplate(savedTemplate);
    setCurrentStep(2);
    toast.success("✅ Plantilla creada exitosamente");
  };

  const handleAutoAssign = async (sections) => {
    try {
      const assignments = await AreaAssignmentService.autoAssignAreas(sections);
      return assignments;
    } catch (error) {
      toast.error("❌ Error en asignación automática: " + error.message);
      throw error;
    }
  };

  const handleManualAssign = () => {
    toast.info("💡 Modo de asignación manual activado");
  };

  const handleAssignmentsChange = (newAssignments) => {
    setAreaAssignments(newAssignments);
  };

  const handleFinalize = async () => {
    if (!template) {
      toast.error("❌ No hay plantilla para finalizar");
      return;
    }

    const validation = AreaAssignmentService.validateAssignments(
      areaAssignments,
      template.sections || []
    );

    if (!validation.isValid) {
      toast.error(
        `❌ ${validation.unassignedSections.length} secciones sin asignar`
      );
      return;
    }

    setLoading(true);
    try {
      // Guardar la plantilla con las asignaciones
      if (template.id) {
        await AreaAssignmentService.saveAreaAssignments(
          template.id,
          areaAssignments
        );
      }

      toast.success("✅ Plantilla finalizada exitosamente");
      navigate("/consolidated-templates");
    } catch (error) {
      toast.error("❌ Error finalizando plantilla: " + error.message);
    } finally {
      setLoading(false);
    }
  };

  const goToHome = () => {
    navigate("/");
  };

  const getCurrentStepContent = () => {
    switch (currentStep) {
      case 1:
        return (
          <div className="space-y-6">
            <div className="bg-blue-50 border border-blue-200 rounded-lg p-4">
              <h3 className="text-lg font-medium text-blue-900 mb-2">
                Paso 1: Crear Plantilla Manualmente
              </h3>
              <p className="text-blue-700">
                Usa el editor de plantillas para crear la estructura de tu
                reporte. Puedes agregar secciones, componentes y configurar el
                diseño.
              </p>
            </div>

            <TemplateEditor
              initialTemplate={template}
              onSave={handleTemplateSave}
              onCancel={() => navigate("/consolidated-templates")}
            />
          </div>
        );

      case 2:
        return (
          <div className="space-y-6">
            <div className="bg-green-50 border border-green-200 rounded-lg p-4">
              <h3 className="text-lg font-medium text-green-900 mb-2">
                Paso 2: Asignar Áreas de Responsabilidad
              </h3>
              <p className="text-green-700">
                Asigna cada sección de la plantilla a un área específica. Puedes
                usar asignación automática con IA o hacerlo manualmente.
              </p>
            </div>

            {template && template.sections && (
              <AreaAssignmentPanel
                sections={template.sections}
                availableAreas={availableAreas}
                onAssignmentsChange={handleAssignmentsChange}
                onAutoAssign={handleAutoAssign}
                onManualAssign={handleManualAssign}
              />
            )}

            <div className="flex justify-between">
              <button
                onClick={() => setCurrentStep(1)}
                className="px-4 py-2 text-gray-600 hover:text-gray-800 transition-colors"
              >
                ← Volver a Plantilla
              </button>

              <button
                onClick={() => setCurrentStep(3)}
                disabled={
                  Object.values(areaAssignments).filter((a) => a.areaId)
                    .length === 0
                }
                className="px-6 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
              >
                Continuar →
              </button>
            </div>
          </div>
        );

      case 3:
        return (
          <div className="space-y-6">
            <div className="bg-purple-50 border border-purple-200 rounded-lg p-4">
              <h3 className="text-lg font-medium text-purple-900 mb-2">
                Paso 3: Revisar y Finalizar
              </h3>
              <p className="text-purple-700">
                Revisa la plantilla y las asignaciones antes de finalizar.
              </p>
            </div>

            <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
              {/* Resumen de la plantilla */}
              <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
                <h4 className="font-semibold text-gray-900 mb-4">
                  Resumen de Plantilla
                </h4>
                {template && (
                  <div className="space-y-3">
                    <div>
                      <span className="text-sm text-gray-600">Nombre:</span>
                      <p className="font-medium">
                        {template.metadata?.description || "Sin nombre"}
                      </p>
                    </div>
                    <div>
                      <span className="text-sm text-gray-600">
                        Tipo:
                      </span>
                      <p className="text-sm">
                        {template.metadata?.type || "Sin tipo"}
                      </p>
                    </div>
                    <div>
                      <span className="text-sm text-gray-600">Secciones:</span>
                      <p className="font-medium">
                        {template.sections?.length || 0} secciones
                      </p>
                    </div>
                    <div>
                      <span className="text-sm text-gray-600">Componentes:</span>
                      <p className="font-medium">
                        {template.sections?.reduce((acc, section) => 
                          acc + (section.components?.length || 0), 0) || 0} componentes
                      </p>
                    </div>
                  </div>
                )}
              </div>

              {/* Resumen de asignaciones */}
              <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
                <h4 className="font-semibold text-gray-900 mb-4">
                  Resumen de Asignaciones
                </h4>
                <div className="space-y-3 max-h-96 overflow-y-auto">
                  {Object.entries(areaAssignments).map(
                    ([sectionId, assignment]) => {
                      const section = template?.sections?.find(
                        (s) => s.sectionId === sectionId
                      );
                      return (
                        <div
                          key={sectionId}
                          className="flex justify-between items-center p-2 bg-gray-50 rounded"
                        >
                          <div>
                            <p className="text-sm font-medium">
                              {section?.title || "Sección"}
                            </p>
                            <p className="text-xs text-gray-600">
                              {assignment.areaName}
                            </p>
                          </div>
                          {assignment.isAutoAssigned && (
                            <span className="text-xs bg-blue-100 text-blue-800 px-2 py-1 rounded">
                              IA
                            </span>
                          )}
                        </div>
                      );
                    }
                  )}
                </div>
              </div>
            </div>

            <div className="flex justify-between">
              <button
                onClick={() => setCurrentStep(2)}
                className="px-4 py-2 text-gray-600 hover:text-gray-800 transition-colors"
              >
                ← Volver a Asignaciones
              </button>

              <button
                onClick={handleFinalize}
                disabled={loading}
                className="px-6 py-2 bg-green-600 text-white rounded-md hover:bg-green-700 disabled:opacity-50 transition-colors flex items-center gap-2"
              >
                {loading ? (
                  <>
                    <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-white"></div>
                    Finalizando...
                  </>
                ) : (
                  <>
                    <CheckCircle className="w-4 h-4" />
                    Finalizar Plantilla
                  </>
                )}
              </button>
            </div>
          </div>
        );

      default:
        return null;
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-blue-50 to-white p-6">
      {/* Header con navegación simple */}
      <div className="max-w-7xl mx-auto mb-6">
        <div className="flex items-center justify-between">
          <button
            onClick={goToHome}
            className="flex items-center gap-2 px-4 py-2 text-gray-600 hover:text-gray-900 hover:bg-white rounded-lg transition-colors"
          >
            <ArrowLeft className="w-4 h-4" />
            Volver al Dashboard
          </button>
          <button
            onClick={goToHome}
            className="flex items-center gap-2 px-4 py-2 text-blue-600 hover:text-blue-700 hover:bg-blue-50 rounded-lg transition-colors"
          >
            <Home className="w-4 h-4" />
            Inicio
          </button>
        </div>
      </div>
      
      <div className="max-w-7xl mx-auto bg-white p-8 rounded-xl shadow-xl">
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-blue-800">
            Constructor Híbrido de Plantillas
          </h1>
          <p className="mt-2 text-gray-600">
            Crea plantillas manualmente y asigna áreas de responsabilidad a
            través de un proceso guiado.
          </p>
        </div>

        {/* Progress Steps */}
        <div className="mb-12">
          <nav aria-label="Progress">
            <ol className="flex items-center justify-center">
              {steps.map((step, stepIdx) => (
                <li
                  key={step.name}
                  className={`relative ${
                    stepIdx !== steps.length - 1 ? "flex-1" : ""
                  }`}
                >
                  {stepIdx !== steps.length - 1 && (
                    <div
                      className="absolute top-5 left-1/2 w-full h-0.5 -z-10"
                      style={{
                        backgroundColor:
                          step.id < currentStep ? "#2563eb" : "#e5e7eb",
                      }}
                    />
                  )}
                  <div className="flex flex-col items-center">
                    <div
                      className={`relative flex h-10 w-10 items-center justify-center rounded-full ${
                        step.id < currentStep
                          ? "bg-blue-600 hover:bg-blue-700"
                          : step.id === currentStep
                          ? "bg-blue-600 ring-4 ring-blue-100"
                          : "bg-gray-200"
                      } transition-all`}
                    >
                      <step.icon
                        className={`h-6 w-6 ${
                          step.id <= currentStep ? "text-white" : "text-gray-400"
                        }`}
                        aria-hidden="true"
                      />
                    </div>
                    <span
                      className={`mt-2 text-sm font-medium ${
                        step.id <= currentStep
                          ? "text-blue-600"
                          : "text-gray-500"
                      }`}
                    >
                      {step.name}
                    </span>
                  </div>
                </li>
              ))}
            </ol>
          </nav>
        </div>

        {/* Error Display */}
        {error && (
          <div className="mb-6 bg-red-50 border border-red-200 rounded-lg p-4">
            <p className="text-red-800">{error}</p>
          </div>
        )}

        {/* Step Content */}
        {getCurrentStepContent()}
      </div>
    </div>
  );
};

export default HybridTemplateBuilderPage;

