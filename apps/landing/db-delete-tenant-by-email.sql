-- ==========================================
-- SCRIPT GENÉRICO: Eliminar Tenant por Email
-- Uso: Cambiar el email en la variable v_email
-- ==========================================

DO $$
DECLARE
    v_email TEXT := 'jagallob@eafit.edu.co'; -- ← CAMBIAR ESTE EMAIL
    v_tenant_id INT;
    v_modules_deleted INT;
    v_users_deleted INT;
    v_payments_deleted INT;
BEGIN
    RAISE NOTICE '━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━';
    RAISE NOTICE '🗑️  ELIMINANDO TENANT PARA: %', v_email;
    RAISE NOTICE '━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━';

    -- Obtener Tenant ID del usuario
    SELECT "TenantId" INTO v_tenant_id
    FROM "Users"
    WHERE "Email" = v_email
    LIMIT 1;

    IF v_tenant_id IS NULL THEN
        RAISE NOTICE '⚠️  No se encontró tenant para el email: %', v_email;
        RAISE NOTICE '✅ No hay nada que eliminar';
        RETURN;
    END IF;

    RAISE NOTICE '📋 Tenant ID encontrado: %', v_tenant_id;

    -- Eliminar módulos
    DELETE FROM "TenantModules" WHERE "TenantId" = v_tenant_id;
    GET DIAGNOSTICS v_modules_deleted = ROW_COUNT;
    RAISE NOTICE '✅ TenantModules eliminados: %', v_modules_deleted;

    -- Eliminar usuarios
    DELETE FROM "Users" WHERE "TenantId" = v_tenant_id;
    GET DIAGNOSTICS v_users_deleted = ROW_COUNT;
    RAISE NOTICE '✅ Users eliminados: %', v_users_deleted;

    -- Eliminar tenant
    DELETE FROM "Tenants" WHERE "Id" = v_tenant_id;
    RAISE NOTICE '✅ Tenant eliminado: %', v_tenant_id;

    -- Eliminar pagos
    DELETE FROM "Payments" WHERE "CustomerEmail" = v_email;
    GET DIAGNOSTICS v_payments_deleted = ROW_COUNT;
    RAISE NOTICE '✅ Payments eliminados: %', v_payments_deleted;

    RAISE NOTICE '━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━';
    RAISE NOTICE '🎉 ELIMINACIÓN COMPLETADA EXITOSAMENTE';
    RAISE NOTICE '━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━';

END $$;

-- Verificación final
SELECT
    'Verificación Final' as titulo,
    COUNT(*) as "registros_restantes",
    'jagallob@eafit.edu.co' as email_verificado
FROM "Users"
WHERE "Email" = 'jagallob@eafit.edu.co';

