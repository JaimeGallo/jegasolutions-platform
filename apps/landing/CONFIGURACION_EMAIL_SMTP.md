# 📧 Configuración de Email SMTP en Render

## 🎯 Variables de Entorno Requeridas

Para que el backend pueda enviar correos de bienvenida, necesitas configurar estas variables de entorno en Render:

### En Render Dashboard:

1. Ve a tu servicio `jegasolutions-platform` en Render
2. Click en **"Environment"** en el menú lateral
3. Agrega estas variables:

```env
Email__Server=smtp.office365.com
Email__Port=587
Email__Username=tu-email@jegasolutions.co
Email__Password=tu-contraseña-smtp
Email__From=tu-email@jegasolutions.co
Email__FromName=JEGASolutions
```

---

## 📝 Ejemplo con Microsoft 365 / Outlook

```env
Email__Server=smtp.office365.com
Email__Port=587
Email__Username=JaimeGallo@jegasolutions.co
Email__Password=TuContraseñaSegura123
Email__From=JaimeGallo@jegasolutions.co
Email__FromName=JEGASolutions
```

---

## 📝 Ejemplo con Gmail

Si usas Gmail, necesitas una "App Password" (no tu contraseña normal):

```env
Email__Server=smtp.gmail.com
Email__Port=587
Email__Username=tu-email@gmail.com
Email__Password=tu-app-password-de-16-digitos
Email__From=tu-email@gmail.com
Email__FromName=JEGASolutions
```

### Cómo obtener App Password de Gmail:

1. Ve a https://myaccount.google.com/security
2. Activa "2-Step Verification"
3. Ve a "App passwords"
4. Genera una contraseña para "Mail"
5. Usa esa contraseña en `Email__Password`

---

## ⚠️ Formato Importante

**NOTA:** En Render, usa **doble underscore** `__` para separar secciones de configuración:

✅ Correcto: `Email__Server`  
❌ Incorrecto: `Email:Server`

---

## 🧪 Probar Configuración

Una vez configuradas las variables, puedes probar el envío de email visitando:

```
https://jegasolutions-platform.onrender.com/test-email
```

Si todo está bien, recibirás un correo en `JaimeGallo@jegasolutions.co`

---

## 🔄 Después de Configurar

1. **Guarda las variables** en Render
2. Render **redesplegará automáticamente** el backend
3. Espera ~2-3 minutos a que el servicio se reinicie
4. **Guarda el webhook** en Wompi (clic en "Guardar")
5. Haz una **nueva prueba de pago**

---

## ✅ Checklist Final

- [ ] Variables `Email__*` configuradas en Render
- [ ] Backend redesplegado
- [ ] Webhook configurado en Wompi (`https://jegasolutions-platform.onrender.com/api/payments/webhook`)
- [ ] Botón "Guardar" presionado en Wompi
- [ ] Prueba de pago realizada
- [ ] Email de bienvenida recibido ✉️
