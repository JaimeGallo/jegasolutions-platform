-- Migration to add approved_by_manager_id column to extra_hours table
-- This column stores the manager ID who approved the extra hours

-- Add approved_by_manager_id column if it doesn't exist
ALTER TABLE extra_hours 
ADD COLUMN IF NOT EXISTS approved_by_manager_id BIGINT NULL;

-- Create index for better query performance
CREATE INDEX IF NOT EXISTS "IX_extra_hours_approved_by_manager" ON extra_hours (approved_by_manager_id);

-- Add foreign key constraint to managers table (optional, depending on your needs)
-- Note: Uncomment if you want referential integrity
-- ALTER TABLE extra_hours 
-- ADD CONSTRAINT "FK_extra_hours_managers" 
-- FOREIGN KEY (approved_by_manager_id) 
-- REFERENCES managers(manager_id) 
-- ON DELETE SET NULL;

-- Verify the column was added
SELECT column_name, data_type, is_nullable 
FROM information_schema.columns 
WHERE table_name = 'extra_hours' 
AND column_name = 'approved_by_manager_id';

