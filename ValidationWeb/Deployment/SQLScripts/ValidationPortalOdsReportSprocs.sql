IF OBJECT_ID ( 'rules.RaceAEOReport', 'P' ) IS NOT NULL   
    DROP PROCEDURE rules.RaceAEOReport;  
GO  

CREATE PROCEDURE [rules].[RaceAEOReport]
	@distid int
AS

BEGIN
	DECLARE @RaceAEOTable TABLE (
		OrgType INT,  -- 100 = SCHOOL, 200 = DISTRICT, 300 = STATE
		EdOrgId INT,
		SchoolName nvarchar(255),
		DistrictEdOrgId int,
		DistrictName nvarchar(255),
		RaceGivenCount int,
		AncestryGivenCount int,
		DistinctEnrollmentCount int,
		DistinctDemographicsCount int
	);


IF @distid IS NOT NULL
BEGIN
	----------------------------------------------------------------------------------------------------------------------------------------------------------------------
	-- SCHOOL LEVEL ------------------------------------------------------------------------------------------------------------------------------------------------------
	----------------------------------------------------------------------------------------------------------------------------------------------------------------------
	-- Load School information, rolling up to District - if District is NULL, then ALL Districts in the State.
	-- REGARDLESS of whether they have any enrollments or demographics.
	INSERT INTO @RaceAEOTable(
		OrgType, 
		EdOrgId, 
		SchoolName, 
		DistrictEdOrgId, 
		DistrictName, 
		DistinctEnrollmentCount, 
		DistinctDemographicsCount,
		RaceGivenCount,
		AncestryGivenCount)
		SELECT 
			100, -- SCHOOL LEVEL
			eorg.EducationOrganizationId AS EdOrgId,
			eorg.NameOfInstitution As SchoolName,
			eorgdist.EducationOrganizationId AS DistrictEdOrgId,
			eorgdist.NameOfInstitution AS DistrictName,
			DistinctEnrollmentCount = enr_distinct.quantity,
			DistinctDemographicsCount = dem_distinct.quantity,
			RaceGivenCount = rac_distinct.quantity,
			AncestryGivenCount = anc_distinct.quantity
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
			OUTER APPLY (SELECT COUNT(DISTINCT seoar.StudentUSI) AS quantity 
				FROM edfi.Student s 
				LEFT OUTER JOIN edfi.StudentEducationOrganizationAssociation seoa ON seoa.StudentUSI = s.StudentUSI
				LEFT OUTER JOIN extension.StudentEducationOrganizationAssociationRace seoar ON seoar.StudentUSI = s.StudentUSI
				WHERE seoa.EducationOrganizationId = eorg.EducationOrganizationId) rac_distinct
			OUTER APPLY (SELECT COUNT(DISTINCT seoaeo.StudentUSI) AS quantity 
				FROM edfi.Student s 
				LEFT OUTER JOIN edfi.StudentEducationOrganizationAssociation seoa ON seoa.StudentUSI = s.StudentUSI
				LEFT OUTER JOIN extension.StudentEducationOrganizationAssociationAncestryEthnicOrigin seoaeo ON seoaeo.StudentUSI = s.StudentUSI
				WHERE seoa.EducationOrganizationId = eorg.EducationOrganizationId) anc_distinct
		WHERE @distid = eorgdist.EducationOrganizationId
		GROUP BY 
			eorg.EducationOrganizationId, 
			eorg.NameOfInstitution, 
			eorgdist.EducationOrganizationId, 
			eorgdist.NameOfInstitution,
			enr_distinct.quantity, 
			dem_distinct.quantity,
			rac_distinct.quantity,
			anc_distinct.quantity
	;
