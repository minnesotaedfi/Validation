/*

Warn on:

when count of SEOA.StudentIndicator records is null for student within district 

*/

DECLARE @RuleId VARCHAR(32) = '60.01.0009';
DECLARE @Message NVARCHAR(MAX) = 'When count of SEOA.StudentIndicator records is null for student within district';
DECLARE @IsError BIT = 0;

WITH 
failed_rows AS (

SELECT
  DISTINCT Student.StudentUniqueId
FROM 
  edfi.StudentEducationOrganizationAssociation
  JOIN
  edfi.Student
    ON Student.StudentUSI = StudentEducationOrganizationAssociation.StudentUSI
  JOIN
  edfi.School
    ON School.SchoolId = StudentEducationOrganizationAssociation.EducationOrganizationId
WHERE
  NOT EXISTS (
    SELECT 1
    FROM
      edfi.StudentEducationOrganizationAssociationStudentIndicator
    WHERE
      StudentEducationOrganizationAssociationStudentIndicator.IndicatorGroup = 'DigitalEquity'
      AND StudentEducationOrganizationAssociationStudentIndicator.EducationOrganizationId = StudentEducationOrganizationAssociation.EducationOrganizationId
      AND StudentEducationOrganizationAssociationStudentIndicator.StudentUSI = StudentEducationOrganizationAssociation.StudentUSI
    ) 
  AND School.LocalEducationAgencyId = @DistrictId

)
INSERT INTO 
	rules.RuleValidationDetail (RuleValidationId, Id, RuleId, IsError, [Message])
SELECT
	@RuleValidationId, StudentUniqueId, @RuleId, @IsError, @Message
FROM failed_rows;
