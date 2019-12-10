/*
MARSSWES validation logic:
Student WHERE specialeducationstatus ='7' AND studentRecordSequenceByStart=1
*/

DECLARE @RuleId NVARCHAR(50) = '10.10.6226';
DECLARE @RuleDescription NVARCHAR(MAX) = '129. Students whose IEP is terminated mid-year '+
	'must have a prior enrollment record showing the IEP, i.e., the earlier enrollment record '+
	'must have a Special Enudcation Status of 4 or 6. There is no meed to report a second '+
	'enrollment record for any early childhood student whose IEP/IFSP is terminated.';
DECLARE @IsError BIT = 1;

WITH Student (
	StudentUniqueId, LocalEducationAgencyId, EnrollmentSequence, GradeLevel, 
	SpecialEducationEvaluationStatus
	) AS (
SELECT 
	S.StudentUniqueId,
	Sc.LocalEducationAgencyId,
	ROW_NUMBER () OVER (
		PARTITION BY S.StudentUniqueId 
		ORDER BY SSA.EntryDate, SSA.SchoolId
		) AS EnrollmentSequence,
	DGL.CodeValue AS GradeLevel,
	DSEES.CodeValue AS SpecialEducationEvaluationStatus
FROM 
	edfi.Student S
	INNER JOIN 
	edfi.StudentSchoolAssociation SSA 
		ON SSA.StudentUSI = S.StudentUSI
	INNER JOIN 
	mn.StudentSchoolAssociationExtension SSAE 
		ON SSAE.StudentUSI = SSA.StudentUSI
		AND SSAE.SchoolId = SSA.SchoolId
		AND SSAE.EntryDate = SSA.EntryDate
	INNER JOIN 
	edfi.School Sc
		ON SSA.SchoolId = Sc.SchoolId
	INNER JOIN 
	edfi.GradeLevelDescriptor GLD 
		ON GLD.GradeLevelDescriptorId = SSA.EntryGradeLevelDescriptorId
	INNER JOIN 
	edfi.Descriptor DGL ON DGL.DescriptorId = GLD.GradeLevelDescriptorId
		AND DGL.Namespace LIKE 'uri://education.mn.gov%'
	LEFT JOIN 
	mn.SpecialEducationEvaluationStatusDescriptor SEESD
		ON SEESD.SpecialEducationEvaluationStatusDescriptorId = SSAE.SpecialEducationEvaluationStatusDescriptorId
	LEFT JOIN 
	edfi.Descriptor DSEES 
		ON DSEES.DescriptorId = SEESD.SpecialEducationEvaluationStatusDescriptorId
		AND DSEES.Namespace LIKE 'uri://education.mn.gov%'
WHERE
	Sc.LocalEducationAgencyId = @DistrictId
	) 
INSERT INTO
	rules.RuleValidationDetail (
		RuleValidationId,
		Id,
		RuleId,
		IsError,
		[Message]
		) 
SELECT
	@RuleValidationId,
	Student.StudentUniqueId,
	@RuleId,
	@IsError,
	@RuleDescription
FROM 
	Student
WHERE
	Student.GradeLevel <> 'PS'
	AND Student.SpecialEducationEvaluationStatus = '7'
	AND Student.EnrollmentSequence = 1
GROUP BY
	Student.StudentUniqueId;
