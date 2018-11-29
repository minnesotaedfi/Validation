IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'rules')
BEGIN
-- The schema must be run in its own batch!
EXEC( 'CREATE SCHEMA rules' );
END