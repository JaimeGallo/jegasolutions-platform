-- Migration to add multi-tenancy support to Extra-Hours module
-- This script adds TenantId, CreatedAt, UpdatedAt, and DeletedAt columns to all entities

-- Add TenantId and audit columns to employees table
ALTER TABLE employees 
ADD COLUMN IF NOT EXISTS "TenantId" INTEGER NOT NULL DEFAULT 1,
ADD COLUMN IF NOT EXISTS "CreatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
ADD COLUMN IF NOT EXISTS "UpdatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
ADD COLUMN IF NOT EXISTS "DeletedAt" TIMESTAMP NULL;

-- Add TenantId and audit columns to extra_hours table
ALTER TABLE extra_hours 
ADD COLUMN IF NOT EXISTS "TenantId" INTEGER NOT NULL DEFAULT 1,
ADD COLUMN IF NOT EXISTS "CreatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
ADD COLUMN IF NOT EXISTS "UpdatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
ADD COLUMN IF NOT EXISTS "DeletedAt" TIMESTAMP NULL;

-- Add TenantId and audit columns to extra_hours_config table
ALTER TABLE extra_hours_config 
ADD COLUMN IF NOT EXISTS "TenantId" INTEGER NOT NULL DEFAULT 1,
ADD COLUMN IF NOT EXISTS "CreatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
ADD COLUMN IF NOT EXISTS "UpdatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
ADD COLUMN IF NOT EXISTS "DeletedAt" TIMESTAMP NULL;

-- Add TenantId and audit columns to managers table
ALTER TABLE managers 
ADD COLUMN IF NOT EXISTS "TenantId" INTEGER NOT NULL DEFAULT 1,
ADD COLUMN IF NOT EXISTS "CreatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
ADD COLUMN IF NOT EXISTS "UpdatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
ADD COLUMN IF NOT EXISTS "DeletedAt" TIMESTAMP NULL;

-- Add TenantId and audit columns to users table
ALTER TABLE users 
ADD COLUMN IF NOT EXISTS "TenantId" INTEGER NOT NULL DEFAULT 1,
ADD COLUMN IF NOT EXISTS "CreatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
ADD COLUMN IF NOT EXISTS "UpdatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
ADD COLUMN IF NOT EXISTS "DeletedAt" TIMESTAMP NULL;

-- Add TenantId and audit columns to compensationrequests table
ALTER TABLE compensationrequests 
ADD COLUMN IF NOT EXISTS "TenantId" INTEGER NOT NULL DEFAULT 1,
ADD COLUMN IF NOT EXISTS "CreatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
ADD COLUMN IF NOT EXISTS "UpdatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
ADD COLUMN IF NOT EXISTS "DeletedAt" TIMESTAMP NULL;

-- Create indexes for TenantId columns for better performance
CREATE INDEX IF NOT EXISTS "IX_employees_tenant_id" ON employees ("TenantId");
CREATE INDEX IF NOT EXISTS "IX_extra_hours_tenant_id" ON extra_hours ("TenantId");
CREATE INDEX IF NOT EXISTS "IX_extra_hours_config_tenant_id" ON extra_hours_config ("TenantId");
CREATE INDEX IF NOT EXISTS "IX_managers_tenant_id" ON managers ("TenantId");
CREATE INDEX IF NOT EXISTS "IX_users_tenant_id" ON users ("TenantId");
CREATE INDEX IF NOT EXISTS "IX_compensation_requests_tenant_id" ON compensationrequests ("TenantId");

-- Create composite indexes for common queries
CREATE INDEX IF NOT EXISTS "IX_extra_hours_tenant_employee" ON extra_hours ("TenantId", "id");
CREATE INDEX IF NOT EXISTS "IX_extra_hours_tenant_date" ON extra_hours ("TenantId", "date");
CREATE INDEX IF NOT EXISTS "IX_employees_tenant_manager" ON employees ("TenantId", "manager_id");

-- Add foreign key constraints for TenantId (if you have a tenants table)
-- Note: Uncomment these lines if you have a tenants table in your system
-- ALTER TABLE employees ADD CONSTRAINT "FK_employees_tenants" FOREIGN KEY ("TenantId") REFERENCES tenants("Id");
-- ALTER TABLE extra_hours ADD CONSTRAINT "FK_extra_hours_tenants" FOREIGN KEY ("TenantId") REFERENCES tenants("Id");
-- ALTER TABLE extra_hours_config ADD CONSTRAINT "FK_extra_hours_config_tenants" FOREIGN KEY ("TenantId") REFERENCES tenants("Id");
-- ALTER TABLE managers ADD CONSTRAINT "FK_managers_tenants" FOREIGN KEY ("TenantId") REFERENCES tenants("Id");
-- ALTER TABLE users ADD CONSTRAINT "FK_users_tenants" FOREIGN KEY ("TenantId") REFERENCES tenants("Id");
-- ALTER TABLE compensationrequests ADD CONSTRAINT "FK_compensation_requests_tenants" FOREIGN KEY ("TenantId") REFERENCES tenants("Id");

-- Update existing records to have TenantId = 1 (default tenant)
-- This ensures existing data is not lost during migration
UPDATE employees SET "TenantId" = 1 WHERE "TenantId" IS NULL;
UPDATE extra_hours SET "TenantId" = 1 WHERE "TenantId" IS NULL;
UPDATE extra_hours_config SET "TenantId" = 1 WHERE "TenantId" IS NULL;
UPDATE managers SET "TenantId" = 1 WHERE "TenantId" IS NULL;
UPDATE users SET "TenantId" = 1 WHERE "TenantId" IS NULL;
UPDATE compensationrequests SET "TenantId" = 1 WHERE "TenantId" IS NULL;

-- Remove default values after migration is complete
-- Note: Run this after confirming all data has been migrated successfully
-- ALTER TABLE employees ALTER COLUMN "TenantId" DROP DEFAULT;
-- ALTER TABLE extra_hours ALTER COLUMN "TenantId" DROP DEFAULT;
-- ALTER TABLE extra_hours_config ALTER COLUMN "TenantId" DROP DEFAULT;
-- ALTER TABLE managers ALTER COLUMN "TenantId" DROP DEFAULT;
-- ALTER TABLE users ALTER COLUMN "TenantId" DROP DEFAULT;
-- ALTER TABLE compensationrequests ALTER COLUMN "TenantId" DROP DEFAULT;
