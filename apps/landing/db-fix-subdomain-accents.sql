-- ==========================================
-- SCRIPT PARA CORREGIR SUBDOMAINS CON ACENTOS
-- ==========================================
-- Este script corrige los tenants que tienen caracteres con acentos
-- en sus subdomains, convirtiéndolos a ASCII limpio.
--
-- Ejemplo: "juqn-pére4452" → "juqn-pere4452"
-- ==========================================

BEGIN;

-- 1️⃣ Ver tenants con subdomains problemáticos (solo lectura)
SELECT
    "Id",
    "CompanyName",
    "Subdomain" as "SubdomainActual",
    CASE
        -- Reemplazar caracteres comunes con acentos
        WHEN "Subdomain" ~ '[áàäâãåā]' THEN regexp_replace("Subdomain", '[áàäâãåā]', 'a', 'g')
        WHEN "Subdomain" ~ '[éèëêē]' THEN regexp_replace("Subdomain", '[éèëêē]', 'e', 'g')
        WHEN "Subdomain" ~ '[íìïîī]' THEN regexp_replace("Subdomain", '[íìïîī]', 'i', 'g')
        WHEN "Subdomain" ~ '[óòöôõøō]' THEN regexp_replace("Subdomain", '[óòöôõøō]', 'o', 'g')
        WHEN "Subdomain" ~ '[úùüûū]' THEN regexp_replace("Subdomain", '[úùüûū]', 'u', 'g')
        WHEN "Subdomain" ~ '[ñ]' THEN regexp_replace("Subdomain", 'ñ', 'n', 'g')
        WHEN "Subdomain" ~ '[ç]' THEN regexp_replace("Subdomain", 'ç', 'c', 'g')
        ELSE "Subdomain"
    END as "SubdomainCorregido",
    "CreatedAt"
FROM "Tenants"
WHERE
    -- Buscar subdomains que contengan caracteres no-ASCII
    "Subdomain" !~ '^[a-z0-9-]+$';

-- 2️⃣ Actualizar tenants con caracteres acentuados
-- Nota: PostgreSQL no tiene una función nativa para remover acentos,
-- así que usamos múltiples reemplazos

UPDATE "Tenants"
SET "Subdomain" =
    -- Remover todos los acentos comunes
    translate(
        "Subdomain",
        'áàäâãåāéèëêēíìïîīóòöôõøōúùüûūñçÁÀÄÂÃÅĀÉÈËÊĒÍÌÏÎĪÓÒÖÔÕØŌÚÙÜÛŪÑÇ',
        'aaaaaaaeeeeeiiiiiooooooouuuuuncAAAAAAAAAEEEEEIIIIIOOOOOOOUUUUUNC'
    ),
    "UpdatedAt" = NOW()
WHERE
    -- Solo actualizar los que tienen caracteres no-ASCII
    "Subdomain" !~ '^[a-z0-9-]+$';

-- 3️⃣ Verificar los cambios
SELECT
    "Id",
    "CompanyName",
    "Subdomain",
    "UpdatedAt"
FROM "Tenants"
ORDER BY "UpdatedAt" DESC
LIMIT 10;

-- 4️⃣ Confirmar cambios
COMMIT;

-- Si algo sale mal, ejecutar:
-- ROLLBACK;

-- ==========================================
-- VERIFICACIÓN POST-ACTUALIZACIÓN
-- ==========================================

-- Verificar que no queden subdomains con caracteres especiales
SELECT
    COUNT(*) as "TenantsConCaracteresEspeciales"
FROM "Tenants"
WHERE "Subdomain" !~ '^[a-z0-9-]+$';

-- Debería retornar 0 si todo está correcto

