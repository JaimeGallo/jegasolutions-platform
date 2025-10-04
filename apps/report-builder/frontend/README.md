# 📊 Report Builder - Frontend

Frontend moderno para el sistema de Report Builder con IA, construido con React + Vite + TailwindCSS.

## 🚀 Quick Start

### Prerequisitos
- Node.js 18+
- npm o pnpm

### Instalación

```bash
cd apps/report-builder/frontend
npm install
```

### Desarrollo

```bash
npm run dev
```

La aplicación estará disponible en `http://localhost:5173`

### Build

```bash
npm run build
```

### Preview Build

```bash
npm run preview
```

---

## 📁 Estructura del Proyecto

```
src/
├── components/          # Componentes reutilizables
│   ├── AIConfigPanel.jsx       ✨ Panel de configuración AI
│   ├── ExcelUpload.jsx         ✨ Upload con drag & drop
│   ├── Header.jsx
│   ├── Layout.jsx
│   ├── PrivateRoute.jsx
│   └── Sidebar.jsx
│
├── contexts/           # Context providers
│   ├── AuthContext.jsx
│   └── TenantContext.jsx
│
├── pages/             # Páginas principales
│   ├── AIAnalysisPage.jsx
│   ├── ConsolidatedTemplatesPage.jsx  ✨ Admin dashboard
│   ├── DashboardPage.jsx
│   ├── ExcelUploadsPage.jsx          ✨ Upload & AI analysis
│   ├── LoginPage.jsx
│   ├── MyTasksPage.jsx               ✨ User tasks
│   ├── ReportsPage.jsx
│   ├── TemplateEditorPage.jsx
│   └── TemplatesPage.jsx
│
├── services/          # API services
│   ├── aiService.js
│   ├── api.js
│   ├── authService.js
│   ├── consolidatedTemplateService.js  ✨ Consolidated templates API
│   ├── excelUploadService.js          ✨ Excel upload & AI API
│   ├── reportService.js
│   └── templateService.js
│
├── App.jsx           # Main app component
├── main.jsx          # Entry point
└── index.css         # Global styles
```

---

## 🎨 Componentes Principales

### 1. ConsolidatedTemplatesPage
**Ruta:** `/consolidated-templates`

Dashboard administrativo para gestionar plantillas consolidadas multi-área.

**Características:**
- Lista de todas las plantillas con progreso visual
- Modal de detalles con secciones
- Acciones: Ver, Editar, Eliminar
- Estados con códigos de color

### 2. MyTasksPage
**Ruta:** `/my-tasks`

Vista personal de tareas asignadas al usuario.

**Características:**
- Filtros por estado (Todas, Pendientes, En Progreso, Completadas)
- Indicadores de urgencia
- Navegación directa a completar tareas

### 3. ExcelUploadsPage
**Ruta:** `/excel-uploads`

Página completa de carga y análisis de Excel con IA.

**Características:**
- Upload con drag & drop
- Lista de archivos cargados
- Selección de proveedor de IA
- Panel de configuración de análisis
- Visualización de resultados

### 4. ExcelUpload Component
Componente reutilizable para upload de Excel.

**Uso:**
```jsx
import ExcelUpload from '../components/ExcelUpload';

<ExcelUpload
  areaId={1}
  period="Abril 2025"
  onUploadSuccess={(result) => console.log(result)}
/>
```

### 5. AIConfigPanel Component
Panel de configuración para análisis con IA.

**Uso:**
```jsx
import AIConfigPanel from '../components/AIConfigPanel';

<AIConfigPanel
  onAnalyze={(config) => console.log(config)}
  isLoading={false}
/>
```

---

## 🔌 API Integration

### Base URL
```javascript
// Development
const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000/api';

// Production
const API_BASE_URL = 'https://api.jegasolutions.co';
```

### Services

#### ConsolidatedTemplateService
```javascript
import { ConsolidatedTemplateService } from './services/consolidatedTemplateService';

// Obtener todas las plantillas
const templates = await ConsolidatedTemplateService.getConsolidatedTemplates();

// Obtener secciones asignadas al usuario
const tasks = await ConsolidatedTemplateService.getMyAssignedSections();
```

#### ExcelUploadService
```javascript
import { ExcelUploadService } from './services/excelUploadService';

// Subir archivo
const result = await ExcelUploadService.uploadExcel(file, areaId, period);

// Analizar con IA
const analysis = await ExcelUploadService.analyzeWithAI(
  uploadId,
  'anthropic',
  'summary',
  'Analiza las ventas del Q1'
);
```

