/*

Warning: Invalid Digital Equity Indicator Name was submitted for this student

Error on:

When SEOA.StudentIndicator.IndicatorName = DigitalDevice
require SEOA.StudentIndicator.Indicator in 
(Desktop/Laptop, Tablet, Chromebook, SmartPhone, None, Other)

*/

DECLARE @RuleId VARCHAR(32) = '60.01.0005';
DECLARE @Message NVARCHAR(MAX) = '';
DECLARE @IsError BIT = 1;

WITH 
failed_rows AS (

SELECT 
FROM
WHERE
  = @DistrictId
)
INSERT INTO 
	rules.RuleValidationDetail (RuleValidationId, Id, RuleId, IsError, [Message])
SELECT TOP 1
	@RuleValidationId, @DistrictId, @RuleId RuleId, @IsError IsError, 
	@Message + CHAR(13)+CHAR(10)+ (
		SELECT TOP 10 
			CourseCode,
			EducationOrganizationId,
			CourseDefinedBy,
			ToCourseCode,
			ToCourseEducationOrganizationId,
			ToCourseDefinedBy
		FROM failed_rows [xml] 
		FOR XML RAW,
			ROOT ('failedRows')
		) [Message]
FROM failed_rows;
