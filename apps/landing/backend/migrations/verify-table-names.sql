-- Script para verificar nombres de tablas en NeonDB
-- Ejecutar primero para ver qué tablas existen

SELECT
    table_name,
    table_schema
FROM information_schema.tables
WHERE table_schema = 'public'
ORDER BY table_name;

-- Ver estructura de columnas
SELECT
    table_name,
    column_name,
    data_type
FROM information_schema.columns
WHERE table_schema = 'public'
  AND table_name IN ('users', 'Users', 'tenants', 'Tenants')
ORDER BY table_name, ordinal_position;

