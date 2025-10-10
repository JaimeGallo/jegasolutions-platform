-- ============================================
-- Verificar las Tablas Existentes en la Base de Datos
-- ============================================

-- Ver todas las tablas en el esquema public
SELECT
    schemaname,
    tablename
FROM pg_tables
WHERE schemaname = 'public'
ORDER BY tablename;

-- Ver todas las tablas con información detallada
SELECT
    table_schema,
    table_name,
    table_type
FROM information_schema.tables
WHERE table_schema = 'public'
ORDER BY table_name;

