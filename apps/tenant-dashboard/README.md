# 🏢 JEGASolutions - Dashboard Central del Tenant

## 📋 Descripción

Dashboard Central unificado para tenants de JEGASolutions que permite:

- **Vista consolidada** de todos los módulos adquiridos
- **Navegación unificada** entre módulos
- **Estadísticas centralizadas** del tenant
- **Gestión de usuarios** y configuración
- **Acceso directo** a cada módulo

## 🚀 Características

### ✅ **Funcionalidades Implementadas:**

1. **Dashboard Principal** (`/dashboard`)

   - Vista de módulos adquiridos
   - Estadísticas del tenant
   - Acciones rápidas

2. **Módulos Soportados:**

   - **GestorHorasExtra** - Gestión de horas extra
   - **ReportBuilder** - Reportes con IA

3. **Navegación Unificada:**

   - Enlaces directos a cada módulo
   - Estado de disponibilidad
   - Información de características

4. **Autenticación:**
   - Login con JWT
   - Contexto de tenant automático
   - Middleware de autenticación

## 🏗️ Arquitectura

### **Frontend (React + Vite)**

```
apps/tenant-dashboard/frontend/
├── src/
│   ├── components/          # Componentes reutilizables
│   ├── contexts/           # Contextos (Auth, Tenant)
│   ├── pages/              # Páginas principales
│   ├── services/          # APIs y servicios
│   └── utils/              # Utilidades
```

### **Backend (ASP.NET Core)**

```
apps/tenant-dashboard/backend/
├── src/
│   ├── JEGASolutions.TenantDashboard.API/      # API Layer
│   ├── JEGASolutions.TenantDashboard.Core/     # Domain + Application
│   └── JEGASolutions.TenantDashboard.Infrastructure/ # Infrastructure
```

## 🔧 Configuración

### 📚 **Documentación Completa:**

- **[Guía de Configuración DNS/Vercel](./VERCEL_SETUP_GUIDE.md)** - Guía paso a paso para configurar DNS wildcard o path-based routing
- **[Configuración DNS Detallada](./DNS_CONFIGURATION.md)** - Explicación técnica completa de las opciones de DNS
- **[Variables de Entorno](./frontend/ENV_SETUP.md)** - Configuración de variables de entorno para desarrollo y producción

### **Variables de Entorno Backend:**

```env
# Database
DATABASE_URL=postgresql://user:pass@host:port/db

# JWT
JWT_SECRET=your-super-secret-jwt-key
JWT_ISSUER=JEGASolutions
JWT_AUDIENCE=JEGASolutions-Users

# CORS
ALLOWED_ORIGINS=https://jegasolutions.co,https://*.jegasolutions.co
```

### **Variables de Entorno Frontend:**

```env
# API URL
VITE_API_URL=http://localhost:5014/api

# Tenant por defecto (solo desarrollo)
VITE_DEV_TENANT=test-tenant
```

Ver **[ENV_SETUP.md](./frontend/ENV_SETUP.md)** para más detalles.

### **Base de Datos:**

```sql
-- Tabla de tenants
CREATE TABLE tenants (
    id SERIAL PRIMARY KEY,
    company_name VARCHAR(255) NOT NULL,
    subdomain VARCHAR(50) UNIQUE NOT NULL,
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT NOW()
);

-- Módulos por tenant
CREATE TABLE tenant_modules (
    id SERIAL PRIMARY KEY,
    tenant_id INTEGER REFERENCES tenants(id),
    module_name VARCHAR(100) NOT NULL,
    status VARCHAR(20) DEFAULT 'ACTIVE',
    purchased_at TIMESTAMP DEFAULT NOW(),
    expires_at TIMESTAMP NULL
);

-- Usuarios por tenant
CREATE TABLE users (
    id SERIAL PRIMARY KEY,
    tenant_id INTEGER REFERENCES tenants(id),
    email VARCHAR(255) NOT NULL,
    first_name VARCHAR(255) NOT NULL,
    last_name VARCHAR(255) NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    role VARCHAR(50) DEFAULT 'user',
    created_at TIMESTAMP DEFAULT NOW(),
    last_login_at TIMESTAMP NULL,
    is_active BOOLEAN DEFAULT TRUE
);
```

## 🚀 Instalación y Uso

### **1. Frontend:**

```bash
cd apps/tenant-dashboard/frontend
npm install
npm run dev
```

### **2. Backend:**

```bash
cd apps/tenant-dashboard/backend
dotnet restore
dotnet run --project src/JEGASolutions.TenantDashboard.API
```

### **3. Base de Datos:**

```bash
# Crear migración
dotnet ef migrations add InitialCreate --project src/JEGASolutions.TenantDashboard.Infrastructure

# Aplicar migración
dotnet ef database update --project src/JEGASolutions.TenantDashboard.API
```

## 🌐 Rutas y URLs

### **Estructura de URLs (Soporta Ambos Métodos):**

#### **Opción A: DNS Wildcard (Subdominios) - Requiere Vercel Pro**

