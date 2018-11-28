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
		SELECT DISTINCT
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

	IF @distid IS NULL
	BEGIN

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
			SELECT DISTINCT
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
	END

	SELECT * FROM @ResidentsEnrolledElsewhereTable ORDER BY DistrictOfEnrollmentName;
END
