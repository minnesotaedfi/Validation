IF OBJECT_ID('MultipleEnrollment','V') IS NOT NULL
	DROP VIEW MultipleEnrollment

GO

CREATE VIEW MultipleEnrollment 
AS

SELECT S.StudentUniqueId AS [MARSSNumber]
	 , SSA1.EnrollmentSequence AS [EnrollmentSequenceLeft]
	 , SSA1.SchoolId AS [SchoolIdLeft]
	 , DSC1.CodeValue AS [SchoolClassificationLeft]
	 , SSAE1.ResidentLocalEducationAgencyId AS [ResidentDistrictLeft]
	 , SSA1.EntryDate AS [StatusBeginDateLeft]
	 , SSA1.ExitWithdrawDate AS [StatusEndDateLeft]
	 , DET1.CodeValue AS [LastLocationOfAttendanceLeft]
	 , DGL1.CodeValue AS [StudentGradeLevelLeft]
	 , DSEES1.CodeValue AS [SpecialEducationEvaluationStatusLeft]
	 , SSA2.EnrollmentSequence AS [EnrollmentSequenceRight]
	 , SSA2.SchoolId AS [SchoolIdRight]
	 , DSC2.CodeValue AS [SchoolClassificationRight]
	 , SSAE2.ResidentLocalEducationAgencyId AS [ResidentDistrictRight]
	 , SSA2.EntryDate AS [StatusBeginDateRight]
	 , SSA2.ExitWithdrawDate AS [StatusEndDateRight]
	 , DET2.CodeValue AS [LastLocationOfAttendanceRight]
	 , DGL2.CodeValue AS [StudentGradeLevelRight]
	 , DSEES2.CodeValue AS [SpecialEducationEvaluationStatusRight]
	 , CASE 
		WHEN SSA2.EnrollmentOrder - SSA1.EnrollmentOrder = 1 THEN 1
		ELSE 0
	   END AS [IsNextEnrollment]
	 , CASE
		WHEN SSA1.ExitWithdrawDate IS NULL OR SSA2.EntryDate <= SSA1.ExitWithdrawDate THEN 1
		ELSE 0
	   END AS [EnrollmentOverlap]
	 , CASE
		WHEN (DSC1.CodeValue IN ('41','42','45') OR DSC2.CodeValue IN ('41','42','45'))
			AND (SSA1.ExitWithdrawDate IS NULL OR SSA2.EntryDate <= SSA1.ExitWithdrawDate)
		THEN 1
		ELSE 0
	   END AS [DualEnrolledIndicator]
	 , CASE
		WHEN DGL1.CodeValue = 'PS' 
			AND (CHARINDEX('K',DGL2.CodeValue)>0 
				OR DGL2.CodeValue = 'EC' 
				OR LEFT(DGL2.CodeValue,1) = 'R' 
				OR DGL2.CodeValue IN ('PA','PB','PC','PD','PE','PF','PG','PH','PI','PJ'))
			THEN 1
		WHEN DGL2.CodeValue = 'PS' 
			AND (CHARINDEX('K',DGL1.CodeValue)>0 
				OR DGL1.CodeValue = 'EC' 
				OR LEFT(DGL1.CodeValue,1) = 'R' 
				OR DGL1.CodeValue IN ('PA','PB','PC','PD','PE','PF','PG','PH','PI','PJ'))
			THEN 1
		WHEN DGL1.CodeValue IN ('PA','PB','PC','PD','PE','PF','PG','PH','PI','PJ')
			AND DGL2.CodeValue = 'EC'
			AND DSEES2.CodeValue = '2'
			THEN 1
		WHEN DGL2.CodeValue IN ('PA','PB','PC','PD','PE','PF','PG','PH','PI','PJ')
			AND DGL1.CodeValue = 'EC'
			AND DSEES1.CodeValue = '2'
			THEN 1
		WHEN LEFT(DGL1.CodeValue,1) = 'R'
			AND DGL2.CodeValue = 'EC'
			AND DSEES2.CodeValue = '2'
			THEN 1
		WHEN LEFT(DGL2.CodeValue,1) = 'R'
			AND DGL1.CodeValue = 'EC'
			AND DSEES1.CodeValue = '2'
			THEN 1
		ELSE 0
	   END AS [ValidSameSchoolOverlap]
FROM edfi.Student S
JOIN (
	SELECT StudentUSI
		 , SchoolId
		 , SchoolYear
		 , EntryDate
		 , EntryGradeLevelDescriptorId
		 , EntryTypeDescriptorId
		 , ExitWithdrawDate
		 , ExitWithdrawTypeDescriptorId
		 , ROW_NUMBER()
			OVER(PARTITION BY StudentUSI ORDER BY EntryDate,SchoolId) AS [EnrollmentSequence]
		 , DENSE_RANK()
			OVER(PARTITION BY StudentUSI ORDER BY EntryDate) AS [EnrollmentOrder]
	FROM edfi.StudentSchoolAssociation SSA 
	) SSA1 ON SSA1.StudentUSI = S.StudentUSI
