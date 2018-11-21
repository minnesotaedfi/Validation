IF EXISTS(SELECT 1 FROM sys.types WHERE name = 'IdIntTable' AND is_table_type = 1 AND SCHEMA_ID('rules') = schema_id)
    DROP TYPE rules.IdIntTable;  
