import { useState } from "react";
import { useAuth } from "../../../utils/useAuth";
import { EmployeeInfo } from "../EmployeeInfo/EmployeeInfo";
import { createCompensationRequest } from "../../../services/compensationRequestService";
import "./SolicitudCompensacionForm.scss";
import { useEffect } from "react";
import PropTypes from "prop-types";

const initialState = {
  workDate: "",
  requestedCompensationDate: "",
  justification: "",
};

const Notification = ({ message, type = "success", onClose }) => {
  // Copiado de FormExtraHour
  // Desaparece automáticamente después de 5 segundos
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

Notification.propTypes = {
  message: PropTypes.string.isRequired,
  type: PropTypes.oneOf(["success", "error", "warning", "info"]),
  onClose: PropTypes.func.isRequired,
};

export default function SolicitudCompensacionForm() {
  const { getEmployeeIdFromToken, getUserRole } = useAuth();
  const [employeeId, setEmployeeId] = useState(null);
  const [reset, setReset] = useState(false);
  const [form, setForm] = useState(initialState);
  const [loading, setLoading] = useState(false);
  const [notification, setNotification] = useState(null);
  const [error, setError] = useState("");

  const isSuperuser = getUserRole() === "superusuario";

  const handleEmployeeIdChange = (id) => {
    setEmployeeId(parseInt(id, 10));
  };

  const handleChange = (e) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setNotification(null);
    setError("");

    const currentEmployeeId = isSuperuser
      ? employeeId
      : getEmployeeIdFromToken() || localStorage.getItem("id");

    console.log("=== DEBUG FRONTEND ===");
    console.log("isSuperuser:", isSuperuser);
    console.log("employeeId:", employeeId);
    console.log("getEmployeeIdFromToken():", getEmployeeIdFromToken());
    console.log("localStorage.getItem('id'):", localStorage.getItem("id"));
    console.log("currentEmployeeId:", currentEmployeeId);
    console.log("=== FIN DEBUG FRONTEND ===");

    if (!currentEmployeeId) {
      setError(
        "No se pudo obtener el ID del empleado. Por favor, inicia sesión de nuevo."
      );
      setLoading(false);
      return;
    }

    const employeeIdToSend = parseInt(currentEmployeeId, 10);
    console.log("employeeIdToSend:", employeeIdToSend);

    try {
      await createCompensationRequest({
        employeeId: employeeIdToSend,
        workDate: form.workDate,
        requestedCompensationDate: form.requestedCompensationDate,
        justification: form.justification,
      });
      setNotification({
        message: "Solicitud enviada correctamente.",
        type: "success",
      });
      setForm(initialState);
      if (isSuperuser) {
        setReset(true);
        setEmployeeId(null);
      }
    } catch (err) {
      setNotification({
        message:
          err.response?.data?.title ||
          err.message ||
          "Error al enviar la solicitud.",
        type: "error",
      });
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="form-container compensacion-component">
      <h1 className="form-title">Solicitar Día de Compensación</h1>
      {notification && (
        <Notification
          message={notification.message}
          type={notification.type}
          onClose={() => setNotification(null)}
        />
      )}
      <form onSubmit={handleSubmit}>
        {isSuperuser && (
          <div className="superuser-employee-selection">
            <h2>Selección de Empleado</h2>
            <EmployeeInfo
              onIdChange={handleEmployeeIdChange}
              reset={reset}
              setReset={setReset}
            />
          </div>
        )}
        <div className="form-group-date-time">
          <div className="form-field">
            <label htmlFor="workDate">
              Fecha trabajada (domingo/festivo){" "}
              <span className="required">*</span>
            </label>
            <input
              type="date"
              id="workDate"
              name="workDate"
              value={form.workDate}
              onChange={handleChange}
              required
              aria-required="true"
            />
          </div>
          <div className="form-field">
            <label htmlFor="requestedCompensationDate">
              Día solicitado como compensación{" "}
              <span className="required">*</span>
            </label>
            <input
              type="date"
              id="requestedCompensationDate"
              name="requestedCompensationDate"
              value={form.requestedCompensationDate}
              onChange={handleChange}
              required
              aria-required="true"
            />
          </div>
        </div>
        <div className="observaciones-container">
          <label htmlFor="justification">
            Justificación o comentario (opcional)
          </label>
          <textarea
            id="justification"
            name="justification"
            value={form.justification}
            onChange={handleChange}
            placeholder="Ingrese cualquier información relevante"
            rows="4"
          />
        </div>
        <div className="submit-container">
          <button
            type="submit"
            disabled={
              loading ||
              (!isSuperuser && !getEmployeeIdFromToken()) ||
              (isSuperuser && !employeeId)
            }
            className={loading ? "loading" : ""}
          >
            {loading ? (
              <span>
                <div className="loading-spinner-button"></div>Enviando...
              </span>
            ) : (
              "Solicitar Día de Compensación"
            )}
          </button>
        </div>
        {error && (
          <div className="error-message" role="alert">
            <p>Error: {error}</p>
          </div>
        )}
        {!isSuperuser && !getEmployeeIdFromToken() && (
          <div className="error-message">
            No se pudo identificar al usuario autenticado.
          </div>
        )}
      </form>
    </div>
  );
}
