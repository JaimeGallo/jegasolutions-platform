import { useNavigate, Outlet, useLocation } from "react-router-dom";
import "./SettingsPage.scss";
import { Settings, Users, Upload } from "lucide-react";

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
        {/* Card 1: Parámetros Horas Extra */}
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

        {/* Card 2: Gestionar Empleados */}
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

        {/* Card 3: Carga Masiva */}
        <div
          className={`settings-card${
            location.pathname.includes("BulkUpload") ? " active" : ""
          }`}
          onClick={() => navigate("/settings/BulkUpload")}
        >
          <div className="settings-card-icon bulk-upload">
            <Upload size={28} />
          </div>
          <div className="settings-card-title">Carga Masiva de Usuarios</div>
          <div className="settings-card-desc">
            Importa múltiples empleados desde archivo CSV o Excel
          </div>
        </div>
      </div>
      <Outlet />
    </div>
  );
};

export default SettingsPage;