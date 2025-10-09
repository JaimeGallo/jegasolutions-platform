# Configuración DNS Multi-Tenant en Vercel

## 📌 Resumen

Este proyecto soporta **3 métodos** para identificar tenants, dependiendo de tu configuración DNS:

### ✅ Métodos Soportados

| Método          | URL Ejemplo                       | Requiere DNS Wildcard | Costo                |
| --------------- | --------------------------------- | --------------------- | -------------------- |
| **Subdomain**   | `tenant1.jegasolutions.co`        | ✅ Sí                 | Vercel Pro ($20/mes) |
| **Path-Based**  | `jegasolutions.co/t/tenant1`      | ❌ No                 | Gratis               |
| **Query Param** | `jegasolutions.co?tenant=tenant1` | ❌ No                 | Gratis               |

---

## 🚀 Opción 1: DNS Wildcard (Subdominios)

### Ventajas

- ✅ URLs profesionales y branded
- ✅ Mejor SEO por tenant
- ✅ Aislamiento visual completo

### Requisitos

- Vercel Pro ($20/mes) o superior
- Configuración DNS wildcard

### Configuración en Vercel

#### Si el dominio está registrado en Vercel:

1. Ve a tu proyecto → **Settings** → **Domains**
2. Agrega: `*.jegasolutions.co`
3. Vercel automáticamente configura SSL wildcard
4. Espera propagación DNS (5-60 minutos)

#### Si el dominio está en otro proveedor (GoDaddy, Namecheap, etc.):

**Paso 1 - En tu proveedor DNS:**

```
Tipo: CNAME
Nombre: *
Valor: cname.vercel-dns.com
TTL: 3600
```

O con record A:

```
Tipo: A
Nombre: *
Valor: 76.76.21.21
TTL: 3600
```

**Paso 2 - En Vercel Dashboard:**

1. Settings → Domains
2. Agregar: `*.jegasolutions.co`
3. Seguir instrucciones de verificación

**Paso 3 - Verificar:**

```bash
nslookup tenant1.jegasolutions.co
# Debe resolver a la IP de Vercel
```

---

## 🎯 Opción 2: Path-Based (Recomendado para Plan Gratuito)

### Ventajas

- ✅ Sin costo adicional
- ✅ No requiere configuración DNS especial
- ✅ Fácil de implementar

### Desventajas

- ❌ URLs menos branded: `/t/tenant1` en lugar de `tenant1.`
- ❌ Menos aislamiento visual

### Configuración

#### Ya está implementado en el código:

- El `TenantContext` detecta automáticamente el tenant desde el path
- Rutas configuradas en `App.jsx` para manejar `/t/:tenant/*`

#### URLs de ejemplo:

```
https://jegasolutions.co/t/acme-corp
https://jegasolutions.co/t/cliente-demo/dashboard
https://jegasolutions.co/t/empresa123/login
```

#### Configuración en Vercel:

1. Desplegar normalmente (sin wildcard DNS)
2. Tu dominio base: `jegasolutions.co`
3. **No requiere configuración adicional**

---

## 🔧 Opción 3: Query Parameters (Desarrollo/Testing)

### Uso

```
https://jegasolutions.co?tenant=tenant1
https://jegasolutions.co/dashboard?tenant=cliente-demo
```

### Cuándo usar

- ✅ Testing rápido
- ✅ Desarrollo local
- ❌ NO recomendado para producción

---

## 🛠️ Configuración Actual del Código

El archivo `TenantContext.jsx` detecta tenants en este orden:

```javascript
// 1. Intenta subdomain primero
if (hostname === 'tenant1.jegasolutions.co') {
  subdomain = 'tenant1';
}

// 2. Si no, busca en path
if (pathname === '/t/tenant1/dashboard') {
  subdomain = 'tenant1';
}

// 3. Si no, busca en query
if (searchParams.get('tenant') === 'tenant1') {
  subdomain = 'tenant1';
}

// 4. En desarrollo, usa variable de entorno
if (import.meta.env.VITE_DEV_TENANT) {
  subdomain = import.meta.env.VITE_DEV_TENANT;
}
```

---

## 🧪 Testing

### En Desarrollo Local

