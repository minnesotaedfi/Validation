CREATE PROCEDURE [rules].ResidentsEnrolledElsewhereStudentDetailsReport
	@distid int
AS

BEGIN
	DECLARE @ResidentsEnrolledElsewhereStudentDetailsTable rules.IdStringTable;


	----------------------------------------------------------------------------------------------------------------------------------------------------------------------
	-- DISTRICT LEVEL ----------------------------------------------------------------------------------------------------------------------------------------------------
	----------------------------------------------------------------------------------------------------------------------------------------------------------------------

	-- Load School information, rolling up to District - if District is NULL, then ALL Districts in the State.
	-- REGARDLESS of whether they have any enrollments or demographics.
	INSERT INTO @ResidentsEnrolledElsewhereStudentDetailsTable(IdNumber)
		SELECT 
			DISTINCT st.StudentUniqueId
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
	;

	IF @distid IS NULL
	BEGIN
		----------------------------------------------------------------------------------------------------------------------------------------------------------------------
		-- STATE LEVEL ----------------------------------------------------------------------------------------------------------------------------------------------------
		----------------------------------------------------------------------------------------------------------------------------------------------------------------------

		-- Load School information, rolling up to District - if District is NULL, then ALL Districts in the State.
		-- REGARDLESS of whether they have any enrollments or demographics.
		INSERT INTO @ResidentsEnrolledElsewhereStudentDetailsTable(IdNumber)
			SELECT 
				DISTINCT st.StudentUniqueId
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
			;
	END

	SELECT DISTINCT * FROM rules.GetStudentEnrollmentDetails(@ResidentsEnrolledElsewhereStudentDetailsTable) enr ORDER BY enr.StudentId;
END