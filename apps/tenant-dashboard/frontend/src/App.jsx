import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from 'react-query';
import { ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';

// Context Providers
import { AuthProvider } from './contexts/AuthContext';
import { TenantProvider } from './contexts/TenantContext';

// Pages
import LoginPage from './pages/LoginPage';
import TenantDashboard from './pages/TenantDashboard';

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
                {/* Rutas con tenant en path: /t/:tenant/* */}
                <Route path="/t/:tenant/login" element={<TenantDashboard />} />
                <Route
                  path="/t/:tenant/dashboard"
                  element={<TenantDashboard />}
                />
                <Route path="/t/:tenant" element={<TenantDashboard />} />

                {/* Rutas base (para subdominios o query params) */}
                <Route path="/login" element={<TenantDashboard />} />
                <Route path="/dashboard" element={<TenantDashboard />} />
                <Route path="/" element={<TenantDashboard />} />
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
