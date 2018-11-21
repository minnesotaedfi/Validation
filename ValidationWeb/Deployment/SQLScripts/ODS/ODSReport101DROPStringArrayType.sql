IF EXISTS(SELECT 1 FROM sys.types WHERE name = 'IdStringTable' AND is_table_type = 1 AND SCHEMA_ID('rules') = schema_id)
    DROP TYPE rules.IdStringTable;  
