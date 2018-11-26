IF OBJECT_ID('StudentEnrollment','V') IS NOT NULL
	DROP VIEW StudentEnrollment

GO

CREATE VIEW StudentEnrollment

AS

SELECT S.StudentUniqueId AS [MARSSNumber]
	 , SSA.SchoolId
	 , DT.CodeValue AS [DistrictType]
	 , RIGHT(CAST(LEA.LocalEducationAgencyId AS NVARCHAR(10)),4) AS [DistrictNumber]
	 , RIGHT(CAST(Sc.SchoolId AS NVARCHAR(10)),3) AS [SchoolNumber]
	 , SSA.EntryDate AS [StatusBeginDate]
	 , ROW_NUMBER ()
			OVER(PARTITION BY S.StudentUniqueId ORDER BY SSA.EntryDate, SSA.SchoolId) AS [EnrollmentSequence]
	 , DET.CodeValue AS [LastLocationOfAttendance]
	 , SSA.ExitWithdrawDate AS [StatusEndDate]
	 , DEWT.CodeValue AS [StatusEnd]
	 , DGL.CodeValue AS [StudentGradeLevel]
	 , DSFSE.CodeValue AS [EconomicIndicatorCode]
	 , SSAE.HomeboundServiceIndicator
	 , HSF.HomelessStudentFlag
	 , M.COEMigrantIndicator
	 , ISI.IndependentStudyIndicator
	 , SE.Section504Placement
	 , SE.Section504PlacementBeginDate
	 , SE.Section504PlacementEndDate
	 , TII.Title1Indicator
	 , DSEES.CodeValue AS [SpecialEducationEvaluationStatus]
	 , E.EnglishLearnerStartDate
	 , RIGHT(CAST(RLEA.LocalEducationAgencyId AS NVARCHAR(10)),4) AS [StudentResidentDistrictNumber]
	 , RDT.CodeValue AS [StudentResidentDistrictType]
	 , DS.CodeValue AS [SchoolClassification]
FROM edfi.Student S
JOIN edfi.StudentSchoolAssociation SSA ON SSA.StudentUSI = S.StudentUSI
JOIN extension.StudentSchoolAssociationExtension SSAE ON SSAE.StudentUSI = SSA.StudentUSI
	AND SSAE.SchoolId = SSA.SchoolId
	AND SSAE.EntryDate = SSA.EntryDate
LEFT JOIN edfi.EntryTypeDescriptor ETD ON ETD.EntryTypeDescriptorId = SSA.EntryTypeDescriptorId
LEFT JOIN edfi.Descriptor DET ON DET.DescriptorId = ETD.EntryTypeDescriptorId
	AND DET.Namespace LIKE 'http://education.mn.gov%'
LEFT JOIN edfi.ExitWithdrawTypeDescriptor EWTD ON EWTD.ExitWithdrawTypeDescriptorId = SSA.ExitWithdrawTypeDescriptorId
LEFT JOIN edfi.Descriptor DEWT ON DEWT.DescriptorId = EWTD.ExitWithdrawTypeDescriptorId
	AND DEWT.Namespace LIKE 'http://education.mn.gov%'
JOIN edfi.GradeLevelDescriptor GLD ON GLD.GradeLevelDescriptorId = SSA.EntryGradeLevelDescriptorId
JOIN edfi.Descriptor DGL ON DGL.DescriptorId = GLD.GradeLevelDescriptorId
	AND DGL.Namespace LIKE 'http://education.mn.gov%'
LEFT JOIN edfi.SchoolFoodServicesEligibilityDescriptor SFSED ON SFSED.SchoolFoodServicesEligibilityDescriptorId = SSAE.SchoolFoodServicesEligibilityDescriptorId
LEFT JOIN edfi.Descriptor DSFSE ON DSFSE.DescriptorId = SFSED.SchoolFoodServicesEligibilityDescriptorId
	AND DSFSE.Namespace LIKE 'http://education.mn.gov%'