END

	----------------------------------------------------------------------------------------------------------------------------------------------------------------------
	-- DISTRICT LEVEL ----------------------------------------------------------------------------------------------------------------------------------------------------
	----------------------------------------------------------------------------------------------------------------------------------------------------------------------

	-- Load School information, rolling up to District - if District is NULL, then ALL Districts in the State.
	-- REGARDLESS of whether they have any enrollments or demographics.
	INSERT INTO @RaceAEOTable(
		OrgType, 
		EdOrgId, 
		SchoolName,      -- Actually, the District name needed here.
		DistrictEdOrgId, -- NULL for District Level
		DistrictName,    -- NULL for District Level
		DistinctEnrollmentCount, 
		DistinctDemographicsCount,
		RaceGivenCount,
		AncestryGivenCount)
		SELECT 
			200, -- DISTRICT LEVEL
			eorgdist.EducationOrganizationId AS DistrictEdOrgId,
			eorgdist.NameOfInstitution AS DistrictName,
			NULL,
			NULL,
			DistinctEnrollmentCount = enr_distinct.quantity,
			DistinctDemographicsCount = dem_distinct.quantity,
			RaceGivenCount = rac_distinct.quantity,
			AncestryGivenCount = anc_distinct.quantity
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
			OUTER APPLY (SELECT COUNT(DISTINCT seoar.StudentUSI) AS quantity 
				FROM edfi.Student s 
				LEFT OUTER JOIN edfi.StudentEducationOrganizationAssociation seoa ON seoa.StudentUSI = s.StudentUSI
				LEFT OUTER JOIN extension.StudentEducationOrganizationAssociationRace seoar ON seoar.StudentUSI = s.StudentUSI
				LEFT OUTER JOIN edfi.School sch ON seoa.EducationOrganizationId = sch.SchoolId
				WHERE eorgdist.EducationOrganizationId IN (seoa.EducationOrganizationId, sch.LocalEducationAgencyId)) rac_distinct
			OUTER APPLY (SELECT COUNT(DISTINCT seoaeo.StudentUSI) AS quantity 
				FROM edfi.Student s 
				LEFT OUTER JOIN edfi.StudentEducationOrganizationAssociation seoa ON seoa.StudentUSI = s.StudentUSI
				LEFT OUTER JOIN extension.StudentEducationOrganizationAssociationAncestryEthnicOrigin seoaeo ON seoaeo.StudentUSI = s.StudentUSI
				LEFT OUTER JOIN edfi.School sch ON seoa.EducationOrganizationId = sch.SchoolId
				WHERE eorgdist.EducationOrganizationId IN (seoa.EducationOrganizationId, sch.LocalEducationAgencyId)) anc_distinct
		WHERE @distid = eorgdist.EducationOrganizationId OR @distid IS NULL
		GROUP BY 
			eorgdist.EducationOrganizationId, 
			eorgdist.NameOfInstitution,
			enr_distinct.quantity, 
			dem_distinct.quantity,
			rac_distinct.quantity,
			anc_distinct.quantity
	;

	----------------------------------------------------------------------------------------------------------------------------------------------------------------------
	-- STATE LEVEL ----------------------------------------------------------------------------------------------------------------------------------------------------
	----------------------------------------------------------------------------------------------------------------------------------------------------------------------

	-- Load School information, rolling up to District - if District is NULL, then ALL Districts in the State.
	-- REGARDLESS of whether they have any enrollments or demographics.
	INSERT INTO @RaceAEOTable(
		OrgType, 
		EdOrgId, 
		SchoolName,      -- Actually, the District name needed here.
		DistrictEdOrgId, -- NULL for District Level
		DistrictName,    -- NULL for District Level
		DistinctEnrollmentCount, 
		DistinctDemographicsCount,
		RaceGivenCount,
		AncestryGivenCount)
		SELECT 
			300, -- STATE LEVEL
			NULL AS DistrictEdOrgId,
			'State of Minnesota' AS SchoolName,
			NULL,
			NULL,
			DistinctEnrollmentCount = enr_distinct.quantity,
			DistinctDemographicsCount = dem_distinct.quantity,
			RaceGivenCount = rac_distinct.quantity,
			AncestryGivenCount = anc_distinct.quantity
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
			OUTER APPLY (SELECT COUNT(DISTINCT seoaeo.StudentUSI) AS quantity 
				FROM edfi.Student s 
				LEFT OUTER JOIN edfi.StudentEducationOrganizationAssociation seoa ON seoa.StudentUSI = s.StudentUSI
				LEFT OUTER JOIN extension.StudentEducationOrganizationAssociationAncestryEthnicOrigin seoaeo ON seoaeo.StudentUSI = s.StudentUSI) anc_distinct
		GROUP BY 
			enr_distinct.quantity, 
			dem_distinct.quantity,
			rac_distinct.quantity,
			anc_distinct.quantity
	;

	SELECT * FROM @RaceAEOTable ORDER BY OrgType DESC, SchoolName;
END
GO

IF OBJECT_ID ( 'rules.MultipleEnrollmentReport', 'P' ) IS NOT NULL   
    DROP PROCEDURE rules.MultipleEnrollmentReport;  
