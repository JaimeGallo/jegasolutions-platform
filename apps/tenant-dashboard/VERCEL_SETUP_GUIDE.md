# Guía Rápida: Configuración Vercel Multi-Tenant

## 🎯 ¿Qué método usar?

```
┌─────────────────────────────────────────────────────────┐
│  ¿Tienes presupuesto para Vercel Pro ($20/mes)?        │
└─────────────────────────────────────────────────────────┘
                      │
        ┌─────────────┴─────────────┐
        │                           │
       SÍ                          NO
        │                           │
        ▼                           ▼
  DNS WILDCARD              PATH-BASED ROUTING
  (Subdominios)             (Sin DNS especial)
        │                           │
        ▼                           ▼
  tenant1.jegasolutions.co    jegasolutions.co/t/tenant1
```

---

## 🚀 OPCIÓN A: Path-Based (GRATIS) - RECOMENDADO

### ✅ Ventajas

- Sin costos adicionales
- No requiere configuración DNS especial
- Ya implementado en el código
- Funciona inmediatamente

### 📝 Pasos de Configuración

#### 1. Configurar tu dominio en Vercel

```
1. Abre: https://vercel.com/dashboard
2. Selecciona tu proyecto: tenant-dashboard
3. Ve a: Settings → Domains
4. Click: "Add Domain"
5. Ingresa: jegasolutions.co (o tu dominio)
6. Click: "Add"
```

#### 2. Configurar DNS en tu proveedor (una sola vez)

**Si el dominio NO está en Vercel:**

```
Tipo: A
Nombre: @
Valor: 76.76.21.21
TTL: 3600

Tipo: CNAME
Nombre: www
Valor: cname.vercel-dns.com
TTL: 3600
```

#### 3. ¡Listo! Usar URLs así:

```
https://jegasolutions.co/t/tenant1
https://jegasolutions.co/t/tenant2
https://jegasolutions.co/t/cliente-demo
```

#### 4. Compartir con tus clientes:

```
Cliente ACME Corp:
URL: https://jegasolutions.co/t/acme
Usuario: admin@acme.com

Cliente Demo Tech:
URL: https://jegasolutions.co/t/demo-tech
Usuario: admin@demo-tech.com
```

---

## 💎 OPCIÓN B: DNS Wildcard (CON VERCEL PRO)

### ✅ Ventajas

- URLs más profesionales
- Mejor branding por cliente
- Mejor aislamiento visual

### ⚠️ Requisitos

- **Vercel Pro:** $20/mes por miembro
- Plan Team: $20/mes (primer miembro), $12/mes adicionales

### 📝 Pasos de Configuración

#### 1. Upgrade a Vercel Pro

```
1. Abre: https://vercel.com/dashboard
2. Ve a: Settings → Billing
3. Click: "Upgrade to Pro"
4. Confirma pago
```

#### 2. Configurar DNS Wildcard en tu proveedor

**Opción A - Record CNAME (Recomendado):**

```
Tipo: CNAME
Nombre: *
Valor: cname.vercel-dns.com
TTL: 3600
```

**Opción B - Record A:**

```
Tipo: A
Nombre: *
Valor: 76.76.21.21
TTL: 3600
```

#### 3. Agregar Wildcard Domain en Vercel

```
1. Vercel Dashboard → Tu Proyecto
2. Settings → Domains
3. Click: "Add Domain"
4. Ingresa: *.jegasolutions.co
5. Click: "Add"
6. Sigue instrucciones de verificación
```

#### 4. Verificar configuración

```bash
# En terminal:
nslookup tenant1.jegasolutions.co
nslookup tenant2.jegasolutions.co

# Debe resolver a IP de Vercel: 76.76.21.21
```

#### 5. Esperar propagación DNS

```
⏱️ Tiempo de propagación:
- Mínimo: 5-10 minutos
- Típico: 1-2 horas
- Máximo: 48 horas
```

#### 6. ¡Listo! Usar URLs así:

```
https://tenant1.jegasolutions.co
https://tenant2.jegasolutions.co
https://cliente-demo.jegasolutions.co
```

---

## 🧪 Verificar que funciona

### Test 1: Verificar DNS

```bash
# Windows PowerShell:
nslookup tenant1.jegasolutions.co

# Linux/Mac:
dig tenant1.jegasolutions.co

# Online:
https://dnschecker.org
```

**Resultado esperado:**

```
Name:    tenant1.jegasolutions.co
Address: 76.76.21.21
```

### Test 2: Verificar SSL

```bash
# Abrir en navegador:
https://tenant1.jegasolutions.co

# Debe mostrar 🔒 (candado verde)
# Sin errores de certificado
```

### Test 3: Verificar Detección de Tenant

```javascript
// Abrir DevTools Console (F12)
// Deberías ver logs:
🌐 Hostname: tenant1.jegasolutions.co
✅ Tenant detectado desde subdomain: tenant1
📦 Tenant data: { id: 1, companyName: "Tenant 1", ... }
```

---

## 🔧 Configuración en Proveedores DNS Populares

### GoDaddy

```
1. Ve a: https://dcc.godaddy.com/domains
2. Click en tu dominio
3. DNS → Manage
4. Add New Record:
   - Type: CNAME
   - Name: *
   - Value: cname.vercel-dns.com
   - TTL: 1 Hour
5. Save
```

### Namecheap

```
1. Ve a: https://ap.www.namecheap.com/domains/list
2. Click "Manage" en tu dominio
3. Advanced DNS tab
4. Add New Record:
   - Type: CNAME Record
   - Host: *
   - Value: cname.vercel-dns.com
   - TTL: Automatic
5. Save
```

