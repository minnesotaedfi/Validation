----------------------------------------------------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------------------------------------------
-- PROCEDURE:  Student Programs --------------------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------------------------------------------

IF OBJECT_ID ( 'rules.StudentProgramsReport', 'P' ) IS NOT NULL   
    DROP PROCEDURE rules.StudentProgramsReport;  
GO  

CREATE PROCEDURE [rules].[StudentProgramsReport]
	@distid int
AS

BEGIN
	DECLARE @StudentProgramsTable TABLE (
		OrgType INT,  -- 100 = SCHOOL, 200 = DISTRICT, 300 = STATE
		EdOrgId INT,
		SchoolName nvarchar(255),
		DistrictEdOrgId int,
		DistrictName nvarchar(255),
		DistinctEnrollmentCount int,
		DistinctDemographicsCount int,
		ADParentCount int,
		IndianNativeCount int,
		MigrantCount int,
		HomelessCount int,
		ImmigrantCount int,
		RecentEnglishCount int,
		SLIFECount int,
		EnglishLearnerIdentifiedCount int,
		EnglishLearnerServedCount int,
		IndependentStudyCount int,
		Section504Count int,
		Title1PartACount int,
		FreeReducedCount int
	);

IF @distid IS NOT NULL
BEGIN
	----------------------------------------------------------------------------------------------------------------------------------------------------------------------
	-- SCHOOL LEVEL ------------------------------------------------------------------------------------------------------------------------------------------------------
	----------------------------------------------------------------------------------------------------------------------------------------------------------------------
	-- Load School information, rolling up to District - if District is NULL, then ALL Districts in the State.
	-- REGARDLESS of whether they have any enrollments or demographics.
	INSERT INTO @StudentProgramsTable(
		OrgType, 
		EdOrgId, 
		SchoolName, 
		DistrictEdOrgId, 
		DistrictName, 
		DistinctEnrollmentCount, 
		DistinctDemographicsCount,
		ADParentCount,
		IndianNativeCount,
		MigrantCount,
		HomelessCount,
		ImmigrantCount,
		RecentEnglishCount,
		SLIFECount,
		EnglishLearnerIdentifiedCount,
		EnglishLearnerServedCount,
		IndependentStudyCount,
		Section504Count,
		Title1PartACount,
		FreeReducedCount)
		SELECT 
			100, -- SCHOOL LEVEL
			eorg.EducationOrganizationId AS EdOrgId,
			eorg.NameOfInstitution As SchoolName,
			eorgdist.EducationOrganizationId AS DistrictEdOrgId,
			eorgdist.NameOfInstitution AS DistrictName,
			DistinctEnrollmentCount = enr_distinct.quantity,
			DistinctDemographicsCount = dem_distinct.quantity,
			ADParentCount = adparent.quantity,
			IndianNativeCount = amindian.quantity,
			MigrantCount = migrant.quantity,
			HomelessCount = homeless.quantity,
			ImmigrantCount = immigrant.quantity,
			RecentEnglish = recenteng.quantity,
			SLIFECount = slife.quantity,
			EnglishLearnerIdentifiedCount = limitedeng.quantity,
			EnglishLearnerServedCount = engserved.quantity,
			IndependentStudyCount = independ.quantity,
			Section504Count = sec504.quantity,
			Title1PartACount = title1.quantity,
			FreeReducedCount = mealselig.quantity
		FROM 
			edfi.School sch 
			LEFT OUTER JOIN edfi.EducationOrganization eorg ON eorg.EducationOrganizationId = sch.SchoolId 
			LEFT OUTER JOIN edfi.LocalEducationAgency lea ON lea.LocalEducationAgencyId = sch.LocalEducationAgencyId 
			LEFT OUTER JOIN edfi.EducationOrganization eorgdist ON eorgdist.EducationOrganizationId = lea.LocalEducationAgencyId 
			-- To include duplicates (multiple records per student), use COUNT(1) instead
			OUTER APPLY (SELECT COUNT(DISTINCT ssa.StudentUSI) AS quantity 
				FROM edfi.Student s 
				LEFT OUTER JOIN edfi.StudentSchoolAssociation ssa ON ssa.StudentUSI = s.StudentUSI
				WHERE ssa.SchoolId = eorg.EducationOrganizationId) enr_distinct
			OUTER APPLY (SELECT COUNT(DISTINCT seoa.StudentUSI) AS quantity 
				FROM edfi.Student s 
				LEFT OUTER JOIN edfi.StudentEducationOrganizationAssociation seoa ON seoa.StudentUSI = s.StudentUSI
				WHERE seoa.EducationOrganizationId = eorg.EducationOrganizationId) dem_distinct
			OUTER APPLY (SELECT COUNT(DISTINCT seoasc.StudentUSI) AS quantity 
				FROM edfi.Student s 
				LEFT OUTER JOIN edfi.StudentEducationOrganizationAssociation seoa ON seoa.StudentUSI = s.StudentUSI
				LEFT OUTER JOIN extension.StudentEducationOrganizationAssociationStudentCharacteristic seoasc ON seoasc.StudentUSI = s.StudentUSI
				LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = seoasc.StudentCharacteristicDescriptorId
				WHERE seoa.EducationOrganizationId = eorg.EducationOrganizationId AND d.CodeValue = 'Active Duty Parent (ADP)') adparent
			OUTER APPLY (SELECT COUNT(DISTINCT seoasc.StudentUSI) AS quantity 
				FROM edfi.Student s 
				LEFT OUTER JOIN edfi.StudentEducationOrganizationAssociation seoa ON seoa.StudentUSI = s.StudentUSI
				LEFT OUTER JOIN extension.StudentEducationOrganizationAssociationStudentCharacteristic seoasc ON seoasc.StudentUSI = s.StudentUSI
				LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = seoasc.StudentCharacteristicDescriptorId
				WHERE seoa.EducationOrganizationId = eorg.EducationOrganizationId AND d.CodeValue = 'American Indian - Alaskan Native (Minnesota)') amindian
			OUTER APPLY (SELECT COUNT(DISTINCT seoasc.StudentUSI) AS quantity 
				FROM edfi.Student s 
				LEFT OUTER JOIN edfi.StudentEducationOrganizationAssociation seoa ON seoa.StudentUSI = s.StudentUSI
				LEFT OUTER JOIN extension.StudentEducationOrganizationAssociationStudentCharacteristic seoasc ON seoasc.StudentUSI = s.StudentUSI
				LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = seoasc.StudentCharacteristicDescriptorId
				WHERE seoa.EducationOrganizationId = eorg.EducationOrganizationId AND d.CodeValue = 'Migrant') migrant
			OUTER APPLY (SELECT COUNT(DISTINCT seoasc.StudentUSI) AS quantity 
				FROM edfi.Student s 
				LEFT OUTER JOIN edfi.StudentEducationOrganizationAssociation seoa ON seoa.StudentUSI = s.StudentUSI
				LEFT OUTER JOIN extension.StudentEducationOrganizationAssociationStudentCharacteristic seoasc ON seoasc.StudentUSI = s.StudentUSI
				LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = seoasc.StudentCharacteristicDescriptorId
				WHERE seoa.EducationOrganizationId = eorg.EducationOrganizationId AND d.CodeValue = 'Homeless') homeless
			OUTER APPLY (SELECT COUNT(DISTINCT seoasc.StudentUSI) AS quantity 
				FROM edfi.Student s 
				LEFT OUTER JOIN edfi.StudentEducationOrganizationAssociation seoa ON seoa.StudentUSI = s.StudentUSI
				LEFT OUTER JOIN extension.StudentEducationOrganizationAssociationStudentCharacteristic seoasc ON seoasc.StudentUSI = s.StudentUSI
				LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = seoasc.StudentCharacteristicDescriptorId
				WHERE seoa.EducationOrganizationId = eorg.EducationOrganizationId AND d.CodeValue = 'Immigrant') immigrant
			OUTER APPLY (SELECT COUNT(DISTINCT seoasc.StudentUSI) AS quantity 
				FROM edfi.Student s 
				LEFT OUTER JOIN edfi.StudentEducationOrganizationAssociation seoa ON seoa.StudentUSI = s.StudentUSI
				LEFT OUTER JOIN extension.StudentEducationOrganizationAssociationStudentCharacteristic seoasc ON seoasc.StudentUSI = s.StudentUSI
				LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = seoasc.StudentCharacteristicDescriptorId
				WHERE seoa.EducationOrganizationId = eorg.EducationOrganizationId AND d.CodeValue = 'Recently Arrived English Learner (RAEL)') recenteng
			OUTER APPLY (SELECT COUNT(DISTINCT seoasc.StudentUSI) AS quantity 
				FROM edfi.Student s 
				LEFT OUTER JOIN edfi.StudentEducationOrganizationAssociation seoa ON seoa.StudentUSI = s.StudentUSI
				LEFT OUTER JOIN extension.StudentEducationOrganizationAssociationStudentCharacteristic seoasc ON seoasc.StudentUSI = s.StudentUSI
				LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = seoasc.StudentCharacteristicDescriptorId
				WHERE seoa.EducationOrganizationId = eorg.EducationOrganizationId AND d.CodeValue = 'SLIFE') slife
			OUTER APPLY (SELECT COUNT(DISTINCT ssaspp.StudentUSI) AS quantity 
				FROM edfi.Student s 
				LEFT OUTER JOIN edfi.StudentEducationOrganizationAssociation seoa ON seoa.StudentUSI = s.StudentUSI
				LEFT OUTER JOIN extension.StudentSchoolAssociationStudentProgramParticipation ssaspp ON ssaspp.StudentUSI = s.StudentUSI
				LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = ssaspp.ProgramCategoryDescriptorId
				WHERE seoa.EducationOrganizationId = eorg.EducationOrganizationId AND d.CodeValue LIKE 'Limited%') limitedeng
			OUTER APPLY (SELECT COUNT(DISTINCT ssaspp.StudentUSI) AS quantity 
				FROM edfi.Student s 
				LEFT OUTER JOIN edfi.StudentEducationOrganizationAssociation seoa ON seoa.StudentUSI = s.StudentUSI
				LEFT OUTER JOIN extension.StudentSchoolAssociationStudentProgramParticipation ssaspp ON ssaspp.StudentUSI = s.StudentUSI
				LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = ssaspp.ProgramCategoryDescriptorId
				WHERE seoa.EducationOrganizationId = eorg.EducationOrganizationId AND d.CodeValue = 'English Learner Served') engserved
			OUTER APPLY (SELECT COUNT(DISTINCT ssaspp.StudentUSI) AS quantity 
				FROM edfi.Student s 
				LEFT OUTER JOIN edfi.StudentEducationOrganizationAssociation seoa ON seoa.StudentUSI = s.StudentUSI
				LEFT OUTER JOIN extension.StudentSchoolAssociationStudentProgramParticipation ssaspp ON ssaspp.StudentUSI = s.StudentUSI
				LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = ssaspp.ProgramCategoryDescriptorId
				WHERE seoa.EducationOrganizationId = eorg.EducationOrganizationId AND d.CodeValue = 'Independent Study') independ
			OUTER APPLY (SELECT COUNT(DISTINCT ssaspp.StudentUSI) AS quantity 
				FROM edfi.Student s 
				LEFT OUTER JOIN edfi.StudentEducationOrganizationAssociation seoa ON seoa.StudentUSI = s.StudentUSI
				LEFT OUTER JOIN extension.StudentSchoolAssociationStudentProgramParticipation ssaspp ON ssaspp.StudentUSI = s.StudentUSI
				LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = ssaspp.ProgramCategoryDescriptorId
				WHERE seoa.EducationOrganizationId = eorg.EducationOrganizationId AND d.CodeValue = 'Section 504 Placement') sec504
			OUTER APPLY (SELECT COUNT(DISTINCT ssaspp.StudentUSI) AS quantity 
				FROM edfi.Student s 
				LEFT OUTER JOIN edfi.StudentEducationOrganizationAssociation seoa ON seoa.StudentUSI = s.StudentUSI
				LEFT OUTER JOIN extension.StudentSchoolAssociationStudentProgramParticipation ssaspp ON ssaspp.StudentUSI = s.StudentUSI
				LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = ssaspp.ProgramCategoryDescriptorId
				WHERE seoa.EducationOrganizationId = eorg.EducationOrganizationId AND d.CodeValue = 'Title I Part A') title1
			OUTER APPLY (SELECT COUNT(DISTINCT s.StudentUSI) AS quantity 
				FROM edfi.Student s 
				LEFT OUTER JOIN edfi.StudentEducationOrganizationAssociation seoa ON seoa.StudentUSI = s.StudentUSI
				LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = s.SchoolFoodServicesEligibilityDescriptorId
				WHERE seoa.EducationOrganizationId = eorg.EducationOrganizationId AND (d.ShortDescription LIKE 'reduced' OR d.ShortDescription LIKE 'free')) mealselig				
		WHERE @distid = eorgdist.EducationOrganizationId
		GROUP BY 
			eorg.EducationOrganizationId, 
			eorg.NameOfInstitution, 
			eorgdist.EducationOrganizationId, 
			eorgdist.NameOfInstitution,
			enr_distinct.quantity, 
			dem_distinct.quantity,
			adparent.quantity,
			amindian.quantity,
			migrant.quantity,
			homeless.quantity,
			immigrant.quantity,
			recenteng.quantity,
			slife.quantity,
			limitedeng.quantity,
			engserved.quantity,
			independ.quantity,
			sec504.quantity,
			title1.quantity,
			mealselig.quantity
	;