GO  

CREATE PROCEDURE [rules].[MultipleEnrollmentReport]
	@distid int
AS

BEGIN
	DECLARE @MultipleEnrollmentsTable TABLE (
		OrgType INT,  -- 100 = SCHOOL, 200 = DISTRICT, 300 = STATE
		EdOrgId INT,
		SchoolName nvarchar(255),
		DistrictEdOrgId int,
		DistrictName nvarchar(255),
		TotalEnrollmentCount int,
		DistinctEnrollmentCount int,
		EnrolledInOtherSchoolsCount int,
		EnrolledInOtherDistrictsCount int
	);

	IF @distid IS NOT NULL
	BEGIN
		----------------------------------------------------------------------------------------------------------------------------------------------------------------------
		-- SCHOOL LEVEL ------------------------------------------------------------------------------------------------------------------------------------------------------
		----------------------------------------------------------------------------------------------------------------------------------------------------------------------
		-- Load School information, rolling up to District - if District is NULL, then ALL Districts in the State.
		-- REGARDLESS of whether they have any multiple enrollments.
		INSERT INTO @MultipleEnrollmentsTable(
			OrgType, 
			EdOrgId, 
			SchoolName, 
			DistrictEdOrgId, 
			DistrictName,
			TotalEnrollmentCount, 
			DistinctEnrollmentCount
			)
			SELECT 
				100, -- SCHOOL LEVEL
				eorg.EducationOrganizationId AS EdOrgId,
				eorg.NameOfInstitution As SchoolName,
				eorgdist.EducationOrganizationId AS DistrictEdOrgId,
				eorgdist.NameOfInstitution AS DistrictName,
				enr.quantity_all AS TotalEnrollmentCount,
				enr.quantity AS DistinctEnrollmentCount
			FROM 
				edfi.School sch 
				LEFT OUTER JOIN edfi.EducationOrganization eorg ON eorg.EducationOrganizationId = sch.SchoolId 
				LEFT OUTER JOIN edfi.LocalEducationAgency lea ON lea.LocalEducationAgencyId = sch.LocalEducationAgencyId 
				LEFT OUTER JOIN edfi.EducationOrganization eorgdist ON eorgdist.EducationOrganizationId = lea.LocalEducationAgencyId 
				-- To include duplicates (multiple records per student), use COUNT(1) instead
				OUTER APPLY (SELECT COUNT(DISTINCT ssa.StudentUSI) AS quantity, COUNT(1) AS quantity_all 
					FROM edfi.Student s 
					LEFT OUTER JOIN edfi.StudentSchoolAssociation ssa ON ssa.StudentUSI = s.StudentUSI
					WHERE ssa.SchoolId = eorg.EducationOrganizationId) enr
			WHERE @distid = eorgdist.EducationOrganizationId
			GROUP BY 
				eorg.EducationOrganizationId, 
				eorg.NameOfInstitution, 
				eorgdist.EducationOrganizationId, 
				eorgdist.NameOfInstitution,
				enr.quantity_all,
				enr.quantity
		;
		UPDATE @MultipleEnrollmentsTable
		SET EnrolledInOtherSchoolsCount = 
				(SELECT COUNT(DISTINCT s.StudentUSI) AS quantity 
				FROM edfi.School sch
				INNER JOIN edfi.StudentSchoolAssociation ssa ON ssa.SchoolId = sch.SchoolId
				INNER JOIN edfi.Student s ON s.StudentUSI = ssa.StudentUSI
				WHERE DistrictEdOrgId = sch.LocalEducationAgencyId 
				GROUP BY s.StudentUSI
				HAVING COUNT(sch.SchoolId) > 1 AND EdOrgId IN (SELECT ssa3.SchoolId FROM edfi.StudentSchoolAssociation ssa3 WHERE s.StudentUSI = ssa3.StudentUSI)),
			EnrolledInOtherDistrictsCount = 
				(SELECT COUNT(DISTINCT st_in_sch.StudentUSI) 
				FROM
					(SELECT s.StudentUSI
					FROM edfi.School sch
					INNER JOIN edfi.StudentSchoolAssociation ssa ON ssa.SchoolId = sch.SchoolId
					INNER JOIN edfi.Student s ON s.StudentUSI = ssa.StudentUSI
					WHERE EdOrgId = sch.SchoolId) st_in_sch
				INNER JOIN edfi.StudentSchoolAssociation ssa2 ON ssa2.StudentUSI = st_in_sch.StudentUSI
				INNER JOIN edfi.School sch2 ON sch2.SchoolId = ssa2.SchoolId
				GROUP BY st_in_sch.StudentUSI
				HAVING COUNT(DISTINCT sch2.LocalEducationAgencyId) > 1)
			;
	END

	----------------------------------------------------------------------------------------------------------------------------------------------------------------------
	-- DISTRICT LEVEL ------------------------------------------------------------------------------------------------------------------------------------------------------
	----------------------------------------------------------------------------------------------------------------------------------------------------------------------
	-- Load School information, rolling up to District - if District is NULL, then ALL Districts in the State.
	-- REGARDLESS of whether they have any multiple enrollments.
	INSERT INTO @MultipleEnrollmentsTable(
		OrgType, 
		EdOrgId, 
		SchoolName, 
		DistrictEdOrgId, 
		DistrictName,
		TotalEnrollmentCount, 
		DistinctEnrollmentCount,
		EnrolledInOtherSchoolsCount,
		EnrolledInOtherDistrictsCount
		)
		SELECT 
			200, -- DISTRICT LEVEL
			eorgdist.EducationOrganizationId AS DistrictEdOrgId,
			eorgdist.NameOfInstitution As DistrictName,
			NULL AS Unused1,
			NULL AS Unused2,
			enr_distinct.quantity_all AS TotalEnrollmentCount,
			enr_distinct.quantity AS DistinctEnrollmentCount,
			enr_multiple.quantity AS EnrolledInOtherSchoolsCount,
			enr_multiple_district.quantity AS EnrolledInOtherDistrictsCount
		FROM 
			edfi.EducationOrganization eorgdist 
			-- ENSURE only districts and not schools by checking that the LEA table has an entry.
			INNER JOIN edfi.LocalEducationAgency lea ON lea.LocalEducationAgencyId = eorgdist.EducationOrganizationId
			OUTER APPLY (SELECT COUNT(1) AS quantity_all, COUNT(DISTINCT ssa.StudentUSI) AS quantity
				FROM edfi.Student s 
				LEFT OUTER JOIN edfi.StudentSchoolAssociation ssa ON ssa.StudentUSI = s.StudentUSI
				LEFT OUTER JOIN edfi.School sch ON ssa.SchoolId = sch.SchoolId
				WHERE sch.LocalEducationAgencyId = eorgdist.EducationOrganizationId) enr_distinct
			OUTER APPLY (SELECT COUNT(DISTINCT s.StudentUSI) AS quantity
				FROM edfi.Student s 
				LEFT OUTER JOIN edfi.StudentSchoolAssociation ssa ON ssa.StudentUSI = s.StudentUSI
				LEFT OUTER JOIN edfi.School sch ON ssa.SchoolId = sch.SchoolId
				WHERE sch.LocalEducationAgencyId = eorgdist.EducationOrganizationId
				GROUP BY s.StudentUSI
				HAVING COUNT(DISTINCT sch.SchoolId) > 1) enr_multiple
			OUTER APPLY (SELECT COUNT(DISTINCT st_in_sch.StudentUSI) as quantity
				FROM
					(SELECT s.StudentUSI
					FROM edfi.School sch
					INNER JOIN edfi.StudentSchoolAssociation ssa ON ssa.SchoolId = sch.SchoolId
					INNER JOIN edfi.Student s ON s.StudentUSI = ssa.StudentUSI
					WHERE eorgdist.EducationOrganizationId = sch.LocalEducationAgencyId) st_in_sch
				INNER JOIN edfi.StudentSchoolAssociation ssa2 ON ssa2.StudentUSI = st_in_sch.StudentUSI
				INNER JOIN edfi.School sch2 ON sch2.SchoolId = ssa2.SchoolId
				GROUP BY st_in_sch.StudentUSI
				HAVING COUNT(DISTINCT sch2.LocalEducationAgencyId) > 1) enr_multiple_district
		WHERE @distid = eorgdist.EducationOrganizationId OR @distid IS NULL
		GROUP BY 
			eorgdist.EducationOrganizationId, 
			eorgdist.NameOfInstitution, 
			enr_distinct.quantity_all,
			enr_distinct.quantity,
			enr_multiple.quantity,
			enr_multiple_district.quantity
	;

	----------------------------------------------------------------------------------------------------------------------------------------------------------------------
	-- STATE LEVEL ----------------------------------------------------------------------------------------------------------------------------------------------------
	----------------------------------------------------------------------------------------------------------------------------------------------------------------------

	INSERT INTO @MultipleEnrollmentsTable(
		OrgType, 
		EdOrgId, 
		SchoolName, 
		DistrictEdOrgId, 
		DistrictName,
		TotalEnrollmentCount, 
		DistinctEnrollmentCount,
		EnrolledInOtherSchoolsCount,
		EnrolledInOtherDistrictsCount
		)
		SELECT 
			300, -- STATE LEVEL
			NULL AS DistrictEdOrgId,
			'State of Minnesota' AS SchoolName,
			NULL,
			NULL,
			TotalEnrollmentCount = enr.quantity_all,
			DistinctEnrollmentCount = enr.quantity,
			EnrolledInOtherSchoolsCount = enr_multiple_school.quantity,
			EnrolledInOtherDistrictssCount = enr_multiple_district.quantity
		FROM 
			edfi.EducationOrganization eorgdist 
			LEFT OUTER JOIN edfi.School sch ON sch.LocalEducationAgencyId  = eorgdist.EducationOrganizationId
			-- To include duplicates (multiple records per student), use COUNT(1) instead
			OUTER APPLY (SELECT COUNT(1) AS quantity_all, COUNT(DISTINCT s.StudentUSI) AS quantity
				FROM edfi.Student s 
				LEFT OUTER JOIN edfi.StudentSchoolAssociation ssa ON ssa.StudentUSI = s.StudentUSI) enr
			OUTER APPLY (SELECT COUNT(DISTINCT s.StudentUSI) AS quantity 
				FROM edfi.Student s 
				INNER JOIN edfi.StudentSchoolAssociation ssa ON ssa.StudentUSI = s.StudentUSI
				INNER JOIN edfi.School sch ON ssa.SchoolId = sch.SchoolId
				GROUP BY s.StudentUSI, sch.LocalEducationAgencyId
				HAVING COUNT(*) > 1) enr_multiple_school
			OUTER APPLY (SELECT COUNT(DISTINCT s.StudentUSI) AS quantity 
				FROM edfi.Student s 
				INNER JOIN edfi.StudentSchoolAssociation ssa ON ssa.StudentUSI = s.StudentUSI
				INNER JOIN edfi.School sch ON ssa.SchoolId = sch.SchoolId
				GROUP BY s.StudentUSI
				HAVING COUNT(DISTINCT sch.LocalEducationAgencyId) > 1) enr_multiple_district
		GROUP BY 
			enr.quantity_all,
			enr.quantity,
			enr_multiple_school.quantity,
			enr_multiple_district.quantity
	;

	SELECT OrgType, 
		EdOrgId, 
		SchoolName, 
		DistrictEdOrgId, 
		DistrictName,
		TotalEnrollmentCount, 
		DistinctEnrollmentCount,
		COALESCE(EnrolledInOtherSchoolsCount,0) AS EnrolledInOtherSchoolsCount,
		COALESCE(EnrolledInOtherDistrictsCount,0) AS EnrolledInOtherDistrictsCount
	FROM @MultipleEnrollmentsTable 
	ORDER BY OrgType DESC, SchoolName;
