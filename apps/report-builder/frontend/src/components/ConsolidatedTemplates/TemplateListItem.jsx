// Item para vista de lista con información completa
import { FileText, Edit, Trash2, Eye, Calendar, Users, TrendingUp } from "lucide-react";
import { ConsolidatedTemplateService } from "../../services/consolidatedTemplateService";

export const TemplateListItem = ({ template, onViewDetails, onEdit, onDelete }) => {
  const statusColor = ConsolidatedTemplateService.getStatusColor(template.status);
  const statusText = ConsolidatedTemplateService.getStatusText(template.status);
  const progressColor = ConsolidatedTemplateService.getProgressColor(
    template.progress || 0
  );

  return (
    <div className="p-6 hover:bg-gray-50 transition-colors">
      <div className="flex items-center gap-4">
        {/* Icono */}
        <div className="p-3 bg-gradient-to-br from-blue-100 to-purple-100 rounded-lg">
          <FileText className="w-6 h-6 text-blue-600" />
        </div>

        {/* Info principal */}
        <div className="flex-1 min-w-0">
          <div className="flex items-start justify-between mb-2">
            <div className="flex-1">
              <h3 className="text-lg font-semibold text-gray-900 mb-1">
                {template.name || "Sin título"}
              </h3>
              <p className="text-sm text-gray-600 line-clamp-1">
                {template.description || "Sin descripción"}
              </p>
            </div>
            <span className={`px-3 py-1 rounded-full text-xs font-semibold ${statusColor}`}>
              {statusText}
            </span>
          </div>

          {/* Progress bar */}
          <div className="mb-3">
            <div className="flex items-center gap-2 mb-1">
              <span className="text-xs text-gray-600">Progreso:</span>
              <div className="flex-1 bg-gray-200 rounded-full h-1.5">
                <div
                  className={`h-1.5 rounded-full ${progressColor} transition-all`}
                  style={{ width: `${template.progress || 0}%` }}
                />
              </div>
              <span className="text-xs font-medium text-gray-700">
                {template.progress || 0}%
              </span>
            </div>
          </div>

          {/* Metadata row */}
          <div className="flex items-center gap-6 text-sm text-gray-600">
            <div className="flex items-center gap-1">
              <Calendar className="w-4 h-4" />
              <span>{template.period || "N/A"}</span>
            </div>
            <div className="flex items-center gap-1">
              <Users className="w-4 h-4" />
              <span>{template.totalSections || 0} secciones</span>
            </div>
            <div className="flex items-center gap-1">
              <TrendingUp className="w-4 h-4" />
              <span>{template.completedSections || 0} completadas</span>
            </div>
            {template.createdAt && (
              <span className="text-gray-500">
                {new Date(template.createdAt).toLocaleDateString("es-ES")}
              </span>
            )}
          </div>
        </div>

        {/* Actions */}
        <div className="flex items-center gap-2">
          <button
            onClick={() => onViewDetails(template)}
            className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-all flex items-center gap-2"
          >
            <Eye className="w-4 h-4" />
            <span className="hidden lg:inline">Ver</span>
          </button>
          <button
            onClick={() => onEdit(template.id)}
            className="p-2 border border-gray-300 rounded-lg hover:bg-gray-100 transition-all"
            title="Editar"
          >
            <Edit className="w-4 h-4 text-gray-600" />
          </button>
          <button
            onClick={() => onDelete(template.id)}
            className="p-2 border border-red-300 rounded-lg hover:bg-red-50 transition-all"
            title="Eliminar"
          >
            <Trash2 className="w-4 h-4 text-red-600" />
          </button>
        </div>
      </div>
    </div>
  );
};