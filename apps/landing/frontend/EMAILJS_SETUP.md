# 📧 Configuración de EmailJS para Formulario de Contacto

## 🎯 ¿Por qué EmailJS?

EmailJS permite enviar emails directamente desde el frontend sin necesidad de un servidor backend propio. Es perfecto para formularios de contacto simples.

**Ventajas:**

- ✅ Gratis hasta 200 emails/mes
- ✅ Fácil de configurar (5 minutos)
- ✅ No requiere backend
- ✅ Confiable y seguro

---

## 🚀 Pasos para Configurar EmailJS

### 1. Crear Cuenta en EmailJS

1. Ve a [https://www.emailjs.com/](https://www.emailjs.com/)
2. Click en **"Sign Up"** (Registrarse)
3. Registra tu cuenta con email y contraseña
4. Verifica tu email

---

### 2. Configurar Servicio de Email

1. En el dashboard de EmailJS, ve a **"Email Services"**
2. Click en **"Add New Service"**
3. Selecciona tu proveedor de email:

   - **Gmail** (recomendado para pruebas)
   - **Outlook/Office 365**
   - O cualquier otro proveedor SMTP

4. **Para Gmail:**

   - Click en "Connect Account"
   - Autoriza a EmailJS a enviar emails desde tu cuenta
   - **IMPORTANTE:** Si usas Gmail, necesitas una "App Password":
     - Ve a https://myaccount.google.com/security
     - Activa "2-Step Verification"
     - Ve a "App passwords"
     - Genera una contraseña para "Mail"
     - Usa esa contraseña en EmailJS

5. **Para Outlook/Office 365:**

   - Ingresa tu email: `JaimeGallo@jegasolutions.co`
   - Ingresa tu contraseña
   - Click en "Connect"

6. Copia el **Service ID** (lo necesitarás después)

---

### 3. Crear Template de Email

1. Ve a **"Email Templates"** en el dashboard
2. Click en **"Create New Template"**
3. Configura el template:

   **Subject (Asunto):**

   ```
   🔔 Nueva Consulta desde JEGASolutions.co
   ```

   **Content (Contenido HTML):**

   ```html
   <!DOCTYPE html>
   <html>
     <head>
       <style>
         body {
           font-family: Arial, sans-serif;
           line-height: 1.6;
           color: #333;
         }
         .container {
           max-width: 600px;
           margin: 0 auto;
           padding: 20px;
           background-color: #f9f9f9;
           border-radius: 10px;
         }
         .header {
           background: linear-gradient(135deg, #1e3a8a 0%, #1f2937 100%);
           color: white;
           padding: 20px;
           border-radius: 10px 10px 0 0;
           text-align: center;
         }
         .content {
           background: white;
           padding: 30px;
           border-radius: 0 0 10px 10px;
         }
         .field {
           margin-bottom: 20px;
         }
         .label {
           font-weight: bold;
           color: #1e3a8a;
           display: block;
           margin-bottom: 5px;
         }
         .value {
           padding: 10px;
           background-color: #f3f4f6;
           border-radius: 5px;
           border-left: 3px solid #fbbf24;
         }
         .footer {
           text-align: center;
           margin-top: 20px;
           color: #6b7280;
           font-size: 12px;
         }
       </style>
     </head>
     <body>
       <div class="container">
         <div class="header">
           <h1>📩 Nueva Consulta Recibida</h1>
         </div>
         <div class="content">
           <div class="field">
             <span class="label">👤 Nombre:</span>
             <div class="value">{{from_name}}</div>
           </div>

           <div class="field">
             <span class="label">📧 Email:</span>
             <div class="value">{{from_email}}</div>
           </div>

           <div class="field">
             <span class="label">💬 Mensaje:</span>
             <div class="value">{{message}}</div>
           </div>
         </div>
         <div class="footer">
           <p>
             Este mensaje fue enviado desde el formulario de contacto de
             JEGASolutions.co
           </p>
         </div>
       </div>
     </body>
   </html>
   ```

   **To Email (Destinatario):**

   ```
   {{to_email}}
   ```

   _(O directamente: `JaimeGallo@jegasolutions.co`)_

4. Click en **"Save"**
5. Copia el **Template ID**

---

### 4. Obtener Public Key

1. Ve a **"Account"** → **"General"**
2. Copia tu **Public Key** (empieza con algo como `user_...`)

---

### 5. Configurar Variables de Entorno

1. En la carpeta `apps/landing/frontend/`, crea o edita el archivo `.env`:

```env
# EmailJS Configuration
VITE_EMAILJS_SERVICE_ID=tu_service_id_aqui
VITE_EMAILJS_TEMPLATE_ID=tu_template_id_aqui
VITE_EMAILJS_PUBLIC_KEY=tu_public_key_aqui
```

2. Reemplaza los valores con los IDs que copiaste:
   - `tu_service_id_aqui` → Service ID del paso 2
   - `tu_template_id_aqui` → Template ID del paso 3
   - `tu_public_key_aqui` → Public Key del paso 4

**Ejemplo real:**

```env
VITE_EMAILJS_SERVICE_ID=service_abc123xyz
VITE_EMAILJS_TEMPLATE_ID=template_xyz789def
VITE_EMAILJS_PUBLIC_KEY=user_1234567890abcdef
```

---

### 6. Configurar en Vercel (Producción)

1. Ve a tu proyecto en [Vercel Dashboard](https://vercel.com)
2. Click en **"Settings"** → **"Environment Variables"**
3. Agrega las 3 variables:

   - `VITE_EMAILJS_SERVICE_ID`
   - `VITE_EMAILJS_TEMPLATE_ID`
   - `VITE_EMAILJS_PUBLIC_KEY`

4. Click en **"Save"**
5. Redeploy tu aplicación

---

## 🧪 Probar Configuración

### En Local:

1. Asegúrate de tener el archivo `.env` configurado
2. Reinicia el servidor de desarrollo:
   ```bash
   npm run dev
   ```
3. Ve a la sección de **Contacto** en tu landing
4. Llena el formulario y envíalo
5. Deberías recibir el email en `JaimeGallo@jegasolutions.co`

### En Producción:

1. Después de configurar las variables en Vercel
2. Haz un nuevo deploy
3. Prueba el formulario en la web en vivo

---

## ✅ Checklist de Configuración

- [ ] Cuenta de EmailJS creada y verificada
- [ ] Servicio de email configurado (Gmail/Outlook)
- [ ] Template de email creado y guardado
- [ ] Service ID copiado
- [ ] Template ID copiado
- [ ] Public Key copiado
- [ ] Archivo `.env` creado en `apps/landing/frontend/`
- [ ] Variables de entorno configuradas en Vercel
- [ ] Aplicación redesplegada
- [ ] Prueba de envío realizada ✉️
- [ ] Email recibido correctamente ✅

---

## 🎯 ¿Qué Hace el Formulario Ahora?

Cuando un usuario llena el formulario de contacto:

1. ✅ Los datos se envían a EmailJS
2. ✅ EmailJS procesa el email con el template que creaste
3. ✅ Recibes un email en `JaimeGallo@jegasolutions.co` con:
   - Nombre del contacto
   - Email del contacto
   - Mensaje/consulta
4. ✅ El usuario ve un mensaje de confirmación
5. ✅ El formulario se limpia automáticamente

---

## 🔒 Seguridad

- ✅ Las credenciales están en variables de entorno (no en el código)
- ✅ EmailJS maneja toda la seguridad del envío
- ✅ No expones tu contraseña de email en el frontend
- ✅ El Public Key es seguro de usar en el cliente

---

## 📊 Límites del Plan Gratuito

- **200 emails/mes** gratis
- Si necesitas más, puedes actualizar a un plan pago:
  - Personal: $7/mes → 1,000 emails
  - Professional: $20/mes → 5,000 emails
  - Enterprise: Custom pricing

---

## 🆘 Troubleshooting

### "Error sending email"

- Verifica que las variables de entorno estén bien escritas
- Revisa que el Service ID, Template ID y Public Key sean correctos
- Asegúrate de haber reiniciado el servidor después de crear el `.env`

### No recibo los emails

- Revisa tu carpeta de SPAM
- Verifica que el email en el template sea correcto
- Revisa los logs en el dashboard de EmailJS

### "Service is not available"

- Verifica que hayas autorizado correctamente tu cuenta de email
- Si usas Gmail, asegúrate de usar App Password

---

## 📧 Alternativa: Usar el Backend Existente

Si prefieres usar tu backend de .NET que ya tiene EmailService configurado:

1. Crear endpoint en `PaymentsController.cs`:

```csharp
[HttpPost("contact")]
public async Task<IActionResult> Contact([FromBody] ContactFormDto request)
{
    try
    {
        var emailService = _serviceProvider.GetRequiredService<IEmailService>();

        var htmlBody = $@"
            <div style='font-family: Arial, sans-serif; padding: 20px;'>
                <h2>Nueva consulta desde JEGASolutions.co</h2>
                <p><strong>Nombre:</strong> {request.Name}</p>
                <p><strong>Email:</strong> {request.Email}</p>
                <p><strong>Mensaje:</strong></p>
                <p>{request.Message}</p>
            </div>";

        await emailService.SendWelcomeEmailAsync(
            ""JaimeGallo@jegasolutions.co"",
            ""Nueva consulta desde la web"",
            htmlBody
        );

        return Ok(new { success = true });
    }
    catch (Exception ex)
    {
        return BadRequest(new { success = false, error = ex.Message });
    }
}
```

2. Actualizar el frontend para llamar a este endpoint en lugar de EmailJS.

**Pros:** Usas tu infraestructura existente  
**Contras:** Requiere cambios en backend y más configuración

---

## 💡 Recomendación

**Usa EmailJS** para el formulario de contacto porque:

- Es más simple y rápido
- No requiere cambios en el backend
- Es perfecto para este caso de uso
- Deja tu backend libre para lógica más compleja

**Usa el backend** solo si:

- Necesitas validaciones complejas
- Quieres guardar los contactos en base de datos
- Necesitas enviar emails automáticos adicionales
