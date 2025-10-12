# Fix: Login del Tenant Dashboard

## 🐛 Problema

El login del tenant-dashboard fallaba con error **405 (Method Not Allowed)** al intentar iniciar sesión con las credenciales del email de bienvenida.

### Síntomas

```
❌ Failed to load resource: the server responded with a status of 405 ()
   URL: api/auth/login
```

### Causa Raíz

1. **Endpoint faltante**: El Landing Backend NO tenía un endpoint de autenticación (`/api/auth/login`)
2. **URL incorrecta**: El `AuthContext.jsx` estaba usando una ruta relativa (`/api/auth/login`) que intentaba llamar al mismo dominio de Vercel en lugar del backend en Render
3. **TenantId no se enviaba**: El login no incluía el `tenantId` en la petición, lo cual es importante para multi-tenancy

## ✅ Solución

### 1. Crear AuthController en el Landing Backend

Se creó un nuevo controller (`AuthController.cs`) que expone dos endpoints:

- **POST `/api/auth/login`**: Autentica usuarios y devuelve un JWT token
- **POST `/api/auth/validate`**: Valida tokens JWT existentes

```csharp
// apps/landing/backend/src/JEGASolutions.Landing.API/Controllers/AuthController.cs

[HttpPost("login")]
public async Task<ActionResult> Login([FromBody] LoginRequest request)
{
    var token = await _authService.AuthenticateAsync(
        request.Email,
        request.Password,
        request.TenantId);

    if (string.IsNullOrEmpty(token))
    {
        return Unauthorized(new { message = "Credenciales inválidas" });
    }

    var user = await _authService.GetUserByEmailAsync(request.Email, request.TenantId);

    return Ok(new { token, user });
}
```

### 2. Actualizar AuthContext.jsx

Se modificó el `AuthContext.jsx` para:

1. **Usar la variable de entorno `VITE_API_URL`** en lugar de ruta relativa
2. **Recibir `tenantId` como prop** y enviarlo en el login
3. **Agregar logs de debugging** para facilitar troubleshooting

```javascript
// apps/tenant-dashboard/frontend/src/contexts/AuthContext.jsx

export const AuthProvider = ({ children, tenantId }) => {
  const login = async (email, password) => {
    const apiUrl = import.meta.env.VITE_API_URL || 'http://localhost:5014/api';

    const response = await fetch(`${apiUrl}/auth/login`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        email,
        password,
        tenantId: tenantId || null,
      }),
    });

    // ... handle response
  };
};
```

### 3. Actualizar App.jsx

Se modificó la estructura de componentes para:

1. **Pasar el `tenantId`** del TenantContext al AuthProvider
2. **Crear componente wrapper `AppContent`** para acceder al TenantContext

```javascript
// apps/tenant-dashboard/frontend/src/App.jsx

function AppContent() {
  const { tenant } = useTenant();

  return (
    <AuthProvider tenantId={tenant?.id}>
      <Router>{/* Routes */}</Router>
    </AuthProvider>
  );
}

function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <TenantProvider>
        <AppContent />
      </TenantProvider>
    </QueryClientProvider>
  );
}
```

## 🔧 Configuración Requerida

### Variables de Entorno (Vercel)

Asegúrate de que la siguiente variable de entorno esté configurada en Vercel:

```bash
VITE_API_URL=https://jegasolutions-platform.onrender.com/api
```

### Deployment del Backend

El backend debe estar desplegado y corriendo en Render para que el login funcione. Asegúrate de hacer deploy del nuevo `AuthController`.

## 🧪 Testing

### Desarrollo Local

```bash
# Terminal 1 - Backend
cd apps/landing/backend
dotnet run

# Terminal 2 - Frontend
cd apps/tenant-dashboard/frontend
npm run dev
```

### Producción

1. Deploy el backend a Render (incluye el nuevo AuthController)
2. Verifica que `VITE_API_URL` esté configurado en Vercel
3. Prueba el login en el tenant dashboard

## 📋 Checklist de Deployment

- [ ] Backend desplegado en Render con AuthController
- [ ] Variable `VITE_API_URL` configurada en Vercel
- [ ] Frontend desplegado en Vercel
- [ ] DNS wildcard configurado para subdominios
- [ ] CORS configurado en el backend para permitir subdominios

## 🔍 Debugging

Si el login sigue fallando, verifica en la consola del navegador:

```javascript
🔐 Attempting login to: https://jegasolutions-platform.onrender.com/api/auth/login
📧 Email: usuario@ejemplo.com
🏢 TenantId: 123
✅ Login successful
```

Si ves errores de CORS:

- Verifica que el backend tenga configurado CORS para subdominios de `.jegasolutions.co`
- Verifica que el backend esté corriendo en Render

Si ves error 401:

- Verifica que el email y contraseña sean correctos
- Verifica que el usuario esté activo en la base de datos
- Verifica que el usuario pertenezca al tenant correcto

## 📚 Referencias

- [ENV_SETUP.md](./frontend/ENV_SETUP.md) - Configuración de variables de entorno
- [VERCEL_SETUP_GUIDE.md](./VERCEL_SETUP_GUIDE.md) - Guía de deployment en Vercel
- [CONFIGURACION_OPCION_B.md](./CONFIGURACION_OPCION_B.md) - Opciones de configuración

## ✨ Resultado

Ahora el login funciona correctamente:

1. El frontend llama al backend correcto en Render
2. Se envía el `tenantId` para validar que el usuario pertenezca al tenant
3. Se devuelve un JWT token válido
4. El usuario puede acceder al dashboard con sus módulos
