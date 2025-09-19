-- Script para corregir la secuencia de la tabla extra_hours
-- Este script debe ejecutarse después de cargar el backup_with_compensation.sql

-- Asegurar que la secuencia existe
CREATE SEQUENCE IF NOT EXISTS public.extra_hours_registry_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;

-- Configurar la secuencia para que sea propiedad de la tabla extra_hours
ALTER SEQUENCE public.extra_hours_registry_seq OWNED BY public.extra_hours.registry;

-- Agregar el valor por defecto a la columna registry
ALTER TABLE public.extra_hours ALTER COLUMN registry SET DEFAULT nextval('public.extra_hours_registry_seq'::regclass);

-- Configurar el valor actual de la secuencia basado en el máximo valor existente
SELECT setval('public.extra_hours_registry_seq', COALESCE((SELECT MAX(registry) FROM public.extra_hours), 0) + 1, true); 