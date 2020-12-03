/*

Error on:

When SEOA.StudentIndicator.IndicatorName = PrimaryLearningDeviceAccess
require SEOA.StudentIndicator in (Shared, Not Shared, Unknown)

*/

DECLARE @RuleId VARCHAR(32) = '60.01.0007';
DECLARE @Message NVARCHAR(MAX) = 'When SEOA.StudentIndicator.IndicatorName = PrimaryLearningDeviceAccess
require SEOA.StudentIndicator in (Shared, Not Shared, Unknown)';
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
  AND StudentEducationOrganizationAssociationStudentIndicator.IndicatorName = 'PrimaryLearningDeviceAccess'
  AND StudentEducationOrganizationAssociationStudentIndicator.Indicator NOT IN (
  'Shared', 'Not Shared', 'Unknown'
  )
  AND School.LocalEducationAgencyId = @DistrictId

)
INSERT INTO 
	rules.RuleValidationDetail (RuleValidationId, Id, RuleId, IsError, [Message])
SELECT
	@RuleValidationId, StudentUniqueId, @RuleId, @IsError, @Message
FROM failed_rows;
