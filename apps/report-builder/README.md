# 📊 JEGASolutions Report Builder

Sistema multi-tenant de construcción y gestión de reportes con capacidades avanzadas de análisis mediante IA y generación automática de narrativas.

## 🏗️ Arquitectura

- **Backend**: ASP.NET Core 9.0 con Clean Architecture
- **Frontend**: React 18 + Vite + TailwindCSS + React Router
- **Base de datos**: PostgreSQL 15 con snake_case naming convention
- **Autenticación**: JWT con SSO integrado (Landing API)
- **IA**: Múltiples proveedores (OpenAI, Anthropic, DeepSeek, Groq, Ollama)
- **Exportación**: PDF, Excel, CSV con motores especializados

## 📋 Requisitos Previos

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Node.js 18+](https://nodejs.org/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [Docker Compose](https://docs.docker.com/compose/)

## 🚀 Inicio Rápido con Docker

### 1. Clonar y navegar al proyecto

```bash
cd apps/report-builder
```

### 2. Crear la migración inicial (primera vez)

**Desde PowerShell o CMD (NO desde Git Bash):**

```powershell
# Navegar al directorio backend
cd backend

# Crear la migración inicial
dotnet ef migrations add InitialCreate `
  --project src/JEGASolutions.ReportBuilder.Data `
  --startup-project src/JEGASolutions.ReportBuilder.API

# Volver al directorio report-builder
cd ..
```

### 3. Iniciar todos los servicios con Docker

```bash
docker-compose up -d
```

Esto iniciará:

- ✅ PostgreSQL en puerto **5433**
- ✅ Backend API en puerto **5000**
- ✅ Frontend en puerto **3001**

### 4. Verificar que todo funciona

```bash
# Ver logs del backend
docker-compose logs -f backend

# Ver logs del frontend
docker-compose logs -f frontend

# Ver todos los logs
docker-compose logs -f
```

### 5. Acceder a la aplicación

- **Frontend**: http://localhost:3001
- **Backend API**: http://localhost:5000
- **Swagger**: http://localhost:5000/swagger

### 6. Credenciales de acceso

**Usuario Admin:**

- Email: `admin@jegasolutions.com`
- Password: `password123`

**Usuario Regular:**

- Email: `user@jegasolutions.com`
- Password: `password123`

## 🛠️ Desarrollo Local (sin Docker)

### Backend

```powershell
cd backend

# Restaurar dependencias
dotnet restore

# Crear migración (primera vez)
dotnet ef migrations add InitialCreate `
  --project src/JEGASolutions.ReportBuilder.Data `
  --startup-project src/JEGASolutions.ReportBuilder.API

# Aplicar migraciones
dotnet ef database update `
  --project src/JEGASolutions.ReportBuilder.Data `
  --startup-project src/JEGASolutions.ReportBuilder.API

# Ejecutar aplicación
cd src/JEGASolutions.ReportBuilder.API
dotnet run
```

La API estará disponible en: http://localhost:5000

### Frontend

```bash
cd frontend

# Instalar dependencias
npm install

# Ejecutar en modo desarrollo
npm run dev
```

El frontend estará disponible en: http://localhost:3001

## ✨ Características Principales

### 🎯 **Generación Automática de Narrativas con IA**

- **Múltiples proveedores de IA**: OpenAI, Anthropic Claude, DeepSeek, Groq, Ollama
- **Generación inline**: Panel expandible en componentes de texto
- **Configuración avanzada**: Idioma, tono, tipo de análisis
- **Análisis automático**: Al cargar datos de Excel
- **Vista previa live**: Resultado inmediato en el editor

### 📊 **Editor de Plantillas Avanzado**

- **Template Editor**: Creación visual de plantillas
- **Hybrid Builder**: Constructor híbrido con pasos guiados
- **Componentes drag & drop**: Texto, gráficos, tablas, KPIs
- **Secciones dinámicas**: Organización modular de contenido
- **Preview en tiempo real**: Visualización instantánea

### 📈 **Análisis de Datos Inteligente**

- **Carga de Excel**: Procesamiento automático de archivos
- **Análisis estadístico**: Tendencias, correlaciones, insights
- **Visualizaciones**: Gráficos automáticos con Chart.js y Recharts
- **KPIs automáticos**: Métricas clave calculadas dinámicamente

### 🔐 **Sistema de Autenticación SSO**

- **Integración con Landing**: SSO centralizado
- **Middleware de acceso**: Validación de módulos por usuario
- **JWT tokens**: Autenticación segura con claims
- **Multi-tenant**: Aislamiento por tenant

### 📤 **Exportación Multi-formato**

- **PDF**: Generación con iTextSharp
- **Excel**: Exportación con ClosedXML
- **CSV**: Datos estructurados
- **Templates personalizados**: Sistema de plantillas reutilizables

### 🔗 **Plantillas Consolidadas**

- **Consolidated Templates**: Plantillas que combinan múltiples fuentes de datos
- **Secciones modulares**: Organización por secciones independientes
- **Reutilización**: Plantillas base que se pueden adaptar
- **Gestión centralizada**: Control de versiones y actualizaciones

## 📦 Estructura del Proyecto

```
apps/report-builder/
├── backend/
│   ├── src/
│   │   ├── JEGASolutions.ReportBuilder.API/          # Controladores, Program.cs, Middleware
│   │   ├── JEGASolutions.ReportBuilder.Core/         # Entidades, DTOs, Interfaces
│   │   ├── JEGASolutions.ReportBuilder.Data/         # DbContext, Migraciones
│   │   └── JEGASolutions.ReportBuilder.Infrastructure/ # Repositorios, Servicios, AI Providers
│   ├── Dockerfile
│   └── JEGASolutions.ReportBuilder.sln
├── frontend/
│   ├── src/
│   │   ├── components/      # Componentes React optimizados
│   │   ├── contexts/        # Context API (Auth, Tenant)
│   │   ├── pages/          # Páginas de la aplicación
│   │   ├── services/       # Servicios de API centralizados
│   │   └── utils/          # Utilidades y feature flags
│   ├── Dockerfile
│   └── package.json
├── db-init/
│   └── 01-init-data.sql    # Datos iniciales (usuarios, áreas, plantillas)
├── docker-compose.yml
└── README.md
```

## 🔐 Autenticación y Seguridad

### Sistema de Autenticación SSO

- **SSO Centralizado**: Integración con Landing API
- **JWT Tokens** con claims: `sub`, `email`, `userId`, `role`, `tenant_id`
- **Expiración**: 60 minutos (configurable)
- **Middleware de acceso**: Validación automática de módulos
- **Multi-tenant**: Aislamiento por tenant automático

### Flujo de Autenticación

1. **Acceso desde Landing**: Usuario se autentica en Landing
2. **Redirección con token**: Landing redirige con JWT token
3. **Validación automática**: Report Builder valida token y acceso al módulo
4. **Sesión activa**: Usuario accede directamente al dashboard

### Endpoints de Autenticación

```bash
# Verificar token (SSO)
GET /api/auth/verify
Authorization: Bearer <token>

# Health check
GET /health
```

### Configuración SSO

```json
{
  "LandingApi": {
    "BaseUrl": "https://jegasolutions-platform.onrender.com",
    "ModuleName": "report-builder"
  },
  "JwtSettings": {
    "SecretKey": "shared-secret-key",
    "Issuer": "JEGASolutions.Landing.API",
    "Audience": "jegasolutions-landing-client",
    "ExpiryMinutes": 60
  }
}
```

## 🗄️ Base de Datos

### Esquema Principal

**Entidades:**

- `users` - Usuarios del sistema
- `areas` - Áreas organizacionales
- `templates` - Plantillas de reportes
- `report_submissions` - Reportes enviados
- `ai_insights` - Insights generados por IA
- `consolidated_templates` - Plantillas consolidadas
- `consolidated_template_sections` - Secciones de plantillas consolidadas
- `excel_uploads` - Archivos Excel cargados

### Multi-Tenancy

Todas las entidades heredan de `TenantEntity`:

- `tenant_id` - ID del tenant (default: 1)
- `created_at` - Fecha de creación
- `updated_at` - Fecha de actualización
- `deleted_at` - Soft delete

### Migraciones

```powershell
# Crear nueva migración
dotnet ef migrations add <NombreMigracion> `
  --project src/JEGASolutions.ReportBuilder.Data `
  --startup-project src/JEGASolutions.ReportBuilder.API

# Aplicar migraciones
dotnet ef database update `
  --project src/JEGASolutions.ReportBuilder.Data `
  --startup-project src/JEGASolutions.ReportBuilder.API

# Listar migraciones
dotnet ef migrations list `
  --project src/JEGASolutions.ReportBuilder.Data `
  --startup-project src/JEGASolutions.ReportBuilder.API

# Revertir última migración
dotnet ef migrations remove `
  --project src/JEGASolutions.ReportBuilder.Data `
  --startup-project src/JEGASolutions.ReportBuilder.API
```

## 🤖 Integración con Múltiples Proveedores de IA

El sistema soporta múltiples proveedores de IA para máxima flexibilidad y disponibilidad.

### Proveedores Soportados

| Proveedor     | Modelo            | Características                             |
| ------------- | ----------------- | ------------------------------------------- |
| **OpenAI**    | GPT-4o-mini       | Análisis avanzado, narrativas profesionales |
| **Anthropic** | Claude-3.5-Sonnet | Excelente para análisis financiero          |
| **DeepSeek**  | DeepSeek-Chat     | Rápido y económico                          |
| **Groq**      | Llama-3.3-70B     | Ultra rápido, ideal para desarrollo         |
| **Ollama**    | Llama3.2          | Local, privacidad total                     |

### Configuración de Proveedores

```json
{
  "AI": {
    "OpenAI": {
      "ApiKey": "sk-...",
      "Model": "gpt-4o-mini"
    },
    "Anthropic": {
      "ApiKey": "sk-ant-...",
      "Model": "claude-3-5-sonnet-20241022",
      "TimeoutSeconds": 120
    },
    "DeepSeek": {
      "ApiKey": "sk-...",
      "Model": "deepseek-chat",
      "Endpoint": "https://api.deepseek.com"
    },
    "Groq": {
      "ApiKey": "gsk_...",
      "Model": "llama-3.3-70b-versatile"
    },
    "Ollama": {
      "Endpoint": "http://localhost:11434",
      "Model": "llama3.2",
      "TimeoutSeconds": 600
    }
  }
}
```

### Variables de Entorno

```bash
# Configurar proveedores (opcional)
OPENAI_API_KEY=sk-...
ANTHROPIC_API_KEY=sk-ant-...
DEEPSEEK_API_KEY=sk-...
GROQ_API_KEY=gsk_...

# Ollama (local)
OLLAMA_ENDPOINT=http://localhost:11434
```

### Características de IA

- **Generación de narrativas**: Texto profesional automático
- **Análisis de datos**: Insights y tendencias
- **Configuración flexible**: Idioma, tono, tipo de análisis
- **Fallback automático**: Si un proveedor falla, usa otro
- **Caché inteligente**: Reutiliza análisis similares

## 📝 Comandos Útiles de Docker

```bash
# Iniciar servicios
docker-compose up -d

# Ver logs
docker-compose logs -f [servicio]

# Detener servicios
docker-compose stop

# Reiniciar un servicio
docker-compose restart [servicio]

# Eliminar contenedores y volúmenes
docker-compose down -v

# Reconstruir imágenes
docker-compose build --no-cache

# Ver estado de servicios
docker-compose ps

# Acceder a la base de datos
docker exec -it reportbuilder-postgres psql -U postgres -d reportbuilder_db

# Ver tablas en la base de datos
docker exec -it reportbuilder-postgres psql -U postgres -d reportbuilder_db -c "\dt"
```

## 🧪 Testing

### Backend

```bash
cd backend
dotnet test
```

### Frontend

```bash
cd frontend
npm test
```

## 🐛 Troubleshooting

### Problema: Error 404 en login (SSO)

**Síntomas:** Error 404 al intentar hacer login desde Landing
**Causa:** URL del API mal configurada en el frontend

**Solución:**

```bash
# Verificar configuración de API URL
echo $VITE_API_URL

# Debe ser: http://localhost:5000 (sin /api)
# El código añade /api automáticamente

# Verificar que el backend responde
curl http://localhost:5000/health
```

### Problema: Backend no puede conectarse a PostgreSQL

**Síntomas:** Error de conexión a base de datos
**Solución:**

```bash
# Verificar que PostgreSQL está corriendo
docker-compose ps

# Ver logs de PostgreSQL
docker-compose logs postgres

# Reiniciar PostgreSQL
docker-compose restart postgres

# Verificar migraciones automáticas
docker-compose logs backend | grep "migrations"
```

### Problema: Puerto 5433 ya está en uso

**Síntomas:** Error al iniciar PostgreSQL
**Solución:**

```bash
# Cambiar el puerto en docker-compose.yml
ports:
  - "5434:5432"  # Usa otro puerto

# Actualizar variables de entorno
DB_HOST=localhost
DB_PORT=5434
```

### Problema: Error de acceso al módulo (SSO)

**Síntomas:** "Access denied" después del login
**Causa:** Usuario no tiene acceso al módulo report-builder

**Solución:**

```bash
# Verificar en Landing API que el usuario tiene acceso
# El middleware ModuleAccessMiddleware valida automáticamente

# Verificar logs del backend
docker-compose logs backend | grep "ModuleAccess"
```

### Problema: IA no responde o falla

**Síntomas:** Error al generar narrativas con IA
**Solución:**

```bash
# Verificar configuración de proveedores
docker-compose logs backend | grep "AI"

# Configurar al menos un proveedor
export OPENAI_API_KEY=sk-...

# Reiniciar backend
docker-compose restart backend
```

### Problema: Frontend no puede conectarse al backend

**Síntomas:** Error de red en el frontend
**Solución:**

```bash
# Verificar que el backend está corriendo
curl http://localhost:5000/health

# Verificar configuración CORS
docker-compose logs backend | grep "CORS"

# Revisar la configuración en frontend
# Debe apuntar a: http://localhost:5000 (sin /api)
```

### Problema: "dotnet: command not found" en Git Bash

**Síntomas:** Error al ejecutar comandos dotnet
**Solución:**

```bash
# Usar PowerShell o CMD en Windows
# O agregar dotnet al PATH en Git Bash

# Alternativa: usar Docker para todo
docker-compose up -d  # Las migraciones se aplican automáticamente
```

### Problema: Error de CORS en producción

**Síntomas:** CORS error en el navegador
**Solución:**

```bash
# Verificar configuración CORS en Program.cs
# Debe incluir el dominio de producción

# Verificar variables de entorno
echo $ASPNETCORE_ENVIRONMENT
```

### Problema: Migraciones no se aplican automáticamente

**Síntomas:** Base de datos sin tablas
**Solución:**

```bash
# Verificar logs de migraciones
docker-compose logs backend | grep "migrations"

# Aplicar migraciones manualmente si es necesario
docker exec -it reportbuilder-backend dotnet ef database update
```

## 📚 Endpoints API Principales

### Autenticación y Salud

- `GET /api/auth/verify` - Verificar token SSO
- `GET /health` - Health check del servicio

### Templates

- `GET /api/templates` - Listar plantillas
- `GET /api/templates/{id}` - Obtener plantilla
- `POST /api/templates` - Crear plantilla
- `PUT /api/templates/{id}` - Actualizar plantilla
- `DELETE /api/templates/{id}` - Eliminar plantilla
- `GET /api/templates/by-type/{type}` - Filtrar por tipo

### Report Submissions

- `GET /api/reportsubmissions` - Listar reportes
- `GET /api/reportsubmissions/{id}` - Obtener reporte
- `POST /api/reportsubmissions` - Crear reporte
- `PUT /api/reportsubmissions/{id}` - Actualizar reporte
- `DELETE /api/reportsubmissions/{id}` - Eliminar reporte

### Consolidated Templates

- `GET /api/consolidatedtemplates` - Listar plantillas consolidadas
- `GET /api/consolidatedtemplates/{id}` - Obtener plantilla consolidada
- `POST /api/consolidatedtemplates` - Crear plantilla consolidada
- `PUT /api/consolidatedtemplates/{id}` - Actualizar plantilla consolidada
- `DELETE /api/consolidatedtemplates/{id}` - Eliminar plantilla consolidada

### Excel Uploads

- `GET /api/exceluploads` - Listar archivos Excel
- `POST /api/exceluploads` - Subir archivo Excel
- `GET /api/exceluploads/{id}` - Obtener archivo Excel
- `DELETE /api/exceluploads/{id}` - Eliminar archivo Excel

### AI Analysis y Narrativas

- `POST /api/aianalysis/analyze` - Analizar datos con IA
- `GET /api/aianalysis/insights/{reportId}` - Obtener insights
- `POST /api/narrative/generate` - Generar narrativa con IA
- `POST /api/analytics/analyze` - Análisis estadístico avanzado

## 🔧 Variables de Entorno

### Backend (docker-compose.yml o appsettings.json)

```env
# Configuración Base
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=http://+:8080

# Base de Datos
DB_HOST=postgres
DB_NAME=reportbuilderdb
DB_USER=postgres
DB_PASSWORD=jega40

# JWT y SSO
JWT_SECRET=SuperClaveUltraSecretaParaReportes123456789RdollarTpercentU!
JWT_ISSUER=ReportBuilderAPI
JWT_AUDIENCE=report-builder-client

# Proveedores de IA (opcionales)
OPENAI_API_KEY=placeholder
ANTHROPIC_API_KEY=
DEEPSEEK_API_KEY=
GROQ_API_KEY=
```

### Frontend

```env
# URL del API (se añade /api automáticamente)
VITE_API_URL=http://localhost:5000
```

### Configuración de Producción

```env
# Landing API Integration
LANDING_API_BASE_URL=https://jegasolutions-platform.onrender.com
LANDING_API_MODULE_NAME=report-builder

# Database (Producción)
DB_HOST=your-production-db-host
DB_NAME=reportbuilder_prod
DB_USER=reportbuilder_user
DB_PASSWORD=your-secure-password

# AI Providers (Producción)
OPENAI_API_KEY=sk-proj-...
ANTHROPIC_API_KEY=sk-ant-...
DEEPSEEK_API_KEY=sk-...
GROQ_API_KEY=gsk_...
```

## 📖 Documentación Adicional

- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Entity Framework Core](https://docs.microsoft.com/ef/core/)
- [ASP.NET Core](https://docs.microsoft.com/aspnet/core/)
- [React](https://react.dev/)
- [Vite](https://vitejs.dev/)

## 🤝 Contribuir

1. Crear una rama para tu feature: `git checkout -b feature/nueva-funcionalidad`
2. Commit tus cambios: `git commit -m 'Agregar nueva funcionalidad'`
3. Push a la rama: `git push origin feature/nueva-funcionalidad`
4. Crear un Pull Request

## 🚀 Estado Actual del Proyecto

### ✅ Funcionalidades Implementadas

- **✅ SSO Integration**: Autenticación centralizada con Landing API
- **✅ Multi-AI Providers**: OpenAI, Anthropic, DeepSeek, Groq, Ollama
- **✅ Template Editor**: Creación visual de plantillas
- **✅ Hybrid Builder**: Constructor híbrido con pasos guiados
- **✅ AI Narrative Generation**: Generación automática de narrativas
- **✅ Excel Processing**: Carga y análisis de archivos Excel
- **✅ Consolidated Templates**: Plantillas que combinan múltiples fuentes
- **✅ Export System**: PDF, Excel, CSV con motores especializados
- **✅ Multi-tenant**: Aislamiento por tenant automático
- **✅ Real-time Preview**: Vista previa en tiempo real
- **✅ Drag & Drop**: Componentes arrastrables
- **✅ Auto-migrations**: Migraciones automáticas en Docker

### 🔄 En Desarrollo

- **🔄 Advanced Analytics**: Análisis estadístico más profundo
- **🔄 Template Sharing**: Compartir plantillas entre usuarios
- **🔄 Batch Processing**: Procesamiento masivo de reportes
- **🔄 API Rate Limiting**: Control de límites de API

### 📊 Métricas del Proyecto

- **Backend**: 5 controladores, 15+ endpoints
- **Frontend**: 30+ componentes React optimizados
- **AI Providers**: 5 proveedores configurados
- **Database**: 8 entidades principales con snake_case
- **Docker**: 3 servicios (PostgreSQL, Backend, Frontend)

### 🎯 Próximas Características

- **📱 Mobile App**: Aplicación móvil nativa
- **🔔 Notifications**: Sistema de notificaciones
- **📊 Advanced Charts**: Más tipos de visualizaciones
- **🤖 AI Training**: Entrenamiento de modelos personalizados
- **🌐 Multi-language**: Soporte multi-idioma completo

## 📄 Licencia

© 2024 JEGASolutions. Todos los derechos reservados.

## 👥 Contacto

Para soporte o preguntas, contacta al equipo de desarrollo de JEGASolutions.

---

**¡Feliz Desarrollo! 🚀**
