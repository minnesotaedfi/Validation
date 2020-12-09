/*

Warning: Invalid DigitalDevice response for this student.

Error on:

When SEOA.StudentIndicator.IndicatorName = DigitalDevice
require SEOA.StudentIndicator.Indicator in 
(Desktop/Laptop, Tablet, Chromebook, SmartPhone, None, Other)

*/

DECLARE @RuleId VARCHAR(32) = '60.01.0005';
DECLARE @Message NVARCHAR(MAX) = 'Warning: Invalid DigitalDevice response for this student.
When SEOA.StudentIndicator.IndicatorName = DigitalDevice
require SEOA.StudentIndicator.Indicator in (Desktop/Laptop, Tablet, Chromebook, SmartPhone, None, Other)';
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
  AND StudentEducationOrganizationAssociationStudentIndicator.IndicatorName = 'DigitalDevice'
  AND StudentEducationOrganizationAssociationStudentIndicator.Indicator NOT IN (
  'Desktop/Laptop', 'Tablet', 'Chromebook', 'SmartPhone', 'None', 'Other'
  )
  AND School.LocalEducationAgencyId = @DistrictId

)
INSERT INTO 
	rules.RuleValidationDetail (RuleValidationId, Id, RuleId, IsError, [Message])
SELECT
	@RuleValidationId, StudentUniqueId, @RuleId, @IsError, @Message
FROM failed_rows;
