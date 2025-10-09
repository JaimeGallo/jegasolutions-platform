# 🔧 Fix: Normalización de Subdomains

## 📋 Problema Detectado

**Síntoma:**

- Se creó un tenant con subdomain `juqn-pére4452` (con acento en la é)
- El navegador/DNS convierte esto a Punycode: `xn--juqn-pre4452-geb`
- La búsqueda en BD falla porque busca `xn--juqn-pre4452-geb` pero está guardado como `juqn-pére4452`
- **Resultado:** 404 - Tenant no encontrado ❌

**Logs del error:**

```
Created tenant with subdomain: juqn-pére4452
Searching for tenant: xn--juqn-pre4452-geb
❌ Tenant no encontrado: xn--juqn-pre4452-geb
```

---

## ✅ Solución Implementada

### 1. **Actualización del PasswordGenerator**

**Archivo:** `apps/landing/backend/src/JEGASolutions.Landing.Infrastructure/Utils/PasswordGenerator.cs`

**Cambios:**

#### ✨ Nuevo método `RemoveAccents()`

```csharp
/// <summary>
/// Remueve acentos y diacríticos de caracteres Unicode.
/// Ejemplos: é→e, ñ→n, ü→u, á→a, ç→c
/// </summary>
private string RemoveAccents(string input)
{
    // Normaliza usando Unicode FormD (descomposición canónica)
    // Filtra marcas diacríticas (acentos)
    // Retorna solo caracteres base
}
```

#### 🔄 Método `CleanSubdomainName()` mejorado

- **Antes:** Permitía caracteres Unicode con acentos (é, ñ, ü, etc.)
- **Ahora:**
  1. Normaliza el input removiendo acentos
  2. Solo permite caracteres ASCII (a-z, 0-9, -)
  3. Ignora emojis y símbolos especiales

**Ejemplos de conversión:**

| Input Original  | Subdomain Generado  | Válido para DNS |
| --------------- | ------------------- | --------------- |
| José García     | jose-garcia3456     | ✅ Sí           |
| María López     | maria-lopez7890     | ✅ Sí           |
| Ñoño Pérez      | nono-perez1234      | ✅ Sí           |
| Tech Corp 🚀    | tech-corp5678       | ✅ Sí           |
| François Müller | francois-muller9012 | ✅ Sí           |

---

## 🔨 Scripts de Corrección

### Opción 1: Corregir tenant actual específico

**Archivo:** `db-fix-current-tenant.sql`

```sql
-- Corrige el subdomain actual de "pére" a "pere"
UPDATE "Tenants"
SET
    "Subdomain" = REPLACE("Subdomain", 'é', 'e'),
    "UpdatedAt" = NOW()
WHERE "Subdomain" LIKE '%pére%';
```

### Opción 2: Corrección masiva de todos los tenants

**Archivo:** `db-fix-subdomain-accents.sql`

```sql
-- Corrige TODOS los subdomains con caracteres acentuados
UPDATE "Tenants"
SET "Subdomain" = translate(
    "Subdomain",
    'áàäâãåāéèëêēíìïîīóòöôõøōúùüûūñçÁÀÄÂ...',
    'aaaaaaaeeeeeiiiiiooooooouuuuuncAAAAA...'
)
WHERE "Subdomain" !~ '^[a-z0-9-]+$';
```

---

## 🧪 Pruebas

### 1. **Prueba con nombres comunes en español**

```bash
# Crear pagos con estos nombres y verificar subdomains generados:
- "José María González"     → jose-maria-gonzalez####
- "Ángela Martínez"         → angela-martinez####
- "Ñoño Hernández"          → nono-hernandez####
```

### 2. **Prueba con caracteres especiales**

```bash
# Verificar que se ignoren emojis y símbolos:
- "Tech Corp 🚀"            → tech-corp####
- "Café & Té S.A.S."        → cafe-te-sas####
- "100% Natural"            → 100-natural####
```

### 3. **Prueba de caracteres internacionales**

```bash
# Nombres en otros idiomas:
- "François Müller"         → francois-muller####
- "Søren Kierkegaard"       → soren-kierkegaard####
- "Владимир" (cirílico)     → (se convierte a "cliente####")
```

---

## 🚀 Despliegue

### Paso 1: Ejecutar script de corrección en BD

```bash
# Conectar a PostgreSQL
psql "YOUR_DATABASE_URL"

# Ejecutar script de corrección
\i apps/landing/db-fix-current-tenant.sql
```

### Paso 2: Desplegar código actualizado

```bash
# Build y deploy del backend
cd apps/landing/backend
dotnet build
# ... proceso de deploy en Render
```

### Paso 3: Verificación

```bash
# 1. Crear un nuevo pago con nombre que tenga acentos
# 2. Verificar en logs que el subdomain generado NO tenga acentos
# 3. Acceder a la URL del tenant y verificar que funcione
```

---

## 📊 Resultados Esperados

### ✅ Antes del Fix

```
Input: "Juqn Péré"
Subdomain creado: "juqn-pére4452"
URL generada: https://juqn-pére4452.jegasolutions.co
DNS convierte a: xn--juqn-pre4452-geb
Búsqueda en BD: "xn--juqn-pre4452-geb" ❌ NO ENCONTRADO
```

### ✅ Después del Fix

```
Input: "Juqn Péré"
Normalizado: "Juqn Pere"
Subdomain creado: "juqn-pere4452"
URL generada: https://juqn-pere4452.jegasolutions.co
DNS: juqn-pere4452 (sin cambios, es ASCII válido)
Búsqueda en BD: "juqn-pere4452" ✅ ENCONTRADO
```

---

## 🔍 Validación Técnica

### Regex de validación

```regex
^[a-z0-9-]+$
```

**Descripción:**

- Solo permite: letras minúsculas (a-z), dígitos (0-9) y guiones (-)
- NO permite: acentos, espacios, símbolos especiales, mayúsculas

### Query de verificación

```sql
-- Verificar que NO existan subdomains con caracteres especiales
SELECT COUNT(*) as "ProblematicSubdomains"
FROM "Tenants"
WHERE "Subdomain" !~ '^[a-z0-9-]+$';

-- Debería retornar 0
```

---

## 📝 Notas Adicionales

1. **Compatibilidad:**

   - ✅ Funciona con .NET 8.0
   - ✅ Compatible con DNS estándar (RFC 1035)
   - ✅ Compatible con Vercel wildcard domains
   - ✅ Compatible con certificados SSL wildcard

2. **Performance:**

   - Impacto mínimo: solo se ejecuta durante creación de tenant
   - No afecta búsquedas existentes

3. **Seguridad:**
   - Previene inyección de caracteres especiales en URLs
   - Evita problemas de encoding/decoding
   - Reduce superficie de ataque

---

## 🆘 Troubleshooting

### Problema: Subdomain todavía tiene acentos después del deploy

**Solución:**

1. Verificar que el código nuevo esté desplegado:

   ```bash
   # Ver logs de Render para confirmar deploy
   ```

2. Limpiar caché si existe:

   ```bash
   # Reiniciar servicio en Render
   ```

3. Ejecutar script de corrección de BD

### Problema: Tenant existente no se encuentra

**Solución:**

1. Ejecutar `db-fix-current-tenant.sql`
2. Verificar que el subdomain en BD coincida con el que busca la app
3. Revisar logs de búsqueda en BD

---

**Fecha de implementación:** 2025-10-09  
**Autor:** JEGASolutions DevOps Team  
**Versión:** 1.0
