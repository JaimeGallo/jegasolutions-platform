# 📊 JEGASolutions Report Builder

Sistema multi-tenant de construcción y gestión de reportes con capacidades de análisis mediante IA.

## 🏗️ Arquitectura

- **Backend**: ASP.NET Core 9.0 con Clean Architecture
- **Frontend**: React 18 + Vite + TailwindCSS
- **Base de datos**: PostgreSQL 15
- **Autenticación**: JWT con BCrypt
- **IA**: Azure OpenAI (opcional)

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

## 📦 Estructura del Proyecto

```
apps/report-builder/
├── backend/
│   ├── src/
│   │   ├── JEGASolutions.ReportBuilder.API/          # Controladores, Program.cs
│   │   ├── JEGASolutions.ReportBuilder.Core/         # Entidades, DTOs, Interfaces
│   │   ├── JEGASolutions.ReportBuilder.Data/         # DbContext, Migraciones
│   │   └── JEGASolutions.ReportBuilder.Infrastructure/ # Repositorios, Servicios
│   ├── Dockerfile
│   └── JEGASolutions.ReportBuilder.sln
├── frontend/
│   ├── src/
│   │   ├── components/      # Componentes React
│   │   ├── contexts/        # Context API (Auth, Tenant)
│   │   ├── pages/          # Páginas de la aplicación
│   │   └── services/       # Servicios de API
│   ├── Dockerfile
│   └── package.json
├── db-init/
│   └── 01-init-data.sql    # Datos iniciales (usuarios, áreas, plantillas)
├── docker-compose.yml
└── README.md
```

## 🔐 Autenticación y Seguridad

### Sistema de Autenticación

- **BCrypt** para hash de contraseñas (work factor 11)
- **JWT Tokens** con claims: `sub`, `email`, `tenant_id`, `role`
- **Expiración**: 60 minutos (configurable)
- **Refresh Token**: Endpoint `/api/auth/refresh` implementado

### Endpoints de Autenticación

```bash
# Login
POST /api/auth/login
Content-Type: application/json
{
  "email": "admin@jegasolutions.com",
  "password": "password123"
}

# Verificar token
GET /api/auth/verify
Authorization: Bearer <token>

# Refresh token
POST /api/auth/refresh
Authorization: Bearer <token>
```

## 🗄️ Base de Datos

### Esquema Principal

**Entidades:**
- `users` - Usuarios del sistema
- `areas` - Áreas organizacionales
- `templates` - Plantillas de reportes
- `report_submissions` - Reportes enviados
- `ai_insights` - Insights generados por IA

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

## 🤖 Integración con OpenAI (Opcional)

El sistema puede funcionar completamente sin una API Key de OpenAI. Por defecto usa el valor `placeholder`.

Para habilitar análisis de IA real:

1. Obtén una API Key de [OpenAI](https://platform.openai.com/)
2. Actualiza la variable de entorno:

```bash
# En docker-compose.yml
OPENAI_API_KEY=tu-api-key-real

# O exporta en tu terminal
export OPENAI_API_KEY=tu-api-key-real
```

3. Reinicia el servicio backend:

```bash
docker-compose restart backend
```

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

### Problema: Backend no puede conectarse a PostgreSQL

**Solución:**
```bash
# Verificar que PostgreSQL está corriendo
docker-compose ps

# Ver logs de PostgreSQL
docker-compose logs postgres

# Reiniciar PostgreSQL
docker-compose restart postgres
```

### Problema: Puerto 5433 ya está en uso

**Solución:**
```bash
# Cambiar el puerto en docker-compose.yml
ports:
  - "5434:5432"  # Usa otro puerto

# Actualizar appsettings.Development.json
"DefaultConnection": "Host=localhost;Port=5434;..."
```

### Problema: Las migraciones no se aplican

**Solución:**
```bash
# Detener todos los servicios
docker-compose down -v

# Crear la migración manualmente
cd backend
dotnet ef migrations add InitialCreate --project src/JEGASolutions.ReportBuilder.Data --startup-project src/JEGASolutions.ReportBuilder.API

# Reiniciar servicios
cd ..
docker-compose up -d
```

### Problema: Frontend no puede conectarse al backend

**Solución:**
```bash
# Verificar que el backend está corriendo
curl http://localhost:5000/api/auth/verify

# Revisar la configuración en frontend/.env o vite.config.js
# Debe apuntar a: http://localhost:5000/api
```

### Problema: "dotnet: command not found" en Git Bash

**Solución:**
```bash
# Usar PowerShell o CMD en Windows
# O agregar dotnet al PATH en Git Bash

# Alternativa: usar Docker para todo
docker-compose up -d  # Las migraciones se aplican automáticamente
```

## 📚 Endpoints API Principales

### Autenticación
- `POST /api/auth/login` - Login
- `GET /api/auth/verify` - Verificar token
- `POST /api/auth/refresh` - Refresh token

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

### AI Analysis
- `POST /api/aianalysis/analyze` - Analizar reporte con IA
- `GET /api/aianalysis/insights/{reportId}` - Obtener insights

## 🔧 Variables de Entorno

### Backend (docker-compose.yml o appsettings.json)

```env
ASPNETCORE_ENVIRONMENT=Development
DB_HOST=postgres
DB_NAME=reportbuilder_db
DB_USER=postgres
DB_PASSWORD=password
JWT_SECRET=your-super-secret-key-minimum-32-characters-long
JWT_ISSUER=ReportBuilder.API
JWT_AUDIENCE=ReportBuilder.Client
OPENAI_API_KEY=placeholder
```

### Frontend

```env
VITE_API_URL=http://localhost:5000/api
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

## 📄 Licencia

© 2024 JEGASolutions. Todos los derechos reservados.

## 👥 Contacto

Para soporte o preguntas, contacta al equipo de desarrollo de JEGASolutions.

---

**¡Feliz Desarrollo! 🚀**

