/*

Error on:

IF SEOA.StudentIndicator.IndicatorName = DigitalDevice and SEOA.StudentIndicator.Indicator <> None, 
error when no record for same student where SEOA.StudentIndicator.IndicatorName = PrimaryLearningDeviceAccess

*/

DECLARE @RuleId VARCHAR(32) = '60.01.0017';
DECLARE @Message NVARCHAR(MAX) = 'When Digital Device is not reported as None, no PrimaryLearningDeviceAccess should not be reported for this student.

If SEOA.StudentIndicator.IndicatorName = DigitalDevice and SEOA.StudentIndicator.Indicator <> None, 
error on no additional record for same student where SEOA.StudentIndicator.IndicatorName = PrimaryLearningDeviceAccess';
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
  AND StudentEducationOrganizationAssociationStudentIndicator.Indicator <> 'None'
  AND NOT EXISTS (
    SELECT 1
    FROM
      edfi.StudentEducationOrganizationAssociationStudentIndicator InternetPerformance
    WHERE
      InternetPerformance.IndicatorGroup = 'DigitalEquity'
      AND InternetPerformance.IndicatorName = 'PrimaryLearningDeviceAccess'
      AND InternetPerformance.EducationOrganizationId = StudentEducationOrganizationAssociationStudentIndicator.EducationOrganizationId
      AND InternetPerformance.StudentUSI = StudentEducationOrganizationAssociationStudentIndicator.StudentUSI
    )
  AND School.LocalEducationAgencyId = @DistrictId

)
INSERT INTO 
	rules.RuleValidationDetail (RuleValidationId, Id, RuleId, IsError, [Message])
SELECT
	@RuleValidationId, StudentUniqueId, @RuleId, @IsError, @Message
FROM failed_rows;
