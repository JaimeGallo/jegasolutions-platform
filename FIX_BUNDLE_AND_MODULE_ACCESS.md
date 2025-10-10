# 🔧 Corrección: Bundle de Módulos y Acceso a Aplicaciones

**Fecha:** 10/10/2025  
**Branch:** feature/centralize-tenant-management

---

## 🐛 Problemas Identificados

### Problema 1: Solo se crea un módulo al pagar por bundle

**Síntoma:** Al pagar por ambos módulos (extra-hours + report-builder), solo se asociaba Extra Hours.

**Causa Raíz:**

```csharp
// ❌ El código llamaba a un método que NO existía
var moduleName = ExtractModuleNameFromReference(payment.Reference);  // NO EXISTE

// ✅ Pero SÍ existía este método (plural) que detecta múltiples módulos
var modules = ExtractModulesFromReference(payment.Reference);  // EXISTE
```

El método `ExtractModulesFromReference` (líneas 873-901) **SÍ detectaba bundles correctamente**, pero el código de creación de módulos (líneas 293 y 353) llamaba a `ExtractModuleNameFromReference` (singular) que **no existe**.

### Problema 2: No carga la página al acceder a los módulos

**Síntoma:** Al hacer click en "Acceder" desde el dashboard del tenant, la página no carga.

**Causa Raíz:**
Las aplicaciones `extra-hours` y `report-builder` son aplicaciones separadas, pero el tenant-dashboard intentaba abrirlas en rutas locales que no existen:

- `https://nombredeempresa.jegasolutions.co/extra-hours` ❌ No existe
- `https://nombredeempresa.jegasolutions.co/report-builder` ❌ No existe

Las aplicaciones reales están (o deberían estar) en:

- `https://extrahours.jegasolutions.co` ✅
- `https://reportbuilder.jegasolutions.co` ✅

---

## ✅ Soluciones Implementadas

### Solución 1: Crear todos los módulos del bundle

**Archivo:** `apps/landing/backend/src/JEGASolutions.Landing.Infrastructure/Services/WompiService.cs`

#### Cambios para usuarios existentes (líneas 290-328):

**Antes:**

```csharp
// ❌ Solo creaba UN módulo
var purchasedModuleName = ExtractModuleNameFromReference(payment.Reference);

var existingModule = await _tenantModuleRepository.FirstOrDefaultAsync(
    tm => tm.TenantId == existingTenant.Id && tm.ModuleName == purchasedModuleName
);

if (existingModule == null)
{
    var newTenantModule = new TenantModule { ... };
    await _tenantModuleRepository.AddAsync(newTenantModule);
}
```

**Ahora:**

```csharp
// ✅ Crea TODOS los módulos del bundle
var purchasedModules = ExtractModulesFromReference(payment.Reference);

foreach (var purchasedModuleName in purchasedModules)
{
    var existingModule = await _tenantModuleRepository.FirstOrDefaultAsync(
        tm => tm.TenantId == existingTenant.Id && tm.ModuleName == purchasedModuleName
    );

    if (existingModule == null)
    {
        var newTenantModule = new TenantModule { ... };
        await _tenantModuleRepository.AddAsync(newTenantModule);
    }
}
```

#### Cambios para usuarios nuevos (líneas 355-376):

**Antes:**

```csharp
// ❌ Solo creaba UN módulo
var moduleName = ExtractModuleNameFromReference(payment.Reference);

var tenantModule = new TenantModule
{
    TenantId = tenant.Id,
    ModuleName = moduleName,
    Status = "ACTIVE",
    PurchasedAt = DateTime.UtcNow
};

await _tenantModuleRepository.AddAsync(tenantModule);
```

**Ahora:**

```csharp
// ✅ Crea TODOS los módulos del bundle
var purchasedModules = ExtractModulesFromReference(payment.Reference);

foreach (var moduleName in purchasedModules)
{
    var tenantModule = new TenantModule
    {
        TenantId = tenant.Id,
        ModuleName = moduleName,
        Status = "ACTIVE",
        PurchasedAt = DateTime.UtcNow
    };

    await _tenantModuleRepository.AddAsync(tenantModule);
}
```

#### Mejoras en el email de bienvenida (líneas 431-446):

**Ahora detecta múltiples módulos y ajusta el mensaje:**

```csharp
// Obtener nombres amigables de los módulos
var moduleFriendlyNames = purchasedModules.Select(m => m switch
{
    "extra-hours" => "Extra Hours",
    "report-builder" => "Report Builder",
    _ => m
}).ToList();

string modulesText = purchasedModules.Count > 1
    ? $"{string.Join(" y ", moduleFriendlyNames)}"  // "Extra Hours y Report Builder"
    : moduleFriendlyNames.First();                  // "Extra Hours"

string modulesPlural = purchasedModules.Count > 1 ? "módulos" : "módulo";

var emailSubject = $"🎉 ¡Bienvenido a JEGASolutions! - {modulesText}";
```

**Resultado:**

- Si compras 1 módulo: "Tu módulo Extra Hours está listo"
- Si compras 2 módulos: "Tus módulos Extra Hours y Report Builder están listos"

---

### Solución 2: URLs configurables para módulos

**Archivo:** `apps/tenant-dashboard/frontend/src/pages/TenantDashboard.jsx`

#### Cambios en getModuleConfig (líneas 39-87):

**Antes:**

```javascript
if (normalized === 'extrahours') {
  return {
    displayName: 'GestorHorasExtra',
    icon: Clock,
    color: 'bg-blue-500',
    description: 'Gestión completa de horas extra y compensaciones',
    features: [...],
    route: '/extra-hours',  // ❌ Ruta local que no existe
  };
}
```

**Ahora:**