JOIN edfi.School Sc1 ON Sc1.SchoolId = SSA1.SchoolId
LEFT JOIN edfi.SchoolClassificationDescriptor SCD1 ON SCD1.SchoolClassificationDescriptorId = Sc1.SchoolClassificationDescriptorId
LEFT JOIN edfi.Descriptor DSC1 ON DSC1.DescriptorId = SCD1.SchoolClassificationDescriptorId
	AND DSC1.Namespace LIKE 'http://education.mn.gov%'
LEFT JOIN edfi.EntryTypeDescriptor ETD1 ON ETD1.EntryTypeDescriptorId = SSA1.EntryTypeDescriptorId
LEFT JOIN edfi.Descriptor DET1 ON DET1.DescriptorId = ETD1.EntryTypeDescriptorId
	AND DET1.Namespace LIKE 'http://education.mn.gov%'
JOIN extension.StudentSchoolAssociationExtension SSAE1 ON SSAE1.StudentUSI = SSA1.StudentUSI
	AND SSAE1.SchoolId = SSA1.SchoolId
	AND SSAE1.EntryDate = SSA1.EntryDate
LEFT JOIN extension.SpecialEducationEvaluationStatusDescriptor SEESD1 
	ON SEESD1.SpecialEducationEvaluationStatusDescriptorId = SSAE1.SpecialEducationEvaluationStatusDescriptorId
LEFT JOIN edfi.Descriptor DSEES1 ON DSEES1.DescriptorId = SEESD1.SpecialEducationEvaluationStatusDescriptorId
	AND DSEES1.Namespace LIKE 'http://education.mn.gov%'
JOIN edfi.GradeLevelDescriptor GLD1 ON GLD1.GradeLevelDescriptorId = SSA1.EntryGradeLevelDescriptorId
JOIN edfi.Descriptor DGL1 ON DGL1.DescriptorId = GLD1.GradeLevelDescriptorId
	AND DGL1.Namespace LIKE 'http://education.mn.gov%'
JOIN (
	SELECT StudentUSI
		 , SchoolId
		 , SchoolYear
		 , EntryDate
		 , EntryGradeLevelDescriptorId
		 , EntryTypeDescriptorId
		 , ExitWithdrawDate
		 , ExitWithdrawTypeDescriptorId
		 , ROW_NUMBER()
			OVER(PARTITION BY StudentUSI ORDER BY EntryDate,SchoolId) AS [EnrollmentSequence]
		 , DENSE_RANK()
			OVER(PARTITION BY StudentUSI ORDER BY EntryDate) AS [EnrollmentOrder]
	FROM edfi.StudentSchoolAssociation SSA 
	) SSA2 ON SSA2.StudentUSI = S.StudentUSI
	AND SSA2.EnrollmentSequence > SSA1.EnrollmentSequence
JOIN edfi.School S2 ON S2.SchoolId = SSA1.SchoolId
LEFT JOIN edfi.SchoolClassificationDescriptor SCD2 ON SCD2.SchoolClassificationDescriptorId = S2.SchoolClassificationDescriptorId
LEFT JOIN edfi.Descriptor DSC2 ON DSC2.DescriptorId = SCD2.SchoolClassificationDescriptorId
	AND DSC2.Namespace LIKE 'http://education.mn.gov%'
LEFT JOIN edfi.EntryTypeDescriptor ETD2 ON ETD2.EntryTypeDescriptorId = SSA2.EntryTypeDescriptorId
LEFT JOIN edfi.Descriptor DET2 ON DET2.DescriptorId = ETD2.EntryTypeDescriptorId
	AND DET2.Namespace LIKE 'http://education.mn.gov%'
JOIN extension.StudentSchoolAssociationExtension SSAE2 ON SSAE2.StudentUSI = SSA2.StudentUSI
	AND SSAE2.SchoolId = SSA2.SchoolId
	AND SSAE2.EntryDate = SSA2.EntryDate
LEFT JOIN extension.SpecialEducationEvaluationStatusDescriptor SEESD2 
	ON SEESD2.SpecialEducationEvaluationStatusDescriptorId = SSAE2.SpecialEducationEvaluationStatusDescriptorId
LEFT JOIN edfi.Descriptor DSEES2 ON DSEES2.DescriptorId = SEESD2.SpecialEducationEvaluationStatusDescriptorId
	AND DSEES2.Namespace LIKE 'http://education.mn.gov%'
JOIN edfi.GradeLevelDescriptor GLD2 ON GLD2.GradeLevelDescriptorId = SSA2.EntryGradeLevelDescriptorId
JOIN edfi.Descriptor DGL2 ON DGL2.DescriptorId = GLD2.GradeLevelDescriptorId
	AND DGL2.Namespace LIKE 'http://education.mn.gov%';