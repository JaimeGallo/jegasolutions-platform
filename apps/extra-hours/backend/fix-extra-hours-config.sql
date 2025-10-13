-- ==========================================
-- Script para Corregir la Tabla extra_hours_config
-- ==========================================
-- Este script corrige el problema de la tabla extra_hours_config
-- ==========================================

-- Verificar que estás en la BD correcta
SELECT current_database();

-- ==========================================
-- PASO 1: Verificar el estado actual
-- ==========================================
SELECT column_name, data_type
FROM information_schema.columns
WHERE table_name = 'extra_hours_config'
AND table_schema = 'public';

-- ==========================================
-- PASO 2: Eliminar la tabla si existe
-- ==========================================
DROP TABLE IF EXISTS extra_hours_config CASCADE;

-- ==========================================
-- PASO 3: Crear la tabla con el esquema correcto (snake_case)
-- ==========================================
CREATE TABLE extra_hours_config (
    id SERIAL PRIMARY KEY,
    diurnal_start TIME DEFAULT '06:00:00',
    diurnal_end TIME DEFAULT '21:00:00',
    diurnal_multiplier DECIMAL(3,2) DEFAULT 1.25,
    nocturnal_multiplier DECIMAL(3,2) DEFAULT 1.75,
    diurnal_holiday_multiplier DECIMAL(3,2) DEFAULT 2.00,
    nocturnal_holiday_multiplier DECIMAL(3,2) DEFAULT 2.50,
    weekly_extra_hours_limit INTEGER DEFAULT 48,
    tenant_id INTEGER DEFAULT 1,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    deleted_at TIMESTAMP NULL
);

-- ==========================================
-- PASO 4: Insertar configuración por defecto
-- ==========================================
INSERT INTO extra_hours_config (id, tenant_id)
VALUES (1, 1)
ON CONFLICT (id) DO NOTHING;

-- ==========================================
-- VERIFICACIÓN
-- ==========================================
SELECT column_name, data_type
FROM information_schema.columns
WHERE table_name = 'extra_hours_config'
AND table_schema = 'public';

-- Debe mostrar:
-- - id (integer)
-- - diurnal_start (time without time zone)
-- - diurnal_end (time without time zone)
-- - diurnal_multiplier (numeric)
-- - nocturnal_multiplier (numeric)
-- - diurnal_holiday_multiplier (numeric)
-- - nocturnal_holiday_multiplier (numeric)
-- - weekly_extra_hours_limit (integer)
-- - tenant_id (integer)
-- - created_at (timestamp without time zone)
-- - updated_at (timestamp without time zone)
-- - deleted_at (timestamp without time zone)