```javascript
if (normalized === 'extrahours') {
  return {
    displayName: 'GestorHorasExtra',
    icon: Clock,
    color: 'bg-blue-500',
    description: 'Gestión completa de horas extra y compensaciones',
    features: [...],
    // ✅ URL configurable vía variable de entorno
    url: import.meta.env.VITE_EXTRA_HOURS_URL || 'https://extrahours.jegasolutions.co',
  };
}
```

#### Cambios en availableModules (líneas 89-104):

**Antes:**

```javascript
const availableModules = modules.map(module => {
  const config = getModuleConfig(module.moduleName);
  return {
    ...
    url: `${window.location.origin}${config.route}`,  // ❌ Construye URL local
  };
});
```

**Ahora:**

```javascript
const availableModules = modules.map(module => {
  const config = getModuleConfig(module.moduleName);
  return {
    ...
    url: config.url,  // ✅ Usa URL configurada
  };
});
```

---

## 📄 Documentación Actualizada

**Archivo:** `apps/tenant-dashboard/frontend/ENV_SETUP.md`

### Nuevas Variables de Entorno:

```bash
# Module URLs - Configure where each module is deployed
VITE_EXTRA_HOURS_URL=https://extrahours.jegasolutions.co
VITE_REPORT_BUILDER_URL=https://reportbuilder.jegasolutions.co
```

### Desarrollo:

```bash
VITE_EXTRA_HOURS_URL=http://localhost:5001
VITE_REPORT_BUILDER_URL=http://localhost:5002
```

---

## 🧪 Testing

### Test 1: Verificar que se crean múltiples módulos

**Escenario:** Realizar un pago con referencia que contenga "EXTRAHOURS" y "REPORTS"

**Resultado esperado:**

```sql
SELECT * FROM TenantModules WHERE TenantId = X;

-- Debería mostrar:
-- | id | TenantId | ModuleName      | Status |
-- |----|----------|-----------------|--------|
-- | 1  | X        | extra-hours     | ACTIVE |
-- | 2  | X        | report-builder  | ACTIVE |
```

### Test 2: Verificar que el email muestra todos los módulos

**Resultado esperado:**

```
Asunto: 🎉 ¡Bienvenido a JEGASolutions! - Extra Hours y Report Builder

Cuerpo:
"Tus módulos Extra Hours y Report Builder están listos"
"Módulos Adquiridos: Extra Hours y Report Builder"
```

### Test 3: Verificar que los módulos abren las URLs correctas

**Acción:** Hacer click en "Acceder" en el dashboard

**Resultado esperado:**

- Extra Hours → Abre `https://extrahours.jegasolutions.co` (o la URL configurada)
- Report Builder → Abre `https://reportbuilder.jegasolutions.co` (o la URL configurada)

---

## 🚀 Próximos Pasos

### 1. Configurar Variables de Entorno en Vercel

```bash
# En Vercel Dashboard → tenant-dashboard → Settings → Environment Variables
VITE_EXTRA_HOURS_URL=https://extrahours.jegasolutions.co
VITE_REPORT_BUILDER_URL=https://reportbuilder.jegasolutions.co
```

### 2. Desplegar las Aplicaciones de Módulos

Las aplicaciones `extra-hours` y `report-builder` necesitan estar desplegadas en sus URLs configuradas.

**Opciones:**

- Desplegar en Vercel con subdominios personalizados
- Desplegar en Render u otro servicio
- Configurar redirects si están en otras URLs

### 3. Redesplegar el Backend

```bash
git add apps/landing/backend/src/JEGASolutions.Landing.Infrastructure/Services/WompiService.cs
git commit -m "fix: create all modules from bundle payment"
git push origin feature/centralize-tenant-management
```

El backend en Render se redesplegará automáticamente.

### 4. Redesplegar el Tenant Dashboard

```bash
git add apps/tenant-dashboard/frontend/src/pages/TenantDashboard.jsx
git add apps/tenant-dashboard/frontend/ENV_SETUP.md
git commit -m "fix: use configurable URLs for module access"
git push origin feature/centralize-tenant-management
```

Vercel redesplegará automáticamente.

---

## 📊 Resumen de Archivos Modificados

```
✅ apps/landing/backend/src/JEGASolutions.Landing.Infrastructure/Services/WompiService.cs
   - Líneas 290-328: Crear múltiples módulos para usuarios existentes
   - Líneas 355-376: Crear múltiples módulos para usuarios nuevos
   - Líneas 431-446: Email adaptado para múltiples módulos
   - Líneas 647, 654-657, 663-665: UI del email adaptada

✅ apps/tenant-dashboard/frontend/src/pages/TenantDashboard.jsx
   - Líneas 39-87: URLs configurables en getModuleConfig
   - Líneas 89-104: Usar URL del config en lugar de construirla

✅ apps/tenant-dashboard/frontend/ENV_SETUP.md
   - Documentación actualizada con nuevas variables de entorno
```

---

## ✨ Beneficios de los Cambios

1. **Múltiples Módulos:** Los clientes que paguen por bundles recibirán todos los módulos correctamente.

2. **Flexibilidad de Deployment:** Los módulos pueden estar desplegados en cualquier URL, configurable vía variables de entorno.

3. **Mejor Experiencia:** Los emails de bienvenida muestran correctamente todos los módulos adquiridos.

4. **Mantenibilidad:** Es fácil cambiar las URLs de los módulos sin modificar código.

---

## 🎯 Estado Actual

```
✅ Backend corregido (múltiples módulos)
✅ Frontend corregido (URLs configurables)
✅ Documentación actualizada
⏳ Falta desplegar cambios
⏳ Falta configurar variables de entorno en Vercel
⏳ Falta desplegar aplicaciones de módulos
```
