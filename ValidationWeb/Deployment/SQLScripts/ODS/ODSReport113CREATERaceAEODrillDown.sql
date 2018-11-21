CREATE PROCEDURE [rules].[RaceAEOStudentDetailsReport]
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
				edfi.EducationOrganization eorgdist 
				LEFT OUTER JOIN edfi.School sch ON sch.LocalEducationAgencyId  = eorgdist.EducationOrganizationId
				INNER JOIN edfi.StudentSchoolAssociation ssa ON ssa.SchoolId = sch.SchoolId
				LEFT OUTER JOIN edfi.Student s ON s.StudentUSI = ssa.StudentUSI
			WHERE sch.SchoolId = @schoolid
			;
		END
		IF @columnIndex = 1
		BEGIN
			INSERT INTO @StudentDetailsTable(IdNumber)
			SELECT DISTINCT s.StudentUniqueId
			FROM 
				edfi.EducationOrganization eorgdist 
				LEFT OUTER JOIN edfi.School sch ON sch.LocalEducationAgencyId  = eorgdist.EducationOrganizationId
				INNER JOIN edfi.StudentEducationOrganizationAssociation seoa ON seoa.EducationOrganizationId = sch.SchoolId
				LEFT OUTER JOIN edfi.Student s ON seoa.StudentUSI = s.StudentUSI
			WHERE sch.SchoolId = @schoolid
			;
		END
		IF @columnIndex = 2
		BEGIN
			INSERT INTO @StudentDetailsTable(IdNumber)
			SELECT DISTINCT s.StudentUniqueId
			FROM 
				edfi.EducationOrganization eorgdist 
				LEFT OUTER JOIN edfi.School sch ON sch.LocalEducationAgencyId  = eorgdist.EducationOrganizationId
				LEFT OUTER JOIN edfi.StudentEducationOrganizationAssociation seoa ON seoa.EducationOrganizationId = sch.SchoolId
				LEFT OUTER JOIN edfi.Student s ON seoa.StudentUSI = s.StudentUSI
				INNER JOIN extension.StudentEducationOrganizationAssociationRace seoar ON seoar.StudentUSI = s.StudentUSI
			WHERE sch.SchoolId = @schoolid
			;
		END
		IF @columnIndex = 3
		BEGIN
			INSERT INTO @StudentDetailsTable(IdNumber)
			SELECT DISTINCT s.StudentUniqueId
			FROM 
				edfi.EducationOrganization eorgdist 
				LEFT OUTER JOIN edfi.School sch ON sch.LocalEducationAgencyId  = eorgdist.EducationOrganizationId
				LEFT OUTER JOIN edfi.StudentEducationOrganizationAssociation seoa ON seoa.EducationOrganizationId = sch.SchoolId
				LEFT OUTER JOIN edfi.Student s ON seoa.StudentUSI = s.StudentUSI
				INNER JOIN extension.StudentEducationOrganizationAssociationAncestryEthnicOrigin seoaeo ON seoaeo.StudentUSI = s.StudentUSI
			WHERE sch.SchoolId = @schoolid
		END
	END
	
	IF @distid IS NOT NULL
	BEGIN
		----------------------------------------------------------------------------------------------------------------------------------------------------------------------
		-- DISTRICT LEVEL ----------------------------------------------------------------------------------------------------------------------------------------------------
		----------------------------------------------------------------------------------------------------------------------------------------------------------------------
		IF @columnIndex = 0
		BEGIN
			INSERT INTO @StudentDetailsTable(IdNumber)
				SELECT DISTINCT s.StudentUniqueId
				FROM 
					edfi.EducationOrganization eorgdist 
					LEFT OUTER JOIN edfi.School sch ON sch.LocalEducationAgencyId  = eorgdist.EducationOrganizationId
					INNER JOIN edfi.StudentSchoolAssociation ssa ON ssa.SchoolId = sch.SchoolId
					LEFT OUTER JOIN edfi.Student s ON s.StudentUSI = ssa.StudentUSI
				WHERE @distid = eorgdist.EducationOrganizationId
			;
		END
		IF @columnIndex = 1
		BEGIN
			INSERT INTO @StudentDetailsTable(IdNumber)
				SELECT s.StudentUniqueId
				FROM 
					edfi.EducationOrganization eorgdist 
					LEFT OUTER JOIN edfi.School sch ON sch.LocalEducationAgencyId  = eorgdist.EducationOrganizationId
					INNER JOIN edfi.StudentEducationOrganizationAssociation seoa ON seoa.EducationOrganizationId = sch.SchoolId
					LEFT OUTER JOIN edfi.Student s ON seoa.StudentUSI = s.StudentUSI
				WHERE @distid = eorgdist.EducationOrganizationId
			;
		END
		IF @columnIndex = 2
		BEGIN
			INSERT INTO @StudentDetailsTable(IdNumber)
				SELECT s.StudentUniqueId
				FROM 
					edfi.EducationOrganization eorgdist 
					LEFT OUTER JOIN edfi.School sch ON sch.LocalEducationAgencyId  = eorgdist.EducationOrganizationId
					LEFT OUTER JOIN edfi.StudentEducationOrganizationAssociation seoa ON seoa.EducationOrganizationId = sch.SchoolId
					LEFT OUTER JOIN edfi.Student s ON seoa.StudentUSI = s.StudentUSI
					INNER JOIN extension.StudentEducationOrganizationAssociationRace seoar ON seoar.StudentUSI = s.StudentUSI
				WHERE @distid = eorgdist.EducationOrganizationId
			;
		END
		IF @columnIndex = 3
		BEGIN
			INSERT INTO @StudentDetailsTable(IdNumber)
				SELECT s.StudentUniqueId
				FROM 
					edfi.EducationOrganization eorgdist 
					LEFT OUTER JOIN edfi.School sch ON sch.LocalEducationAgencyId  = eorgdist.EducationOrganizationId
					LEFT OUTER JOIN edfi.StudentEducationOrganizationAssociation seoa ON seoa.EducationOrganizationId = sch.SchoolId
					LEFT OUTER JOIN edfi.Student s ON seoa.StudentUSI = s.StudentUSI
					INNER JOIN extension.StudentEducationOrganizationAssociationAncestryEthnicOrigin seoaeo ON seoaeo.StudentUSI = s.StudentUSI
				WHERE @distid = eorgdist.EducationOrganizationId
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
					edfi.EducationOrganization eorgdist 
					LEFT OUTER JOIN edfi.School sch ON sch.LocalEducationAgencyId  = eorgdist.EducationOrganizationId
					INNER JOIN edfi.StudentSchoolAssociation ssa ON ssa.SchoolId = sch.SchoolId
					LEFT OUTER JOIN edfi.Student s ON s.StudentUSI = ssa.StudentUSI
			;
		END
		IF @columnIndex = 1
		BEGIN
			INSERT INTO @StudentDetailsTable(IdNumber)
				SELECT s.StudentUniqueId
				FROM 
					edfi.EducationOrganization eorgdist 
					LEFT OUTER JOIN edfi.School sch ON sch.LocalEducationAgencyId  = eorgdist.EducationOrganizationId
					INNER JOIN edfi.StudentEducationOrganizationAssociation seoa ON seoa.EducationOrganizationId = sch.SchoolId
					LEFT OUTER JOIN edfi.Student s ON seoa.StudentUSI = s.StudentUSI
			;
		END
		IF @columnIndex = 2
		BEGIN
			INSERT INTO @StudentDetailsTable(IdNumber)
				SELECT s.StudentUniqueId
				FROM 
					edfi.EducationOrganization eorgdist 
					LEFT OUTER JOIN edfi.School sch ON sch.LocalEducationAgencyId  = eorgdist.EducationOrganizationId
					LEFT OUTER JOIN edfi.StudentEducationOrganizationAssociation seoa ON seoa.EducationOrganizationId = sch.SchoolId
					LEFT OUTER JOIN edfi.Student s ON seoa.StudentUSI = s.StudentUSI
					INNER JOIN extension.StudentEducationOrganizationAssociationRace seoar ON seoar.StudentUSI = s.StudentUSI
			;
		END
		IF @columnIndex = 3
		BEGIN
			INSERT INTO @StudentDetailsTable(IdNumber)
				SELECT s.StudentUniqueId
				FROM 
					edfi.EducationOrganization eorgdist 
					LEFT OUTER JOIN edfi.School sch ON sch.LocalEducationAgencyId  = eorgdist.EducationOrganizationId
					LEFT OUTER JOIN edfi.StudentEducationOrganizationAssociation seoa ON seoa.EducationOrganizationId = sch.SchoolId
					LEFT OUTER JOIN edfi.Student s ON seoa.StudentUSI = s.StudentUSI
					INNER JOIN extension.StudentEducationOrganizationAssociationAncestryEthnicOrigin seoaeo ON seoaeo.StudentUSI = s.StudentUSI
			;
		END
		;
	END

	SELECT DISTINCT * FROM rules.GetStudentEnrollmentDetails(@StudentDetailsTable) enr ORDER BY enr.StudentId;
END