import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { toast } from "react-toastify";
import { ConsolidatedTemplateService } from "../services/consolidatedTemplateService";
import { useAuth } from "../contexts/AuthContext";

const MyTasksPage = () => {
  const navigate = useNavigate();
  const { user } = useAuth();
  const [tasks, setTasks] = useState([]);
  const [loading, setLoading] = useState(true);
  const [filter, setFilter] = useState("all"); // all, pending, in_progress, completed

  useEffect(() => {
    fetchTasks();
  }, []);

  const fetchTasks = async () => {
    try {
      setLoading(true);
      const assignedSections =
        await ConsolidatedTemplateService.getMyAssignedSections();
      setTasks(assignedSections);
    } catch (err) {
      console.error("Error al cargar tareas:", err);
      toast.error("Error al cargar las tareas asignadas");
    } finally {
      setLoading(false);
    }
  };

  const handleCompleteSection = (task) => {
    // Navegar a la página de completado de sección
    navigate(
      `/consolidated-templates/${task.consolidatedTemplateId}/sections/${task.id}`
    );
  };

  const filteredTasks = tasks.filter((task) => {
    if (filter === "all") return true;
    return task.status === filter;
  });

  const getTaskCounts = () => {
    return {
      all: tasks.length,
      pending: tasks.filter((t) => t.status === "pending").length,
      in_progress: tasks.filter((t) => t.status === "in_progress").length,
      completed: tasks.filter((t) => t.status === "completed").length,
    };
  };

  const counts = getTaskCounts();

  if (loading) {
    return (
      <div className="min-h-screen bg-gray-50 p-6">
        <div className="max-w-6xl mx-auto">
          <div className="text-center py-12">
            <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto"></div>
            <p className="mt-4 text-gray-600">Cargando tareas...</p>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-50 to-blue-50 p-6">
      <div className="max-w-6xl mx-auto">
        <div className="mb-8">
          <h1 className="text-4xl font-bold text-gray-900 mb-2">
            Mis Tareas Pendientes
          </h1>
          <p className="text-gray-600 text-lg">
            Hola <span className="font-semibold">{user?.name || "Usuario"}</span>,
            estas son las secciones de informes asignadas a tu área.
          </p>
        </div>

        {/* Filtros */}
        <div className="bg-white rounded-lg shadow-md p-4 mb-6">
          <div className="flex flex-wrap gap-3">
            <button
              onClick={() => setFilter("all")}
              className={`px-4 py-2 rounded-lg font-medium transition-all ${
                filter === "all"
                  ? "bg-blue-600 text-white"
                  : "bg-gray-100 text-gray-700 hover:bg-gray-200"
              }`}
            >
              📋 Todas ({counts.all})
            </button>
            <button
              onClick={() => setFilter("pending")}
              className={`px-4 py-2 rounded-lg font-medium transition-all ${
                filter === "pending"
                  ? "bg-yellow-600 text-white"
                  : "bg-gray-100 text-gray-700 hover:bg-gray-200"
              }`}
            >
              ⏳ Pendientes ({counts.pending})
            </button>
            <button
              onClick={() => setFilter("in_progress")}
              className={`px-4 py-2 rounded-lg font-medium transition-all ${
                filter === "in_progress"
                  ? "bg-purple-600 text-white"
                  : "bg-gray-100 text-gray-700 hover:bg-gray-200"
              }`}
            >
              🚀 En Progreso ({counts.in_progress})
            </button>
            <button
              onClick={() => setFilter("completed")}
              className={`px-4 py-2 rounded-lg font-medium transition-all ${
                filter === "completed"
                  ? "bg-green-600 text-white"
                  : "bg-gray-100 text-gray-700 hover:bg-gray-200"
              }`}
            >
              ✅ Completadas ({counts.completed})
            </button>
          </div>
        </div>

        {/* Lista de tareas */}
        {filteredTasks.length === 0 ? (
          <div className="bg-white rounded-lg shadow-md text-center py-12">
            <div className="text-6xl mb-4">🎉</div>
            <h3 className="text-xl font-semibold text-gray-800 mb-2">
              {filter === "all"
                ? "¡Todo al día!"
                : filter === "pending"
                ? "No tienes tareas pendientes"
                : filter === "in_progress"
                ? "No tienes tareas en progreso"
                : "No tienes tareas completadas"}
            </h3>
            <p className="text-gray-600">
              {filter === "all"
                ? "No tienes secciones de informes pendientes por completar."
                : "Cambia el filtro para ver otras tareas."}
            </p>
          </div>
        ) : (
          <div className="space-y-4">
            {filteredTasks.map((task) => (
              <div
                key={task.id}
                className="bg-white rounded-lg shadow-md p-6 hover:shadow-lg transition-shadow"
              >
                <div className="flex justify-between items-start">
                  <div className="flex-1">
                    <div className="flex items-center gap-3 mb-3">
                      <span
                        className={`px-3 py-1 text-xs font-semibold rounded-full ${ConsolidatedTemplateService.getStatusColor(
                          task.status
                        )}`}
                      >
                        {ConsolidatedTemplateService.getStatusText(task.status)}
                      </span>
                      {task.isUrgent && (
                        <span className="px-3 py-1 text-xs font-semibold rounded-full bg-red-100 text-red-800">
                          🔥 Urgente
                        </span>
                      )}
                    </div>

                    <h2 className="text-xl font-semibold text-gray-900 mb-2">
                      {task.sectionTitle}
                    </h2>

                    <p className="text-sm text-gray-600 mb-3">
                      {task.sectionDescription || "Sin descripción"}
                    </p>

                    <div className="flex flex-wrap items-center gap-4 text-sm text-gray-500">
                      <span>
                        📋 Del informe:{" "}
                        <strong className="text-gray-700">
                          {task.consolidatedTemplateName}
                        </strong>
                      </span>
                      <span>
                        📅 Período:{" "}
                        <strong className="text-gray-700">{task.period}</strong>
                      </span>
                      {task.sectionDeadline && (
                        <span>
                          ⏰ Límite:{" "}
                          <strong className="text-gray-700">
                            {new Date(task.sectionDeadline).toLocaleDateString()}
                          </strong>
                        </span>
                      )}
                    </div>
                  </div>

                  <button
                    onClick={() => handleCompleteSection(task)}
                    className={`
                      px-6 py-3 rounded-lg font-semibold transition-all
                      ${
                        task.status === "completed"
                          ? "bg-gray-300 text-gray-600"
                          : task.status === "in_progress"
                          ? "bg-purple-600 hover:bg-purple-700 text-white"
                          : "bg-blue-600 hover:bg-blue-700 text-white"
                      }
                    `}
                  >
                    {task.status === "completed"
                      ? "✅ Ver Completada"
                      : task.status === "in_progress"
                      ? "📝 Continuar"
                      : "🚀 Empezar"}
                  </button>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
};

export default MyTasksPage;

