import { useState, useEffect } from "react";
import PropTypes from "prop-types";
import { getEmployeesByManager } from "../../../services/employeeService";
import { useAuth } from "../../../utils/useAuth";
import { API_CONFIG } from "../../../environments/api.config";
import { getAuthHeaders } from "../../../environments/http-headers";
import "./IngresoAutorizacionForm.scss";
import { AutoComplete, Input } from "antd";

const initialState = {
  employeeId: null,
  employeeName: "",
  date: "",
  estimatedEntryTime: "",
  estimatedExitTime: "",
  taskDescription: "",
  managerName: "",
  managerEmail: "",
};

const Notification = ({ message, type = "success", onClose }) => {
  useEffect(() => {
    const timer = setTimeout(() => {
      onClose();
    }, 5000);
    return () => clearTimeout(timer);
  }, [onClose]);
  return (
    <div className={`notification notification-${type}`}>
      <p>{message}</p>
      <button
        onClick={onClose}
        className="close-btn"
        aria-label="Cerrar notificación"
      >
        ×
      </button>
    </div>
  );
};

// Agregar PropTypes para el componente Notification
Notification.propTypes = {
  message: PropTypes.string.isRequired,
  type: PropTypes.oneOf(["success", "error", "warning", "info"]),
  onClose: PropTypes.func.isRequired,
};

// Definir defaultProps si es necesario
Notification.defaultProps = {
  type: "success",
};

export default function IngresoAutorizacionForm() {
  const { getUserName, getUserEmail } = useAuth();
  const [form, setForm] = useState(initialState);
  const [loading, setLoading] = useState(false);
  const [notification, setNotification] = useState(null);
  const [error, setError] = useState("");
  const [employees, setEmployees] = useState([]);
  const [filteredOptions, setFilteredOptions] = useState([]);

  // Cargar empleados a cargo y datos del manager al montar
  useEffect(() => {
    const fetchData = async () => {
      try {
        const empleados = await getEmployeesByManager();
        setEmployees(empleados);
      } catch (err) {
        setError("Error al cargar empleados a cargo.");
      }

      // Obtener nombre y email del manager directamente del token
      const name = getUserName() || "";
      const email = getUserEmail() || "";
      setForm((prev) => ({
        ...prev,
        managerName: name,
        managerEmail: email,
      }));
    };
    fetchData();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []); // Solo se ejecuta una vez al montar

  // Autocompletado por nombre o ID
  const handleSearch = (value) => {
    if (!value) {
      setFilteredOptions([]);
      return;
    }
    const lower = value.toLowerCase();
    setFilteredOptions(
      employees
        .filter(
          (e) =>
            e.name.toLowerCase().includes(lower) || String(e.id).includes(lower)
        )
        .map((e) => ({
          value: e.id,
          label: `${e.name} (ID: ${e.id})`,
        }))
    );
  };

  const handleSelect = (value) => {
    const selectedEmployee = employees.find((e) => e.id === value);
    if (selectedEmployee) {
      setForm((prev) => ({
        ...prev,
        employeeId: selectedEmployee.id,
        employeeName: selectedEmployee.name,
      }));
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);

    try {
      const response = await fetch(
        `${API_CONFIG.BASE_URL}/api/IngresoAutorizacion`,
        {
          method: "POST",
          headers: {
            ...getAuthHeaders(),
            "Content-Type": "application/json",
          },
          body: JSON.stringify({
            employeeId: form.employeeId,
            employeeName: form.employeeName,
            date: form.date,
            estimatedEntryTime: form.estimatedEntryTime,
            estimatedExitTime: form.estimatedExitTime,
            taskDescription: form.taskDescription,
            managerName: form.managerName,
            managerEmail: form.managerEmail,
          }),
        }
      );

      if (response.ok) {
        setNotification({
          type: "success",
          message: "Autorización enviada correctamente",
        });
        setForm(initialState);
      } else {
        const errorData = await response.json();
        setNotification({
          type: "error",
          message: errorData.message || "Error al enviar autorización",
        });
      }
    } catch (error) {
      console.error("Error:", error);
      setNotification({
        type: "error",
        message: "Error de conexión. Intente nuevamente.",
      });
    } finally {
      setLoading(false);
    }
  };

  const closeNotification = () => {
    setNotification(null);
  };

  return (
    <div className="form-container autorizacion-component">
      <div className="form-main-container">
        <h1 className="form-title">Autorizar Ingreso en Día de Descanso</h1>
        {notification && (
          <Notification
            message={notification.message}
            type={notification.type}
            onClose={closeNotification}
          />
        )}
        {error && (
          <div className="error-message">
            <p>{error}</p>
          </div>
        )}
        <form onSubmit={handleSubmit}>
          <div className="form-group-date-time">
            <div className="form-field">
              <label htmlFor="employee">
                Empleado <span className="required">*</span>
              </label>
              <AutoComplete
                options={filteredOptions}
                onSearch={handleSearch}
                onSelect={handleSelect}
                placeholder="Buscar por nombre o ID"
                style={{ width: "100%" }}
                value={form.employeeName}
                onChange={(value) =>
                  setForm((prev) => ({ ...prev, employeeName: value }))
                }
              />
            </div>
            <div className="form-field">
              <label htmlFor="date">
                Fecha <span className="required">*</span>
              </label>
              <Input
                type="date"
                id="date"
                value={form.date}
                onChange={(e) =>
                  setForm((prev) => ({ ...prev, date: e.target.value }))
                }
                required
              />
            </div>
          </div>

          <div className="form-group-date-time">
            <div className="form-field">
              <label htmlFor="estimatedEntryTime">
                Hora de entrada estimada <span className="required">*</span>
              </label>
              <Input
                type="time"
                id="estimatedEntryTime"
                value={form.estimatedEntryTime}
                onChange={(e) =>
                  setForm((prev) => ({
                    ...prev,
                    estimatedEntryTime: e.target.value,
                  }))
                }
                required
              />
            </div>
            <div className="form-field">
              <label htmlFor="estimatedExitTime">
                Hora de salida estimada <span className="required">*</span>
              </label>
              <Input
                type="time"
                id="estimatedExitTime"
                value={form.estimatedExitTime}
                onChange={(e) =>
                  setForm((prev) => ({
                    ...prev,
                    estimatedExitTime: e.target.value,
                  }))
                }
                required
              />
            </div>
          </div>

          <div className="observaciones-container">
            <label htmlFor="taskDescription">
              Descripción de la tarea <span className="required">*</span>
            </label>
            <textarea
              id="taskDescription"
              value={form.taskDescription}
              onChange={(e) =>
                setForm((prev) => ({
                  ...prev,
                  taskDescription: e.target.value,
                }))
              }
              placeholder="Describa brevemente la tarea a realizar..."
              required
            />
          </div>

          <div className="submit-container">
            <button
              type="submit"
              disabled={loading}
              className={loading ? "loading" : ""}
            >
              {loading ? (
                <>
                  <div className="loading-spinner-button"></div>
                  Enviando...
                </>
              ) : (
                "Enviar Autorización"
              )}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
