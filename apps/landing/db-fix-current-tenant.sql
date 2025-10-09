-- ==========================================
-- SCRIPT RÁPIDO PARA CORREGIR EL TENANT ACTUAL
-- Tenant: "juqn-pére4452" → "juqn-pere4452"
-- ==========================================

BEGIN;

-- 1️⃣ Ver el tenant problemático
SELECT
    "Id",
    "CompanyName",
    "Subdomain",
    "CreatedAt"
FROM "Tenants"
WHERE "Subdomain" LIKE '%pére%' OR "Subdomain" LIKE '%pere%';

-- 2️⃣ Actualizar el subdomain (remover acento de la é)
UPDATE "Tenants"
SET
    "Subdomain" = REPLACE("Subdomain", 'é', 'e'),
    "UpdatedAt" = NOW()
WHERE "Subdomain" LIKE '%pére%';

-- 3️⃣ Verificar el cambio
SELECT
    "Id",
    "CompanyName",
    "Subdomain",
    "UpdatedAt"
FROM "Tenants"
WHERE "Subdomain" LIKE '%pere%';

-- 4️⃣ Confirmar
COMMIT;

-- ==========================================
-- PRUEBA: Intenta acceder ahora a:
-- https://juqn-pere4452.jegasolutions.co
-- (sin acento en la é)
-- ==========================================

