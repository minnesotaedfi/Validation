/*

Error on: 
	Course with Level Type of B with Level Type of G or E

*/

DECLARE @RuleId VARCHAR(32) = '50.01.0013';
DECLARE @Message NVARCHAR(MAX) = 'Error on: Course with Level Type of B with Level Type of G or E.';
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
	CourseLevelType = 'B'
	AND EXISTS (
		SELECT 1
		FROM
			course_level_types inside
		WHERE
			inside.EducationOrganizationId = course_level_types.EducationOrganizationId
			AND inside.CourseCode = course_level_types.CourseCode
			AND inside.CourseLevelType IN ('E','G')
		)
)
INSERT INTO 
	rules.RuleValidationDetail (RuleValidationId, Id, RuleId, IsError, [Message])
SELECT TOP 1
	@RuleValidationId, @DistrictId, @RuleId RuleId, @IsError IsError, 
	@Message + ' Course code: ' + (SELECT TOP 1 CourseCode FROM failed_rows) [Message]
FROM failed_rows;
