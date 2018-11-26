IF EXISTS (SELECT * 
		FROM INFORMATION_SCHEMA.ROUTINES
		WHERE ROUTINE_NAME = 'ContainsInvalidChar'
			AND ROUTINE_SCHEMA = 'dbo')
	DROP FUNCTION [dbo].ContainsInvalidChar
GO

CREATE FUNCTION ContainsInvalidChar
(
	@String AS VARCHAR(200)
)
RETURNS VARCHAR(200)
AS  
BEGIN

   IF LEN(LTRIM(RTRIM(@String)))>0
   BEGIN
	DECLARE @InvalidChar VARCHAR(200)	
	DECLARE @Index INT
	DECLARE @ASCIIChar INT

	SET @Index = 1
	SET @InvalidChar=0
	BEGIN     	 
      		WHILE @Index <= LEN(@String)
		BEGIN
    		SET @ASCIIChar=ASCII(SUBSTRING(@String, @Index, 1))
    		IF NOT @ASCIIChar BETWEEN 65 AND 90
		AND NOT @ASCIIChar BETWEEN 97 AND 122 
		AND NOT @ASCIIChar IN (32, 39, 45)
		SET @InvalidChar = 1
	        SET @Index = @Index + 1
    		END
	END    
   END
   ELSE
   BEGIN
	SET @InvalidChar = 0
   END

   RETURN (@InvalidChar)
END