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
				INNER JOIN edfi.StudentSchoolAssociation ssa ON ssa.StudentUSI = s.StudentUSI
				INNER JOIN edfi.School sch ON ssa.SchoolId = sch.SchoolId
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

	IF @distid IS NULL
	BEGIN
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
	END

	SELECT * FROM @RaceAEOTable ORDER BY OrgType DESC, SchoolName;
END
