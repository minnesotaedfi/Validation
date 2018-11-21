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