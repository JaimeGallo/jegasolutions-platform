// Tarjeta visual mejorada con más información y mejor UX

import React from "react";
import {
  FileText,
  Calendar,
  Users,
  TrendingUp,
  Trash2,
  Edit,
  Eye,
  Clock,
  AlertCircle,
  CheckCircle2,
} from "lucide-react";
import { ConsolidatedTemplateService } from "../../services/consolidatedTemplateService";

const TemplateCard = ({ template, onViewDetails, onEdit, onDelete }) => {
  const statusColor = ConsolidatedTemplateService.getStatusColor(template.status);
  const statusText = ConsolidatedTemplateService.getStatusText(template.status);
  const progressColor = ConsolidatedTemplateService.getProgressColor(
    template.progress || 0
  );

  // Calcular si está vencida
  const isOverdue =
    template.deadline &&
    new Date(template.deadline) < new Date() &&
    template.status !== "completed";

  // Icono de estado
  const getStatusIcon = () => {
    switch (template.status) {
      case "completed":
        return <CheckCircle2 className="w-5 h-5 text-green-500" />;
      case "in_progress":
        return <Clock className="w-5 h-5 text-blue-500" />;
      case "draft":
        return <FileText className="w-5 h-5 text-gray-500" />;
      default:
        return <AlertCircle className="w-5 h-5 text-yellow-500" />;
    }
  };

  return (
    <div className="bg-white rounded-xl shadow-md hover:shadow-2xl transition-all duration-300 overflow-hidden border border-gray-100 group">
      {/* Header con gradiente */}
      <div className="h-2 bg-gradient-to-r from-blue-500 via-purple-500 to-pink-500"></div>

      <div className="p-6">
        {/* Top section */}
        <div className="flex items-start justify-between mb-4">
          <div className="flex items-center gap-3">
            <div className="p-3 bg-gradient-to-br from-blue-100 to-purple-100 rounded-lg">
              <FileText className="w-6 h-6 text-blue-600" />
            </div>
            <div className="flex-1">
              <h3 className="font-bold text-lg text-gray-900 line-clamp-2 group-hover:text-blue-600 transition-colors">
                {template.name || "Sin título"}
              </h3>
              {template.period && (
                <p className="text-sm text-gray-500 mt-1">
                  <Calendar className="w-3 h-3 inline mr-1" />
                  {template.period}
                </p>
              )}
            </div>
          </div>

          {getStatusIcon()}
        </div>

        {/* Descripción */}
        {template.description && (
          <p className="text-sm text-gray-600 mb-4 line-clamp-2">
            {template.description}
          </p>
        )}

        {/* Status badge */}
        <div className="flex items-center gap-2 mb-4">
          <span
            className={`px-3 py-1 rounded-full text-xs font-semibold ${statusColor}`}
          >
            {statusText}
          </span>

          {isOverdue && (
            <span className="px-3 py-1 rounded-full text-xs font-semibold bg-red-100 text-red-800 flex items-center gap-1">
              <AlertCircle className="w-3 h-3" />
              Vencida
            </span>
          )}
        </div>

        {/* Progress bar */}
        <div className="mb-4">
          <div className="flex items-center justify-between mb-2">
            <span className="text-sm font-medium text-gray-700">Progreso</span>
            <span className="text-sm font-bold text-gray-900">
              {template.progress || 0}%
            </span>
          </div>
          <div className="w-full bg-gray-200 rounded-full h-2.5 overflow-hidden">
            <div
              className={`h-2.5 rounded-full ${progressColor} transition-all duration-500 ease-out`}
              style={{ width: `${template.progress || 0}%` }}
            />
          </div>
        </div>

        {/* Stats grid */}
        <div className="grid grid-cols-3 gap-3 mb-4">
          <div className="text-center p-3 bg-blue-50 rounded-lg">
            <div className="flex items-center justify-center gap-1 text-blue-600 mb-1">
              <Users className="w-4 h-4" />
            </div>
            <p className="text-lg font-bold text-gray-900">
              {template.totalSections || 0}
            </p>
            <p className="text-xs text-gray-600">Secciones</p>
          </div>

          <div className="text-center p-3 bg-green-50 rounded-lg">
            <div className="flex items-center justify-center gap-1 text-green-600 mb-1">
              <CheckCircle2 className="w-4 h-4" />
            </div>
            <p className="text-lg font-bold text-green-700">
              {template.completedSections || 0}
            </p>
            <p className="text-xs text-gray-600">Completas</p>
          </div>

          <div className="text-center p-3 bg-orange-50 rounded-lg">
            <div className="flex items-center justify-center gap-1 text-orange-600 mb-1">
              <Clock className="w-4 h-4" />
            </div>
            <p className="text-lg font-bold text-orange-700">
              {template.pendingSections || 0}
            </p>
            <p className="text-xs text-gray-600">Pendientes</p>
          </div>
        </div>

        {/* Metadata */}
        <div className="space-y-2 mb-4">
          {template.deadline && (
            <div className="flex items-center gap-2 text-sm">
              <Calendar className="w-4 h-4 text-gray-400" />
              <span
                className={
                  isOverdue ? "text-red-600 font-medium" : "text-gray-600"
                }
              >
                Vence: {new Date(template.deadline).toLocaleDateString("es-ES")}
              </span>
            </div>
          )}

          {template.createdByUserName && (
            <div className="flex items-center gap-2 text-sm text-gray-600">
              <Users className="w-4 h-4 text-gray-400" />
              <span>Creado por: {template.createdByUserName}</span>
            </div>
          )}

          {template.createdAt && (
            <div className="flex items-center gap-2 text-sm text-gray-500">
              <Clock className="w-4 h-4 text-gray-400" />
              <span>
                {new Date(template.createdAt).toLocaleDateString("es-ES")}
              </span>
            </div>
          )}
        </div>

        {/* Actions */}
        <div className="flex items-center gap-2 pt-4 border-t border-gray-100">
          <button
            onClick={() => onViewDetails(template)}
            className="flex-1 flex items-center justify-center gap-2 px-4 py-2.5 bg-gradient-to-r from-blue-600 to-purple-600 text-white rounded-lg hover:from-blue-700 hover:to-purple-700 transition-all shadow-sm font-medium"
          >
            <Eye className="w-4 h-4" />
            <span>Ver Detalle</span>
          </button>

          <button
            onClick={() => onEdit(template.id)}
            className="p-2.5 border-2 border-gray-300 rounded-lg hover:bg-gray-100 hover:border-gray-400 transition-all"
            title="Editar"
          >
            <Edit className="w-4 h-4 text-gray-600" />
          </button>

          <button
            onClick={() => onDelete(template.id)}
            className="p-2.5 border-2 border-red-300 rounded-lg hover:bg-red-50 hover:border-red-400 transition-all"
            title="Eliminar"
          >
            <Trash2 className="w-4 h-4 text-red-600" />
          </button>
        </div>
      </div>
    </div>
  );
};

export default TemplateCard;