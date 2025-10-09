# ✅ Verificación Wildcard DNS - Lista de Chequeo

Ya agregaste `*.jegasolutions.co` en Vercel. Ahora sigue estos pasos para verificar que todo funcione.

---

## 🎯 Paso 1: Verificar que el proyecto esté desplegado

### En Vercel Dashboard:

```
1. Ve a: https://vercel.com/dashboard
2. Busca tu proyecto: tenant-dashboard
3. Verifica que haya un deployment exitoso (verde ✓)
```

Si **NO** hay deployment:

```bash
# En tu terminal:
cd apps/tenant-dashboard/frontend
vercel --prod
```

---

## 🔧 Paso 2: Configurar Variables de Entorno en Vercel

### Ir a Environment Variables:

```
1. Tu proyecto → Settings → Environment Variables
2. Verifica/Agrega estas variables:
```

| Variable       | Valor                              | Environment |
| -------------- | ---------------------------------- | ----------- |
| `VITE_API_URL` | `https://api.jegasolutions.co/api` | Production  |

### Cómo agregar:

```
1. Click en "Add New"
2. Key: VITE_API_URL
3. Value: https://api.jegasolutions.co/api
4. Environment: Production (marca solo Production)
5. Click "Save"
```

⚠️ **Importante:** Después de agregar variables, necesitas **redesplegar**:

```
1. Ve a Deployments
2. Click en el último deployment
3. Click en "..." (tres puntos)
4. Click "Redeploy"
```

---

## 🌐 Paso 3: Verificar Dominios en Vercel

### Ve a Settings → Domains

Deberías tener estos 3 dominios agregados:

```
✅ jegasolutions.co           → Tu dominio principal
✅ www.jegasolutions.co       → Redirect a principal
✅ *.jegasolutions.co         → Wildcard (subdominios)
```

### Cómo verificar que están activos:

Cada dominio debe mostrar:

- Status: **Active** (no "Invalid" o "Pending")
- SSL: **Enabled** (candado verde)

Si alguno está en "Pending":

```
⏱️ Espera 5-10 minutos más
🔄 Refresca la página
```

---

## 🧪 Paso 4: Probar DNS

### Opción A: Online (Más fácil)

Ve a: https://dnschecker.org

```
1. Escribe: tenant1.jegasolutions.co
2. Select: A
3. Click "Search"

Resultado esperado:
- Debe resolver a una IP de Vercel (76.76.21.21 o similar)
- Debe estar en verde en varios lugares del mundo
```

### Opción B: Terminal

```bash
# Windows PowerShell:
nslookup tenant1.jegasolutions.co

# Mac/Linux:
dig tenant1.jegasolutions.co
```

**Resultado esperado:**

```
Name:    tenant1.jegasolutions.co
Address: 76.76.21.21
```

---

## 🎨 Paso 5: Probar en Navegador

### Prueba estos subdominios:

```
https://demo.jegasolutions.co
https://test.jegasolutions.co
https://tenant1.jegasolutions.co
```

### Lo que deberías ver:

**Opción 1: Tu aplicación carga** ✅

```
✅ La aplicación se muestra
✅ En consola (F12) ves logs:
   🌐 Hostname: demo.jegasolutions.co
   ✅ Tenant detectado desde subdomain: demo
   🔌 Conectando a: https://api.jegasolutions.co/api/tenants/by-subdomain/demo
```

**Opción 2: Error "No se pudo detectar tenant"** ⚠️

```
Esto es NORMAL si aún no tienes ese tenant en tu base de datos.

El wildcard DNS está funcionando si ves:
- La aplicación carga (no 404)
- En consola (F12) ves: "Tenant detectado desde subdomain: demo"
- Luego error de API: "Tenant no encontrado" o similar

Esto significa que el DNS funciona, solo necesitas crear el tenant en la BD.
```

**Opción 3: Error 404 o "This site can't be reached"** ❌

```
Posibles causas:
1. DNS no ha propagado → Espera 10-30 minutos
2. No tienes Vercel Pro → Verifica tu plan
3. Wildcard no agregado correctamente → Verifica Domains
```

---

## 🔍 Paso 6: Debug en DevTools

Abre cualquier subdominio (ej: `https://test.jegasolutions.co`) y:

```
1. Presiona F12 (DevTools)
2. Ve a Console
3. Busca estos logs:
```

### Logs esperados (TODO FUNCIONA ✅):

```javascript
🌐 Hostname: test.jegasolutions.co
📍 Pathname: /
✅ Tenant detectado desde subdomain: test
✅ Tenant final: test
🔌 Conectando a: https://api.jegasolutions.co/api/tenants/by-subdomain/test
```

Si después ves error de API:

```javascript
❌ Error al cargar tenant: Request failed with status code 404
// O similar
```

**Esto es NORMAL** - El DNS funciona, solo necesitas:

1. Tener el backend funcionando en `https://api.jegasolutions.co`
2. Tener el tenant "test" creado en tu base de datos

---

## 📊 Interpretación de Resultados

### ✅ TODO FUNCIONA si ves:

```
1. ✅ DNS resuelve correctamente (dnschecker.org)
2. ✅ Aplicación carga en subdominios
3. ✅ Console muestra: "Tenant detectado desde subdomain: X"
4. ✅ Hace request a tu API backend
```

**Siguiente paso:** Asegurarte de que tu backend esté funcionando y tengas tenants creados.

