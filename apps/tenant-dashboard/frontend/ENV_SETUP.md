# Variables de Entorno - Tenant Dashboard

## 🔧 Configuración de Desarrollo

Crea un archivo `.env.local` en `apps/tenant-dashboard/frontend/`:

```bash
# API Backend URL
VITE_API_URL=http://localhost:5014/api

# Tenant por defecto en desarrollo
VITE_DEV_TENANT=test-tenant

# Module URLs (donde están desplegados los módulos)
VITE_EXTRA_HOURS_URL=http://localhost:5001
VITE_REPORT_BUILDER_URL=http://localhost:5002
```

---

## 🚀 Configuración de Producción (Vercel)

### En Vercel Dashboard:

```
1. Ve a tu proyecto → Settings → Environment Variables
2. Agrega las siguientes variables:
```

| Variable                  | Valor                                             | Entorno    |
| ------------------------- | ------------------------------------------------- | ---------- |
| `VITE_API_URL`            | `https://jegasolutions-platform.onrender.com/api` | Production |
| `VITE_EXTRA_HOURS_URL`    | `https://extrahours.jegasolutions.co`             | Production |
| `VITE_REPORT_BUILDER_URL` | `https://reportbuilder.jegasolutions.co`          | Production |

⚠️ **NO** configurar `VITE_DEV_TENANT` en producción.

⚠️ **Importante:** Las URLs de los módulos deben apuntar a donde estén desplegadas las aplicaciones extra-hours y report-builder. Asegúrate de que estas aplicaciones estén corriendo antes de configurar estas variables.

---

## 🧪 Testing con Subdominios Locales

Para probar subdominios en tu máquina local:

### Windows:

```
1. Abre Notepad como Administrador
2. Abre: C:\Windows\System32\drivers\etc\hosts
3. Agrega al final:

127.0.0.1 tenant1.localhost
127.0.0.1 tenant2.localhost
127.0.0.1 demo.localhost
```

### Mac/Linux:

```bash
sudo nano /etc/hosts

# Agrega al final:
127.0.0.1 tenant1.localhost
127.0.0.1 tenant2.localhost
127.0.0.1 demo.localhost
```

### Luego acceder:

```
http://tenant1.localhost:5173
http://tenant2.localhost:5173
http://demo.localhost:5173
```

---

## 📝 Variables Disponibles

### `VITE_API_URL`

**Descripción:** URL base del backend API  
**Requerido:** Sí  
**Ejemplos:**

- Desarrollo: `http://localhost:5014/api`
- Producción: `https://jegasolutions-platform.onrender.com/api`

### `VITE_DEV_TENANT`

**Descripción:** Tenant por defecto cuando no se detecta de la URL  
**Requerido:** No (solo desarrollo)  
**Ejemplo:** `test-tenant`

**Cuándo se usa:**

- Solo en modo desarrollo (`npm run dev`)
- Solo si NO se detecta tenant de subdomain/path/query
- NO se usa en producción

### `VITE_EXTRA_HOURS_URL`

**Descripción:** URL donde está desplegada la aplicación Extra Hours  
**Requerido:** Sí (para que funcione el módulo)  
**Ejemplos:**

- Desarrollo: `http://localhost:5001`
- Producción: `https://extrahours.jegasolutions.co`

**Nota:** Esta URL se usa cuando el usuario hace click en "Acceder" al módulo Extra Hours.

### `VITE_REPORT_BUILDER_URL`

**Descripción:** URL donde está desplegada la aplicación Report Builder  
**Requerido:** Sí (para que funcione el módulo)  
**Ejemplos:**

- Desarrollo: `http://localhost:5002`
- Producción: `https://reportbuilder.jegasolutions.co`

**Nota:** Esta URL se usa cuando el usuario hace click en "Acceder" al módulo Report Builder.

---

## 🎯 Métodos de Detección de Tenant

El frontend detecta el tenant en este orden:

### 1. Subdomain (Prioridad Alta)

```
URL: https://tenant1.jegasolutions.co
Tenant: tenant1
```

### 2. Path (Prioridad Media)

```
URL: https://jegasolutions.co/t/tenant1
Tenant: tenant1
```

### 3. Query Parameter (Prioridad Baja)

```
URL: https://jegasolutions.co?tenant=tenant1
Tenant: tenant1
```

### 4. Variable de Entorno (Fallback)

```
VITE_DEV_TENANT=test-tenant
Tenant: test-tenant
```

---

## 🔍 Debugging

### Ver qué tenant se detectó:

1. Abre DevTools (F12)
2. Ve a la pestaña Console
3. Busca logs:

```javascript
🌐 Hostname: tenant1.jegasolutions.co
📍 Pathname: /dashboard
✅ Tenant detectado desde subdomain: tenant1
🔌 Conectando a: https://api.jegasolutions.co/api/tenants/by-subdomain/tenant1
📦 Tenant data: { id: 1, companyName: "Tenant 1", subdomain: "tenant1" }
```

### Si ves este error:

```
❌ Error al cargar tenant: No se pudo detectar el tenant
```

**Verifica:**

1. Que la URL sea válida:

   - ✅ `tenant1.jegasolutions.co`
   - ✅ `jegasolutions.co/t/tenant1`
   - ✅ `jegasolutions.co?tenant=tenant1`
   - ❌ `jegasolutions.co` (sin tenant)

2. En desarrollo, que tengas `VITE_DEV_TENANT` configurado

---

## 📦 Ejemplo Completo

### `.env.local` (Desarrollo)

```bash
VITE_API_URL=http://localhost:5014/api
VITE_DEV_TENANT=demo-tenant
VITE_EXTRA_HOURS_URL=http://localhost:5001
VITE_REPORT_BUILDER_URL=http://localhost:5002
```

### Vercel (Producción)

```bash
VITE_API_URL=https://jegasolutions-platform.onrender.com/api
VITE_EXTRA_HOURS_URL=https://extrahours.jegasolutions.co
VITE_REPORT_BUILDER_URL=https://reportbuilder.jegasolutions.co
# VITE_DEV_TENANT no se configura en producción
```

### Comandos:

```bash
# Desarrollo
cd apps/tenant-dashboard/frontend
npm run dev

# Producción (build)
npm run build
npm run preview  # Preview local del build

# Desplegar a Vercel
vercel --prod
```

---

## 🎉 Listo

Con esta configuración, tu frontend puede detectar tenants de múltiples formas y funciona tanto con DNS wildcard como sin él.