LEFT JOIN 
	(SELECT SEOA.StudentUSI
		  , SEOA.EducationOrganizationId
		  , 1 AS [HomelessStudentFlag]
	 FROM edfi.StudentEducationOrganizationAssociation SEOA
	 JOIN extension.StudentEducationOrganizationAssociationExtension SEOAE ON SEOAE.StudentUSI = SEOA.StudentUSI
		AND SEOAE.EducationOrganizationId = SEOA.EducationOrganizationId
		AND SEOAE.ResponsibilityDescriptorId = SEOA.ResponsibilityDescriptorId
	 JOIN extension.StudentEducationOrganizationAssociationStudentCharacteristic SEOASC ON SEOASC.StudentUSI = SEOAE.StudentUSI
		AND SEOASC.EducationOrganizationId = SEOAE.EducationOrganizationId
		AND SEOASC.ResponsibilityDescriptorId = SEOAE.ResponsibilityDescriptorId
	 JOIN edfi.StudentCharacteristicDescriptor SCD ON SCD.StudentCharacteristicDescriptorId = SEOASC.StudentCharacteristicDescriptorId
	 JOIN edfi.Descriptor DSC ON DSC.DescriptorId = SCD.StudentCharacteristicDescriptorId
		AND DSC.CodeValue = 'Homeless'
		AND DSC.Namespace LIKE 'http://education.mn.gov%'
	 JOIN edfi.ResponsibilityDescriptor RD ON RD.ResponsibilityDescriptorId = SEOA.ResponsibilityDescriptorId
	 JOIN edfi.Descriptor DR ON DR.DescriptorId = RD.ResponsibilityDescriptorId
		AND DR.CodeValue = 'Demographic'
		AND DR.Namespace LIKE 'http://education.mn.gov%'
	 ) HSF ON HSF.StudentUSI = SSA.StudentUSI
		   AND HSF.EducationOrganizationId = SSA.SchoolId
LEFT JOIN (
	SELECT SEOA.StudentUSI
		 , SEOA.EducationOrganizationId
		 , 1 AS [COEMigrantIndicator]
	FROM edfi.StudentEducationOrganizationAssociation SEOA
	 JOIN extension.StudentEducationOrganizationAssociationExtension SEOAE ON SEOAE.StudentUSI = SEOA.StudentUSI
		AND SEOAE.EducationOrganizationId = SEOA.EducationOrganizationId
		AND SEOAE.ResponsibilityDescriptorId = SEOA.ResponsibilityDescriptorId
	 JOIN extension.StudentEducationOrganizationAssociationStudentCharacteristic SEOASC ON SEOASC.StudentUSI = SEOAE.StudentUSI
		AND SEOASC.EducationOrganizationId = SEOAE.EducationOrganizationId
		AND SEOASC.ResponsibilityDescriptorId = SEOAE.ResponsibilityDescriptorId
	JOIN edfi.StudentCharacteristicDescriptor SCD ON SCD.StudentCharacteristicDescriptorId = SEOASC.StudentCharacteristicDescriptorId
	JOIN edfi.Descriptor DSC ON DSC.DescriptorId = SCD.StudentCharacteristicDescriptorId
		AND DSC.CodeValue = 'Migrant'
		AND DSC.Namespace LIKE 'http://education.mn.gov%'
	JOIN edfi.ResponsibilityDescriptor RD ON RD.ResponsibilityDescriptorId = SEOA.ResponsibilityDescriptorId
	JOIN edfi.Descriptor DR ON DR.DescriptorId = RD.ResponsibilityDescriptorId
		AND DR.CodeValue = 'Demographic'
		AND DR.Namespace LIKE 'http://education.mn.gov%'
	) M ON M.StudentUSI = SSA.StudentUSI
		  AND M.EducationOrganizationId = SSA.SchoolId
LEFT JOIN 
	(SELECT SSASPP.StudentUSI
		  , SSASPP.SchoolId
		  , SSASPP.EntryDate
		  , 1 AS [IndependentStudyIndicator]
	 FROM extension.StudentSchoolAssociationStudentProgramParticipation SSASPP
	 JOIN extension.ProgramCategoryDescriptor PCD ON PCD.ProgramCategoryDescriptorId = SSASPP.ProgramCategoryDescriptorId
	 JOIN edfi.Descriptor DPC ON DPC.DescriptorId = PCD.ProgramCategoryDescriptorId
		AND DPC.CodeValue = 'Independent Study'
		AND DPC.Namespace LIKE 'http://education.mn.gov%'
	 ) ISI ON ISI.StudentUSI = SSAE.StudentUSI
		   AND ISI.SchoolId = SSAE.SchoolId
		   AND ISI.EntryDate = SSAE.EntryDate
