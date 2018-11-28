IF EXISTS (SELECT * 
		FROM INFORMATION_SCHEMA.ROUTINES
		WHERE ROUTINE_NAME = 'ShortString'
			AND ROUTINE_SCHEMA = 'dbo')
	DROP FUNCTION [dbo].ShortString
GO

CREATE FUNCTION dbo.ShortString (@String VARCHAR(200))

RETURNS BIT 

BEGIN

DECLARE @ShortString AS BIT

SELECT @ShortString = 
	   CASE
		WHEN LEN(@String) < 2 THEN 1
		WHEN LEN(@String) = 2 AND (
			ASCII(SUBSTRING(@String,2,1)) NOT BETWEEN 65 AND 90 
		 OR ASCII(SUBSTRING(@String,2,1)) <> 39 
		 OR ASCII(SUBSTRING(@String,2,1)) NOT BETWEEN 97 AND 122
			) THEN 1
		ELSE 0 
	   END
RETURN @ShortString;

END;