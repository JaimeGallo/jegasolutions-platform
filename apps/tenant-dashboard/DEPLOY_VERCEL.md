# 🚀 Guía: Desplegar tenant-dashboard en Vercel

## Paso 1: Instalar Vercel CLI (si no lo tienes)

```bash
npm install -g vercel
```

## Paso 2: Login en Vercel

```bash
vercel login
```

Sigue las instrucciones para autenticarte.

---

## Paso 3: Desplegar el proyecto

### Opción A: Desde Terminal (Recomendado)

```bash
# Ve al directorio del frontend
cd apps/tenant-dashboard/frontend

# Primero, hacer un build local para verificar que funciona
npm install
npm run build

# Si el build es exitoso, desplegar a Vercel
vercel
```

#### Durante el proceso, responde:

```
? Set up and deploy "~/jegasolutions-platform/apps/tenant-dashboard/frontend"?
→ Y (Yes)

? Which scope do you want to deploy to?
→ Selecciona tu cuenta/organización

? Link to existing project?
→ N (No - crear nuevo proyecto)

? What's your project's name?
→ tenant-dashboard (o el nombre que prefieras)

? In which directory is your code located?
→ ./ (presiona Enter)

? Want to override the settings? [y/N]
→ N (No)
```

Vercel detectará automáticamente que es un proyecto Vite y lo desplegará.

---

### Opción B: Desde Vercel Dashboard (Alternativa)

```
1. Ve a: https://vercel.com/new
2. Click "Import Git Repository"
3. Conecta tu repositorio GitHub
4. Selecciona el repositorio: jegasolutions-platform
5. Configure Project:
   - Project Name: tenant-dashboard
   - Framework Preset: Vite
   - Root Directory: apps/tenant-dashboard/frontend
   - Build Command: npm run build
   - Output Directory: dist
6. Click "Deploy"
```

---

## Paso 4: Configurar Variables de Entorno

Una vez desplegado:

```
1. Ve al proyecto en Vercel Dashboard
2. Settings → Environment Variables
3. Agrega:
```

| Key            | Value                              | Environment |
| -------------- | ---------------------------------- | ----------- |
| `VITE_API_URL` | `https://api.jegasolutions.co/api` | Production  |

```
4. Click "Save"
```

---

## Paso 5: Agregar Dominios al Proyecto

```
1. En el proyecto → Settings → Domains
2. Click "Add Domain"
3. Agrega en este orden:

   a) jegasolutions.co
      → Click "Add"
      → Vercel lo verificará automáticamente

   b) www.jegasolutions.co
      → Click "Add"
      → Choose: "Redirect to jegasolutions.co"

   c) *.jegasolutions.co
      → Click "Add"
      → Vercel lo configurará automáticamente
```

⚠️ **Importante:** Vercel puede pedirte que remuevas estos dominios de otros proyectos si ya están asignados. Confírmalo.

---

## Paso 6: Redesplegar (Importante)

Después de agregar variables de entorno:

```
1. Ve a: Deployments
2. Click en el último deployment
3. Click "..." (tres puntos)
4. Click "Redeploy"
5. Marca: "Use existing Build Cache" (opcional)
6. Click "Redeploy"
```

---

## Paso 7: Verificar que funciona

### Test 1: Dominio principal

```
https://jegasolutions.co
```

Debería cargar tu aplicación.

### Test 2: Subdominios wildcard

```
https://demo.jegasolutions.co
https://test.jegasolutions.co
https://tenant1.jegasolutions.co
```

Deberían cargar tu aplicación y detectar el tenant desde el subdomain.

### Test 3: DevTools Console (F12)

Abre cualquier subdominio y verifica en la consola:

```javascript
🌐 Hostname: demo.jegasolutions.co
✅ Tenant detectado desde subdomain: demo
🔌 Conectando a: https://api.jegasolutions.co/api/tenants/by-subdomain/demo
```

---

## 🔧 Archivo vercel.json

Ya creé el archivo `vercel.json` en tu proyecto con la configuración necesaria:

```json
{
  "buildCommand": "npm run build",
  "outputDirectory": "dist",
  "framework": "vite",
  "rewrites": [
    {
      "source": "/(.*)",
      "destination": "/index.html"
    }
  ]
}
```

Este archivo asegura que todas las rutas se redirijan a `index.html` (necesario para SPA).

---

## 📊 Resumen de Comandos

```bash
# 1. Instalar dependencias
cd apps/tenant-dashboard/frontend
npm install

# 2. Build local (verificar que funciona)
npm run build

# 3. Desplegar a Vercel
vercel

# 4. Desplegar a producción
vercel --prod
```

---

## 🚨 Problemas Comunes

### Error: "No framework detected"

**Solución:**

```
Verifica que estés en el directorio correcto:
cd apps/tenant-dashboard/frontend

Verifica que package.json tenga:
"scripts": {
  "build": "vite build"
}
```

---

### Error: "Build failed"

**Solución:**

```bash
# Prueba el build local primero:
npm run build

# Si falla, revisa los errores
# Asegúrate de que todas las dependencias estén instaladas:
npm install
```

---

### Dominio ya asignado a otro proyecto

**Solución:**

```
1. Ve al proyecto que tiene el dominio
2. Settings → Domains
3. Click en el dominio
4. Click "Remove"
5. Regresa al nuevo proyecto y agrégalo
```

---

## ✅ Checklist de Deployment

```
[ ] Vercel CLI instalado
[ ] npm install ejecutado
[ ] npm run build exitoso (local)
[ ] vercel ejecutado
[ ] Proyecto creado en Vercel
[ ] Variables de entorno configuradas (VITE_API_URL)
[ ] Dominios agregados (jegasolutions.co, www, *.jegasolutions.co)
[ ] Redespliegue después de variables
[ ] Test: https://jegasolutions.co carga
[ ] Test: https://demo.jegasolutions.co detecta tenant
[ ] DevTools console muestra logs correctos
```

---

## 🎯 Siguiente Paso

Ejecuta estos comandos y dime qué resultado obtienes:

```bash
cd apps/tenant-dashboard/frontend
npm install
npm run build
vercel
```

Si hay algún error, cópialo y lo resolvemos juntos! 🚀
