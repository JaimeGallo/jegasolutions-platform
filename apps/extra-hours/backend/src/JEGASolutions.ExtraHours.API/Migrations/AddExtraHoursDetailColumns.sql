-- Migration to add detailed hour columns to extra_hours table
-- This allows storing breakdown of diurnal, nocturnal, and holiday hours

-- Add columns for detailed hour tracking
ALTER TABLE extra_hours
ADD COLUMN IF NOT EXISTS diurnal numeric(5,2) DEFAULT 0 NOT NULL,
ADD COLUMN IF NOT EXISTS nocturnal numeric(5,2) DEFAULT 0 NOT NULL,
ADD COLUMN IF NOT EXISTS diurnal_holiday numeric(5,2) DEFAULT 0 NOT NULL,
ADD COLUMN IF NOT EXISTS nocturnal_holiday numeric(5,2) DEFAULT 0 NOT NULL;

-- Make type column nullable since we'll have detailed breakdowns
ALTER TABLE extra_hours
ALTER COLUMN type DROP NOT NULL;

-- Update type column default to 'extra' for existing records
UPDATE extra_hours
SET type = 'extra'
WHERE type IS NULL;

-- Create index for better query performance on type
CREATE INDEX IF NOT EXISTS idx_extra_hours_type ON extra_hours(type);

-- Verify columns were added
SELECT column_name, data_type, is_nullable, column_default
FROM information_schema.columns
WHERE table_name = 'extra_hours'
AND column_name IN ('diurnal', 'nocturnal', 'diurnal_holiday', 'nocturnal_holiday', 'type', 'total_hours')
ORDER BY column_name;