END

	----------------------------------------------------------------------------------------------------------------------------------------------------------------------
	-- DISTRICT LEVEL ----------------------------------------------------------------------------------------------------------------------------------------------------
	----------------------------------------------------------------------------------------------------------------------------------------------------------------------

	-- Load School information, rolling up to District - if District is NULL, then ALL Districts in the State.
	-- REGARDLESS of whether they have any enrollments or demographics.
	INSERT INTO @StudentProgramsTable(
		OrgType, 
		EdOrgId, 
		SchoolName,      -- Actually, the District name needed here.
		DistrictEdOrgId, -- NULL for District Level
		DistrictName,    -- NULL for District Level
		DistinctEnrollmentCount, 
		DistinctDemographicsCount,
		ADParentCount,
		IndianNativeCount,
		MigrantCount,
		HomelessCount,
		ImmigrantCount,
		RecentEnglishCount,
		SLIFECount,
		EnglishLearnerIdentifiedCount,
		EnglishLearnerServedCount,
		IndependentStudyCount,
		Section504Count,
		Title1PartACount,
		FreeReducedCount)
		SELECT 
			200, -- DISTRICT LEVEL
			eorgdist.EducationOrganizationId AS DistrictEdOrgId,
			eorgdist.NameOfInstitution AS DistrictName,
			NULL,
			NULL,
			DistinctEnrollmentCount = enr_distinct.quantity,
			DistinctDemographicsCount = dem_distinct.quantity,
			ADParentCount = adparent.quantity,
			IndianNativeCount = amindian.quantity,
			MigrantCount = migrant.quantity,
			HomelessCount = homeless.quantity,
			ImmigrantCount = immigrant.quantity,
			RecentEnglish = recenteng.quantity,
			SLIFECount = slife.quantity,
			EnglishLearnerIdentifiedCount = limitedeng.quantity,
			EnglishLearnerServedCount = engserved.quantity,
			IndependentStudyCount = independ.quantity,
			Section504Count = sec504.quantity,
			Title1PartACount = title1.quantity,
			FreeReducedCount = mealselig.quantity
		FROM 
			edfi.EducationOrganization eorgdist 
			INNER JOIN edfi.LocalEducationAgency lea ON lea.LocalEducationAgencyId = eorgdist.EducationOrganizationId
			-- To include duplicates (multiple records per student), use COUNT(1) instead
			OUTER APPLY (SELECT COUNT(DISTINCT ssa.StudentUSI) AS quantity 
				FROM edfi.Student s 
				LEFT OUTER JOIN edfi.StudentSchoolAssociation ssa ON ssa.StudentUSI = s.StudentUSI
				LEFT OUTER JOIN edfi.School sch ON ssa.SchoolId = sch.SchoolId
				WHERE sch.LocalEducationAgencyId = eorgdist.EducationOrganizationId) enr_distinct
			OUTER APPLY (SELECT COUNT(DISTINCT seoa.StudentUSI) AS quantity 
				FROM edfi.Student s 
				LEFT OUTER JOIN edfi.StudentEducationOrganizationAssociation seoa ON seoa.StudentUSI = s.StudentUSI
				LEFT OUTER JOIN edfi.School sch ON sch.SchoolId = seoa.EducationOrganizationId
				WHERE eorgdist.EducationOrganizationId IN (seoa.EducationOrganizationId, sch.LocalEducationAgencyId)) dem_distinct
			OUTER APPLY (SELECT COUNT(DISTINCT seoasc.StudentUSI) AS quantity 
				FROM edfi.Student s 
				LEFT OUTER JOIN edfi.StudentEducationOrganizationAssociation seoa ON seoa.StudentUSI = s.StudentUSI
				LEFT OUTER JOIN edfi.School sch ON seoa.EducationOrganizationId = sch.SchoolId
				LEFT OUTER JOIN extension.StudentEducationOrganizationAssociationStudentCharacteristic seoasc ON seoasc.StudentUSI = s.StudentUSI
				LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = seoasc.StudentCharacteristicDescriptorId
				WHERE eorgdist.EducationOrganizationId IN (seoa.EducationOrganizationId, sch.LocalEducationAgencyId) AND d.CodeValue = 'Active Duty Parent (ADP)') adparent
			OUTER APPLY (SELECT COUNT(DISTINCT seoasc.StudentUSI) AS quantity 
				FROM edfi.Student s 
				LEFT OUTER JOIN edfi.StudentEducationOrganizationAssociation seoa ON seoa.StudentUSI = s.StudentUSI
				LEFT OUTER JOIN edfi.School sch ON seoa.EducationOrganizationId = sch.SchoolId
				LEFT OUTER JOIN extension.StudentEducationOrganizationAssociationStudentCharacteristic seoasc ON seoasc.StudentUSI = s.StudentUSI
				LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = seoasc.StudentCharacteristicDescriptorId
				WHERE eorgdist.EducationOrganizationId IN (seoa.EducationOrganizationId, sch.LocalEducationAgencyId) AND d.CodeValue = 'American Indian - Alaskan Native (Minnesota)') amindian
			OUTER APPLY (SELECT COUNT(DISTINCT seoasc.StudentUSI) AS quantity 
				FROM edfi.Student s 
				LEFT OUTER JOIN edfi.StudentEducationOrganizationAssociation seoa ON seoa.StudentUSI = s.StudentUSI
				LEFT OUTER JOIN edfi.School sch ON seoa.EducationOrganizationId = sch.SchoolId
				LEFT OUTER JOIN extension.StudentEducationOrganizationAssociationStudentCharacteristic seoasc ON seoasc.StudentUSI = s.StudentUSI
				LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = seoasc.StudentCharacteristicDescriptorId
				WHERE eorgdist.EducationOrganizationId IN (seoa.EducationOrganizationId, sch.LocalEducationAgencyId) AND d.CodeValue = 'Migrant') migrant
			OUTER APPLY (SELECT COUNT(DISTINCT seoasc.StudentUSI) AS quantity 
				FROM edfi.Student s 
				LEFT OUTER JOIN edfi.StudentEducationOrganizationAssociation seoa ON seoa.StudentUSI = s.StudentUSI
				LEFT OUTER JOIN edfi.School sch ON seoa.EducationOrganizationId = sch.SchoolId
				LEFT OUTER JOIN extension.StudentEducationOrganizationAssociationStudentCharacteristic seoasc ON seoasc.StudentUSI = s.StudentUSI
				LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = seoasc.StudentCharacteristicDescriptorId
				WHERE eorgdist.EducationOrganizationId IN (seoa.EducationOrganizationId, sch.LocalEducationAgencyId) AND d.CodeValue = 'Homeless') homeless
			OUTER APPLY (SELECT COUNT(DISTINCT seoasc.StudentUSI) AS quantity 
				FROM edfi.Student s 
				LEFT OUTER JOIN edfi.StudentEducationOrganizationAssociation seoa ON seoa.StudentUSI = s.StudentUSI
				LEFT OUTER JOIN edfi.School sch ON seoa.EducationOrganizationId = sch.SchoolId
				LEFT OUTER JOIN extension.StudentEducationOrganizationAssociationStudentCharacteristic seoasc ON seoasc.StudentUSI = s.StudentUSI
				LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = seoasc.StudentCharacteristicDescriptorId
				WHERE eorgdist.EducationOrganizationId IN (seoa.EducationOrganizationId, sch.LocalEducationAgencyId) AND d.CodeValue = 'Immigrant') immigrant
			OUTER APPLY (SELECT COUNT(DISTINCT seoasc.StudentUSI) AS quantity 
				FROM edfi.Student s 
				LEFT OUTER JOIN edfi.StudentEducationOrganizationAssociation seoa ON seoa.StudentUSI = s.StudentUSI
				LEFT OUTER JOIN edfi.School sch ON seoa.EducationOrganizationId = sch.SchoolId
				LEFT OUTER JOIN extension.StudentEducationOrganizationAssociationStudentCharacteristic seoasc ON seoasc.StudentUSI = s.StudentUSI
				LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = seoasc.StudentCharacteristicDescriptorId
				WHERE eorgdist.EducationOrganizationId IN (seoa.EducationOrganizationId, sch.LocalEducationAgencyId) AND d.CodeValue = 'Recently Arrived English Learner (RAEL)') recenteng
			OUTER APPLY (SELECT COUNT(DISTINCT seoasc.StudentUSI) AS quantity 
				FROM edfi.Student s 
				LEFT OUTER JOIN edfi.StudentEducationOrganizationAssociation seoa ON seoa.StudentUSI = s.StudentUSI
				LEFT OUTER JOIN edfi.School sch ON seoa.EducationOrganizationId = sch.SchoolId
				LEFT OUTER JOIN extension.StudentEducationOrganizationAssociationStudentCharacteristic seoasc ON seoasc.StudentUSI = s.StudentUSI
				LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = seoasc.StudentCharacteristicDescriptorId
				WHERE eorgdist.EducationOrganizationId IN (seoa.EducationOrganizationId, sch.LocalEducationAgencyId) AND d.CodeValue = 'SLIFE') slife
			OUTER APPLY (SELECT COUNT(DISTINCT ssaspp.StudentUSI) AS quantity 
				FROM edfi.Student s 
				LEFT OUTER JOIN edfi.StudentEducationOrganizationAssociation seoa ON seoa.StudentUSI = s.StudentUSI
				LEFT OUTER JOIN edfi.School sch ON seoa.EducationOrganizationId = sch.SchoolId
				LEFT OUTER JOIN extension.StudentSchoolAssociationStudentProgramParticipation ssaspp ON ssaspp.StudentUSI = s.StudentUSI
				LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = ssaspp.ProgramCategoryDescriptorId
				WHERE eorgdist.EducationOrganizationId IN (seoa.EducationOrganizationId, sch.LocalEducationAgencyId) AND d.CodeValue LIKE 'Limited%') limitedeng
			OUTER APPLY (SELECT COUNT(DISTINCT ssaspp.StudentUSI) AS quantity 
				FROM edfi.Student s 
				LEFT OUTER JOIN edfi.StudentEducationOrganizationAssociation seoa ON seoa.StudentUSI = s.StudentUSI
				LEFT OUTER JOIN edfi.School sch ON seoa.EducationOrganizationId = sch.SchoolId
				LEFT OUTER JOIN extension.StudentSchoolAssociationStudentProgramParticipation ssaspp ON ssaspp.StudentUSI = s.StudentUSI
				LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = ssaspp.ProgramCategoryDescriptorId
				WHERE eorgdist.EducationOrganizationId IN (seoa.EducationOrganizationId, sch.LocalEducationAgencyId) AND d.CodeValue = 'English Learner Served') engserved
			OUTER APPLY (SELECT COUNT(DISTINCT ssaspp.StudentUSI) AS quantity 
				FROM edfi.Student s 
				LEFT OUTER JOIN edfi.StudentEducationOrganizationAssociation seoa ON seoa.StudentUSI = s.StudentUSI
				LEFT OUTER JOIN edfi.School sch ON seoa.EducationOrganizationId = sch.SchoolId
				LEFT OUTER JOIN extension.StudentSchoolAssociationStudentProgramParticipation ssaspp ON ssaspp.StudentUSI = s.StudentUSI
				LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = ssaspp.ProgramCategoryDescriptorId
				WHERE eorgdist.EducationOrganizationId IN (seoa.EducationOrganizationId, sch.LocalEducationAgencyId) AND d.CodeValue = 'Independent Study') independ
			OUTER APPLY (SELECT COUNT(DISTINCT ssaspp.StudentUSI) AS quantity 
				FROM edfi.Student s 
				LEFT OUTER JOIN edfi.StudentEducationOrganizationAssociation seoa ON seoa.StudentUSI = s.StudentUSI
				LEFT OUTER JOIN edfi.School sch ON seoa.EducationOrganizationId = sch.SchoolId
				LEFT OUTER JOIN extension.StudentSchoolAssociationStudentProgramParticipation ssaspp ON ssaspp.StudentUSI = s.StudentUSI
				LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = ssaspp.ProgramCategoryDescriptorId
				WHERE eorgdist.EducationOrganizationId IN (seoa.EducationOrganizationId, sch.LocalEducationAgencyId) AND d.CodeValue = 'Section 504 Placement') sec504
			OUTER APPLY (SELECT COUNT(DISTINCT ssaspp.StudentUSI) AS quantity 
				FROM edfi.Student s 
				LEFT OUTER JOIN edfi.StudentEducationOrganizationAssociation seoa ON seoa.StudentUSI = s.StudentUSI
				LEFT OUTER JOIN edfi.School sch ON seoa.EducationOrganizationId = sch.SchoolId
				LEFT OUTER JOIN extension.StudentSchoolAssociationStudentProgramParticipation ssaspp ON ssaspp.StudentUSI = s.StudentUSI
				LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = ssaspp.ProgramCategoryDescriptorId
				WHERE eorgdist.EducationOrganizationId IN (seoa.EducationOrganizationId, sch.LocalEducationAgencyId) AND d.CodeValue = 'Title I Part A') title1
			OUTER APPLY (SELECT COUNT(DISTINCT s.StudentUSI) AS quantity 
				FROM edfi.Student s 
				LEFT OUTER JOIN edfi.StudentEducationOrganizationAssociation seoa ON seoa.StudentUSI = s.StudentUSI
				LEFT OUTER JOIN edfi.School sch ON seoa.EducationOrganizationId = sch.SchoolId
				LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = s.SchoolFoodServicesEligibilityDescriptorId
				WHERE  eorgdist.EducationOrganizationId IN (seoa.EducationOrganizationId, sch.LocalEducationAgencyId) AND (d.ShortDescription LIKE 'reduced' OR d.ShortDescription LIKE 'free')) mealselig
		WHERE @distid = eorgdist.EducationOrganizationId OR @distid IS NULL
		GROUP BY 
			eorgdist.EducationOrganizationId, 
			eorgdist.NameOfInstitution,
			enr_distinct.quantity, 
			dem_distinct.quantity,
			adparent.quantity,
			amindian.quantity,
			migrant.quantity,
			homeless.quantity,
			immigrant.quantity,
			recenteng.quantity,
			slife.quantity,
			limitedeng.quantity,
			engserved.quantity,
			independ.quantity,
			sec504.quantity,
			title1.quantity,
			mealselig.quantity
	;

	IF @distid IS NULL
	BEGIN
		----------------------------------------------------------------------------------------------------------------------------------------------------------------------
		-- STATE LEVEL ----------------------------------------------------------------------------------------------------------------------------------------------------
		----------------------------------------------------------------------------------------------------------------------------------------------------------------------
		-- Load School information, rolling up to District - if District is NULL, then ALL Districts in the State.
		-- REGARDLESS of whether they have any enrollments or demographics.
		INSERT INTO @StudentProgramsTable(
			OrgType, 
			EdOrgId, 
			SchoolName,      -- Actually, the District name needed here.
			DistrictEdOrgId, -- NULL for District Level
			DistrictName,    -- NULL for District Level
			DistinctEnrollmentCount, 
			DistinctDemographicsCount,
			ADParentCount,
			IndianNativeCount,
			MigrantCount,
			HomelessCount,
			ImmigrantCount,
			RecentEnglishCount,
			SLIFECount,
			EnglishLearnerIdentifiedCount,
			EnglishLearnerServedCount,
			IndependentStudyCount,
			Section504Count,
			Title1PartACount,
			FreeReducedCount)
			SELECT 
				300, -- STATE LEVEL
				NULL AS DistrictEdOrgId,
				'State of Minnesota' AS SchoolName,
				NULL,
				NULL,
				DistinctEnrollmentCount = enr_distinct.quantity,
				DistinctDemographicsCount = dem_distinct.quantity,
				ADParentCount = adparent.quantity,
				IndianNativeCount = amindian.quantity,
				MigrantCount = migrant.quantity,
				HomelessCount = homeless.quantity,
				ImmigrantCount = immigrant.quantity,
				RecentEnglish = recenteng.quantity,
				SLIFECount = slife.quantity,
				EnglishLearnerIdentifiedCount = limitedeng.quantity,
				EnglishLearnerServedCount = engserved.quantity,
				IndependentStudyCount = independ.quantity,
				Section504Count = sec504.quantity,
				Title1PartACount = title1.quantity,
				FreeReducedCount = mealselig.quantity
			FROM 
				edfi.EducationOrganization eorgdist 
				LEFT OUTER JOIN edfi.School sch ON sch.LocalEducationAgencyId  = eorgdist.EducationOrganizationId
				-- To include duplicates (multiple records per student), use COUNT(1) instead
				OUTER APPLY (SELECT COUNT(DISTINCT ssa.StudentUSI) AS quantity 
					FROM edfi.Student s 
					LEFT OUTER JOIN edfi.StudentSchoolAssociation ssa ON ssa.StudentUSI = s.StudentUSI) enr_distinct
				OUTER APPLY (SELECT COUNT(DISTINCT seoa.StudentUSI) AS quantity 
					FROM edfi.Student s 
					LEFT OUTER JOIN edfi.StudentEducationOrganizationAssociation seoa ON seoa.StudentUSI = s.StudentUSI) dem_distinct
				OUTER APPLY (SELECT COUNT(DISTINCT seoar.StudentUSI) AS quantity 
					FROM edfi.Student s 
					LEFT OUTER JOIN edfi.StudentEducationOrganizationAssociation seoa ON seoa.StudentUSI = s.StudentUSI
					LEFT OUTER JOIN extension.StudentEducationOrganizationAssociationRace seoar ON seoar.StudentUSI = s.StudentUSI) rac_distinct
				OUTER APPLY (SELECT COUNT(DISTINCT seoasc.StudentUSI) AS quantity 
					FROM edfi.Student s 
					LEFT OUTER JOIN edfi.StudentEducationOrganizationAssociation seoa ON seoa.StudentUSI = s.StudentUSI
					LEFT OUTER JOIN extension.StudentEducationOrganizationAssociationStudentCharacteristic seoasc ON seoasc.StudentUSI = s.StudentUSI
					LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = seoasc.StudentCharacteristicDescriptorId
					WHERE d.CodeValue = 'Active Duty Parent (ADP)') adparent
				OUTER APPLY (SELECT COUNT(DISTINCT seoasc.StudentUSI) AS quantity 
					FROM edfi.Student s 
					LEFT OUTER JOIN edfi.StudentEducationOrganizationAssociation seoa ON seoa.StudentUSI = s.StudentUSI
					LEFT OUTER JOIN extension.StudentEducationOrganizationAssociationStudentCharacteristic seoasc ON seoasc.StudentUSI = s.StudentUSI
					LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = seoasc.StudentCharacteristicDescriptorId
					WHERE d.CodeValue = 'American Indian - Alaskan Native (Minnesota)') amindian
				OUTER APPLY (SELECT COUNT(DISTINCT seoasc.StudentUSI) AS quantity 
					FROM edfi.Student s 
					LEFT OUTER JOIN edfi.StudentEducationOrganizationAssociation seoa ON seoa.StudentUSI = s.StudentUSI
					LEFT OUTER JOIN extension.StudentEducationOrganizationAssociationStudentCharacteristic seoasc ON seoasc.StudentUSI = s.StudentUSI
					LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = seoasc.StudentCharacteristicDescriptorId
					WHERE d.CodeValue = 'Migrant') migrant
				OUTER APPLY (SELECT COUNT(DISTINCT seoasc.StudentUSI) AS quantity 
					FROM edfi.Student s 
					LEFT OUTER JOIN edfi.StudentEducationOrganizationAssociation seoa ON seoa.StudentUSI = s.StudentUSI
					LEFT OUTER JOIN extension.StudentEducationOrganizationAssociationStudentCharacteristic seoasc ON seoasc.StudentUSI = s.StudentUSI
					LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = seoasc.StudentCharacteristicDescriptorId
					WHERE d.CodeValue = 'Homeless') homeless
				OUTER APPLY (SELECT COUNT(DISTINCT seoasc.StudentUSI) AS quantity 
					FROM edfi.Student s 
					LEFT OUTER JOIN edfi.StudentEducationOrganizationAssociation seoa ON seoa.StudentUSI = s.StudentUSI
					LEFT OUTER JOIN extension.StudentEducationOrganizationAssociationStudentCharacteristic seoasc ON seoasc.StudentUSI = s.StudentUSI
					LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = seoasc.StudentCharacteristicDescriptorId
					WHERE d.CodeValue = 'Immigrant') immigrant
				OUTER APPLY (SELECT COUNT(DISTINCT seoasc.StudentUSI) AS quantity 
					FROM edfi.Student s 
					LEFT OUTER JOIN edfi.StudentEducationOrganizationAssociation seoa ON seoa.StudentUSI = s.StudentUSI
					LEFT OUTER JOIN extension.StudentEducationOrganizationAssociationStudentCharacteristic seoasc ON seoasc.StudentUSI = s.StudentUSI
					LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = seoasc.StudentCharacteristicDescriptorId
					WHERE d.CodeValue = 'Recently Arrived English Learner (RAEL)') recenteng
				OUTER APPLY (SELECT COUNT(DISTINCT seoasc.StudentUSI) AS quantity 
					FROM edfi.Student s 
					LEFT OUTER JOIN edfi.StudentEducationOrganizationAssociation seoa ON seoa.StudentUSI = s.StudentUSI
					LEFT OUTER JOIN extension.StudentEducationOrganizationAssociationStudentCharacteristic seoasc ON seoasc.StudentUSI = s.StudentUSI
					LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = seoasc.StudentCharacteristicDescriptorId
					WHERE d.CodeValue = 'SLIFE') slife
				OUTER APPLY (SELECT COUNT(DISTINCT ssaspp.StudentUSI) AS quantity 
					FROM edfi.Student s 
					LEFT OUTER JOIN edfi.StudentEducationOrganizationAssociation seoa ON seoa.StudentUSI = s.StudentUSI
					LEFT OUTER JOIN extension.StudentSchoolAssociationStudentProgramParticipation ssaspp ON ssaspp.StudentUSI = s.StudentUSI
					LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = ssaspp.ProgramCategoryDescriptorId
					WHERE d.CodeValue LIKE 'Limited%') limitedeng
				OUTER APPLY (SELECT COUNT(DISTINCT ssaspp.StudentUSI) AS quantity 
					FROM edfi.Student s 
					LEFT OUTER JOIN edfi.StudentEducationOrganizationAssociation seoa ON seoa.StudentUSI = s.StudentUSI
					LEFT OUTER JOIN extension.StudentSchoolAssociationStudentProgramParticipation ssaspp ON ssaspp.StudentUSI = s.StudentUSI
					LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = ssaspp.ProgramCategoryDescriptorId
					WHERE d.CodeValue = 'English Learner Served') engserved
				OUTER APPLY (SELECT COUNT(DISTINCT ssaspp.StudentUSI) AS quantity 
					FROM edfi.Student s 
					LEFT OUTER JOIN edfi.StudentEducationOrganizationAssociation seoa ON seoa.StudentUSI = s.StudentUSI
					LEFT OUTER JOIN extension.StudentSchoolAssociationStudentProgramParticipation ssaspp ON ssaspp.StudentUSI = s.StudentUSI
					LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = ssaspp.ProgramCategoryDescriptorId
					WHERE d.CodeValue = 'Independent Study') independ
				OUTER APPLY (SELECT COUNT(DISTINCT ssaspp.StudentUSI) AS quantity 
					FROM edfi.Student s 
					LEFT OUTER JOIN edfi.StudentEducationOrganizationAssociation seoa ON seoa.StudentUSI = s.StudentUSI
					LEFT OUTER JOIN extension.StudentSchoolAssociationStudentProgramParticipation ssaspp ON ssaspp.StudentUSI = s.StudentUSI
					LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = ssaspp.ProgramCategoryDescriptorId
					WHERE d.CodeValue = 'Section 504 Placement') sec504
				OUTER APPLY (SELECT COUNT(DISTINCT ssaspp.StudentUSI) AS quantity 
					FROM edfi.Student s 
					LEFT OUTER JOIN edfi.StudentEducationOrganizationAssociation seoa ON seoa.StudentUSI = s.StudentUSI
					LEFT OUTER JOIN extension.StudentSchoolAssociationStudentProgramParticipation ssaspp ON ssaspp.StudentUSI = s.StudentUSI
					LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = ssaspp.ProgramCategoryDescriptorId
					WHERE d.CodeValue = 'Title I Part A') title1
				OUTER APPLY (SELECT COUNT(DISTINCT s.StudentUSI) AS quantity 
					FROM edfi.Student s 
					LEFT OUTER JOIN edfi.StudentEducationOrganizationAssociation seoa ON seoa.StudentUSI = s.StudentUSI
					LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = s.SchoolFoodServicesEligibilityDescriptorId
					WHERE (d.ShortDescription LIKE 'reduced' OR d.ShortDescription LIKE 'free')) mealselig
			GROUP BY 
				enr_distinct.quantity, 
				dem_distinct.quantity,
				adparent.quantity,
				amindian.quantity,
				migrant.quantity,
				homeless.quantity,
				immigrant.quantity,
				recenteng.quantity,
				slife.quantity,
				limitedeng.quantity,
				engserved.quantity,
				independ.quantity,
				sec504.quantity,
				title1.quantity,
				mealselig.quantity
		;
	END

	SELECT * FROM @StudentProgramsTable ORDER BY OrgType DESC, SchoolName;
END
