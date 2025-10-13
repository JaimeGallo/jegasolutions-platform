-- ==========================================
-- Script para Corregir la Tabla __EFMigrationsHistory
-- ==========================================
-- Este script corrige el problema de naming en la tabla de migraciones
-- ==========================================

-- Verificar que estás en la BD correcta
SELECT current_database();

-- ==========================================
-- PASO 1: Verificar el estado actual
-- ==========================================
SELECT column_name, data_type
FROM information_schema.columns
WHERE table_name = '__EFMigrationsHistory'
AND table_schema = 'public';

-- ==========================================
-- PASO 2: Eliminar la tabla problemática
-- ==========================================
DROP TABLE IF EXISTS "__EFMigrationsHistory" CASCADE;

-- ==========================================
-- PASO 3: Crear la tabla con snake_case (para coincidir con UseSnakeCaseNamingConvention)
-- ==========================================
CREATE TABLE "__EFMigrationsHistory" (
    migration_id character varying(150) NOT NULL,
    product_version character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY (migration_id)
);

-- ==========================================
-- PASO 4: Insertar migración inicial si es necesario
-- ==========================================
INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
VALUES ('20240101000000_InitialCreate', '8.0.0')
ON CONFLICT (migration_id) DO NOTHING;

-- ==========================================
-- VERIFICACIÓN
-- ==========================================
SELECT column_name, data_type
FROM information_schema.columns
WHERE table_name = '__EFMigrationsHistory'
AND table_schema = 'public';

-- Debe mostrar:
-- - migration_id (character varying)
-- - product_version (character varying)