### Cloudflare

```
1. Ve a: https://dash.cloudflare.com
2. Selecciona tu dominio
3. DNS → Records
4. Add record:
   - Type: CNAME
   - Name: *
   - Target: cname.vercel-dns.com
   - Proxy status: DNS only (nube gris)
   - TTL: Auto
5. Save
```

⚠️ **Importante con Cloudflare:** Desactiva proxy (nube gris) para wildcard.

---

## 📊 Comparación de Opciones

| Característica        | Path-Based                    | DNS Wildcard                |
| --------------------- | ----------------------------- | --------------------------- |
| **Costo**             | Gratis                        | $20/mes                     |
| **URL Cliente 1**     | `jegasolutions.co/t/cliente1` | `cliente1.jegasolutions.co` |
| **URL Cliente 2**     | `jegasolutions.co/t/cliente2` | `cliente2.jegasolutions.co` |
| **Configuración DNS** | Básica                        | Wildcard                    |
| **SSL**               | Incluido                      | Wildcard incluido           |
| **Tiempo Setup**      | 0 min                         | 5-60 min                    |
| **Branding**          | Medio                         | Alto                        |
| **SEO por tenant**    | Medio                         | Alto                        |
| **Profesionalidad**   | Media                         | Alta                        |

---

## 🎨 URLs de Ejemplo

### Con Path-Based:

```
Landing:      https://jegasolutions.co
Cliente ACME: https://jegasolutions.co/t/acme
Cliente Demo: https://jegasolutions.co/t/demo
Extra Hours:  https://jegasolutions.co/t/acme/extra-hours
Reports:      https://jegasolutions.co/t/acme/reports
```

### Con DNS Wildcard:

```
Landing:      https://www.jegasolutions.co
Cliente ACME: https://acme.jegasolutions.co
Cliente Demo: https://demo.jegasolutions.co
Extra Hours:  https://acme.jegasolutions.co/extra-hours
Reports:      https://acme.jegasolutions.co/reports
```

---

## 🚨 Problemas Comunes

### ❌ Error: "No se pudo detectar el tenant"

**Causa:** URL incorrecta

**Solución:**

```
❌ Incorrecto: https://jegasolutions.co
❌ Incorrecto: https://jegasolutions.co/tenant1

✅ Correcto (Path): https://jegasolutions.co/t/tenant1
✅ Correcto (Wildcard): https://tenant1.jegasolutions.co
```

### ❌ Error: DNS no resuelve

**Causa:** Propagación DNS pendiente o configuración incorrecta

**Solución:**

```bash
# Verificar configuración:
nslookup tenant1.jegasolutions.co

# Si no resuelve:
1. Verificar que el record * esté configurado
2. Esperar hasta 48 horas
3. Limpiar caché DNS: ipconfig /flushdns (Windows)
```

### ❌ Error: SSL Certificate Invalid

**Causa:** Plan gratuito de Vercel no soporta SSL wildcard

**Solución:**

```
Opción 1: Upgrade a Vercel Pro
Opción 2: Usar path-based routing (gratis)
```

### ❌ Error: 404 Not Found en subdomain

**Causa:** Dominio no agregado en Vercel

**Solución:**

```
1. Vercel → Settings → Domains
2. Agregar: *.jegasolutions.co
3. Verificar
```

---

## 🎯 Mi Recomendación

### Para Startups / MVP:

```
✅ Usa PATH-BASED (Gratis)

Razón:
- Cero costos
- Funciona perfectamente
- Puedes cambiar después
```

### Para Empresas Establecidas:

```
✅ Usa DNS WILDCARD (Vercel Pro)

Razón:
- URLs más profesionales
- Mejor branding
- Mejor percepción del cliente
```

---

## 🔄 Migrar de Path-Based a Wildcard

Si empiezas con path-based y después quieres migrar:

```
1. Upgrade a Vercel Pro
2. Configurar DNS wildcard
3. Agregar *.jegasolutions.co en Vercel
4. ¡Listo! Ambos métodos funcionarán simultáneamente

Clientes pueden seguir usando:
- https://jegasolutions.co/t/cliente (viejo)
- https://cliente.jegasolutions.co (nuevo)

Puedes migrar clientes gradualmente.
```

---

## 📞 Ayuda Adicional

**Documentación Oficial:**

- Vercel Domains: https://vercel.com/docs/concepts/projects/domains
- Vercel Wildcard: https://vercel.com/docs/concepts/projects/domains/wildcard-domains

**Herramientas Útiles:**

- DNS Checker: https://dnschecker.org
- SSL Checker: https://www.sslshopper.com/ssl-checker.html
- Vercel Status: https://www.vercel-status.com

---

## ✅ Checklist Final

### Para Path-Based (Gratis):

```
[ ] Dominio principal configurado en Vercel
[ ] DNS apuntando a Vercel
[ ] Código actualizado (ya hecho ✅)
[ ] Desplegado en producción
[ ] Probado con: jegasolutions.co/t/test-tenant
```

### Para DNS Wildcard (Pro):

```
[ ] Upgrade a Vercel Pro completado
[ ] Record wildcard (*) configurado en DNS
[ ] *.jegasolutions.co agregado en Vercel
[ ] Esperado propagación DNS (mínimo 10 min)
[ ] SSL wildcard verificado
[ ] Probado con: tenant1.jegasolutions.co
```

---

**🎉 ¡Ya está todo listo! Tu código soporta ambos métodos.**