---

### ⚠️ DNS FUNCIONA PERO FALTA BACKEND si ves:

```
1. ✅ DNS resuelve correctamente
2. ✅ Aplicación carga en subdominios
3. ✅ Console muestra: "Tenant detectado desde subdomain: X"
4. ❌ Error: "Network Error" o "Failed to fetch"
```

**Problema:** El frontend funciona pero el backend no está accesible.

**Solución:**

```
1. Verifica que tu backend esté desplegado
2. Verifica la URL del backend: https://api.jegasolutions.co
3. Verifica CORS en el backend
4. Verifica que la variable VITE_API_URL esté configurada en Vercel
```

---

### ❌ DNS NO FUNCIONA si ves:

```
1. ❌ DNS no resuelve (dnschecker.org muestra error)
2. ❌ "This site can't be reached" en navegador
3. ❌ No carga nada
```

**Posibles causas:**

#### Causa 1: No tienes Vercel Pro

```
Verificar:
1. Vercel Dashboard → Settings → Billing
2. ¿Dice "Hobby" o "Pro"?

Si dice "Hobby":
- Wildcard DNS NO está disponible
- Necesitas upgrade a Pro ($20/mes)
- O usar path-based: jegasolutions.co/t/tenant
```

#### Causa 2: Propagación DNS pendiente

```
Solución:
⏱️ Espera 10-60 minutos
🔄 Limpia caché DNS:
   Windows: ipconfig /flushdns
   Mac: sudo dscacheutil -flushcache
```

#### Causa 3: Wildcard no agregado correctamente

```
Verificar:
1. Vercel → Tu Proyecto → Settings → Domains
2. ¿Está *.jegasolutions.co listado?
3. ¿Dice "Active" con SSL enabled?

Si no:
- Elimínalo y agrégalo de nuevo
- Espera 5-10 minutos
```

---

## 🎯 Siguiente Paso según tu resultado

### Si DNS funciona pero backend falla:

**Necesitas:**

1. Desplegar tu backend en algún lugar (Render, Railway, etc.)
2. Configurar CORS para aceptar wildcard subdominios
3. Actualizar `VITE_API_URL` en Vercel

Ver: [Backend deployment guide needed]

---

### Si DNS funciona y backend funciona pero "Tenant no encontrado":

**Necesitas:**

1. Crear tenants en tu base de datos
2. Cada tenant debe tener un `subdomain` que coincida con la URL

Ejemplo de datos de prueba:

```sql
-- Crear tenant de prueba
INSERT INTO tenants (company_name, subdomain, is_active)
VALUES ('Demo Company', 'demo', true);

INSERT INTO tenants (company_name, subdomain, is_active)
VALUES ('Test Company', 'test', true);

-- Ahora podrás acceder a:
-- https://demo.jegasolutions.co
-- https://test.jegasolutions.co
```

---

### Si DNS funciona completamente:

**¡Felicidades! 🎉**

Ahora puedes:

1. Crear más tenants en tu BD
2. Cada uno tendrá su URL: `{subdomain}.jegasolutions.co`
3. Compartir URLs con tus clientes

Ejemplo:

```
Cliente ACME → subdomain: "acme"
URL: https://acme.jegasolutions.co

Cliente Demo → subdomain: "demo-tech"
URL: https://demo-tech.jegasolutions.co
```

---

## 🚨 Problemas Comunes

### Error: "Invalid Configuration" en Vercel

**Causa:** No tienes Vercel Pro

**Solución:**

```
Opción A: Upgrade a Pro ($20/mes)
Opción B: Usar path-based (gratis):
  - URLs: jegasolutions.co/t/tenant
  - Tu código ya lo soporta
```

---

### Error: SSL Certificate Invalid

**Causa:** Certificado wildcard aún generándose

**Solución:**

```
⏱️ Espera 10-30 minutos
🔄 Refresca la página de Domains
```

Si después de 1 hora sigue:

```
❌ No tienes Vercel Pro
✅ Upgrade requerido
```

---

### Subdominios cargan pero con errores CORS

**Causa:** Backend no acepta requests de subdominios

**Solución en Backend:**

```csharp
// En tu backend (Program.cs o Startup.cs)
services.AddCors(options =>
{
    options.AddPolicy("AllowJegaSolutions", builder =>
    {
        builder
            .WithOrigins(
                "https://jegasolutions.co",
                "https://www.jegasolutions.co"
            )
            .SetIsOriginAllowed(origin =>
            {
                // Permitir todos los subdominios de jegasolutions.co
                return origin.EndsWith(".jegasolutions.co");
            })
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});
```

---

## 📞 ¿Qué hacer ahora?

**Ejecuta estos pasos en orden:**

```
[ ] Paso 1: Verificar deployment en Vercel
[ ] Paso 2: Configurar VITE_API_URL en Vercel
[ ] Paso 3: Verificar dominios en Settings → Domains
[ ] Paso 4: Probar DNS con dnschecker.org
[ ] Paso 5: Probar en navegador (https://test.jegasolutions.co)
[ ] Paso 6: Ver DevTools Console (F12) para debug
```

**Luego comparte los resultados:**

- ¿Qué ves en el navegador?
- ¿Qué logs ves en la consola (F12)?
- ¿Qué dice dnschecker.org?

Con esa información puedo ayudarte con el siguiente paso! 🚀
