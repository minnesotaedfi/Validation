/*

Error on:

When SEOA.StudentIndicator.IndicatorName = PrimaryLearningDeviceProvider
require SEOA.StudentIndicator.Indicator in (Personal, School, Other)

*/

DECLARE @RuleId VARCHAR(32) = '60.01.0006';
DECLARE @Message NVARCHAR(MAX) = 'Invalid PrimaryLearningDeviceProvider response for this student.
Error on:
When SEOA.StudentIndicator.IndicatorName = PrimaryLearningDeviceProvider
require SEOA.StudentIndicator.Indicator in (Personal, School, Other)';
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
  AND StudentEducationOrganizationAssociationStudentIndicator.IndicatorName = 'PrimaryLearningDeviceProvider'
  AND StudentEducationOrganizationAssociationStudentIndicator.Indicator NOT IN (
  'Personal', 'School', 'Other'
  )
  AND School.LocalEducationAgencyId = @DistrictId

)
INSERT INTO 
	rules.RuleValidationDetail (RuleValidationId, Id, RuleId, IsError, [Message])
SELECT
	@RuleValidationId, StudentUniqueId, @RuleId, @IsError, @Message
FROM failed_rows;