END
GO

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

	SELECT * FROM @StudentProgramsTable ORDER BY OrgType DESC, SchoolName;
END
GO

IF OBJECT_ID ( 'rules.ChangeOfEnrollment', 'P' ) IS NOT NULL   
    DROP PROCEDURE rules.ChangeOfEnrollment;  
GO  

CREATE PROCEDURE [rules].ChangeOfEnrollment
	@distid int
AS

BEGIN
	DECLARE @ChangeOfEnrollmentTable TABLE (
		IsCurrentDistrict bit,
		CurrentDistEdOrgId int,
		CurrentDistrictName varchar(255),
		CurrentSchoolEdOrgId int,
		CurrentSchoolName varchar(255),
		CurrentEdOrgEnrollmentDate datetime,
		CurrentEdOrgExitDate datetime,
		CurrentGrade varchar(255),
		PastDistEdOrgId int,
		PastDistrictName varchar(255),
		PastSchoolEdOrgId int,
		PastSchoolName varchar(255),
		PastEdOrgEnrollmentDate datetime,
		PastEdOrgExitDate datetime,
		PastGrade varchar(255),
		StudentID varchar(255),
		StudentLastName varchar(255),
		StudentFirstName varchar(255),
		StudentMiddleName varchar(255)
	);
	DECLARE @ThirtyDaysAgo datetime = DATEADD(DAY, -30, GETDATE());

	IF @distid IS NOT NULL
	BEGIN
		----------------------------------------------------------------------------------------------------------------------------------------------------------------------
		-- As Current District  ----------------------------------------------------------------------------------------------------------------------------------------------
		----------------------------------------------------------------------------------------------------------------------------------------------------------------------
		INSERT INTO @ChangeOfEnrollmentTable(
			IsCurrentDistrict,
			CurrentDistEdOrgId,
			CurrentDistrictName,
			CurrentSchoolEdOrgId,
			CurrentSchoolName,
			CurrentEdOrgEnrollmentDate,
			CurrentEdOrgExitDate,
			CurrentGrade,
			PastDistEdOrgId,
			PastDistrictName,
			PastSchoolEdOrgId,
			PastSchoolName,
			PastEdOrgEnrollmentDate,
			PastEdOrgExitDate,
			PastGrade,
			StudentID,
			StudentLastName,
			StudentFirstName,
			StudentMiddleName,
			StudentBirthDate
			)
			SELECT 
				1, -- Gaining/Current School Enrollment
				currdisteorg.EducationOrganizationId,
				currdisteorg.NameOfInstitution,
				currschedorg.EducationOrganizationId,
				currschedorg.NameOfInstitution,
				currssa.EntryDate,
				currssa.ExitWithdrawDate,
				currgld.CodeValue,
				pastdisteorg.EducationOrganizationId,
				pastdisteorg.NameOfInstitution,
				pastschedorg.EducationOrganizationId,
				pastschedorg.NameOfInstitution,
				pastssa.EntryDate,
				pastssa.ExitWithdrawDate,
				pastgld.CodeValue,
				st.StudentUniqueId,
				st.LastSurname,
				st.FirstName,
				st.MiddleName,
				st.BirthDate
			FROM 
				edfi.EducationOrganization currdisteorg 
				INNER JOIN edfi.LocalEducationAgency currlea ON currlea.ParentLocalEducationAgencyId = currdisteorg.EducationOrganizationId 
				INNER JOIN edfi.School currsch ON currsch.LocalEducationAgencyId = currlea.LocalEducationAgencyId 
				LEFT OUTER JOIN edfi.EducationOrganization currschedorg ON currschedorg.EducationOrganizationId = currsch.SchoolId
				INNER JOIN edfi.StudentSchoolAssociation currssa ON currssa.SchoolId = currsch.SchoolId
				LEFT OUTER JOIN edfi.Descriptor currgld ON currssa.EntryGradeLevelDescriptorId = currgld.DescriptorId
				INNER JOIN edfi.Student st ON st.StudentUSI = currssa.StudentUSI
				INNER JOIN edfi.StudentSchoolAssociation pastssa ON pastssa.StudentUSI = st.StudentUSI AND pastssa.SchoolId != currsch.SchoolId
				LEFT OUTER JOIN edfi.Descriptor pastgld ON pastssa.EntryGradeLevelDescriptorId = pastgld.DescriptorId
				INNER JOIN edfi.School pastsch ON pastssa.SchoolId = pastsch.SchoolId
				LEFT OUTER JOIN edfi.EducationOrganization pastschedorg ON pastschedorg.EducationOrganizationId = pastsch.SchoolId
				INNER JOIN edfi.LocalEducationAgency pastlea ON pastlea.LocalEducationAgencyId = pastsch.LocalEducationAgencyId
				INNER JOIN edfi.EducationOrganization pastdisteorg ON pastdisteorg.EducationOrganizationId = pastlea.ParentLocalEducationAgencyId
			WHERE @distid = currdisteorg.EducationOrganizationId
				AND currssa.EntryDate IS NOT NULL 
				AND currssa.EntryDate > @ThirtyDaysAgo
				AND pastssa.EntryDate < @ThirtyDaysAgo
		;

		----------------------------------------------------------------------------------------------------------------------------------------------------------------------
		-- As Past District  ----------------------------------------------------------------------------------------------------------------------------------------------
		----------------------------------------------------------------------------------------------------------------------------------------------------------------------
		-- The ONLY TWO DIFFERENCES from the above SQL statement are the IsCurrentDistrict = 0 and the WHERE clause.
		INSERT INTO @ChangeOfEnrollmentTable(
			IsCurrentDistrict,
			CurrentDistEdOrgId,
			CurrentDistrictName,
			CurrentSchoolEdOrgId,
			CurrentSchoolName,
			CurrentEdOrgEnrollmentDate,
			CurrentEdOrgExitDate,
			CurrentGrade,
			PastDistEdOrgId,
			PastDistrictName,
			PastSchoolEdOrgId,
			PastSchoolName,
			PastEdOrgEnrollmentDate,
			PastEdOrgExitDate,
			PastGrade,
			StudentID,
			StudentLastName,
			StudentFirstName,
			StudentMiddleName,
			StudentBirthDate
			)
			SELECT 
				0, -- Gaining/Current School Enrollment
				currdisteorg.EducationOrganizationId,
				currdisteorg.NameOfInstitution,
				currschedorg.EducationOrganizationId,
				currschedorg.NameOfInstitution,
				currssa.EntryDate,
				currssa.ExitWithdrawDate,
				currgld.CodeValue,
				pastdisteorg.EducationOrganizationId,
				pastdisteorg.NameOfInstitution,
				pastschedorg.EducationOrganizationId,
				pastschedorg.NameOfInstitution,
				pastssa.EntryDate,
				pastssa.ExitWithdrawDate,
				pastgld.CodeValue,
				st.StudentUniqueId,
				st.LastSurname,
				st.FirstName,
				st.MiddleName,
				st.BirthDate
			FROM 
				edfi.EducationOrganization currdisteorg 
				INNER JOIN edfi.LocalEducationAgency currlea ON currlea.ParentLocalEducationAgencyId = currdisteorg.EducationOrganizationId 
				INNER JOIN edfi.School currsch ON currsch.LocalEducationAgencyId = currlea.LocalEducationAgencyId 
				LEFT OUTER JOIN edfi.EducationOrganization currschedorg ON currschedorg.EducationOrganizationId = currsch.SchoolId
				INNER JOIN edfi.StudentSchoolAssociation currssa ON currssa.SchoolId = currsch.SchoolId
				LEFT OUTER JOIN edfi.Descriptor currgld ON currssa.EntryGradeLevelDescriptorId = currgld.DescriptorId
				INNER JOIN edfi.Student st ON st.StudentUSI = currssa.StudentUSI
				INNER JOIN edfi.StudentSchoolAssociation pastssa ON pastssa.StudentUSI = st.StudentUSI AND pastssa.SchoolId != currsch.SchoolId
				LEFT OUTER JOIN edfi.Descriptor pastgld ON pastssa.EntryGradeLevelDescriptorId = pastgld.DescriptorId
				INNER JOIN edfi.School pastsch ON pastssa.SchoolId = pastsch.SchoolId
				LEFT OUTER JOIN edfi.EducationOrganization pastschedorg ON pastschedorg.EducationOrganizationId = pastsch.SchoolId
				INNER JOIN edfi.LocalEducationAgency pastlea ON pastlea.LocalEducationAgencyId = pastsch.LocalEducationAgencyId
				INNER JOIN edfi.EducationOrganization pastdisteorg ON pastdisteorg.EducationOrganizationId = pastlea.ParentLocalEducationAgencyId
			WHERE @distid = pastdisteorg.EducationOrganizationId
				AND currssa.EntryDate IS NOT NULL 
				AND currssa.EntryDate > @ThirtyDaysAgo
				AND pastssa.ExitWithdrawDate > @ThirtyDaysAgo
		;
	END

	SELECT * FROM @ChangeOfEnrollmentTable ORDER BY StudentLastName, StudentFirstName, StudentMiddleName;