```
Landing: jegasolutions.co
├── /login                    # Login global
├── /pricing                  # Precios y módulos
└── /contact                  # Contacto

Tenant Dashboard: cliente.jegasolutions.co
├── /                         # Dashboard principal
├── /dashboard               # Dashboard principal
├── /login                   # Login del tenant
└── /settings                # Configuración

Módulos:
├── cliente.jegasolutions.co/extra-hours      # GestorHorasExtra
└── cliente.jegasolutions.co/report-builder  # ReportBuilder
```

#### **Opción B: Path-Based (Sin DNS especial) - Plan Gratuito**

```
Landing: jegasolutions.co
├── /login                    # Login global
├── /pricing                  # Precios y módulos
└── /contact                  # Contacto

Tenant Dashboard: jegasolutions.co/t/cliente
├── /t/cliente                # Dashboard principal
├── /t/cliente/dashboard      # Dashboard principal
├── /t/cliente/login          # Login del tenant
└── /t/cliente/settings       # Configuración

Módulos:
├── jegasolutions.co/t/cliente/extra-hours      # GestorHorasExtra
└── jegasolutions.co/t/cliente/report-builder  # ReportBuilder
```

> 💡 **El código soporta ambos métodos automáticamente.** Ver [VERCEL_SETUP_GUIDE.md](./VERCEL_SETUP_GUIDE.md) para configuración.

### **APIs Disponibles:**

```http
GET /api/tenants/{subdomain}           # Obtener datos del tenant
GET /api/tenants/{subdomain}/modules    # Módulos del tenant
GET /api/tenants/{subdomain}/stats      # Estadísticas del tenant
```

## 🎯 Flujo de Usuario

### **1. Acceso al Dashboard:**

**Con Subdomain (DNS Wildcard):**

1. Usuario accede a `cliente.jegasolutions.co`
2. Sistema detecta subdomain automáticamente
3. Carga datos del tenant y módulos
4. Muestra dashboard con módulos disponibles

**Con Path-Based:**

1. Usuario accede a `jegasolutions.co/t/cliente`
2. Sistema detecta tenant desde el path
3. Carga datos del tenant y módulos
4. Muestra dashboard con módulos disponibles

> 💡 La detección es automática, el sistema intenta subdomain primero, luego path, luego query param.

### **2. Navegación entre Módulos:**

1. Usuario ve tarjetas de módulos adquiridos
2. Hace clic en módulo deseado
3. Se abre en nueva pestaña: `cliente.jegasolutions.co/modulo`
4. Mantiene contexto de tenant

### **3. Gestión de Usuarios:**

1. Admin accede a configuración
2. Gestiona usuarios del tenant
3. Asigna roles y permisos
4. Controla acceso a módulos

## 🔐 Seguridad

### **Multi-Tenancy:**

- ✅ Aislamiento por subdomain
- ✅ Filtrado automático por TenantId
- ✅ JWT con claims de tenant
- ✅ Middleware de autorización

### **Autenticación:**

- ✅ JWT tokens seguros
- ✅ Refresh tokens
- ✅ Roles y permisos
- ✅ Sesiones por tenant

## 📊 Monitoreo

### **Métricas Disponibles:**

- Total de módulos adquiridos
- Módulos activos
- Número de usuarios
- Última actividad
- Uso por módulo

### **Logs:**

- Accesos por tenant
- Uso de módulos
- Errores de autenticación
- Performance de APIs

## 🚀 Despliegue

### **📖 Guía Completa de Despliegue:**

Ver **[VERCEL_SETUP_GUIDE.md](./VERCEL_SETUP_GUIDE.md)** para instrucciones detalladas paso a paso.

### **Frontend (Vercel):**

```bash
cd apps/tenant-dashboard/frontend

# Configurar variables de entorno en Vercel Dashboard:
# VITE_API_URL=https://api.jegasolutions.co/api

# Deploy
vercel --prod
```

**Opciones DNS:**

- **Sin wildcard (Gratis):** Usa URLs como `jegasolutions.co/t/cliente`
- **Con wildcard (Pro $20/mes):** Usa URLs como `cliente.jegasolutions.co`

Ver [DNS_CONFIGURATION.md](./DNS_CONFIGURATION.md) para más detalles.

### **Backend (Render):**

```bash
# Configurar variables de entorno
# Deploy automático desde GitHub
```

### **Base de Datos (PostgreSQL):**

```bash
# Crear instancia en Render
# Configurar conexión
# Ejecutar migraciones
```

## 🎉 Resultado Final

**Dashboard Central del Tenant COMPLETADO** ✅

- ✅ **Punto de entrada unificado** para tenants
- ✅ **Vista consolidada** de módulos adquiridos
- ✅ **Navegación unificada** entre módulos
- ✅ **Estadísticas centralizadas**
- ✅ **Gestión de usuarios**
- ✅ **Multi-tenancy completa**

**La funcionalidad multi-tenant está ahora 100% completa** con el Dashboard Central como punto de entrada principal para todos los tenants.
