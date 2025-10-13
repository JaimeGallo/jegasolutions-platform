-- =============================================
-- SCRIPT DE LIMPIEZA DE BASE DE DATOS
-- =============================================
-- ⚠️  ADVERTENCIA: Este script eliminará TODOS los datos
-- ⚠️  Solo ejecutar en desarrollo/testing
-- =============================================

-- 1. Eliminar datos de user_module_access (tabla SSO)
DELETE FROM user_module_access;

-- 2. Eliminar usuarios
DELETE FROM "Users";

-- 3. Eliminar tenants
DELETE FROM "Tenants";

-- 4. Eliminar tenant_modules
DELETE FROM "TenantModules";

-- 5. Resetear secuencias (auto-increment)
ALTER SEQUENCE "Users_Id_seq" RESTART WITH 1;
ALTER SEQUENCE "Tenants_Id_seq" RESTART WITH 1;
ALTER SEQUENCE "TenantModules_Id_seq" RESTART WITH 1;
ALTER SEQUENCE user_module_access_id_seq RESTART WITH 1;

-- 6. Verificar limpieza
SELECT 'Users' as tabla, COUNT(*) as registros FROM "Users"
UNION ALL
SELECT 'Tenants' as tabla, COUNT(*) as registros FROM "Tenants"
UNION ALL
SELECT 'TenantModules' as tabla, COUNT(*) as registros FROM "TenantModules"
UNION ALL
SELECT 'user_module_access' as tabla, COUNT(*) as registros FROM user_module_access;

-- =============================================
-- RESULTADO ESPERADO:
-- tabla              | registros
-- -------------------|----------
-- Users              | 0
-- Tenants            | 0
-- TenantModules      | 0
-- user_module_access | 0
-- =============================================