---

## 🎨 Estilos y Diseño

### TailwindCSS
El proyecto usa TailwindCSS para estilos. Configuración en `tailwind.config.js`.

### Paleta de Colores
```css
/* Primary */
blue-600: #2563eb
purple-600: #9333ea

/* Success */
green-600: #22c55e

/* Warning */
yellow-600: #eab308

/* Danger */
red-600: #ef4444
```

### Gradientes Comunes
```jsx
// Background gradientes
className="bg-gradient-to-br from-blue-50 to-white"
className="bg-gradient-to-br from-gray-50 to-blue-50"

// Button gradientes
className="bg-gradient-to-r from-blue-600 to-purple-600"
className="hover:from-blue-700 hover:to-purple-700"
```

---

## 🔐 Autenticación

### AuthContext
```jsx
import { useAuth } from './contexts/AuthContext';

const { user, login, logout, isAuthenticated } = useAuth();
```

### PrivateRoute
```jsx
<Route
  path="/protected"
  element={
    <PrivateRoute>
      <ProtectedPage />
    </PrivateRoute>
  }
/>
```

---

## 🌐 Multi-Tenancy

### TenantContext
```jsx
import { useTenant } from './contexts/TenantContext';

const { tenantId, tenantName } = useTenant();
```

---

## 📦 Dependencias Principales

```json
{
  "dependencies": {
    "react": "^18.2.0",
    "react-dom": "^18.2.0",
    "react-router-dom": "^6.8.1",
    "axios": "^1.6.2",
    "react-toastify": "^9.1.3",
    "chart.js": "^4.4.0",
    "react-chartjs-2": "^5.2.0",
    "react-hook-form": "^7.48.2",
    "react-query": "^3.39.3",
    "lucide-react": "^0.294.0",
    "clsx": "^2.0.0"
  }
}
```

---

## 🧪 Testing

### Manual Testing
1. Iniciar el backend en `http://localhost:5000`
2. Iniciar el frontend en `http://localhost:5173`
3. Login con credenciales de prueba
4. Navegar a cada página y verificar funcionalidad

### Testing Checklist
- [ ] Login/Logout funciona
- [ ] Navegación entre páginas
- [ ] Upload de Excel
- [ ] Análisis con IA
- [ ] Gestión de plantillas consolidadas
- [ ] Vista de tareas personales

---

## 🚀 Deployment

### Variables de Entorno

Crear archivo `.env` en la raíz del frontend:

```env
VITE_API_URL=https://api.jegasolutions.co
```

### Build para Producción

```bash
npm run build
```

El build se genera en la carpeta `dist/`

### Deploy a Vercel

```bash
# Instalar Vercel CLI
npm i -g vercel

# Deploy
vercel --prod
```

### Configuración Vercel
- Framework Preset: Vite
- Build Command: `npm run build`
- Output Directory: `dist`
- Environment Variables: `VITE_API_URL`

---

## 📝 Notas de Desarrollo

### Hot Module Replacement (HMR)
Vite incluye HMR por defecto. Los cambios se reflejan instantáneamente.

### ESLint
```bash
npm run lint
```

### Formato de Código
Se recomienda usar Prettier con la configuración del proyecto.

---

## 🐛 Troubleshooting

### Error: Cannot connect to API
**Solución:** Verificar que el backend esté corriendo y la variable `VITE_API_URL` esté configurada correctamente.

### Error: 401 Unauthorized
**Solución:** El token ha expirado. Hacer logout y login nuevamente.

### Error: Upload failed
**Solución:** Verificar que el archivo sea .xlsx, .xls o .csv y no exceda 50MB.

---

## 📚 Recursos

- [React Docs](https://react.dev/)
- [Vite Docs](https://vitejs.dev/)
- [TailwindCSS Docs](https://tailwindcss.com/)
- [React Router Docs](https://reactrouter.com/)
- [Axios Docs](https://axios-http.com/)
- [React Toastify Docs](https://fkhadra.github.io/react-toastify/)

---

## 👥 Contribución

Para contribuir al proyecto:
1. Crear un branch feature
2. Hacer commits con mensajes descriptivos
3. Asegurar que no hay errores de lint
4. Crear Pull Request

---

## 📄 Licencia

Propiedad de JEGASolutions - Todos los derechos reservados.

---

**🎉 Happy Coding!**

