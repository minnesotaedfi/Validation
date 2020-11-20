/*

Warn on:

When count of SEOA.StudentIndicator records is null for district 

*/

DECLARE @RuleId VARCHAR(32) = '60.01.0008';
DECLARE @Message NVARCHAR(MAX) = 'When count of SEOA.StudentIndicator records is null for district';
DECLARE @IsError BIT = 0;

WITH 
failed_rows AS (

SELECT
  DISTINCT School.SchoolId
FROM 
  edfi.StudentEducationOrganizationAssociation
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
    ) 
  AND School.LocalEducationAgencyId = @DistrictId

)
INSERT INTO 
	rules.RuleValidationDetail (RuleValidationId, Id, RuleId, IsError, [Message])
SELECT
	@RuleValidationId, SchoolId, @RuleId, @IsError, @Message
FROM failed_rows;
