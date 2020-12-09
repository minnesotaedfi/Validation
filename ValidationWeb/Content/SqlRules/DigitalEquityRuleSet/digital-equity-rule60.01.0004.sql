/*

Warning: Invalid response for Internet Performance for this student.

Error on:

When SEOA.StudentIndicator.IndicatorName = InternetPerformance 
require SEOA.StudentIndicator.Indicator in (Yes - No issues, Yes - But not consistent, No)

*/

DECLARE @RuleId VARCHAR(32) = '60.01.0004';
DECLARE @Message NVARCHAR(MAX) = 'Warning: Invalid response for Internet Performance for this student.
When SEOA.StudentIndicator.IndicatorName = InternetPerformance 
require SEOA.StudentIndicator.Indicator in (Yes - No issues, Yes - But not consistent, No)';
DECLARE @IsError BIT = 1;

WITH 
failed_rows AS (

SELECT
  DISTINCT Student.StudentUniqueId
FROM 
  edfi.StudentEducationOrganizationAssociationStudentIndicator
  JOIN
  edfi.Student
    ON Student.StudentUSI = StudentEducationOrganizationAssociationStudentIndicator.StudentUSI
  JOIN
  edfi.School
    ON School.SchoolId = StudentEducationOrganizationAssociationStudentIndicator.EducationOrganizationId
WHERE
  StudentEducationOrganizationAssociationStudentIndicator.IndicatorGroup = 'DigitalEquity'
  AND StudentEducationOrganizationAssociationStudentIndicator.IndicatorName = 'InternetPerformance'
  AND StudentEducationOrganizationAssociationStudentIndicator.Indicator NOT IN (
  'Yes - No issues', 'Yes - But not consistent', 'No'
  )
  AND School.LocalEducationAgencyId = @DistrictId

)
INSERT INTO 
	rules.RuleValidationDetail (RuleValidationId, Id, RuleId, IsError, [Message])
SELECT
	@RuleValidationId, StudentUniqueId, @RuleId, @IsError, @Message
FROM failed_rows;