**Opción 1 - Variable de entorno:**

```bash
# En .env.local
VITE_DEV_TENANT=test-tenant
```

**Opción 2 - Path:**

```
http://localhost:5173/t/test-tenant
```

**Opción 3 - Query:**

```
http://localhost:5173?tenant=test-tenant
```

**Opción 4 - Subdomain (requiere configurar hosts):**

```bash
# Windows: C:\Windows\System32\drivers\etc\hosts
# Mac/Linux: /etc/hosts

127.0.0.1 tenant1.localhost
127.0.0.1 tenant2.localhost
```

Luego: `http://tenant1.localhost:5173`

---

## 📋 Despliegue en Vercel

### Paso 1: Configurar Variables de Entorno

En Vercel Dashboard → Settings → Environment Variables:

```
VITE_API_URL=https://api.jegasolutions.co/api
```

### Paso 2: Desplegar

```bash
cd apps/tenant-dashboard/frontend
vercel --prod
```

### Paso 3: Configurar Dominio

#### Para Path-Based (Sin wildcard):

```
1. Vercel → Settings → Domains
2. Agregar: jegasolutions.co
3. ✅ Listo
```

#### Para Subdomain (Con wildcard):

```
1. Vercel → Settings → Domains
2. Agregar: *.jegasolutions.co
3. Seguir instrucciones DNS
4. Esperar propagación
```

---

## 🎨 Personalización de URLs para Clientes

### Para Clientes que quieren su propio dominio

**Ejemplo:** Cliente quiere usar `dashboard.cliente.com` en lugar de `cliente.jegasolutions.co`

#### Configuración:

**Paso 1 - Cliente configura DNS:**

```
Tipo: CNAME
Nombre: dashboard
Valor: cname.vercel-dns.com
```

**Paso 2 - En Vercel:**

```
1. Settings → Domains
2. Agregar: dashboard.cliente.com
3. Verificar
```

**Paso 3 - Actualizar Backend:**

```sql
-- En tabla Tenants
UPDATE Tenants
SET CustomDomain = 'dashboard.cliente.com'
WHERE Subdomain = 'cliente';
```

**Paso 4 - Actualizar TenantContext.jsx:**

```javascript
// Buscar tenant por custom domain
const response = await axios.get(`${apiUrl}/tenants/by-domain/${hostname}`);
```

---

## ⚠️ Troubleshooting

### Error: "No se pudo detectar el tenant"

**Solución:**

- Verifica que estés usando una de las URLs válidas:
  - `tenant.jegasolutions.co` (con wildcard DNS)
  - `jegasolutions.co/t/tenant` (path-based)
  - `jegasolutions.co?tenant=tenant` (query param)

### Error: DNS no resuelve subdominios

**Solución:**

- Verifica configuración DNS con: `nslookup tenant1.jegasolutions.co`
- Espera propagación DNS (hasta 48 horas)
- Verifica que el record wildcard (\*) esté configurado

### Error: SSL Certificate inválido en subdominios

**Solución:**

- Vercel plan gratuito NO soporta SSL wildcard
- Upgrade a Vercel Pro o usa path-based routing

---

## 🎯 Recomendación Final

### Si tienes presupuesto:

✅ **Usa DNS Wildcard** (Vercel Pro)

- URLs profesionales: `cliente.jegasolutions.co`
- Mejor experiencia de usuario

### Si NO tienes presupuesto:

✅ **Usa Path-Based** (Gratis)

- URLs funcionales: `jegasolutions.co/t/cliente`
- Cero costos adicionales
- Igual de funcional

---

## 📞 Soporte

Si necesitas ayuda con la configuración DNS:

- **Vercel Docs:** https://vercel.com/docs/concepts/projects/domains
- **DNS Checker:** https://dnschecker.org

---

## 🔄 Actualización del Código

El código actual **ya soporta ambos métodos** sin necesidad de cambios adicionales.

Para cambiar entre métodos, simplemente usa la URL apropiada:

- Con DNS wildcard: `https://tenant1.jegasolutions.co`
- Sin DNS wildcard: `https://jegasolutions.co/t/tenant1`

**No se requiere recompilación ni redeployment.**
