/*
Warning: Invalid response for Internet Access In Residencee for this student.

Error on:

When SEOA.StudentIndicator.IndicatorName = InternetAccessInResidence
require SEOA.StudentIndicator.Indicator in (Yes, No - Not Available, No - Not Affordable, No - Other)

*/

DECLARE @RuleId VARCHAR(32) = '60.01.0002';
DECLARE @Message NVARCHAR(MAX) = 'Warning: Invalid response for Internet Access In Residence for this student.
When SEOA.StudentIndicator.IndicatorName = InternetAccessInResidence
require SEOA.StudentIndicator.Indicator in (Yes, No - Not Available, No - Not Affordable, No - Other)';
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
  AND StudentEducationOrganizationAssociationStudentIndicator.IndicatorName = 'InternetAccessInResidence'
  AND StudentEducationOrganizationAssociationStudentIndicator.Indicator NOT IN (
  'Yes', 'No - Not Available', 'No - Not Affordable', 'No - Other'
  )
  AND School.LocalEducationAgencyId = @DistrictId

)
INSERT INTO 
	rules.RuleValidationDetail (RuleValidationId, Id, RuleId, IsError, [Message])
SELECT
	@RuleValidationId, StudentUniqueId, @RuleId, @IsError, @Message
FROM failed_rows;
