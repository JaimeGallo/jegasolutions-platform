-- ==========================================
-- ELIMINAR TENANT 12 PARA PROBAR NUEVAMENTE
-- ==========================================
-- Usuario ID: 8
-- Tenant ID: 12
-- Email: jagallob@eafit.edu.co
-- Company: JuanFactory
-- ==========================================

BEGIN;

-- Ver información antes de eliminar
SELECT '=== DATOS A ELIMINAR ===' as info;

SELECT * FROM "Tenants" WHERE "Id" = 12;
SELECT * FROM "Users" WHERE "TenantId" = 12;
SELECT * FROM "TenantModules" WHERE "TenantId" = 12;
SELECT * FROM "Payments" WHERE "CustomerEmail" = 'jagallob@eafit.edu.co';

-- Eliminar en orden (respetando foreign keys)
DELETE FROM "TenantModules" WHERE "TenantId" = 12;
DELETE FROM "Users" WHERE "TenantId" = 12;
DELETE FROM "Tenants" WHERE "Id" = 12;
DELETE FROM "Payments" WHERE "CustomerEmail" = 'jagallob@eafit.edu.co';

-- Verificar eliminación (debe retornar 0 en todo)
SELECT '=== VERIFICACIÓN ===' as info;
SELECT
    'Tenants' as tabla, COUNT(*) as restantes FROM "Tenants" WHERE "Id" = 12
UNION ALL
SELECT 'Users', COUNT(*) FROM "Users" WHERE "TenantId" = 12
UNION ALL
SELECT 'TenantModules', COUNT(*) FROM "TenantModules" WHERE "TenantId" = 12
UNION ALL
SELECT 'Payments', COUNT(*) FROM "Payments" WHERE "CustomerEmail" = 'jagallob@eafit.edu.co';

COMMIT;

-- ==========================================
-- ✅ LISTO PARA PROBAR NUEVAMENTE
-- ==========================================
--
-- El próximo pago con jagallob@eafit.edu.co:
--
-- 1. ✅ Creará subdomain SIN acentos (normalización ASCII)
-- 2. ✅ Enviará email de bienvenida
-- 3. ✅ Dashboard mostrará módulo como "Activo" (fix case-sensitive)
-- 4. ✅ Sin sección "Acciones Rápidas" (comentada)
--
-- ==========================================


