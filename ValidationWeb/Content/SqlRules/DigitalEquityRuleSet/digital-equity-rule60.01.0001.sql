/*
Warning: Your district has not collected any Digital Equity records for students enrolled in the district.

Error on:

When SEOA.StudentIndicator.IndicatorName exists it must match one of following string values: 

InternetAccessInResidence
InternetAccessTypeInResidence
InternetPerformance
DigitalDevice
PrimaryLearningDeviceProvider
PrimaryLearningDeviceAccess

*/

DECLARE @RuleId VARCHAR(32) = '60.01.0001';
DECLARE @Message NVARCHAR(MAX) = 'Warning: You have submitted an invalid Digital Equity Indicator.
When SEOA.StudentIndicator.IndicatorName exists it must match one of following string values: 
InternetAccessInResidence
InternetAccessTypeInResidence
InternetPerformance
DigitalDevice
PrimaryLearningDeviceProvider
PrimaryLearningDeviceAccess';
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
  AND StudentEducationOrganizationAssociationStudentIndicator.IndicatorName NOT IN (
  'InternetAccessInResidence',
  'InternetAccessTypeInResidence',
  'InternetPerformance',
  'DigitalDevice',
  'PrimaryLearningDeviceProvider',
  'PrimaryLearningDeviceAccess'
  )
  AND School.LocalEducationAgencyId = @DistrictId

)
INSERT INTO 
	rules.RuleValidationDetail (RuleValidationId, Id, RuleId, IsError, [Message])
SELECT
	@RuleValidationId, StudentUniqueId, @RuleId, @IsError, @Message
FROM failed_rows;
