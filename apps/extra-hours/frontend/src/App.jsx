import "./App.scss";
import { BrowserRouter as Router, Route, Routes } from "react-router-dom";
import BulkUploadPage from "./pages/Settings/BulkUploadPage";

// Providers
import { AuthProvider } from "./utils/AuthProvider";
import { ConfigProvider } from "./utils/ConfigProvider";

// Layout & Route Components
import Layout from "./components/layout/Layout";
import ProtectedRoute from "./components/auth/ProtectedRoute";

// Page Components
import ExtraHoursMenu from "./components/layout/ExtraHoursMenu/ExtraHoursMenu";
import LoginPage from "./pages/LoginPage";
import ReportsPage from "./pages/ReportsPage";
import AddExtrahour from "./pages/AddExtrahour";
import UpdateDeleteApprovePage from "./pages/UpdateDeleteApprovePage";
import SolicitudCompensacionPage from "./pages/SolicitudCompensacionPage";
import IngresoAutorizacionPage from "./pages/IngresoAutorizacionPage";
import GestionSolicitudesCompensacionPage from "./pages/GestionSolicitudesCompensacionPage";

// Settings Page Components
import SettingsPage from "./pages/Settings/SettingsPage";
import ExtraHoursSettingsPage from "./pages/Settings/ExtraHoursSettingsPage";
import EmployeeManagementPage from "./pages/Settings/EmployeeManagementPage";
// import UpdateDeletePersonal from "./components/UpdateDeletePersonal/UpdateDeletePersonal";

// Helper component to avoid repetition
const ProtectedRouteWithLayout = ({ allowedRoles, element }) => (
  <ProtectedRoute
    allowedRoles={allowedRoles}
    element={<Layout>{element}</Layout>}
  />
);

function App() {
  return (
    <Router>
      <AuthProvider>
        <ConfigProvider>
          <Routes>
            <Route path="/" element={<LoginPage />} />

            <Route
              path="/menu"
              element={
                <ProtectedRouteWithLayout 
                  allowedRoles={["empleado", "manager", "superusuario"]}
                  element={<ExtraHoursMenu />} 
                />
              }
            />
            <Route
              path="/add"
              element={
                <ProtectedRouteWithLayout
                  allowedRoles={["empleado", "manager", "superusuario"]}
                  element={<AddExtrahour />}
                />
              }
            />
            <Route
              path="/reports"
              element={
                <ProtectedRouteWithLayout
                  allowedRoles={["manager", "superusuario", "empleado"]}
                  element={<ReportsPage />}
                />
              }
            />
            <Route
              path="/ManagementExtraHour"
              element={
                <ProtectedRouteWithLayout
                  allowedRoles={["manager", "superusuario"]}
                  element={<UpdateDeleteApprovePage />}
                />
              }
            />
            <Route
              path="/settings"
              element={
                <ProtectedRouteWithLayout
                  allowedRoles={["superusuario"]}
                  element={<SettingsPage />}
                />
              }
            >
              <Route
                path="ExtraHoursSettings"
                element={<ExtraHoursSettingsPage />}
              />
              <Route
                path="EmployeeManagement"
                element={<EmployeeManagementPage />}
              />
               <Route
              path="BulkUpload"
              element={<BulkUploadPage />}
            />
            </Route>
           
            <Route
              path="/solicitud-compensacion"
              element={
                <ProtectedRouteWithLayout
                  allowedRoles={["empleado"]}
                  element={<SolicitudCompensacionPage />}
                />
              }
            />
            <Route
              path="/autorizacion-ingreso"
              element={
                <ProtectedRouteWithLayout
                  allowedRoles={["manager", "superusuario"]}
                  element={<IngresoAutorizacionPage />}
                />
              }
            />
            <Route
              path="/gestion-compensacion"
              element={
                <ProtectedRouteWithLayout
                  allowedRoles={["manager", "superusuario"]}
                  element={<GestionSolicitudesCompensacionPage />}
                />
              }
            />
          </Routes>
        </ConfigProvider>
      </AuthProvider>
    </Router>
  );
}

export default App;
