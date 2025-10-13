-- ==========================================
-- Script para Crear Tablas en Extra Hours (snake_case)
-- ==========================================
-- Usar SOLO si las migraciones automáticas fallaron
-- Este script crea todas las tablas en snake_case
-- ==========================================

-- Verificar que estás en la BD correcta
SELECT current_database();

-- ⚠️ IMPORTANTE: Eliminar tabla de migraciones si existe con naming incorrecto
DROP TABLE IF EXISTS "__EFMigrationsHistory" CASCADE;

-- Crear tabla de migraciones de EF Core (snake_case)
CREATE TABLE "__EFMigrationsHistory" (
    migration_id character varying(150) NOT NULL,
    product_version character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY (migration_id)
);

-- Crear tabla de usuarios
CREATE TABLE IF NOT EXISTS users (
    id SERIAL PRIMARY KEY,
    email VARCHAR(255) NOT NULL UNIQUE,
    username VARCHAR(255) NOT NULL,
    password VARCHAR(255) NOT NULL,
    name VARCHAR(255),
    role VARCHAR(50) DEFAULT 'employee',
    tenant_id INTEGER DEFAULT 1,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    deleted_at TIMESTAMP NULL
);

-- Crear tabla de managers PRIMERO (porque employees la referencia)
CREATE TABLE IF NOT EXISTS managers (
    manager_id INTEGER PRIMARY KEY REFERENCES users(id) ON DELETE CASCADE,
    department VARCHAR(255),
    tenant_id INTEGER DEFAULT 1,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    deleted_at TIMESTAMP NULL
);

-- Crear tabla de empleados
CREATE TABLE IF NOT EXISTS employees (
    id INTEGER PRIMARY KEY REFERENCES users(id) ON DELETE CASCADE,
    position VARCHAR(255),
    manager_id INTEGER REFERENCES managers(manager_id) ON DELETE SET NULL,
    tenant_id INTEGER DEFAULT 1,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    deleted_at TIMESTAMP NULL
);

-- Crear tabla de extra hours
CREATE TABLE IF NOT EXISTS extra_hours (
    id SERIAL PRIMARY KEY,
    employee_id INTEGER NOT NULL REFERENCES employees(id) ON DELETE CASCADE,
    date TIMESTAMP NOT NULL,
    start_time TIME NOT NULL,
    end_time TIME NOT NULL,
    total_hours DECIMAL(5,2) NOT NULL,
    type VARCHAR(50) NOT NULL,
    status VARCHAR(50) DEFAULT 'pending',
    notes TEXT,
    approved_by INTEGER REFERENCES managers(manager_id),
    approved_at TIMESTAMP,
    tenant_id INTEGER DEFAULT 1,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    deleted_at TIMESTAMP NULL
);

-- Crear tabla de configuración
CREATE TABLE IF NOT EXISTS extra_hours_config (
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

-- Insertar configuración por defecto
INSERT INTO extra_hours_config (id, tenant_id)
VALUES (1, 1)
ON CONFLICT (id) DO NOTHING;

-- Crear tabla de compensation_requests
CREATE TABLE IF NOT EXISTS compensation_requests (
    id SERIAL PRIMARY KEY,
    employee_id INTEGER NOT NULL REFERENCES employees(id) ON DELETE CASCADE,
    work_date TIMESTAMP NOT NULL,
    hours_worked DECIMAL(5,2) NOT NULL,
    requested_compensation_date TIMESTAMP NOT NULL,
    status VARCHAR(50) DEFAULT 'pending',
    requested_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    approved_by_id INTEGER REFERENCES users(id),
    decided_at TIMESTAMP,
    notes TEXT,
    tenant_id INTEGER DEFAULT 1,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    deleted_at TIMESTAMP NULL
);

-- Crear tabla de excel_uploads
CREATE TABLE IF NOT EXISTS excel_uploads (
    id SERIAL PRIMARY KEY,
    filename VARCHAR(255) NOT NULL,
    uploaded_by INTEGER NOT NULL REFERENCES users(id),
    uploaded_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    processed BOOLEAN DEFAULT FALSE,
    error_message TEXT,
    tenant_id INTEGER DEFAULT 1,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    deleted_at TIMESTAMP NULL
);

-- Crear índices para tenant_id
CREATE INDEX IF NOT EXISTS ix_users_tenant_id ON users(tenant_id);
CREATE INDEX IF NOT EXISTS ix_employees_tenant_id ON employees(tenant_id);
CREATE INDEX IF NOT EXISTS ix_managers_tenant_id ON managers(tenant_id);
CREATE INDEX IF NOT EXISTS ix_extra_hours_tenant_id ON extra_hours(tenant_id);
CREATE INDEX IF NOT EXISTS ix_extra_hours_config_tenant_id ON extra_hours_config(tenant_id);
CREATE INDEX IF NOT EXISTS ix_compensation_requests_tenant_id ON compensation_requests(tenant_id);
CREATE INDEX IF NOT EXISTS ix_excel_uploads_tenant_id ON excel_uploads(tenant_id);

-- Registrar migración (para que EF Core no intente aplicarla de nuevo)
INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
VALUES ('20240101000000_InitialCreate', '8.0.0')
ON CONFLICT (migration_id) DO NOTHING;

-- Verificar tablas creadas
SELECT table_name
FROM information_schema.tables
WHERE table_schema = 'public'
ORDER BY table_name;

-- ==========================================
-- RESULTADO ESPERADO
-- ==========================================
-- Deberías ver estas tablas:
-- - __EFMigrationsHistory
-- - compensation_requests
-- - employees
-- - excel_uploads
-- - extra_hours
-- - extra_hours_config
-- - managers
-- - users
-- ==========================================

