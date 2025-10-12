-- ==========================================
-- Migración: Agregar tabla user_module_access para SSO
-- ==========================================
-- Esta tabla controla qué usuarios tienen acceso a qué módulos
-- ==========================================

-- Crear tabla de permisos de usuarios por módulo
CREATE TABLE IF NOT EXISTS user_module_access (
    id SERIAL PRIMARY KEY,
    user_id INTEGER NOT NULL REFERENCES "Users"("Id") ON DELETE CASCADE,
    tenant_id INTEGER NOT NULL REFERENCES "Tenants"("Id") ON DELETE CASCADE,
    module_name VARCHAR(100) NOT NULL,  -- 'Extra Hours', 'Report Builder'
    role VARCHAR(50) NOT NULL,          -- 'employee', 'manager', 'admin'
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    -- Un usuario solo puede tener un rol por módulo
    UNIQUE(user_id, module_name)
);

-- Índices para optimizar queries
CREATE INDEX IF NOT EXISTS idx_user_module_access_user_id ON user_module_access(user_id);
CREATE INDEX IF NOT EXISTS idx_user_module_access_tenant_id ON user_module_access(tenant_id);
CREATE INDEX IF NOT EXISTS idx_user_module_access_module_name ON user_module_access(module_name);
CREATE INDEX IF NOT EXISTS idx_user_module_access_active ON user_module_access(is_active) WHERE is_active = TRUE;

-- Trigger para actualizar updated_at automáticamente
CREATE OR REPLACE FUNCTION update_user_module_access_updated_at()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trigger_user_module_access_updated_at
    BEFORE UPDATE ON user_module_access
    FOR EACH ROW
    EXECUTE FUNCTION update_user_module_access_updated_at();

-- ==========================================
-- MIGRAR DATOS EXISTENTES (OPCIONAL)
-- ==========================================
-- Si ya tienes usuarios con módulos activos, ejecuta esto:

-- Asignar acceso automático al owner del tenant con todos sus módulos
INSERT INTO user_module_access (user_id, tenant_id, module_name, role)
SELECT
    u."Id" as user_id,
    u."TenantId" as tenant_id,
    tm."ModuleName" as module_name,
    'admin' as role
FROM "Users" u
INNER JOIN "Tenants" t ON t."Id" = u."TenantId"
INNER JOIN "TenantModules" tm ON tm."TenantId" = u."TenantId"
WHERE tm."Status" = 'ACTIVE'
  AND NOT EXISTS (
      SELECT 1 FROM user_module_access uma
      WHERE uma.user_id = u."Id" AND uma.module_name = tm."ModuleName"
  )
ON CONFLICT (user_id, module_name) DO NOTHING;

-- Verificar datos insertados
SELECT
    u."Email",
    t."CompanyName",
    uma.module_name,
    uma.role,
    uma.is_active
FROM user_module_access uma
INNER JOIN "Users" u ON u."Id" = uma.user_id
INNER JOIN "Tenants" t ON t."Id" = uma.tenant_id
ORDER BY t."CompanyName", uma.module_name;

-- ==========================================
-- RESULTADO ESPERADO
-- ==========================================
-- Tabla creada con:
-- - Control de acceso por usuario y módulo
-- - Roles específicos por módulo
-- - Índices para performance
-- - Trigger para actualizar timestamps
-- ==========================================

