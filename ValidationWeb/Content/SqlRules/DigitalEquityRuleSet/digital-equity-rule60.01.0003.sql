/*

Error on: You have submitted an invalid response for Internet Access Type In Residence for this student

When SEOA.StudentIndicator.IndicatorName = InternetAccessTypeInResidence
require SEOA.StudentIndicator.Indicator in (ResidentialBroadband, CellularNetwork, SchoolProvidedHotSpot
, Satellite, Dial-up, Other, Unknown)

*/

DECLARE @RuleId VARCHAR(32) = '60.01.0003';
DECLARE @Message NVARCHAR(MAX) = 'You have submitted an invalid response for Internet Access Type In Residence for this student

Error on: When SEOA.StudentIndicator.IndicatorName = InternetAccessTypeInResidence
require SEOA.StudentIndicator.Indicator in (ResidentialBroadband, CellularNetwork, SchoolProvidedHotSpot
, Satellite, Dial-up, Other, Unknown)';
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
  AND StudentEducationOrganizationAssociationStudentIndicator.IndicatorName = 'InternetAccessTypeInResidence'
  AND StudentEducationOrganizationAssociationStudentIndicator.Indicator NOT IN (
  'ResidentialBroadband', 'CellularNetwork', 'SchoolProvidedHotSpot', 'Satellite', 'Dial-up', 'Other', 'Unknown'
  )
  AND School.LocalEducationAgencyId = @DistrictId

)
INSERT INTO 
	rules.RuleValidationDetail (RuleValidationId, Id, RuleId, IsError, [Message])
SELECT
	@RuleValidationId, StudentUniqueId, @RuleId, @IsError, @Message
FROM failed_rows;
