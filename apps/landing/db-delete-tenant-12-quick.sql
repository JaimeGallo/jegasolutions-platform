-- ELIMINAR TENANT 12 - VERSIÓN RÁPIDA
BEGIN;
DELETE FROM "TenantModules" WHERE "TenantId" = 12;
DELETE FROM "Users" WHERE "TenantId" = 12;
DELETE FROM "Tenants" WHERE "Id" = 12;
DELETE FROM "Payments" WHERE "CustomerEmail" = 'jagallob@eafit.edu.co';
COMMIT;


