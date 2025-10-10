-- ===================================================================
-- Script para LIMPIAR la base de datos y empezar desde cero
-- ===================================================================
-- ⚠️ ADVERTENCIA: Este script elimina TODOS los datos de las tablas
-- Solo ejecutar en ambiente de DESARROLLO/TESTING
-- ===================================================================

-- Deshabilitar restricciones de foreign keys temporalmente
SET session_replication_role = 'replica';

-- ============================================
-- TABLAS DE LANDING
-- ============================================

-- Limpiar TenantModules (debe ir primero por foreign keys)
DELETE FROM "TenantModules";
ALTER SEQUENCE "TenantModules_Id_seq" RESTART WITH 1;

-- Limpiar Users
DELETE FROM "Users";
ALTER SEQUENCE "Users_Id_seq" RESTART WITH 1;

-- Limpiar Tenants
DELETE FROM "Tenants";
ALTER SEQUENCE "Tenants_Id_seq" RESTART WITH 1;

-- Limpiar Payments
DELETE FROM "Payments";
ALTER SEQUENCE "Payments_Id_seq" RESTART WITH 1;

-- ============================================
-- TABLAS DE EXTRA HOURS
-- ============================================

-- Limpiar compensation_requests (si existe)
DELETE FROM compensation_requests WHERE true;

-- Limpiar extra_hours
DELETE FROM extra_hours WHERE true;

-- Limpiar employees
DELETE FROM employees WHERE true;

-- Limpiar managers
DELETE FROM managers WHERE true;

-- Limpiar extra_hours_config
DELETE FROM extra_hours_config WHERE true;

-- Limpiar users de Extra Hours
DELETE FROM users WHERE true;
-- Reiniciar secuencia (si existe)
-- ALTER SEQUENCE users_id_seq RESTART WITH 1;

-- ============================================
-- TABLAS DE REPORT BUILDER (si existen)
-- ============================================

-- Limpiar report_submissions
DELETE FROM report_submissions WHERE true;

-- Limpiar excel_uploads
DELETE FROM excel_uploads WHERE true;

-- Limpiar consolidated_template_sections
DELETE FROM consolidated_template_sections WHERE true;

-- Limpiar consolidated_templates
DELETE FROM consolidated_templates WHERE true;

-- Limpiar templates
DELETE FROM templates WHERE true;

-- Limpiar ai_insights
DELETE FROM ai_insights WHERE true;

-- Limpiar areas
DELETE FROM areas WHERE true;

-- Rehabilitar restricciones de foreign keys
SET session_replication_role = 'origin';

-- ============================================
-- VERIFICACIÓN
-- ============================================

-- Verificar que todas las tablas estén vacías
SELECT 'Payments' as tabla, COUNT(*) as registros FROM "Payments"
UNION ALL
SELECT 'Tenants', COUNT(*) FROM "Tenants"
UNION ALL
SELECT 'TenantModules', COUNT(*) FROM "TenantModules"
UNION ALL
SELECT 'Users (Landing)', COUNT(*) FROM "Users"
UNION ALL
SELECT 'users (Extra Hours)', COUNT(*) FROM users
UNION ALL
SELECT 'employees', COUNT(*) FROM employees
UNION ALL
SELECT 'managers', COUNT(*) FROM managers
UNION ALL
SELECT 'extra_hours', COUNT(*) FROM extra_hours
UNION ALL
SELECT 'templates', COUNT(*) FROM templates
UNION ALL
SELECT 'areas', COUNT(*) FROM areas;

-- ✅ Todos los registros deben mostrar 0

