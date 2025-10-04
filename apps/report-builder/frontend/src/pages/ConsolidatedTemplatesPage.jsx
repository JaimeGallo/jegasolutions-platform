// Versión Mejorada con Tarjetas Visuales, Filtros Avanzados, y Paginación

import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { toast } from "react-toastify";
import { 
  Plus, 
  Search, 
  Filter,
  RefreshCw,
  Grid,
  List,
  ChevronLeft,
  ChevronRight
} from "lucide-react";
import { ConsolidatedTemplateService } from "../services/consolidatedTemplateService";
import TemplateCard from "../components/ConsolidatedTemplates/TemplateCard";
import { TemplateListItem } from "../components/ConsolidatedTemplates/TemplateListItem";
import { FilterPanel } from "../components/ConsolidatedTemplates/FilterPanel";
import { TemplateDetailModal } from "../components/ConsolidatedTemplates/TemplateDetailModal";
import { CreateTemplateModal } from "../components/ConsolidatedTemplates/CreateTemplateModal";

const ConsolidatedTemplatesPage = () => {
  const navigate = useNavigate();
  
  // Estados principales
  const [templates, setTemplates] = useState([]);
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);
  
  // Estados de modales
  const [selectedTemplate, setSelectedTemplate] = useState(null);
  const [showDetailsModal, setShowDetailsModal] = useState(false);
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [showFilterPanel, setShowFilterPanel] = useState(false);
  
  // Estados de vista y filtros
  const [viewMode, setViewMode] = useState("grid"); // "grid" o "list"
  const [filters, setFilters] = useState({
    search: "",
    status: "all",
    sortBy: "recent",
    dateFrom: null,
    dateTo: null,
  });
  
  // Estados de paginación
  const [currentPage, setCurrentPage] = useState(1);
  const [itemsPerPage] = useState(9); // 9 items para grid 3x3

  useEffect(() => {
    loadTemplates();
  }, []);

  const loadTemplates = async () => {
    try {
      setLoading(true);
      const data = await ConsolidatedTemplateService.getConsolidatedTemplates();
      setTemplates(data || []);
    } catch (err) {
      console.error("Error cargando plantillas:", err);
      toast.error("Error al cargar las plantillas consolidadas");
      setTemplates([]);
    } finally {
      setLoading(false);
    }
  };

  const handleRefresh = async () => {
    setRefreshing(true);
    await loadTemplates();
    setRefreshing(false);
    toast.success("Plantillas actualizadas");
  };

  const handleViewDetails = async (template) => {
    try {
      const fullTemplate = await ConsolidatedTemplateService.getConsolidatedTemplate(template.id);
      setSelectedTemplate(fullTemplate);
      setShowDetailsModal(true);
    } catch (err) {
      console.error("Error obteniendo detalles:", err);
      toast.error("Error al obtener los detalles de la plantilla");
    }
  };

  const handleDeleteTemplate = async (templateId) => {
    if (!window.confirm("¿Estás seguro de que quieres eliminar esta plantilla consolidada?")) {
      return;
    }

    try {
      await ConsolidatedTemplateService.deleteConsolidatedTemplate(templateId);
      toast.success("Plantilla eliminada exitosamente");
      await loadTemplates();
    } catch (err) {
      console.error("Error eliminando plantilla:", err);
      toast.error("Error al eliminar la plantilla");
    }
  };

  const handleCreateTemplate = async (templateData) => {
    try {
      await ConsolidatedTemplateService.createFromReports(templateData);
      toast.success("Plantilla creada exitosamente");
      setShowCreateModal(false);
      await loadTemplates();
    } catch (err) {
      console.error("Error creando plantilla:", err);
      toast.error("Error al crear la plantilla");
      throw err;
    }
  };

  // Filtrado y ordenamiento
  const getFilteredAndSortedTemplates = () => {
    let filtered = [...templates];

    // Filtro de búsqueda
    if (filters.search) {
      const searchLower = filters.search.toLowerCase();
      filtered = filtered.filter(
        (t) =>
          t.name?.toLowerCase().includes(searchLower) ||
          t.description?.toLowerCase().includes(searchLower) ||
          t.period?.toLowerCase().includes(searchLower)
      );
    }

    // Filtro de estado
    if (filters.status !== "all") {
      filtered = filtered.filter((t) => t.status === filters.status);
    }

    // Filtro de fecha
    if (filters.dateFrom) {
      filtered = filtered.filter(
        (t) => new Date(t.createdAt) >= new Date(filters.dateFrom)
      );
    }
    if (filters.dateTo) {
      filtered = filtered.filter(
        (t) => new Date(t.createdAt) <= new Date(filters.dateTo)
      );
    }

    // Ordenamiento
    switch (filters.sortBy) {
      case "recent":
        filtered.sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt));
        break;
      case "oldest":
        filtered.sort((a, b) => new Date(a.createdAt) - new Date(b.createdAt));
        break;
      case "name":
        filtered.sort((a, b) => (a.name || "").localeCompare(b.name || ""));
        break;
      case "progress":
        filtered.sort((a, b) => (b.progress || 0) - (a.progress || 0));
        break;
      default:
        break;
    }

    return filtered;
  };

  // Paginación
  const filteredTemplates = getFilteredAndSortedTemplates();
  const totalPages = Math.ceil(filteredTemplates.length / itemsPerPage);
  const startIndex = (currentPage - 1) * itemsPerPage;
  const paginatedTemplates = filteredTemplates.slice(
    startIndex,
    startIndex + itemsPerPage
  );

  // Reset página cuando cambian filtros
  useEffect(() => {
    setCurrentPage(1);
  }, [filters]);

  if (loading) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-blue-50 via-white to-purple-50 p-6">
        <div className="max-w-7xl mx-auto">
          <div className="flex items-center justify-center h-64">
            <div className="text-center">
              <div className="animate-spin rounded-full h-16 w-16 border-b-4 border-blue-600 mx-auto"></div>
              <p className="mt-4 text-gray-600 text-lg">
                Cargando plantillas consolidadas...
              </p>
            </div>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-blue-50 via-white to-purple-50 p-6">
      <div className="max-w-7xl mx-auto">
        {/* Header */}
        <div className="mb-8">
          <div className="flex items-center justify-between flex-wrap gap-4">
            <div>
              <h1 className="text-4xl font-bold bg-gradient-to-r from-blue-600 to-purple-600 bg-clip-text text-transparent mb-2">
                Plantillas Consolidadas
              </h1>
              <p className="text-gray-600 text-lg">
                Gestiona tus plantillas de informes multi-área con IA
              </p>
            </div>

            <div className="flex items-center gap-3">
              <button
                onClick={() => navigate("/hybrid-builder")}
                className="flex items-center gap-2 px-4 py-2 bg-white border-2 border-purple-600 text-purple-600 rounded-lg hover:bg-purple-50 transition-all shadow-sm"
              >
                <Plus className="w-5 h-5" />
                <span className="hidden sm:inline">Constructor Híbrido</span>
              </button>

              <button
                onClick={() => setShowCreateModal(true)}
                className="flex items-center gap-2 px-6 py-3 bg-gradient-to-r from-blue-600 to-purple-600 text-white rounded-lg hover:from-blue-700 hover:to-purple-700 transition-all shadow-lg"
              >
                <Plus className="w-5 h-5" />
                <span>Nueva Plantilla</span>
              </button>
            </div>
          </div>
        </div>

        {/* Barra de búsqueda y controles */}
        <div className="bg-white rounded-xl shadow-md p-4 mb-6">
          <div className="flex items-center gap-4 flex-wrap">
            {/* Búsqueda */}
            <div className="flex-1 min-w-[250px]">
              <div className="relative">
                <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 w-5 h-5 text-gray-400" />
                <input
                  type="text"
                  placeholder="Buscar plantillas..."
                  value={filters.search}
                  onChange={(e) =>
                    setFilters({ ...filters, search: e.target.value })
                  }
                  className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-all"
                />
              </div>
            </div>

            {/* Botón de filtros */}
            <button
              onClick={() => setShowFilterPanel(!showFilterPanel)}
              className={`flex items-center gap-2 px-4 py-2 rounded-lg transition-all ${
                showFilterPanel
                  ? "bg-blue-600 text-white"
                  : "bg-gray-100 text-gray-700 hover:bg-gray-200"
              }`}
            >
              <Filter className="w-5 h-5" />
              <span className="hidden sm:inline">Filtros</span>
              {(filters.status !== "all" || filters.dateFrom || filters.dateTo) && (
                <span className="bg-red-500 text-white text-xs rounded-full w-2 h-2"></span>
              )}
            </button>

            {/* Refresh */}
            <button
              onClick={handleRefresh}
              disabled={refreshing}
              className="flex items-center gap-2 px-4 py-2 bg-gray-100 text-gray-700 rounded-lg hover:bg-gray-200 transition-all disabled:opacity-50"
            >
              <RefreshCw className={`w-5 h-5 ${refreshing ? "animate-spin" : ""}`} />
              <span className="hidden sm:inline">Actualizar</span>
            </button>

            {/* Toggle de vista */}
            <div className="flex items-center gap-1 bg-gray-100 rounded-lg p-1">
              <button
                onClick={() => setViewMode("grid")}
                className={`p-2 rounded ${
                  viewMode === "grid"
                    ? "bg-white shadow text-blue-600"
                    : "text-gray-600 hover:text-gray-900"
                }`}
              >
                <Grid className="w-5 h-5" />
              </button>
              <button
                onClick={() => setViewMode("list")}
                className={`p-2 rounded ${
                  viewMode === "list"
                    ? "bg-white shadow text-blue-600"
                    : "text-gray-600 hover:text-gray-900"
                }`}
              >
                <List className="w-5 h-5" />
              </button>
            </div>
          </div>

          {/* Panel de filtros expandible */}
          {showFilterPanel && (
            <FilterPanel filters={filters} setFilters={setFilters} />
          )}
        </div>

        {/* Estadísticas rápidas */}
        <div className="grid grid-cols-2 md:grid-cols-4 gap-4 mb-6">
          <div className="bg-white rounded-lg shadow p-4">
            <p className="text-sm text-gray-600 mb-1">Total</p>
            <p className="text-2xl font-bold text-gray-900">{templates.length}</p>
          </div>
          <div className="bg-green-50 rounded-lg shadow p-4">
            <p className="text-sm text-green-700 mb-1">Completadas</p>
            <p className="text-2xl font-bold text-green-600">
              {templates.filter((t) => t.status === "completed").length}
            </p>
          </div>
          <div className="bg-blue-50 rounded-lg shadow p-4">
            <p className="text-sm text-blue-700 mb-1">En Progreso</p>
            <p className="text-2xl font-bold text-blue-600">
              {templates.filter((t) => t.status === "in_progress").length}
            </p>
          </div>
          <div className="bg-yellow-50 rounded-lg shadow p-4">
            <p className="text-sm text-yellow-700 mb-1">Borrador</p>
            <p className="text-2xl font-bold text-yellow-600">
              {templates.filter((t) => t.status === "draft").length}
            </p>
          </div>
        </div>

        {/* Contenido principal */}
        {filteredTemplates.length === 0 ? (
          <div className="bg-white rounded-xl shadow-lg text-center py-16 px-6">
            <div className="text-6xl mb-4">📋</div>
            <h3 className="text-2xl font-semibold text-gray-800 mb-2">
              {templates.length === 0
                ? "No hay plantillas consolidadas"
                : "No se encontraron plantillas"}
            </h3>
            <p className="text-gray-600 mb-8 max-w-md mx-auto">
              {templates.length === 0
                ? "Crea tu primera plantilla consolidada para comenzar a trabajar con informes multi-área"
                : "Intenta ajustar los filtros de búsqueda"}
            </p>
            {templates.length === 0 && (
              <button
                onClick={() => setShowCreateModal(true)}
                className="px-8 py-3 bg-gradient-to-r from-blue-600 to-purple-600 text-white rounded-lg hover:from-blue-700 hover:to-purple-700 transition-all shadow-lg font-medium"
              >
                Crear Primera Plantilla
              </button>
            )}
          </div>
        ) : (
          <>
            {/* Vista Grid */}
            {viewMode === "grid" && (
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 mb-6">
                {paginatedTemplates.map((template) => (
                  <TemplateCard
                    key={template.id}
                    template={template}
                    onViewDetails={handleViewDetails}
                    onDelete={handleDeleteTemplate}
                    onEdit={(id) => navigate(`/consolidated-templates/${id}/edit`)}
                  />
                ))}
              </div>
            )}

            {/* Vista Lista */}
            {viewMode === "list" && (
              <div className="bg-white rounded-xl shadow-lg overflow-hidden mb-6">
                <div className="divide-y divide-gray-200">
                  {paginatedTemplates.map((template) => (
                    <TemplateListItem
                      key={template.id}
                      template={template}
                      onViewDetails={handleViewDetails}
                      onDelete={handleDeleteTemplate}
                      onEdit={(id) => navigate(`/consolidated-templates/${id}/edit`)}
                    />
                  ))}
                </div>
              </div>
            )}

            {/* Paginación */}
            {totalPages > 1 && (
              <div className="flex items-center justify-center gap-2">
                <button
                  onClick={() => setCurrentPage((p) => Math.max(1, p - 1))}
                  disabled={currentPage === 1}
                  className="p-2 rounded-lg bg-white border border-gray-300 hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed transition-all"
                >
                  <ChevronLeft className="w-5 h-5" />
                </button>

                <div className="flex items-center gap-1">
                  {[...Array(totalPages)].map((_, i) => (
                    <button
                      key={i}
                      onClick={() => setCurrentPage(i + 1)}
                      className={`px-4 py-2 rounded-lg transition-all ${
                        currentPage === i + 1
                          ? "bg-blue-600 text-white shadow-lg"
                          : "bg-white border border-gray-300 text-gray-700 hover:bg-gray-50"
                      }`}
                    >
                      {i + 1}
                    </button>
                  ))}
                </div>

                <button
                  onClick={() => setCurrentPage((p) => Math.min(totalPages, p + 1))}
                  disabled={currentPage === totalPages}
                  className="p-2 rounded-lg bg-white border border-gray-300 hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed transition-all"
                >
                  <ChevronRight className="w-5 h-5" />
                </button>

                <span className="ml-4 text-sm text-gray-600">
                  Mostrando {startIndex + 1}-
                  {Math.min(startIndex + itemsPerPage, filteredTemplates.length)} de{" "}
                  {filteredTemplates.length}
                </span>
              </div>
            )}
          </>
        )}
      </div>

      {/* Modales */}
      {showDetailsModal && selectedTemplate && (
        <TemplateDetailModal
          template={selectedTemplate}
          onClose={() => {
            setShowDetailsModal(false);
            setSelectedTemplate(null);
          }}
          onEdit={(id) => {
            setShowDetailsModal(false);
            navigate(`/consolidated-templates/${id}/edit`);
          }}
          onDelete={async (id) => {
            await handleDeleteTemplate(id);
            setShowDetailsModal(false);
          }}
        />
      )}

      {showCreateModal && (
        <CreateTemplateModal
          onClose={() => setShowCreateModal(false)}
          onCreate={handleCreateTemplate}
        />
      )}
    </div>
  );
};

export default ConsolidatedTemplatesPage;