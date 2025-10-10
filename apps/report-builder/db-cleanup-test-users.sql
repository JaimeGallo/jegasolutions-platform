-- ============================================
-- Script de Limpieza de Usuarios de Prueba
-- Report Builder - JEGASolutions
-- ============================================
-- Este script elimina los usuarios de prueba creados recientemente
-- y todos los datos relacionados

-- ============================================
-- PASO 1: Verificar los usuarios a eliminar
-- ============================================
SELECT
    id,
    tenant_id,
    email,
    full_name,
    role,
    created_at,
    is_active
FROM "users"
WHERE email IN (
    'jagallob@eafit.edu.co',
    'jaialgallo@hotmail.com',
    'jaialgallo@gmail.com'
);

-- ============================================
-- PASO 2: Eliminar datos relacionados
-- ============================================
-- Si hay reportes creados por estos usuarios
DELETE FROM "reports"
WHERE "created_by_user_id" IN (
    SELECT id FROM "users"
    WHERE email IN (
        'jagallob@eafit.edu.co',
        'jaialgallo@hotmail.com',
        'jaialgallo@gmail.com'
    )
);

-- Si hay templates creadas por estos usuarios
DELETE FROM "templates"
WHERE "created_by" IN (
    SELECT id FROM "users"
    WHERE email IN (
        'jagallob@eafit.edu.co',
        'jaialgallo@hotmail.com',
        'jaialgallo@gmail.com'
    )
);

-- ============================================
-- PASO 3: Eliminar los usuarios
-- ============================================
DELETE FROM "users"
WHERE email IN (
    'jagallob@eafit.edu.co',
    'jaialgallo@hotmail.com',
    'jaialgallo@gmail.com'
);

-- ============================================
-- PASO 4: Verificar la limpieza
-- ============================================
SELECT
    'Usuarios restantes' as tipo,
    COUNT(*) as cantidad
FROM "users"
UNION ALL
SELECT
    'Usuarios activos' as tipo,
    COUNT(*) as cantidad
FROM "users"
WHERE is_active = true;

-- ============================================
-- PASO 5: Reporte final
-- ============================================
DO $$
DECLARE
    user_count INTEGER;
BEGIN
    SELECT COUNT(*) INTO user_count FROM "users";

    RAISE NOTICE '============================================';
    RAISE NOTICE 'Limpieza completada';
    RAISE NOTICE '- Total de usuarios en el sistema: %', user_count;
    RAISE NOTICE '============================================';
END $$;

