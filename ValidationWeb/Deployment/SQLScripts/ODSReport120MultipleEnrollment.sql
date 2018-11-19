----------------------------------------------------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------------------------------------------
-- PROCEDURE:  Multiple Enrollments  ------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------------------------------------------

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

	IF @distid IS NULL
	BEGIN
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
	END

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

----------------------------------------------------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------------------------------------------
-- PROCEDURE:  Multiple Enrollments StudentDetails --------------------------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------------------------------------------

IF OBJECT_ID ( 'rules.MultipleEnrollmentStudentDetailsReport', 'P' ) IS NOT NULL   
    DROP PROCEDURE rules.MultipleEnrollmentStudentDetailsReport;  
GO  

CREATE PROCEDURE [rules].[MultipleEnrollmentStudentDetailsReport]
	@schoolid int,
	@distid int,
	@columnIndex int
AS

BEGIN
	DECLARE @StudentDetailsTable rules.IdStringTable;

	IF @schoolid IS NOT NULL
	BEGIN
		----------------------------------------------------------------------------------------------------------------------------------------------------------------------
		-- SCHOOL LEVEL ------------------------------------------------------------------------------------------------------------------------------------------------------
		----------------------------------------------------------------------------------------------------------------------------------------------------------------------
		IF @columnIndex = 0
		BEGIN
			INSERT INTO @StudentDetailsTable(IdNumber)
			SELECT DISTINCT s.StudentUniqueId
			FROM 
				edfi.School sch 
				LEFT OUTER JOIN edfi.EducationOrganization eorg ON eorg.EducationOrganizationId = sch.SchoolId 
				LEFT OUTER JOIN edfi.LocalEducationAgency lea ON lea.LocalEducationAgencyId = sch.LocalEducationAgencyId 
				LEFT OUTER JOIN edfi.EducationOrganization eorgdist ON eorgdist.EducationOrganizationId = lea.LocalEducationAgencyId 
				LEFT OUTER JOIN edfi.StudentSchoolAssociation ssa ON ssa.SchoolId = sch.SchoolId
				LEFT OUTER JOIN edfi.Student s ON s.StudentUSI = ssa.StudentUSI
			WHERE @schoolid = sch.SchoolId
		;
		END
		IF @columnIndex = 1
		BEGIN
			INSERT INTO @StudentDetailsTable(IdNumber)
			SELECT DISTINCT s.StudentUniqueId
			FROM 
				edfi.School sch 
				LEFT OUTER JOIN edfi.EducationOrganization eorg ON eorg.EducationOrganizationId = sch.SchoolId 
				LEFT OUTER JOIN edfi.LocalEducationAgency lea ON lea.LocalEducationAgencyId = sch.LocalEducationAgencyId 
				LEFT OUTER JOIN edfi.EducationOrganization eorgdist ON eorgdist.EducationOrganizationId = lea.LocalEducationAgencyId 
				LEFT OUTER JOIN edfi.StudentSchoolAssociation ssa ON ssa.SchoolId = sch.SchoolId
				LEFT OUTER JOIN edfi.Student s ON s.StudentUSI = ssa.StudentUSI
			WHERE s.StudentUSI IN (SELECT ssa2.StudentUSI FROM edfi.StudentSchoolAssociation ssa2 WHERE ssa2.SchoolId = @schoolid) AND sch.SchoolId != @schoolid
			GROUP BY s.StudentUniqueId
			HAVING COUNT(1) > 0
			;
		END
		IF @columnIndex = 2
		BEGIN
			INSERT INTO @StudentDetailsTable(IdNumber)
			SELECT DISTINCT s.StudentUniqueId
			FROM 
				edfi.School sch 
				LEFT OUTER JOIN edfi.EducationOrganization eorg ON eorg.EducationOrganizationId = sch.SchoolId 
				LEFT OUTER JOIN edfi.LocalEducationAgency lea ON lea.LocalEducationAgencyId = sch.LocalEducationAgencyId 
				LEFT OUTER JOIN edfi.EducationOrganization eorgdist ON eorgdist.EducationOrganizationId = lea.LocalEducationAgencyId 
				LEFT OUTER JOIN edfi.StudentSchoolAssociation ssa ON ssa.SchoolId = sch.SchoolId
				LEFT OUTER JOIN edfi.Student s ON s.StudentUSI = ssa.StudentUSI
			WHERE s.StudentUSI IN (SELECT ssa2.StudentUSI FROM edfi.StudentSchoolAssociation ssa2 INNER JOIN edfi.School sch2 ON sch2.SchoolId = ssa2.SchoolId WHERE sch2.SchoolId = @schoolid)
			GROUP BY s.StudentUniqueId
			HAVING COUNT(eorg.EducationOrganizationId) > 1
			;
		END
	END

	IF @distid IS NOT NULL
	BEGIN
		----------------------------------------------------------------------------------------------------------------------------------------------------------------------
		-- DISTRICT LEVEL ------------------------------------------------------------------------------------------------------------------------------------------------------
		----------------------------------------------------------------------------------------------------------------------------------------------------------------------
		IF @columnIndex = 0
		BEGIN
			INSERT INTO @StudentDetailsTable(IdNumber)
			SELECT DISTINCT s.StudentUniqueId
			FROM 
				edfi.School sch 
				LEFT OUTER JOIN edfi.EducationOrganization eorg ON eorg.EducationOrganizationId = sch.SchoolId 
				LEFT OUTER JOIN edfi.LocalEducationAgency lea ON lea.LocalEducationAgencyId = sch.LocalEducationAgencyId 
				LEFT OUTER JOIN edfi.EducationOrganization eorgdist ON eorgdist.EducationOrganizationId = lea.LocalEducationAgencyId 
				LEFT OUTER JOIN edfi.StudentSchoolAssociation ssa ON ssa.SchoolId = sch.SchoolId
				LEFT OUTER JOIN edfi.Student s ON s.StudentUSI = ssa.StudentUSI
			WHERE @distid = eorgdist.EducationOrganizationId
			;
		END
		IF @columnIndex = 1
		BEGIN
			INSERT INTO @StudentDetailsTable(IdNumber)
			SELECT DISTINCT s.StudentUniqueId
			FROM 
				edfi.School sch 
				LEFT OUTER JOIN edfi.EducationOrganization eorg ON eorg.EducationOrganizationId = sch.SchoolId 
				LEFT OUTER JOIN edfi.LocalEducationAgency lea ON lea.LocalEducationAgencyId = sch.LocalEducationAgencyId 
				LEFT OUTER JOIN edfi.EducationOrganization eorgdist ON eorgdist.EducationOrganizationId = lea.LocalEducationAgencyId 
				LEFT OUTER JOIN edfi.StudentSchoolAssociation ssa ON ssa.SchoolId = sch.SchoolId
				LEFT OUTER JOIN edfi.Student s ON s.StudentUSI = ssa.StudentUSI
			GROUP BY s.StudentUniqueId, eorgdist.EducationOrganizationId
			HAVING COUNT(ssa.SchoolId) > 1 AND eorgdist.EducationOrganizationId = @distid
			;
		END
		IF @columnIndex = 2
		BEGIN
			INSERT INTO @StudentDetailsTable(IdNumber)
			SELECT DISTINCT s.StudentUniqueId
			FROM 
				edfi.School sch 
				LEFT OUTER JOIN edfi.EducationOrganization eorg ON eorg.EducationOrganizationId = sch.SchoolId 
				LEFT OUTER JOIN edfi.LocalEducationAgency lea ON lea.LocalEducationAgencyId = sch.LocalEducationAgencyId 
				LEFT OUTER JOIN edfi.EducationOrganization eorgdist ON eorgdist.EducationOrganizationId = lea.LocalEducationAgencyId 
				LEFT OUTER JOIN edfi.StudentSchoolAssociation ssa ON ssa.SchoolId = sch.SchoolId
				LEFT OUTER JOIN edfi.Student s ON s.StudentUSI = ssa.StudentUSI
				LEFT OUTER JOIN edfi.EducationOrganization selecteddist ON selecteddist.EducationOrganizationId = @distid
			GROUP BY s.StudentUniqueId
			HAVING COUNT(eorgdist.EducationOrganizationId) > 1 AND COUNT(selecteddist.EducationOrganizationId) > 0
			;
		END
	END

	IF @distid IS NULL AND @schoolid IS NULL
	BEGIN
		----------------------------------------------------------------------------------------------------------------------------------------------------------------------
		-- STATE LEVEL ----------------------------------------------------------------------------------------------------------------------------------------------------
		----------------------------------------------------------------------------------------------------------------------------------------------------------------------
		IF @columnIndex = 0
		BEGIN
			INSERT INTO @StudentDetailsTable(IdNumber)
			SELECT DISTINCT s.StudentUniqueId
			FROM 
				edfi.School sch 
				LEFT OUTER JOIN edfi.EducationOrganization eorg ON eorg.EducationOrganizationId = sch.SchoolId 
				LEFT OUTER JOIN edfi.LocalEducationAgency lea ON lea.LocalEducationAgencyId = sch.LocalEducationAgencyId 
				LEFT OUTER JOIN edfi.EducationOrganization eorgdist ON eorgdist.EducationOrganizationId = lea.LocalEducationAgencyId 
				LEFT OUTER JOIN edfi.StudentSchoolAssociation ssa ON ssa.SchoolId = sch.SchoolId
				LEFT OUTER JOIN edfi.Student s ON s.StudentUSI = ssa.StudentUSI
			;
		END
		IF @columnIndex = 1
		BEGIN
			INSERT INTO @StudentDetailsTable(IdNumber)
			SELECT DISTINCT s.StudentUniqueId
			FROM 
				edfi.School sch 
				LEFT OUTER JOIN edfi.EducationOrganization eorg ON eorg.EducationOrganizationId = sch.SchoolId 
				LEFT OUTER JOIN edfi.LocalEducationAgency lea ON lea.LocalEducationAgencyId = sch.LocalEducationAgencyId 
				LEFT OUTER JOIN edfi.EducationOrganization eorgdist ON eorgdist.EducationOrganizationId = lea.LocalEducationAgencyId 
				LEFT OUTER JOIN edfi.StudentSchoolAssociation ssa ON ssa.SchoolId = sch.SchoolId
				LEFT OUTER JOIN edfi.Student s ON s.StudentUSI = ssa.StudentUSI
			GROUP BY s.StudentUniqueId, eorgdist.EducationOrganizationId
			HAVING COUNT(ssa.SchoolId) > 1 
			;
		END
		IF @columnIndex = 2
		BEGIN
			INSERT INTO @StudentDetailsTable(IdNumber)
			SELECT DISTINCT s.StudentUniqueId
			FROM 
				edfi.School sch 
				LEFT OUTER JOIN edfi.EducationOrganization eorg ON eorg.EducationOrganizationId = sch.SchoolId 
				LEFT OUTER JOIN edfi.LocalEducationAgency lea ON lea.LocalEducationAgencyId = sch.LocalEducationAgencyId 
				LEFT OUTER JOIN edfi.EducationOrganization eorgdist ON eorgdist.EducationOrganizationId = lea.LocalEducationAgencyId 
				LEFT OUTER JOIN edfi.StudentSchoolAssociation ssa ON ssa.SchoolId = sch.SchoolId
				LEFT OUTER JOIN edfi.Student s ON s.StudentUSI = ssa.StudentUSI
			GROUP BY s.StudentUniqueId
			HAVING COUNT(eorgdist.EducationOrganizationId) > 1 
			;
		END
	END

	SELECT DISTINCT * FROM rules.GetStudentEnrollmentDetails(@StudentDetailsTable) enr ORDER BY enr.StudentId;
END