/*

Error on:
	Courses with associations are not Defined By the same type of Education Organization

*/

DECLARE @RuleId VARCHAR(32) = '50.00.0002';
DECLARE @Message NVARCHAR(MAX) = 'Courses with associations are not Defined By the same type of Education Organization.';
DECLARE @IsError BIT = 1;

WITH 
failed_rows AS (

SELECT 
	Course.CourseCode,
	Course.EducationOrganizationId,
	CourseDefinedByDescriptor.CodeValue CourseDefinedBy
	
FROM (
	SELECT
		CourseCode,
		EducationOrganizationId
	FROM
		mn.CourseCourseAssociation
	WHERE
		EducationOrganizationId = @DistrictId
	UNION
	SELECT
		ToCourseCode,
		ToCourseEducationOrganizationId
	FROM
		mn.CourseCourseAssociation
	WHERE
		EducationOrganizationId = @DistrictId
	) CourseList
	INNER JOIN
	edfi.Course
		ON CourseList.CourseCode = Course.CourseCode
		AND CourseList.EducationOrganizationId = Course.EducationOrganizationId
	LEFT JOIN (
		SELECT
			Descriptor.DescriptorId,
			Descriptor.CodeValue
		FROM
			edfi.CourseDefinedByDescriptor
			INNER JOIN
			edfi.Descriptor 
				ON CourseDefinedByDescriptor.CourseDefinedByDescriptorId = Descriptor.DescriptorId
		) CourseDefinedByDescriptor
		ON Course.CourseDefinedByDescriptorId = CourseDefinedByDescriptor.DescriptorId

	
WHERE
	CourseDefinedByDescriptor.CodeValue = 'SEA' AND NOT EXISTS (SELECT 1 FROM edfi.StateEducationAgency WHERE StateEducationAgencyId = Course.EducationOrganizationId)
	OR
	CourseDefinedByDescriptor.CodeValue = 'LEA' AND NOT EXISTS (SELECT 1 FROM edfi.LocalEducationAgency WHERE LocalEducationAgencyId = Course.EducationOrganizationId)
	OR
	CourseDefinedByDescriptor.CodeValue = 'College' AND NOT EXISTS (SELECT 1 FROM edfi.PostSecondaryInstitution WHERE PostSecondaryInstitutionId = Course.EducationOrganizationId)
	OR
	CourseDefinedByDescriptor.CodeValue NOT IN ('SEA','LEA','College')
	
)
INSERT INTO 
	rules.RuleValidationDetail (RuleValidationId, Id, RuleId, IsError, [Message])
SELECT TOP 1
	@RuleValidationId, @DistrictId, @RuleId RuleId, @IsError IsError, 
	@Message + ' Course code: '+ (SELECT TOP 1 CourseCode FROM failed_rows) [Message]
FROM failed_rows;
