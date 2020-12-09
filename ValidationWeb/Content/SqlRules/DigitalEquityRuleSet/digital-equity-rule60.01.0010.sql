/*

Error on:

If SEOA.StudentIndicator.IndicatorName = InternetAccessInResidence and SEOA.StudentIndicator.Indicator <> Yes, 
error on additional record for same student where SEOA.StudentIndicator.IndicatorName = InternetAccessTypeInResidence

*/

DECLARE @RuleId VARCHAR(32) = '60.01.0010';
DECLARE @Message NVARCHAR(MAX) = 'Invalid InternetAccessTypeInResidence for this student.

Error on: 

If SEOA.StudentIndicator.IndicatorName = InternetAccessInResidence and SEOA.StudentIndicator.Indicator <> Yes, 
error on additional record for same student where SEOA.StudentIndicator.IndicatorName = InternetAccessTypeInResidence';
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
      edfi.StudentEducationOrganizationAssociationStudentIndicator InternetAccessTypeInResidence
    WHERE
      InternetAccessTypeInResidence.IndicatorGroup = 'DigitalEquity'
      AND InternetAccessTypeInResidence.IndicatorName = 'InternetAccessTypeInResidence'
      AND InternetAccessTypeInResidence.EducationOrganizationId = StudentEducationOrganizationAssociationStudentIndicator.EducationOrganizationId
      AND InternetAccessTypeInResidence.StudentUSI = StudentEducationOrganizationAssociationStudentIndicator.StudentUSI
    )
  AND School.LocalEducationAgencyId = @DistrictId

)
INSERT INTO 
	rules.RuleValidationDetail (RuleValidationId, Id, RuleId, IsError, [Message])
SELECT
	@RuleValidationId, StudentUniqueId, @RuleId, @IsError, @Message
FROM failed_rows;
