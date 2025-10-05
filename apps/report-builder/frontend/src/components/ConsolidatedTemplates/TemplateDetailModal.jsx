// Modal mejorado con vista detallada completa

import { X, Edit, Trash2, Calendar, Users, CheckCircle2, Clock } from "lucide-react";
import { ConsolidatedTemplateService } from "../../services/consolidatedTemplateService";

export const TemplateDetailModal = ({ template, onClose, onEdit, onDelete }) => {
  if (!template) return null;

  const statusColor = ConsolidatedTemplateService.getStatusColor(template.status);
  const statusText = ConsolidatedTemplateService.getStatusText(template.status);
  const progressColor = ConsolidatedTemplateService.getProgressColor(
    template.progress || 0
  );

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50 animate-fadeIn">
      <div className="bg-white rounded-2xl max-w-4xl w-full max-h-[90vh] overflow-hidden shadow-2xl animate-slideUp">
        {/* Header con gradiente */}
        <div className="bg-gradient-to-r from-blue-600 to-purple-600 p-6 text-white">
          <div className="flex items-start justify-between">
            <div className="flex-1">
              <h2 className="text-2xl font-bold mb-2">
                {template.name || "Sin título"}
              </h2>
              <p className="text-blue-100">{template.description}</p>
            </div>
            <button
              onClick={onClose}
              className="p-2 hover:bg-white hover:bg-opacity-20 rounded-lg transition-all"
            >
              <X className="w-6 h-6" />
            </button>
          </div>

          {/* Stats row */}
          <div className="grid grid-cols-3 gap-4 mt-6">
            <div className="bg-white bg-opacity-20 rounded-lg p-3 backdrop-blur-sm">
              <p className="text-sm text-blue-100 mb-1">Progreso</p>
              <p className="text-2xl font-bold">{template.progress || 0}%</p>
            </div>
            <div className="bg-white bg-opacity-20 rounded-lg p-3 backdrop-blur-sm">
              <p className="text-sm text-blue-100 mb-1">Secciones</p>
              <p className="text-2xl font-bold">{template.totalSections || 0}</p>
            </div>
            <div className="bg-white bg-opacity-20 rounded-lg p-3 backdrop-blur-sm">
              <p className="text-sm text-blue-100 mb-1">Completadas</p>
              <p className="text-2xl font-bold">{template.completedSections || 0}</p>
            </div>
          </div>
        </div>

        {/* Contenido */}
        <div className="p-6 overflow-y-auto max-h-[calc(90vh-280px)]">
          {/* Información general */}
          <div className="grid grid-cols-2 gap-6 mb-6">
            <div>
              <h3 className="text-lg font-semibold text-gray-900 mb-4">
                Información General
              </h3>
              <div className="space-y-3">
                <div>
                  <p className="text-sm text-gray-600 mb-1">Estado</p>
                  <span className={`inline-block px-3 py-1 rounded-full text-sm font-medium ${statusColor}`}>
                    {statusText}
                  </span>
                </div>
                {template.period && (
                  <div>
                    <p className="text-sm text-gray-600 mb-1">Período</p>
                    <p className="font-medium text-gray-900 flex items-center gap-2">
                      <Calendar className="w-4 h-4" />
                      {template.period}
                    </p>
                  </div>
                )}
                {template.deadline && (
                  <div>
                    <p className="text-sm text-gray-600 mb-1">Fecha límite</p>
                    <p className="font-medium text-gray-900">
                      {new Date(template.deadline).toLocaleDateString("es-ES", {
                        weekday: "long",
                        year: "numeric",
                        month: "long",
                        day: "numeric",
                      })}
                    </p>
                  </div>
                )}
                {template.createdByUserName && (
                  <div>
                    <p className="text-sm text-gray-600 mb-1">Creado por</p>
                    <p className="font-medium text-gray-900 flex items-center gap-2">
                      <Users className="w-4 h-4" />
                      {template.createdByUserName}
                    </p>
                  </div>
                )}
                {template.createdAt && (
                  <div>
                    <p className="text-sm text-gray-600 mb-1">Fecha de creación</p>
                    <p className="font-medium text-gray-900">
                      {new Date(template.createdAt).toLocaleDateString("es-ES")}
                    </p>
                  </div>
                )}
              </div>
            </div>

            {/* Progress visual */}
            <div>
              <h3 className="text-lg font-semibold text-gray-900 mb-4">
                Progreso Detallado
              </h3>
              <div className="space-y-4">
                <div>
                  <div className="flex justify-between mb-2">
                    <span className="text-sm font-medium text-gray-700">
                      Completitud General
                    </span>
                    <span className="text-sm font-bold text-gray-900">
                      {template.progress || 0}%
                    </span>
                  </div>
                  <div className="w-full bg-gray-200 rounded-full h-3">
                    <div
                      className={`h-3 rounded-full ${progressColor} transition-all`}
                      style={{ width: `${template.progress || 0}%` }}
                    />
                  </div>
                </div>

                <div className="grid grid-cols-2 gap-3">
                  <div className="bg-green-50 rounded-lg p-3">
                    <div className="flex items-center gap-2 text-green-700 mb-1">
                      <CheckCircle2 className="w-4 h-4" />
                      <span className="text-xs font-medium">Completadas</span>
                    </div>
                    <p className="text-2xl font-bold text-green-600">
                      {template.completedSections || 0}
                    </p>
                  </div>
                  <div className="bg-orange-50 rounded-lg p-3">
                    <div className="flex items-center gap-2 text-orange-700 mb-1">
                      <Clock className="w-4 h-4" />
                      <span className="text-xs font-medium">Pendientes</span>
                    </div>
                    <p className="text-2xl font-bold text-orange-600">
                      {template.pendingSections || 0}
                    </p>
                  </div>
                </div>
              </div>
            </div>
          </div>

          {/* Secciones */}
          <div>
            <h3 className="text-lg font-semibold text-gray-900 mb-4">
              Secciones ({template.sections?.length || 0})
            </h3>
            <div className="space-y-3 max-h-96 overflow-y-auto pr-2">
              {template.sections && template.sections.length > 0 ? (
                template.sections.map((section, index) => (
                  <div
                    key={section.id || index}
                    className="border border-gray-200 rounded-lg p-4 hover:shadow-md transition-all"
                  >
                    <div className="flex justify-between items-start">
                      <div className="flex-1">
                        <div className="flex items-center gap-2 mb-2">
                          <span className="text-sm font-medium text-gray-500">
                            #{index + 1}
                          </span>
                          <h4 className="font-semibold text-gray-900">
                            {section.sectionTitle || section.title || "Sin título"}
                          </h4>
                        </div>
                        <p className="text-sm text-gray-600 mb-2">
                          {section.sectionDescription || section.description || "Sin descripción"}
                        </p>
                        <div className="flex items-center gap-4 text-xs text-gray-500">
                          {section.areaName && (
                            <span className="flex items-center gap-1">
                              <Users className="w-3 h-3" />
                              Área: {section.areaName}
                            </span>
                          )}
                          {section.assignedToUserName && (
                            <span>Asignado a: {section.assignedToUserName}</span>
                          )}
                        </div>
                      </div>
                      <span
                        className={`px-2 py-1 rounded-full text-xs font-medium ${ConsolidatedTemplateService.getStatusColor(
                          section.status
                        )}`}
                      >
                        {ConsolidatedTemplateService.getStatusText(section.status)}
                      </span>
                    </div>
                  </div>
                ))
              ) : (
                <p className="text-center text-gray-500 py-8">
                  No hay secciones definidas
                </p>
              )}
            </div>
          </div>
        </div>

        {/* Footer con acciones */}
        <div className="border-t border-gray-200 p-6 bg-gray-50 flex items-center justify-between">
          <button
            onClick={onClose}
            className="px-6 py-2 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-100 transition-all"
          >
            Cerrar
          </button>
          <div className="flex items-center gap-3">
            <button
              onClick={() => onDelete(template.id)}
              className="px-6 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700 transition-all flex items-center gap-2"
            >
              <Trash2 className="w-4 h-4" />
              Eliminar
            </button>
            <button
              onClick={() => onEdit(template.id)}
              className="px-6 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-all flex items-center gap-2"
            >
              <Edit className="w-4 h-4" />
              Editar Plantilla
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};