LEFT JOIN 
	(SELECT SSASPP.StudentUSI
		  , SSASPP.SchoolId
		  , SSASPP.EntryDate
		  , 1 AS [Section504Placement]
		  , SSASPP.BeginDate AS [Section504PlacementBeginDate]
		  , SSASPP.EndDate AS [Section504PlacementEndDate]
	 FROM extension.StudentSchoolAssociationStudentProgramParticipation SSASPP
	 JOIN extension.ProgramCategoryDescriptor PCD ON PCD.ProgramCategoryDescriptorId = SSASPP.ProgramCategoryDescriptorId
	 JOIN edfi.Descriptor DPC ON DPC.DescriptorId = PCD.ProgramCategoryDescriptorId
		AND DPC.CodeValue = 'Section 504 Placement'
		AND DPC.Namespace LIKE 'http://education.mn.gov%'
	 ) SE ON SE.StudentUSI = SSAE.StudentUSI
		  AND SE.SchoolId = SSAE.SchoolId
		  AND SE.EntryDate = SSAE.EntryDate
LEFT JOIN 
	(SELECT SSASPP.StudentUSI
		  , SSASPP.SchoolId
		  , SSASPP.EntryDate
		  , 1 AS [Title1Indicator]
	 FROM extension.StudentSchoolAssociationStudentProgramParticipation SSASPP
	 JOIN extension.ProgramCategoryDescriptor PCD ON PCD.ProgramCategoryDescriptorId = SSASPP.ProgramCategoryDescriptorId
	 JOIN edfi.Descriptor DPC ON DPC.DescriptorId = PCD.ProgramCategoryDescriptorId
		AND DPC.CodeValue = 'Title I Part A'
		AND DPC.Namespace LIKE 'http://education.mn.gov%'
	 ) TII ON TII.StudentUSI = SSAE.StudentUSI
		  AND TII.SchoolId = SSAE.SchoolId
		  AND TII.EntryDate = SSAE.EntryDate
LEFT JOIN extension.SpecialEducationEvaluationStatusDescriptor SEESD 
	ON SEESD.SpecialEducationEvaluationStatusDescriptorId = SSAE.SpecialEducationEvaluationStatusDescriptorId
LEFT JOIN edfi.Descriptor DSEES ON DSEES.DescriptorId = SEESD.SpecialEducationEvaluationStatusDescriptorId
	AND DSEES.Namespace LIKE 'http://education.mn.gov%'
LEFT JOIN (
	SELECT SSASPP.StudentUSI
		 , SSASPP.SchoolId
		 , SSASPP.EntryDate
		 , SSASPP.BeginDate AS [EnglishLearnerStartDate]
	FROM extension.StudentSchoolAssociationStudentProgramParticipation SSASPP
	JOIN extension.ProgramCategoryDescriptor PCD ON PCD.ProgramCategoryDescriptorId = SSASPP.ProgramCategoryDescriptorId
	JOIN edfi.Descriptor DPC ON DPC.DescriptorId = PCD.ProgramCategoryDescriptorId
		AND DPC.CodeValue = 'English Learner Served'
		AND DPC.Namespace LIKE 'http://education.mn.gov%'
	) E ON E.StudentUSI = SSAE.StudentUSI
		  AND E.SchoolId = SSAE.SchoolId
		  AND E.EntryDate = SSAE.EntryDate
LEFT JOIN edfi.LocalEducationAgency RLEA ON RLEA.LocalEducationAgencyId = SSAE.ResidentLocalEducationAgencyId
LEFT JOIN edfi.DistrictTypeDescriptor RDTD ON RDTD.DistrictTypeDescriptorId = RLEA.DistrictTypeDescriptorId
LEFT JOIN edfi.Descriptor RDT ON RDT.DescriptorId = RDTD.DistrictTypeDescriptorId
	AND RDT.Namespace LIKE 'http://education.mn.gov%'
JOIN edfi.School Sc ON Sc.SchoolId = SSA.SchoolId
LEFT JOIN edfi.SchoolClassificationDescriptor SD ON SD.SchoolClassificationDescriptorId = Sc.SchoolClassificationDescriptorId
LEFT JOIN edfi.Descriptor DS ON DS.DescriptorId = SD.SchoolClassificationDescriptorId
	AND DS.Namespace LIKE 'http://education.mn.gov%'
JOIN edfi.LocalEducationAgency LEA ON LEA.LocalEducationAgencyId = Sc.LocalEducationAgencyId
LEFT JOIN edfi.DistrictTypeDescriptor DTD ON DTD.DistrictTypeDescriptorId = LEA.DistrictTypeDescriptorId
LEFT JOIN edfi.Descriptor DT ON DT.DescriptorId = DTD.DistrictTypeDescriptorId
	AND DT.Namespace LIKE 'http://education.mn.gov%';