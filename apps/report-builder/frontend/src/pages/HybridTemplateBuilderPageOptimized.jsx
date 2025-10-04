import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { FileText, Users, CheckCircle, ArrowLeft, ArrowRight, Home } from "lucide-react";
import { toast } from "react-toastify";
import TemplateEditorOptimized from "../components/TemplateEditor/TemplateEditorOptimized";
import AreaAssignmentOptimized from "../components/AreaAssignmentOptimized";
import FinalizationPanel from "../components/FinalizationPanel";
import AreaAssignmentService from "../services/areaAssignmentService";

const HybridTemplateBuilderPageOptimized = () => {
  const navigate = useNavigate();
  const [currentStep, setCurrentStep] = useState(1);
  const [template, setTemplate] = useState(null);
  const [availableAreas, setAvailableAreas] = useState([]);
  const [areaAssignments, setAreaAssignments] = useState({});
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const steps = [
    { id: 1, name: "Crear Plantilla", icon: FileText, description: "Diseña tu template visualmente" },
    { id: 2, name: "Asignar Áreas", icon: Users, description: "Asigna responsables a cada sección" },
    { id: 3, name: "Finalizar", icon: CheckCircle, description: "Revisa y guarda" },
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

  const handleTemplateCancel = () => {
    if (confirm("¿Salir sin guardar?")) {
      navigate("/consolidated-templates");
    }
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

  const canProceedToStep = (step) => {
    if (step === 2) return template !== null;
    if (step === 3) return Object.values(areaAssignments).filter(a => a.areaId).length > 0;
    return true;
  };

  const getCurrentStepContent = () => {
    switch (currentStep) {
      case 1:
        return (
          <TemplateEditorOptimized
            initialTemplate={template}
            onSave={handleTemplateSave}
            onCancel={handleTemplateCancel}
          />
        );

      case 2:
        return (
          <AreaAssignmentOptimized
            sections={template?.sections || []}
            availableAreas={availableAreas}
            assignments={areaAssignments}
            onAssignmentsChange={handleAssignmentsChange}
            onAutoAssign={handleAutoAssign}
            onBack={() => setCurrentStep(1)}
            onNext={() => setCurrentStep(3)}
            canProceed={canProceedToStep(3)}
          />
        );

      case 3:
        return (
          <FinalizationPanel
            template={template}
            areaAssignments={areaAssignments}
            onBack={() => setCurrentStep(2)}
            onFinalize={handleFinalize}
            loading={loading}
          />
        );

      default:
        return null;
    }
  };

  return (
    <div className="min-h-screen bg-gray-50 flex flex-col">
      {/* Header with Progress */}
      <header className="bg-white border-b border-gray-200 shadow-sm sticky top-0 z-50">
        <div className="px-6 py-4">
          <div className="flex items-center justify-between mb-4">
            {/* Left: Title + Home */}
            <div className="flex items-center gap-4">
              <button
                onClick={goToHome}
                className="flex items-center gap-2 px-3 py-2 text-gray-600 hover:text-gray-900 hover:bg-gray-100 rounded-lg transition-colors"
              >
                <ArrowLeft className="w-5 h-5" />
                <span className="hidden sm:inline">Volver</span>
              </button>
              <div className="h-8 w-px bg-gray-300"></div>
              <div>
                <h1 className="text-2xl font-bold text-blue-800">
                  Constructor Híbrido de Plantillas
                </h1>
                <p className="text-sm text-gray-600">
                  Crea plantillas y asigna áreas de responsabilidad
                </p>
              </div>
            </div>

            {/* Right: Home Button */}
            <button
              onClick={goToHome}
              className="flex items-center gap-2 px-4 py-2 text-blue-600 hover:text-blue-700 hover:bg-blue-50 rounded-lg transition-colors"
            >
              <Home className="w-5 h-5" />
              <span className="hidden sm:inline">Dashboard</span>
            </button>
          </div>

          {/* Progress Steps */}
          <div className="flex items-center justify-center gap-4">
            {steps.map((step, index) => {
              const isActive = step.id === currentStep;
              const isCompleted = step.id < currentStep;
              const canAccess = canProceedToStep(step.id);

              return (
                <div key={step.id} className="flex items-center">
                  {/* Step */}
                  <button
                    onClick={() => canAccess && setCurrentStep(step.id)}
                    disabled={!canAccess}
                    className={`flex items-center gap-3 px-4 py-3 rounded-lg transition-all ${
                      isActive
                        ? "bg-blue-600 text-white shadow-lg ring-4 ring-blue-100"
                        : isCompleted
                        ? "bg-green-100 text-green-800 hover:bg-green-200"
                        : "bg-gray-100 text-gray-500 cursor-not-allowed"
                    }`}
                  >
                    <div className={`p-2 rounded-full ${
                      isActive ? "bg-blue-500" : isCompleted ? "bg-green-600" : "bg-gray-300"
                    }`}>
                      <step.icon className="w-5 h-5 text-white" />
                    </div>
                    <div className="text-left">
                      <div className="font-semibold">{step.name}</div>
                      <div className={`text-xs ${isActive ? "text-blue-100" : "text-gray-500"}`}>
                        {step.description}
                      </div>
                    </div>
                    {isCompleted && (
                      <CheckCircle className="w-5 h-5 text-green-600" />
                    )}
                  </button>

                  {/* Connector */}
                  {index < steps.length - 1 && (
                    <ArrowRight className={`w-6 h-6 mx-2 ${
                      step.id < currentStep ? "text-green-600" : "text-gray-300"
                    }`} />
                  )}
                </div>
              );
            })}
          </div>
        </div>

        {/* Error Display */}
        {error && (
          <div className="px-6 pb-4">
            <div className="bg-red-50 border border-red-200 rounded-lg p-3">
              <p className="text-red-800 text-sm">{error}</p>
            </div>
          </div>
        )}
      </header>

      {/* Step Content - Full Height */}
      <main className="flex-1 overflow-hidden">
        {getCurrentStepContent()}
      </main>
    </div>
  );
};

export default HybridTemplateBuilderPageOptimized;

