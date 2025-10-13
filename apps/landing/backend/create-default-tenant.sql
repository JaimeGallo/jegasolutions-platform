-- ===================================================================
-- Script para crear tenant por defecto para Vercel deployment
-- ===================================================================
-- Este script crea un tenant por defecto que se usa cuando no se detecta
-- un tenant específico en la URL del tenant-dashboard
-- ===================================================================

-- Crear tenant por defecto
INSERT INTO "Tenants" (
    "CompanyName",
    "Subdomain",
    "IsActive",
    "CreatedAt"
) VALUES (
    'JEGASolutions Default',
    'default-tenant',
    true,
    CURRENT_TIMESTAMP
) ON CONFLICT ("Subdomain") DO NOTHING;

-- Obtener el ID del tenant por defecto
DO $$
DECLARE
    default_tenant_id INTEGER;
BEGIN
    -- Obtener el ID del tenant por defecto
    SELECT "Id" INTO default_tenant_id
    FROM "Tenants"
    WHERE "Subdomain" = 'default-tenant';

    -- Crear módulos por defecto para el tenant
    INSERT INTO "TenantModules" (
        "TenantId",
        "ModuleName",
        "Status",
        "PurchasedAt"
    ) VALUES
        (default_tenant_id, 'Extra Hours', 'ACTIVE', CURRENT_TIMESTAMP),
        (default_tenant_id, 'Report Builder', 'ACTIVE', CURRENT_TIMESTAMP)
    ON CONFLICT ("TenantId", "ModuleName") DO NOTHING;

    RAISE NOTICE 'Tenant por defecto creado con ID: %', default_tenant_id;
END $$;

-- Verificar que se creó correctamente
SELECT
    t."Id",
    t."CompanyName",
    t."Subdomain",
    t."IsActive",
    COUNT(tm."Id") as "ModuleCount"
FROM "Tenants" t
LEFT JOIN "TenantModules" tm ON t."Id" = tm."TenantId"
WHERE t."Subdomain" = 'default-tenant'
GROUP BY t."Id", t."CompanyName", t."Subdomain", t."IsActive";
