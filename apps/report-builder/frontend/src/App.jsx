import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import { QueryClient, QueryClientProvider } from "react-query";
import { ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";

// Context Providers
import { AuthProvider } from "./contexts/AuthContext";
import { TenantProvider } from "./contexts/TenantContext";

// Pages
import LoginPage from "./pages/LoginPage";
import DashboardPage from "./pages/DashboardPage";
import TemplatesPage from "./pages/TemplatesPage";
import ReportsPage from "./pages/ReportsPage";
import AIAnalysisPage from "./pages/AIAnalysisPage";
import TemplateEditorPage from "./pages/TemplateEditorPage";
import ConsolidatedTemplatesPage from "./pages/ConsolidatedTemplatesPage";
import MyTasksPage from "./pages/MyTasksPage";
import ExcelUploadsPage from "./pages/ExcelUploadsPage";
import HybridTemplateBuilderPageOptimized from "./pages/HybridTemplateBuilderPageOptimized";

// Components
import PrivateRoute from "./components/PrivateRoute";
import Layout from "./components/Layout";

// Create a client
const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      retry: 1,
      refetchOnWindowFocus: false,
    },
  },
});

function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <AuthProvider>
        <TenantProvider>
          <Router>
            <div className="min-h-screen bg-gray-50">
              <Routes>
                <Route path="/login" element={<LoginPage />} />

                {/* Template Editor - Fullscreen sin sidebar */}
                <Route
                  path="/templates/create"
                  element={
                    <PrivateRoute>
                      <TemplateEditorPage />
                    </PrivateRoute>
                  }
                />
                <Route
                  path="/templates/:id/edit"
                  element={
                    <PrivateRoute>
                      <TemplateEditorPage />
                    </PrivateRoute>
                  }
                />

                {/* Hybrid Builder - Fullscreen sin sidebar */}
                <Route
                  path="/hybrid-builder"
                  element={
                    <PrivateRoute>
                      <HybridTemplateBuilderPageOptimized />
                    </PrivateRoute>
                  }
                />

                <Route
                  path="/"
                  element={
                    <PrivateRoute>
                      <Layout />
                    </PrivateRoute>
                  }
                >
                  <Route index element={<DashboardPage />} />
                  <Route path="templates" element={<TemplatesPage />} />
                  <Route path="reports" element={<ReportsPage />} />
                  <Route
                    path="reports/:id/analysis"
                    element={<AIAnalysisPage />}
                  />
                  <Route
                    path="consolidated-templates"
                    element={<ConsolidatedTemplatesPage />}
                  />
                  <Route path="my-tasks" element={<MyTasksPage />} />
                  <Route path="excel-uploads" element={<ExcelUploadsPage />} />
                </Route>
              </Routes>

              <ToastContainer
                position="top-right"
                autoClose={3000}
                hideProgressBar={false}
                newestOnTop={true}
                closeOnClick
                pauseOnHover
                draggable
                theme="colored"
              />
            </div>
          </Router>
        </TenantProvider>
      </AuthProvider>
    </QueryClientProvider>
  );
}

export default App;