END
GO

IF OBJECT_ID ( 'rules.ResidentsEnrolledElsewhereReport', 'P' ) IS NOT NULL   
    DROP PROCEDURE rules.ResidentsEnrolledElsewhereReport;  
GO  

CREATE PROCEDURE [rules].[ResidentsEnrolledElsewhereReport]
	@distid int
AS

BEGIN
	DECLARE @ResidentsEnrolledElsewhereTable TABLE (
		OrgType int,  -- 100 = SCHOOL, 200 = DISTRICT, 300 = STATE
		EdOrgId int,
		EdOrgName nvarchar(255),
		DistrictOfEnrollmentId int,
		DistrictOfEnrollmentName nvarchar(255),
		ResidentsEnrolled int
	);


	----------------------------------------------------------------------------------------------------------------------------------------------------------------------
	-- DISTRICT LEVEL ----------------------------------------------------------------------------------------------------------------------------------------------------
	----------------------------------------------------------------------------------------------------------------------------------------------------------------------

	-- Load School information, rolling up to District - if District is NULL, then ALL Districts in the State.
	-- REGARDLESS of whether they have any enrollments or demographics.
	INSERT INTO @ResidentsEnrolledElsewhereTable(
		OrgType, 
		EdOrgId, 
		EdOrgName,
		DistrictOfEnrollmentId, 
		DistrictOfEnrollmentName, 
		ResidentsEnrolled)
		SELECT 
			200, -- DISTRICT LEVEL
			eorghomedist.EducationOrganizationId,
			eorghomedist.NameOfInstitution,
			otherdisteorg.EducationOrganizationId,
			otherdisteorg.NameOfInstitution,
			COUNT(1)
		FROM 
			edfi.EducationOrganization eorghomedist 
			INNER JOIN edfi.School homesch ON homesch.LocalEducationAgencyId = eorghomedist.EducationOrganizationId
			INNER JOIN extension.StudentSchoolAssociationExtension homessae ON homessae.SchoolId = homesch.SchoolId AND homessae.ResidentLocalEducationAgencyId = @distid
			INNER JOIN edfi.Student st ON st.StudentUSI = homessae.StudentUSI
			-- We have all the students in the home district, find districts where the student is enrolled outside this district
			INNER JOIN edfi.StudentSchoolAssociation otherssa ON otherssa.StudentUSI = st.StudentUSI
			INNER JOIN edfi.School othersch ON othersch.SchoolId = otherssa.SchoolId
			INNER JOIN edfi.EducationOrganization otherdisteorg ON otherdisteorg.EducationOrganizationId = othersch.LocalEducationAgencyId
		WHERE @distid = eorghomedist.EducationOrganizationId AND eorghomedist.EducationOrganizationId != othersch.LocalEducationAgencyId
		GROUP BY 
			eorghomedist.EducationOrganizationId,
			eorghomedist.NameOfInstitution,
			otherdisteorg.EducationOrganizationId,
			otherdisteorg.NameOfInstitution
	;

	----------------------------------------------------------------------------------------------------------------------------------------------------------------------
	-- STATE LEVEL ----------------------------------------------------------------------------------------------------------------------------------------------------
	----------------------------------------------------------------------------------------------------------------------------------------------------------------------

	-- Load School information, rolling up to District - if District is NULL, then ALL Districts in the State.
	-- REGARDLESS of whether they have any enrollments or demographics.
	INSERT INTO @ResidentsEnrolledElsewhereTable(
		OrgType, 
		EdOrgId, 
		EdOrgName,
		DistrictOfEnrollmentId, 
		DistrictOfEnrollmentName, 
		ResidentsEnrolled)
		SELECT 
			300, -- STATE LEVEL
			eorghomedist.EducationOrganizationId,
			eorghomedist.NameOfInstitution,
			otherdisteorg.EducationOrganizationId,
			otherdisteorg.NameOfInstitution,
			COUNT(1)
		FROM 
			edfi.EducationOrganization eorghomedist 
			INNER JOIN edfi.School homesch ON homesch.LocalEducationAgencyId = eorghomedist.EducationOrganizationId
			INNER JOIN extension.StudentSchoolAssociationExtension homessae ON homessae.SchoolId = homesch.SchoolId
			INNER JOIN edfi.Student st ON st.StudentUSI = homessae.StudentUSI
			-- We have all the students in the home district, find districts where the student is enrolled outside this district
			INNER JOIN edfi.StudentSchoolAssociation otherssa ON otherssa.StudentUSI = st.StudentUSI
			INNER JOIN edfi.School othersch ON othersch.SchoolId = otherssa.SchoolId
			INNER JOIN edfi.EducationOrganization otherdisteorg ON otherdisteorg.EducationOrganizationId = othersch.LocalEducationAgencyId
		WHERE eorghomedist.EducationOrganizationId != othersch.LocalEducationAgencyId
		GROUP BY 
			eorghomedist.EducationOrganizationId,
			eorghomedist.NameOfInstitution,
			otherdisteorg.EducationOrganizationId,
			otherdisteorg.NameOfInstitution
	;

	SELECT * FROM @ResidentsEnrolledElsewhereTable ORDER BY DistrictOfEnrollmentName;
END
GO
