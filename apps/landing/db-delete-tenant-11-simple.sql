-- ==========================================
-- VERSIÓN SIMPLE: Eliminar Tenant 11
-- Email: jagallob@eafit.edu.co
-- ==========================================

BEGIN;

-- Eliminar en orden (respetando foreign keys)
DELETE FROM "TenantModules" WHERE "TenantId" = 11;
DELETE FROM "Users" WHERE "TenantId" = 11;
DELETE FROM "Tenants" WHERE "Id" = 11;
DELETE FROM "Payments" WHERE "CustomerEmail" = 'jagallob@eafit.edu.co';

-- Verificar (debe retornar 0 en todas las filas)
SELECT
    'Tenants' as tabla, COUNT(*) as cantidad FROM "Tenants" WHERE "Id" = 11
UNION ALL
    SELECT 'Users', COUNT(*) FROM "Users" WHERE "TenantId" = 11
UNION ALL
    SELECT 'TenantModules', COUNT(*) FROM "TenantModules" WHERE "TenantId" = 11
UNION ALL
    SELECT 'Payments', COUNT(*) FROM "Payments" WHERE "CustomerEmail" = 'jagallob@eafit.edu.co';

COMMIT;

