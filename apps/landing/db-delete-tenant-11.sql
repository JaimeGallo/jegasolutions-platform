-- ==========================================
-- SCRIPT PARA ELIMINAR TENANT 11 Y PROBAR NUEVAMENTE
-- ==========================================
-- Usuario ID: 7
-- Tenant ID: 11
-- Email: jagallob@eafit.edu.co
-- Subdomain: juqn-pére4452 (o juqn-pere4452 si ya fue corregido)
-- ==========================================

BEGIN;

-- 📋 PASO 1: Ver qué se va a eliminar (información previa)
SELECT '=== TENANT A ELIMINAR ===' as info;

SELECT
    "Id",
    "CompanyName",
    "Subdomain",
    "IsActive",
    "CreatedAt"
FROM "Tenants"
WHERE "Id" = 11;

SELECT '=== USUARIOS DEL TENANT ===' as info;

SELECT
    "Id",
    "Email",
    "FirstName",
    "LastName",
    "Role",
    "TenantId"
FROM "Users"
WHERE "TenantId" = 11;

SELECT '=== MÓDULOS DEL TENANT ===' as info;

SELECT
    "Id",
    "TenantId",
    "ModuleName",
    "Status",
    "PurchasedAt"
FROM "TenantModules"
WHERE "TenantId" = 11;

SELECT '=== PAGOS ASOCIADOS ===' as info;

SELECT
    "Id",
    "Reference",
    "CustomerEmail",
    "CustomerName",
    "Amount",
    "Status",
    "CreatedAt"
FROM "Payments"
WHERE "CustomerEmail" = 'jagallob@eafit.edu.co';

-- ==========================================
-- ELIMINACIÓN EN ORDEN (respetando foreign keys)
-- ==========================================

SELECT '=== INICIANDO ELIMINACIÓN ===' as info;

-- 1️⃣ Eliminar módulos del tenant (TenantModules depende de Tenants)
DELETE FROM "TenantModules"
WHERE "TenantId" = 11;

SELECT 'TenantModules eliminados' as paso;

-- 2️⃣ Eliminar usuarios del tenant (Users depende de Tenants)
DELETE FROM "Users"
WHERE "TenantId" = 11;

SELECT 'Users eliminados' as paso;

-- 3️⃣ Eliminar el tenant
DELETE FROM "Tenants"
WHERE "Id" = 11;

SELECT 'Tenant eliminado' as paso;

-- 4️⃣ OPCIONAL: Eliminar pagos asociados al email
-- (Descomenta si quieres eliminar el historial de pagos también)
DELETE FROM "Payments"
WHERE "CustomerEmail" = 'jagallob@eafit.edu.co';

SELECT 'Payments eliminados' as paso;

-- ==========================================
-- VERIFICACIÓN FINAL
-- ==========================================

SELECT '=== VERIFICACIÓN FINAL ===' as info;

SELECT
    'Tenants' as tabla,
    COUNT(*) as "cantidad_restante"
FROM "Tenants"
WHERE "Id" = 11

UNION ALL

SELECT
    'Users',
    COUNT(*)
FROM "Users"
WHERE "TenantId" = 11

UNION ALL

SELECT
    'TenantModules',
    COUNT(*)
FROM "TenantModules"
WHERE "TenantId" = 11

UNION ALL

SELECT
    'Payments',
    COUNT(*)
FROM "Payments"
WHERE "CustomerEmail" = 'jagallob@eafit.edu.co';

-- ⚠️ Todas las cantidades deberían ser 0

-- ==========================================
-- CONFIRMAR O REVERTIR
-- ==========================================

-- ✅ Si todo se ve bien, CONFIRMAR:
COMMIT;

-- ❌ Si algo salió mal, REVERTIR:
-- ROLLBACK;

SELECT '✅ ELIMINACIÓN COMPLETADA' as resultado;
SELECT 'Ahora puedes hacer un nuevo pago con jagallob@eafit.edu.co' as siguiente_paso;

