-- ==========================================
-- Script para limpiar tablas de módulos de la BD Landing
-- ==========================================
-- Estas tablas pertenecen a las bases de datos específicas:
-- - Extra Hours DB: employees, extra_hours, managers, etc.
-- - Report Builder DB: templates, report_submissions, ai_insights, etc.
--
-- ⚠️ IMPORTANTE:
-- 1. Hacer backup antes de ejecutar
-- 2. Verificar que las tablas existen en las BDs de los módulos
-- 3. Ejecutar en la BD Landing (NeonDB principal)
-- ==========================================

BEGIN;

-- ==========================================
-- TABLAS DE EXTRA HOURS
-- ==========================================
DROP TABLE IF EXISTS compensation_requests CASCADE;
DROP TABLE IF EXISTS extra_hours CASCADE;
DROP TABLE IF EXISTS extra_hours_config CASCADE;
DROP TABLE IF EXISTS employees CASCADE;
DROP TABLE IF EXISTS managers CASCADE;
DROP TABLE IF EXISTS excel_uploads CASCADE;

-- ==========================================
-- TABLAS DE REPORT BUILDER
-- ==========================================
DROP TABLE IF EXISTS ai_insights CASCADE;
DROP TABLE IF EXISTS areas CASCADE;
DROP TABLE IF EXISTS consolidated_templates CASCADE;
DROP TABLE IF EXISTS report_submissions CASCADE;
DROP TABLE IF EXISTS templates CASCADE;

-- ==========================================
-- TABLA COMPARTIDA (verificar si está en uso)
-- ==========================================
-- ⚠️ CUIDADO: La tabla 'users' podría estar siendo usada
-- Si Landing tiene su propia tabla users, NO la elimines
-- DROP TABLE IF EXISTS users CASCADE;

-- ==========================================
-- VERIFICACIÓN: Tablas restantes en Landing
-- ==========================================
-- Ejecuta esta query para ver qué tablas quedan:
-- SELECT table_name
-- FROM information_schema.tables
-- WHERE table_schema = 'public'
-- AND table_type = 'BASE TABLE'
-- ORDER BY table_name;

COMMIT;

-- ==========================================
-- ROLLBACK SI ALGO SALE MAL
-- ==========================================
-- Si algo sale mal, ejecuta:
-- ROLLBACK;

-- ==========================================
-- RESUMEN DE LO QUE SE ELIMINARÁ
-- ==========================================
-- Extra Hours (6 tablas):
--   ✓ compensation_requests
--   ✓ extra_hours
--   ✓ extra_hours_config
--   ✓ employees
--   ✓ managers
--   ✓ excel_uploads
--
-- Report Builder (5 tablas):
--   ✓ ai_insights
--   ✓ areas
--   ✓ consolidated_templates
--   ✓ report_submissions
--   ✓ templates
--
-- NO eliminadas:
--   ⚠️ users (puede estar en uso por Landing)
-- ==========================================

