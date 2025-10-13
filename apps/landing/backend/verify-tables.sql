-- ==========================================
-- Script de Verificación de Tablas
-- ==========================================
-- Ejecuta este script ANTES de limpiar para ver qué tablas tienes
-- ==========================================

-- 1. Ver TODAS las tablas en la base de datos Landing
SELECT
    table_name,
    CASE
        WHEN table_name IN ('compensation_requests', 'extra_hours', 'extra_hours_config',
                           'employees', 'managers', 'excel_uploads')
        THEN '🔴 Extra Hours'
        WHEN table_name IN ('ai_insights', 'areas', 'consolidated_templates',
                           'report_submissions', 'templates')
        THEN '🔴 Report Builder'
        WHEN table_name = 'users'
        THEN '⚠️ Compartida'
        ELSE '✅ Landing'
    END as origen,
    pg_size_pretty(pg_total_relation_size(quote_ident(table_name))) as tamaño
FROM information_schema.tables
WHERE table_schema = 'public'
  AND table_type = 'BASE TABLE'
ORDER BY origen, table_name;

-- ==========================================
-- 2. Contar registros en las tablas que se van a eliminar
-- ==========================================
SELECT 'compensation_requests' as tabla, COUNT(*) as registros FROM compensation_requests
UNION ALL
SELECT 'extra_hours', COUNT(*) FROM extra_hours
UNION ALL
SELECT 'extra_hours_config', COUNT(*) FROM extra_hours_config
UNION ALL
SELECT 'employees', COUNT(*) FROM employees
UNION ALL
SELECT 'managers', COUNT(*) FROM managers
UNION ALL
SELECT 'excel_uploads', COUNT(*) FROM excel_uploads
UNION ALL
SELECT 'ai_insights', COUNT(*) FROM ai_insights
UNION ALL
SELECT 'areas', COUNT(*) FROM areas
UNION ALL
SELECT 'consolidated_templates', COUNT(*) FROM consolidated_templates
UNION ALL
SELECT 'report_submissions', COUNT(*) FROM report_submissions
UNION ALL
SELECT 'templates', COUNT(*) FROM templates;

-- ==========================================
-- 3. Ver dependencias (foreign keys)
-- ==========================================
SELECT
    tc.constraint_name,
    tc.table_name,
    kcu.column_name,
    ccu.table_name AS foreign_table_name,
    ccu.column_name AS foreign_column_name
FROM information_schema.table_constraints AS tc
JOIN information_schema.key_column_usage AS kcu
    ON tc.constraint_name = kcu.constraint_name
    AND tc.table_schema = kcu.table_schema
JOIN information_schema.constraint_column_usage AS ccu
    ON ccu.constraint_name = tc.constraint_name
    AND ccu.table_schema = tc.table_schema
WHERE tc.constraint_type = 'FOREIGN KEY'
  AND tc.table_name IN (
    'compensation_requests', 'extra_hours', 'extra_hours_config',
    'employees', 'managers', 'excel_uploads',
    'ai_insights', 'areas', 'consolidated_templates',
    'report_submissions', 'templates'
  )
ORDER BY tc.table_name;

