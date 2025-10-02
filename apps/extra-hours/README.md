# Extra Hours Module - Local Development Setup

Este módulo de horas extra está configurado para funcionar localmente con Docker usando una base de datos PostgreSQL cargada desde un backup SQL.

## Configuración Inicial

### 1. Archivo de Environment Variables

Copia el archivo de ejemplo y configura las variables:

```bash
cp env.example .env
```

Edita el archivo `.env` con tus valores específicos si es necesario.

### 2. Ejecutar con Docker

```bash
# Desde el directorio apps/extra-hours/
docker-compose up --build
```

Este comando:
- ✅ Inicia PostgreSQL con health check
- ✅ Carga automáticamente el backup SQL (`backup_with_compensation.sql`)
- ✅ Espera a que la BD esté healthy antes de iniciar el backend
- ✅ NO intenta crear tablas (usa las del backup)
- ✅ Inicia el backend API en el puerto 7086
- ✅ Inicia el frontend en el puerto 5173

### 3. Endpoints Disponibles

- **Backend API**: http://localhost:7086
- **Swagger UI**: http://localhost:7086/swagger
- **Frontend**: http://localhost:5173
- **Login Endpoint**: `POST http://localhost:7086/api/auth/login`

### 4. Usuarios de Prueba

Puedes usar cualquier usuario del backup SQL para hacer login. Algunos ejemplos:

```json
{
  "email": "jaialgallo@gmail.com",
  "password": "password123"
}
```

```json
{
  "email": "laura.gomez@example.com", 
  "password": "password123"
}
```

```json
{
  "email": "jagallob@eafit.edu.co",
  "password": "password123"
}
```

**Nota**: Todas las contraseñas en el backup están hasheadas con BCrypt. La contraseña original para todos los usuarios es `password123`.

## Arquitectura del Proyecto

### Clean Architecture Structure
```
src/
├── JEGASolutions.ExtraHours.API/           # Controllers, Middleware
├── JEGASolutions.ExtraHours.Core/          # Entities, DTOs, Interfaces
├── JEGASolutions.ExtraHours.Data/          # DbContext, Migrations
└── JEGASolutions.ExtraHours.Infrastructure/ # Repositories, Services
```

### Entidades Principales
- **User**: Usuarios del sistema con roles
- **Employee**: Empleados vinculados a usuarios
- **ExtraHour**: Registro de horas extra
- **Manager**: Gestores/supervisores
- **CompensationRequest**: Solicitudes de compensación

### Compatibilidad con Backup SQL

El proyecto está configurado para ser compatible con el backup SQL existente:

- ✅ **Columnas de auditoría opcionales**: `TenantId`, `CreatedAt`, `UpdatedAt`, `DeletedAt` son opcionales
- ✅ **Valores por defecto**: `TenantId` default = 1, timestamps automáticos
- ✅ **Mapeo de password**: Columna `password` del SQL → `passwordHash` en el modelo
- ✅ **Sin creación automática de tablas**: Usa las tablas del backup
- ✅ **Health checks**: Espera a que PostgreSQL esté listo

## Solución de Problemas

### Error de conexión a base de datos
```bash
# Verificar que PostgreSQL esté corriendo
docker-compose ps

# Ver logs de la base de datos
docker-compose logs db

# Reiniciar servicios
docker-compose down
docker-compose up --build
```

### Error de autenticación JWT
Verifica que las variables JWT estén configuradas en tu archivo `.env`:
```
JWT_SECRET_KEY=your-super-secret-jwt-key-here-make-it-long-and-secure-at-least-32-characters
```

### Problemas con el backend
```bash
# Ver logs del backend
docker-compose logs backend

# Ejecutar solo la base de datos
docker-compose up db

# Acceder al contenedor del backend
docker-compose exec backend bash
```

## Desarrollo

### Ejecutar tests
```bash
docker-compose up test-backend
```

### Acceder a la base de datos
```bash
# Conectar a PostgreSQL
docker-compose exec db psql -U postgres -d postgres

# Ver tablas
\dt

# Ver usuarios
SELECT id, email, name, role FROM users;
```

### Hot Reload (Desarrollo)
Para desarrollo con hot reload, puedes montar el código fuente:

```yaml
# Añadir en docker-compose.yml bajo backend:
volumes:
  - ./backend/src:/src
```
