# 🔧 API URL Configuration Fix

## Problema Identificado

El login del report-builder estaba fallando con error **404 Not Found** porque:

1. **Backend:** Las rutas están definidas con el prefijo `/api` (ejemplo: `/api/auth/login`)
2. **Frontend en producción:** La variable de entorno `VITE_API_URL` apuntaba a `https://jegasolutions-report-builder-api.onrender.com` sin el sufijo `/api`
3. **Resultado:** Las llamadas iban a `/auth/login` en vez de `/api/auth/login`

## Solución Implementada

Se creó un sistema centralizado de configuración de API URL que garantiza consistencia:

### 1. **Nuevo archivo:** `src/services/apiConfig.js`

```javascript
export const getApiBaseUrl = () => {
  const baseUrl = import.meta.env.VITE_API_URL || "http://localhost:5000";
  const cleanUrl = baseUrl.endsWith('/') ? baseUrl.slice(0, -1) : baseUrl;
  return cleanUrl.endsWith('/api') ? cleanUrl : `${cleanUrl}/api`;
};
```

Esta función:
- ✅ Toma la URL base de la variable de entorno `VITE_API_URL`
- ✅ Elimina barras finales (`/`) si existen
- ✅ Añade `/api` automáticamente si no está presente
- ✅ No duplica `/api` si ya está en la URL

### 2. **Archivos actualizados:**

Todos los servicios ahora usan `getApiBaseUrl()` de forma centralizada:

- ✅ `src/services/api.js` (axios instance)
- ✅ `src/services/authService.js`
- ✅ `src/services/narrativeService.js`
- ✅ `src/services/analysisService.js`
- ✅ `src/services/areaAssignmentService.js`
- ✅ `src/services/aiService.js`

### 3. **Rutas actualizadas:**

Todas las rutas ahora son relativas al base URL (sin `/api` redundante):

**Antes:**
```javascript
fetch(`${baseUrl}/api/auth/login`, ...)  // Resultaba en error si baseUrl no tenía /api
```

**Ahora:**
```javascript
const baseUrl = getApiBaseUrl();  // Siempre incluye /api
fetch(`${baseUrl}/auth/login`, ...)  // Correcto: /api/auth/login
```

## Configuración Requerida

### Desarrollo Local

```bash
# .env o vite.config.js
VITE_API_URL=http://localhost:5000
# El código añadirá /api automáticamente
```

### Producción (Render)

La variable de entorno puede configurarse de cualquiera de estas formas:

**Opción 1 - Solo dominio (Recomendado):**
```
VITE_API_URL=https://jegasolutions-report-builder-api.onrender.com
```

**Opción 2 - Con /api explícito:**
```
VITE_API_URL=https://jegasolutions-report-builder-api.onrender.com/api
```

Ambas funcionan correctamente gracias a la lógica inteligente de `getApiBaseUrl()`.

## Pasos para Desplegar

### 1. Commit y Push de los cambios

```bash
git add apps/report-builder/frontend/src/services/
git commit -m "fix: Centralizar configuración de API URL para evitar errores 404"
git push origin feature/centralize-tenant-management
```

### 2. Redesplegar el Frontend en Render

**Opción A - Despliegue Automático:**
Si tienes auto-deploy habilitado en Render, los cambios se desplegarán automáticamente al hacer push.

**Opción B - Despliegue Manual:**
1. Ve a tu dashboard de Render: https://dashboard.render.com
2. Busca el servicio del frontend de Report Builder
3. Haz clic en "Manual Deploy" → "Deploy latest commit"
4. Espera a que termine el build (~2-3 minutos)

### 3. Verificar Variables de Entorno en Render

1. En el dashboard de Render, ve al servicio del **Frontend**
2. Ve a la pestaña **Environment**
3. Verifica que `VITE_API_URL` esté configurada correctamente:
   ```
   VITE_API_URL=https://jegasolutions-report-builder-api.onrender.com
   ```
4. Si la cambias, Render hará un redeploy automático

### 4. Verificar que funciona

1. Ve a: https://reportbuilder.jegasolutions.co (o la URL de tu frontend)
2. Intenta hacer login con las credenciales del correo de bienvenida
3. Abre las DevTools (F12) → Network tab
4. Verifica que las llamadas vayan a:
   ```
   https://jegasolutions-report-builder-api.onrender.com/api/auth/login
   ```
   (Debe ser **200 OK** o **401 Unauthorized** si las credenciales son incorrectas, NO **404**)

## URLs de Endpoints Correctas

| Endpoint | URL Completa |
|----------|--------------|
| Login | `https://jegasolutions-report-builder-api.onrender.com/api/auth/login` |
| Verify | `https://jegasolutions-report-builder-api.onrender.com/api/auth/verify` |
| Templates | `https://jegasolutions-report-builder-api.onrender.com/api/templates` |
| Areas | `https://jegasolutions-report-builder-api.onrender.com/api/Areas` |

## Beneficios de esta Solución

✅ **Robustez:** Funciona independientemente de cómo esté configurada `VITE_API_URL`
✅ **Consistencia:** Un solo punto de configuración para todos los servicios
✅ **Mantenibilidad:** Más fácil de mantener y actualizar
✅ **Prevención de errores:** Evita duplicación o falta de `/api` en rutas
✅ **Desarrollo y producción:** Funciona igual en ambos ambientes

## Troubleshooting

### Si aún ves errores 404 después del despliegue:

1. **Verifica que se desplegó la nueva versión:**
   - En Render, ve a "Logs" y busca la fecha del último despliegue
   - Debe ser posterior a este commit

2. **Limpia el caché del navegador:**
   - Presiona `Ctrl + Shift + R` (Windows) o `Cmd + Shift + R` (Mac)
   - O abre en modo incógnito para probar

3. **Verifica la configuración de CORS en el backend:**
   - El backend debe permitir el origen del frontend en producción
   - Esto ya está configurado en `Program.cs` para `*.jegasolutions.co`

4. **Revisa los logs del backend en Render:**
   - Ve al servicio del backend en Render → "Logs"
   - Busca las llamadas entrantes y verifica que lleguen a `/api/auth/login`

---

**Fecha:** 2025-10-10
**Autor:** AI Assistant
**Estado:** ✅ Implementado y testeado localmente

