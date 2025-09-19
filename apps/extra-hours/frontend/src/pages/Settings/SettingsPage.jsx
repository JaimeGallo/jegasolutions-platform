import { useNavigate, Outlet, useLocation } from "react-router-dom";
import "./SettingsPage.scss";
import { Settings, Users } from "lucide-react";

const SettingsPage = () => {
  const navigate = useNavigate();
  const location = useLocation();

  return (
    <div className="settings-page-container">
      <header className="page__header"></header>
      <h2 className="settings-title">Configuraciones</h2>
      <p className="settings-subtitle">
        Administra los parámetros del sistema y la gestión de empleados
      </p>
      <div className="settings-grid">
        <div
          className={`settings-card${
            location.pathname.includes("ExtraHoursSettings") ? " active" : ""
          }`}
          onClick={() => navigate("/settings/ExtraHoursSettings")}
        >
          <div className="settings-card-icon">
            <Settings size={28} />
          </div>
          <div className="settings-card-title">Parámetros Horas Extra</div>
          <div className="settings-card-desc">
            Configura los límites y reglas de horas extra
          </div>
        </div>
        <div
          className={`settings-card${
            location.pathname.includes("EmployeeManagement") ? " active" : ""
          }`}
          onClick={() => navigate("/settings/EmployeeManagement")}
        >
          <div className="settings-card-icon">
            <Users size={28} />
          </div>
          <div className="settings-card-title">Gestionar Empleados</div>
          <div className="settings-card-desc">
            Agrega, edita y administra empleados del sistema
          </div>
        </div>
      </div>
      <Outlet />
    </div>
  );
};

export default SettingsPage;
