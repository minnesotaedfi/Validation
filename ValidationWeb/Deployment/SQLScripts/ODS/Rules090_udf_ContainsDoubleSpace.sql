IF EXISTS (SELECT * 
		FROM INFORMATION_SCHEMA.ROUTINES
		WHERE ROUTINE_NAME = 'ContainsDoubleSpace'
			AND ROUTINE_SCHEMA = 'dbo')
	DROP FUNCTION [dbo].[ContainsDoubleSpace]
GO

CREATE FUNCTION dbo.ContainsDoubleSpace (@String VARCHAR(200))

RETURNS BIT 

BEGIN

DECLARE @DoubleSpace AS BIT

SELECT @DoubleSpace = 
	   CASE
		WHEN CHARINDEX('  ',@String) > 0 THEN 1
		ELSE 0
	   END
RETURN @DoubleSpace;

END;