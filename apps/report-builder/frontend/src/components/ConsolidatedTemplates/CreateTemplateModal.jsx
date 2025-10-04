// Modal para crear plantilla desde reportes existentes

import { useState } from "react";
import { X, Plus, FileText, Calendar as CalendarIcon, AlertCircle } from "lucide-react";

export const CreateTemplateModal = ({ onClose, onCreate }) => {
  const [formData, setFormData] = useState({
    name: "",
    description: "",
    period: "",
    reportIds: [],
    deadline: "",
  });
  const [loading, setLoading] = useState(false);
  const [errors, setErrors] = useState({});

  // Simular lista de reportes disponibles
  const [availableReports] = useState([
    { id: 1, name: "Informe Financiero Q1", period: "Q1 2025" },
    { id: 2, name: "Reporte Operativo Marzo", period: "Marzo 2025" },
    { id: 3, name: "Análisis de Ventas Q1", period: "Q1 2025" },
  ]);

  const validate = () => {
    const newErrors = {};
    if (!formData.name.trim()) newErrors.name = "El nombre es requerido";
    if (!formData.period.trim()) newErrors.period = "El período es requerido";
    if (formData.reportIds.length === 0)
      newErrors.reportIds = "Selecciona al menos un reporte";

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!validate()) return;

    setLoading(true);
    try {
      await onCreate(formData);
      onClose();
    } catch (err) {
      setErrors({ submit: err.message });
    } finally {
      setLoading(false);
    }
  };

  const toggleReport = (reportId) => {
    setFormData((prev) => ({
      ...prev,
      reportIds: prev.reportIds.includes(reportId)
        ? prev.reportIds.filter((id) => id !== reportId)
        : [...prev.reportIds, reportId],
    }));
    if (errors.reportIds) setErrors({ ...errors, reportIds: null });
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
      <div className="bg-white rounded-2xl max-w-2xl w-full max-h-[90vh] overflow-hidden shadow-2xl">
        {/* Header */}
        <div className="bg-gradient-to-r from-blue-600 to-purple-600 p-6 text-white">
          <div className="flex items-center justify-between">
            <div>
              <h2 className="text-2xl font-bold mb-1">Nueva Plantilla Consolidada</h2>
              <p className="text-blue-100">
                Crea una plantilla desde reportes existentes
              </p>
            </div>
            <button
              onClick={onClose}
              className="p-2 hover:bg-white hover:bg-opacity-20 rounded-lg transition-all"
            >
              <X className="w-6 h-6" />
            </button>
          </div>
        </div>

        {/* Formulario */}
        <form onSubmit={handleSubmit} className="p-6 overflow-y-auto max-h-[calc(90vh-200px)]">
          {/* Nombre */}
          <div className="mb-4">
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Nombre de la plantilla *
            </label>
            <input
              type="text"
              value={formData.name}
              onChange={(e) => {
                setFormData({ ...formData, name: e.target.value });
                if (errors.name) setErrors({ ...errors, name: null });
              }}
              className={`w-full px-4 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 ${
                errors.name ? "border-red-500" : "border-gray-300"
              }`}
              placeholder="Ej: Informe Consolidado Q1 2025"
            />
            {errors.name && (
              <p className="text-red-600 text-sm mt-1 flex items-center gap-1">
                <AlertCircle className="w-4 h-4" />
                {errors.name}
              </p>
            )}
          </div>

          {/* Descripción */}
          <div className="mb-4">
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Descripción
            </label>
            <textarea
              value={formData.description}
              onChange={(e) =>
                setFormData({ ...formData, description: e.target.value })
              }
              rows={3}
              className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
              placeholder="Describe el propósito de esta plantilla consolidada..."
            />
          </div>

          {/* Período y Deadline en grid */}
          <div className="grid grid-cols-2 gap-4 mb-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Período *
              </label>
              <input
                type="text"
                value={formData.period}
                onChange={(e) => {
                  setFormData({ ...formData, period: e.target.value });
                  if (errors.period) setErrors({ ...errors, period: null });
                }}
                className={`w-full px-4 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 ${
                  errors.period ? "border-red-500" : "border-gray-300"
                }`}
                placeholder="Ej: Q1 2025"
              />
              {errors.period && (
                <p className="text-red-600 text-sm mt-1">{errors.period}</p>
              )}
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Fecha límite
              </label>
              <input
                type="date"
                value={formData.deadline}
                onChange={(e) =>
                  setFormData({ ...formData, deadline: e.target.value })
                }
                className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
              />
            </div>
          </div>

          {/* Selección de reportes */}
          <div className="mb-6">
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Reportes fuente *
            </label>
            <p className="text-xs text-gray-500 mb-3">
              Selecciona los reportes que se consolidarán en esta plantilla
            </p>
            <div className="space-y-2 max-h-64 overflow-y-auto border border-gray-200 rounded-lg p-3">
              {availableReports.map((report) => (
                <label
                  key={report.id}
                  className={`flex items-center gap-3 p-3 rounded-lg cursor-pointer transition-all ${
                    formData.reportIds.includes(report.id)
                      ? "bg-blue-50 border-2 border-blue-500"
                      : "bg-gray-50 border-2 border-transparent hover:bg-gray-100"
                  }`}
                >
                  <input
                    type="checkbox"
                    checked={formData.reportIds.includes(report.id)}
                    onChange={() => toggleReport(report.id)}
                    className="w-5 h-5 text-blue-600 rounded"
                  />
                  <FileText className="w-5 h-5 text-gray-400" />
                  <div className="flex-1">
                    <p className="font-medium text-gray-900">{report.name}</p>
                    <p className="text-xs text-gray-500 flex items-center gap-1">
                      <CalendarIcon className="w-3 h-3" />
                      {report.period}
                    </p>
                  </div>
                </label>
              ))}
            </div>
            {errors.reportIds && (
              <p className="text-red-600 text-sm mt-1 flex items-center gap-1">
                <AlertCircle className="w-4 h-4" />
                {errors.reportIds}
              </p>
            )}
            <p className="text-sm text-gray-600 mt-2">
              Seleccionados: {formData.reportIds.length}
            </p>
          </div>

          {errors.submit && (
            <div className="mb-4 p-3 bg-red-50 border border-red-200 rounded-lg text-red-700 text-sm">
              {errors.submit}
            </div>
          )}
        </form>

        {/* Footer */}
        <div className="border-t border-gray-200 p-6 bg-gray-50 flex items-center justify-end gap-3">
          <button
            type="button"
            onClick={onClose}
            className="px-6 py-2 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-100 transition-all"
          >
            Cancelar
          </button>
          <button
            onClick={handleSubmit}
            disabled={loading}
            className="px-6 py-2 bg-gradient-to-r from-blue-600 to-purple-600 text-white rounded-lg hover:from-blue-700 hover:to-purple-700 transition-all disabled:opacity-50 flex items-center gap-2"
          >
            {loading ? (
              <>
                <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-white"></div>
                Creando...
              </>
            ) : (
              <>
                <Plus className="w-4 h-4" />
                Crear Plantilla
              </>
            )}
          </button>
        </div>
      </div>
    </div>
  );
};