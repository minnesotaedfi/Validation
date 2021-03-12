/*

Error on: 
	Course with Level Types of both G and E

*/

DECLARE @RuleId VARCHAR(32) = '50.01.0014';
DECLARE @Message NVARCHAR(MAX) = 'Error on: Course with Level Types of both G and E. The entity ID returned is a district ed-org.';
DECLARE @IsError BIT = 1;

WITH 
course_level_types AS (
SELECT
	EducationOrganizationId,
	CourseCode,
	CodeValue AS CourseLevelType
FROM 
	mn.CourseLevelType
	INNER JOIN 
	edfi.Descriptor 
		ON CourseLevelTypeDescriptorId = DescriptorId
WHERE
	EducationOrganizationId = @DistrictId
GROUP BY
	EducationOrganizationId,
	CourseCode,
	CodeValue
	),
failed_rows AS (
SELECT
	EducationOrganizationId,
	CourseCode
FROM
	course_level_types
WHERE
	CourseLevelType = 'G'
	AND EXISTS (
		SELECT 1
		FROM
			course_level_types inside
		WHERE
			inside.EducationOrganizationId = course_level_types.EducationOrganizationId
			AND inside.CourseCode = course_level_types.CourseCode
			AND inside.CourseLevelType = 'E'
		)
)
INSERT INTO 
	rules.RuleValidationDetail (RuleValidationId, Id, RuleId, IsError, [Message])
SELECT TOP 1
	@RuleValidationId, @DistrictId, @RuleId RuleId, @IsError IsError, 
	@Message + ' Course code: ' + (SELECT TOP 1 CourseCode FROM failed_rows) [Message]
FROM failed_rows;
