-- ===================================================================
-- Script para agregar usuarios existentes a la tabla de Extra Hours
-- ===================================================================
--
-- Este script copia usuarios de la tabla "Users" (Landing) a la tabla
-- "users" (Extra Hours) para que puedan iniciar sesión en ambos sistemas.
--
-- Se ejecuta SOLO UNA VEZ para usuarios creados antes de esta corrección.
-- Los nuevos usuarios se crearán automáticamente en ambas tablas.
-- ===================================================================

-- Insertar usuarios existentes en la tabla users de Extra Hours
INSERT INTO users (email, name, username, password, role, "TenantId")
SELECT
    u."Email" as email,
    CONCAT(u."FirstName", ' ', COALESCE(u."LastName", '')) as name,
    SPLIT_PART(u."Email", '@', 1) as username,
    u."PasswordHash" as password,
    'superusuario' as role,  -- Rol por defecto en Extra Hours
    u."TenantId" as "TenantId"
FROM "Users" u
WHERE u."IsActive" = true
AND NOT EXISTS (
    SELECT 1 FROM users eh WHERE eh.email = u."Email"
);

-- Verificar cuántos usuarios se insertaron
SELECT
    COUNT(*) as usuarios_insertados,
    'Usuarios copiados de Landing a Extra Hours' as descripcion
FROM users
WHERE email IN (SELECT "Email" FROM "Users" WHERE "IsActive" = true);

-- Ver los usuarios insertados
SELECT
    id,
    email,
    name,
    username,
    role,
    "TenantId"
FROM users
ORDER BY id DESC;

