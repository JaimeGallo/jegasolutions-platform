import { useParams, useNavigate } from "react-router-dom";
import { useQuery } from "react-query";
import { ArrowLeft, Home, User, Search } from "lucide-react";
import { templateService } from "../services/templateService";
import { toast } from "react-toastify";
import { useTenant } from "../contexts/TenantContext";
import TemplateEditorOptimized from "../components/TemplateEditor/TemplateEditorOptimized";

const TemplateEditorPage = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const { tenantName } = useTenant();
  const isEditing = !!id;

  const { data: template, isLoading } = useQuery(
    ["template", id],
    () => templateService.getTemplate(id),
    { enabled: isEditing }
  );

  const handleSave = async (templateData) => {
    try {
      // Prepare data for API
      const dataToSave = {
        name: templateData.metadata?.description || "New Template",
        areaId: null,
        configuration: templateData,
      };

      if (isEditing) {
        await templateService.updateTemplate(id, dataToSave);
        toast.success("✅ Plantilla actualizada exitosamente");
      } else {
        await templateService.createTemplate(dataToSave);
        toast.success("✅ Plantilla creada exitosamente");
      }
      navigate("/templates");
    } catch (error) {
      console.error("Error saving template:", error);
      toast.error("❌ Error al guardar la plantilla");
    }
  };

  const handleCancel = () => {
    navigate("/templates");
  };

  if (isLoading) {
    return (
      <div className="flex items-center justify-center min-h-screen bg-gray-50">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50 flex flex-col">
      {/* Header Navigation - Compact */}
      <header className="bg-white border-b border-gray-200 shadow-sm sticky top-0 z-50">
        <div className="px-4 py-3">
          <div className="flex items-center justify-between gap-4">
            {/* Left: Logo + Navigation */}
            <div className="flex items-center gap-4">
              {/* Back Button */}
              <button
                onClick={() => navigate("/templates")}
                className="flex items-center gap-2 px-3 py-2 text-gray-600 hover:text-gray-900 hover:bg-gray-100 rounded-lg transition-colors"
                title="Volver a Templates"
              >
                <ArrowLeft className="w-5 h-5" />
                <span className="hidden sm:inline font-medium">Volver</span>
              </button>

              {/* Divider */}
              <div className="h-8 w-px bg-gray-300"></div>

              {/* Logo + Title */}
              <div className="flex items-center gap-3">
                <h1 className="text-xl font-bold text-gray-900">Report Builder</h1>
                <span className="hidden md:inline text-sm text-gray-500">
                  / {isEditing ? "Editar Template" : "Nuevo Template"}
                </span>
              </div>
            </div>

            {/* Center: Search Bar */}
            <div className="flex-1 max-w-md hidden lg:block">
              <div className="relative">
                <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 w-4 h-4 text-gray-400" />
                <input
                  type="text"
                  placeholder="Buscar reports, templates..."
                  className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent text-sm"
                />
              </div>
            </div>

            {/* Right: User Info + Actions */}
            <div className="flex items-center gap-3">
              {/* Tenant Info */}
              <div className="hidden md:flex flex-col items-end">
                <span className="text-sm font-medium text-gray-900">{tenantName}</span>
                <span className="text-xs text-gray-500">Template Editor</span>
              </div>

              {/* Divider */}
              <div className="h-8 w-px bg-gray-300"></div>

              {/* Home Button */}
              <button
                onClick={() => navigate("/")}
                className="flex items-center gap-2 px-3 py-2 text-blue-600 hover:text-blue-700 hover:bg-blue-50 rounded-lg transition-colors"
                title="Ir al Dashboard"
              >
                <Home className="w-5 h-5" />
                <span className="hidden sm:inline font-medium">Dashboard</span>
              </button>

              {/* User Icon */}
              <button className="p-2 text-gray-600 hover:text-gray-900 hover:bg-gray-100 rounded-lg transition-colors">
                <User className="w-5 h-5" />
              </button>
            </div>
          </div>
        </div>
      </header>

      {/* Template Editor - Full Width Optimized */}
      <main className="flex-1 overflow-hidden">
        <TemplateEditorOptimized
          initialTemplate={template?.configuration}
          onSave={handleSave}
          onCancel={handleCancel}
        />
      </main>
    </div>
  );
};

export default TemplateEditorPage;
