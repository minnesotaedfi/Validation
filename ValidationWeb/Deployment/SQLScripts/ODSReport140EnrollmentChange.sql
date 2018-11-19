----------------------------------------------------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------------------------------------------
-- PROCEDURE:  Change of Enrollment  ------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------------------------------------------

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
		StudentMiddleName varchar(255),
		StudentBirthDate datetime
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
				INNER JOIN edfi.School currsch ON currsch.LocalEducationAgencyId = currdisteorg.EducationOrganizationId 
				LEFT OUTER JOIN edfi.EducationOrganization currschedorg ON currschedorg.EducationOrganizationId = currsch.SchoolId
				INNER JOIN edfi.StudentSchoolAssociation currssa ON currssa.SchoolId = currsch.SchoolId
				LEFT OUTER JOIN edfi.Descriptor currgld ON currssa.EntryGradeLevelDescriptorId = currgld.DescriptorId
				INNER JOIN edfi.Student st ON st.StudentUSI = currssa.StudentUSI
				INNER JOIN edfi.StudentSchoolAssociation pastssa ON pastssa.StudentUSI = st.StudentUSI AND pastssa.SchoolId != currsch.SchoolId
				LEFT OUTER JOIN edfi.Descriptor pastgld ON pastssa.EntryGradeLevelDescriptorId = pastgld.DescriptorId
				INNER JOIN edfi.School pastsch ON pastssa.SchoolId = pastsch.SchoolId
				LEFT OUTER JOIN edfi.EducationOrganization pastschedorg ON pastschedorg.EducationOrganizationId = pastsch.SchoolId
				INNER JOIN edfi.EducationOrganization pastdisteorg ON pastdisteorg.EducationOrganizationId = pastsch.LocalEducationAgencyId
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
				INNER JOIN edfi.School currsch ON currsch.LocalEducationAgencyId = currdisteorg.EducationOrganizationId 
				LEFT OUTER JOIN edfi.EducationOrganization currschedorg ON currschedorg.EducationOrganizationId = currsch.SchoolId
				INNER JOIN edfi.StudentSchoolAssociation currssa ON currssa.SchoolId = currsch.SchoolId
				LEFT OUTER JOIN edfi.Descriptor currgld ON currssa.EntryGradeLevelDescriptorId = currgld.DescriptorId
				INNER JOIN edfi.Student st ON st.StudentUSI = currssa.StudentUSI
				INNER JOIN edfi.StudentSchoolAssociation pastssa ON pastssa.StudentUSI = st.StudentUSI AND pastssa.SchoolId != currsch.SchoolId
				LEFT OUTER JOIN edfi.Descriptor pastgld ON pastssa.EntryGradeLevelDescriptorId = pastgld.DescriptorId
				INNER JOIN edfi.School pastsch ON pastssa.SchoolId = pastsch.SchoolId
				LEFT OUTER JOIN edfi.EducationOrganization pastschedorg ON pastschedorg.EducationOrganizationId = pastsch.SchoolId
				INNER JOIN edfi.EducationOrganization pastdisteorg ON pastdisteorg.EducationOrganizationId = pastsch.SchoolId
			WHERE @distid = pastdisteorg.EducationOrganizationId
				AND currssa.EntryDate IS NOT NULL 
				AND currssa.EntryDate > @ThirtyDaysAgo
				AND pastssa.ExitWithdrawDate > @ThirtyDaysAgo
		;
	END

	SELECT * FROM @ChangeOfEnrollmentTable ORDER BY StudentLastName, StudentFirstName, StudentMiddleName;

END
