-- ==========================================
-- Script para RECREAR la Base de Datos de Extra Hours
-- ==========================================
-- ⚠️ IMPORTANTE:
-- 1. Este script ELIMINA TODOS LOS DATOS de Extra Hours
-- 2. Solo ejecutar si NO hay datos importantes
-- 3. Conectarse a la BD de Extra Hours (NO a NeonDB Landing)
-- 4. Después de ejecutar, hacer redeploy del backend
-- ==========================================

-- Verificar que estás en la BD correcta
SELECT current_database();
-- Debe mostrar el nombre de la BD de Extra Hours, NO "neondb" o la BD de Landing

-- ==========================================
-- PASO 1: ELIMINAR TODO EL SCHEMA PUBLIC
-- ==========================================
DROP SCHEMA public CASCADE;

-- ==========================================
-- PASO 2: RECREAR EL SCHEMA PUBLIC
-- ==========================================
CREATE SCHEMA public;

-- ==========================================
-- PASO 3: DAR PERMISOS AL USUARIO
-- ==========================================
-- Reemplaza 'your_user' con el usuario de tu base de datos
-- Puedes ver el usuario actual con: SELECT current_user;
GRANT ALL ON SCHEMA public TO neondb_owner;
GRANT ALL ON SCHEMA public TO PUBLIC;

-- ==========================================
-- VERIFICACIÓN
-- ==========================================
-- Ver que no haya tablas
SELECT table_name
FROM information_schema.tables
WHERE table_schema = 'public';
-- Debe mostrar 0 filas

-- ==========================================
-- RESULTADO ESPERADO
-- ==========================================
-- ✅ Schema public recreado
-- ✅ Sin tablas
-- ✅ Listo para que EF Core cree todo con snake_case

-- ==========================================
-- PRÓXIMO PASO
-- ==========================================
-- 1. Ejecutar este script en la BD de Extra Hours
-- 2. Hacer redeploy del backend de Extra Hours en Render
-- 3. EF Core creará automáticamente todas las tablas con snake_case
-- 4. Probar el login en https://extrahours.jegasolutions.co
-- ==========================================

