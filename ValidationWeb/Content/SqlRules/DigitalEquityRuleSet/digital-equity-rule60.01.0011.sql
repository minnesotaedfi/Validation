/*

Error on:

IF SEOA.StudentIndicator.IndicatorName = InternetAccessInResidence and SEOA.StudentIndicator.Indicator <> Yes, 
error on additional record for same student where SEOA.StudentIndicator.IndicatorName = InternetPerformance

*/

DECLARE @RuleId VARCHAR(32) = '60.01.0011';
DECLARE @Message NVARCHAR(MAX) = 'When Internet Access in Residence is not reported as Yes, Internet Performance should not be reported for this student.

If SEOA.StudentIndicator.IndicatorName = InternetAccessInResidence and SEOA.StudentIndicator.Indicator <> Yes, 
error on additional record for same student where SEOA.StudentIndicator.IndicatorName = InternetPerformance';
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
  AND StudentEducationOrganizationAssociationStudentIndicator.Indicator <> 'Yes'
  AND EXISTS (
    SELECT 1
    FROM
      edfi.StudentEducationOrganizationAssociationStudentIndicator InternetPerformance
    WHERE
      InternetPerformance.IndicatorGroup = 'DigitalEquity'
      AND InternetPerformance.IndicatorName = 'InternetPerformance'
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
