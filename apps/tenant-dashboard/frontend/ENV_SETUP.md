# Variables de Entorno - Tenant Dashboard

## 🔧 Configuración de Desarrollo

Crea un archivo `.env.local` en `apps/tenant-dashboard/frontend/`:

```bash
# API Backend URL
VITE_API_URL=http://localhost:5014/api

# Tenant por defecto en desarrollo
VITE_DEV_TENANT=test-tenant
```

---

## 🚀 Configuración de Producción (Vercel)

### En Vercel Dashboard:

```
1. Ve a tu proyecto → Settings → Environment Variables
2. Agrega las siguientes variables:
```

| Variable       | Valor                                      | Entorno            |
| -------------- | ------------------------------------------ | ------------------ |
| `VITE_API_URL` | `https://api.jegasolutions.co/api`         | Production         |
| `VITE_API_URL` | `https://api-staging.jegasolutions.co/api` | Preview (opcional) |

⚠️ **NO** configurar `VITE_DEV_TENANT` en producción.

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
- Producción: `https://api.jegasolutions.co/api`

### `VITE_DEV_TENANT`

**Descripción:** Tenant por defecto cuando no se detecta de la URL  
**Requerido:** No (solo desarrollo)  
**Ejemplo:** `test-tenant`

**Cuándo se usa:**

- Solo en modo desarrollo (`npm run dev`)
- Solo si NO se detecta tenant de subdomain/path/query
- NO se usa en producción

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
```

### Vercel (Producción)

```bash
VITE_API_URL=https://api.jegasolutions.co/api
# VITE_DEV_TENANT no se configura
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
