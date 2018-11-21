CREATE PROCEDURE [rules].[StudentProgramsStudentDetailsReport]
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
			SELECT DISTINCT st.StudentUniqueId
			FROM 
				edfi.School sch 
					LEFT OUTER JOIN edfi.EducationOrganization eorg ON eorg.EducationOrganizationId = sch.SchoolId 
					LEFT OUTER JOIN edfi.LocalEducationAgency lea ON lea.LocalEducationAgencyId = sch.LocalEducationAgencyId 
					LEFT OUTER JOIN edfi.EducationOrganization eorgdist ON eorgdist.EducationOrganizationId = lea.LocalEducationAgencyId
					LEFT OUTER JOIN extension.StudentEducationOrganizationAssociationStudentCharacteristic seoasc ON seoasc.EducationOrganizationId = eorg.EducationOrganizationId
					LEFT OUTER JOIN edfi.Student st ON st.StudentUSI = seoasc.StudentUSI
					LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = seoasc.StudentCharacteristicDescriptorId
			WHERE eorg.EducationOrganizationId = eorg.EducationOrganizationId AND d.CodeValue = 'Active Duty Parent (ADP)'
		END
		IF @columnIndex = 3
		BEGIN
			INSERT INTO @StudentDetailsTable(IdNumber)
			SELECT DISTINCT st.StudentUniqueId
			FROM 
				edfi.School sch 
					LEFT OUTER JOIN edfi.EducationOrganization eorg ON eorg.EducationOrganizationId = sch.SchoolId 
					LEFT OUTER JOIN edfi.LocalEducationAgency lea ON lea.LocalEducationAgencyId = sch.LocalEducationAgencyId 
					LEFT OUTER JOIN edfi.EducationOrganization eorgdist ON eorgdist.EducationOrganizationId = lea.LocalEducationAgencyId
					LEFT OUTER JOIN extension.StudentEducationOrganizationAssociationStudentCharacteristic seoasc ON seoasc.EducationOrganizationId = eorg.EducationOrganizationId
					LEFT OUTER JOIN edfi.Student st ON st.StudentUSI = seoasc.StudentUSI
					LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = seoasc.StudentCharacteristicDescriptorId
			WHERE eorg.EducationOrganizationId = eorg.EducationOrganizationId AND d.CodeValue = 'American Indian - Alaskan Native (Minnesota)'
		END
		IF @columnIndex = 4
		BEGIN
			INSERT INTO @StudentDetailsTable(IdNumber)
			SELECT DISTINCT st.StudentUniqueId
			FROM 
				edfi.School sch 
					LEFT OUTER JOIN edfi.EducationOrganization eorg ON eorg.EducationOrganizationId = sch.SchoolId 
					LEFT OUTER JOIN edfi.LocalEducationAgency lea ON lea.LocalEducationAgencyId = sch.LocalEducationAgencyId 
					LEFT OUTER JOIN edfi.EducationOrganization eorgdist ON eorgdist.EducationOrganizationId = lea.LocalEducationAgencyId
					LEFT OUTER JOIN extension.StudentEducationOrganizationAssociationStudentCharacteristic seoasc ON seoasc.EducationOrganizationId = eorg.EducationOrganizationId
					LEFT OUTER JOIN edfi.Student st ON st.StudentUSI = seoasc.StudentUSI
					LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = seoasc.StudentCharacteristicDescriptorId
			WHERE eorg.EducationOrganizationId = eorg.EducationOrganizationId AND d.CodeValue = 'Migrant'
		END
		IF @columnIndex = 5
		BEGIN
			INSERT INTO @StudentDetailsTable(IdNumber)
			SELECT DISTINCT st.StudentUniqueId
			FROM 
				edfi.School sch 
					LEFT OUTER JOIN edfi.EducationOrganization eorg ON eorg.EducationOrganizationId = sch.SchoolId 
					LEFT OUTER JOIN edfi.LocalEducationAgency lea ON lea.LocalEducationAgencyId = sch.LocalEducationAgencyId 
					LEFT OUTER JOIN edfi.EducationOrganization eorgdist ON eorgdist.EducationOrganizationId = lea.LocalEducationAgencyId
					LEFT OUTER JOIN extension.StudentEducationOrganizationAssociationStudentCharacteristic seoasc ON seoasc.EducationOrganizationId = eorg.EducationOrganizationId
					LEFT OUTER JOIN edfi.Student st ON st.StudentUSI = seoasc.StudentUSI
					LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = seoasc.StudentCharacteristicDescriptorId
			WHERE eorg.EducationOrganizationId = eorg.EducationOrganizationId AND d.CodeValue = 'Homeless'
		END
		IF @columnIndex = 6
		BEGIN
			INSERT INTO @StudentDetailsTable(IdNumber)
			SELECT DISTINCT st.StudentUniqueId
			FROM 
				edfi.School sch 
					LEFT OUTER JOIN edfi.EducationOrganization eorg ON eorg.EducationOrganizationId = sch.SchoolId 
					LEFT OUTER JOIN edfi.LocalEducationAgency lea ON lea.LocalEducationAgencyId = sch.LocalEducationAgencyId 
					LEFT OUTER JOIN edfi.EducationOrganization eorgdist ON eorgdist.EducationOrganizationId = lea.LocalEducationAgencyId
					LEFT OUTER JOIN extension.StudentEducationOrganizationAssociationStudentCharacteristic seoasc ON seoasc.EducationOrganizationId = eorg.EducationOrganizationId
					LEFT OUTER JOIN edfi.Student st ON st.StudentUSI = seoasc.StudentUSI
					LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = seoasc.StudentCharacteristicDescriptorId
			WHERE eorg.EducationOrganizationId = eorg.EducationOrganizationId AND d.CodeValue = 'Immigrant'
		END
		IF @columnIndex = 7
		BEGIN
			INSERT INTO @StudentDetailsTable(IdNumber)
			SELECT DISTINCT st.StudentUniqueId
			FROM 
				edfi.School sch 
					LEFT OUTER JOIN edfi.EducationOrganization eorg ON eorg.EducationOrganizationId = sch.SchoolId 
					LEFT OUTER JOIN edfi.LocalEducationAgency lea ON lea.LocalEducationAgencyId = sch.LocalEducationAgencyId 
					LEFT OUTER JOIN edfi.EducationOrganization eorgdist ON eorgdist.EducationOrganizationId = lea.LocalEducationAgencyId
					LEFT OUTER JOIN extension.StudentEducationOrganizationAssociationStudentCharacteristic seoasc ON seoasc.EducationOrganizationId = eorg.EducationOrganizationId
					LEFT OUTER JOIN edfi.Student st ON st.StudentUSI = seoasc.StudentUSI
					LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = seoasc.StudentCharacteristicDescriptorId
			WHERE eorg.EducationOrganizationId = eorg.EducationOrganizationId AND d.CodeValue = 'Recently Arrived English Learner (RAEL)'
		END
		IF @columnIndex = 8
		BEGIN
			INSERT INTO @StudentDetailsTable(IdNumber)
			SELECT DISTINCT st.StudentUniqueId
			FROM 
				edfi.School sch 
					LEFT OUTER JOIN edfi.EducationOrganization eorg ON eorg.EducationOrganizationId = sch.SchoolId 
					LEFT OUTER JOIN edfi.LocalEducationAgency lea ON lea.LocalEducationAgencyId = sch.LocalEducationAgencyId 
					LEFT OUTER JOIN edfi.EducationOrganization eorgdist ON eorgdist.EducationOrganizationId = lea.LocalEducationAgencyId
					LEFT OUTER JOIN extension.StudentEducationOrganizationAssociationStudentCharacteristic seoasc ON seoasc.EducationOrganizationId = eorg.EducationOrganizationId
					LEFT OUTER JOIN edfi.Student st ON st.StudentUSI = seoasc.StudentUSI
					LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = seoasc.StudentCharacteristicDescriptorId
			WHERE eorg.EducationOrganizationId = eorg.EducationOrganizationId AND d.CodeValue = 'SLIFE'
		END
		IF @columnIndex = 9
		BEGIN
			INSERT INTO @StudentDetailsTable(IdNumber)
			SELECT DISTINCT st.StudentUniqueId
			FROM 
				edfi.School sch 
					LEFT OUTER JOIN edfi.EducationOrganization eorg ON eorg.EducationOrganizationId = sch.SchoolId 
					LEFT OUTER JOIN edfi.LocalEducationAgency lea ON lea.LocalEducationAgencyId = sch.LocalEducationAgencyId 
					LEFT OUTER JOIN edfi.EducationOrganization eorgdist ON eorgdist.EducationOrganizationId = lea.LocalEducationAgencyId
					LEFT OUTER JOIN extension.StudentSchoolAssociationStudentProgramParticipation ssaspp ON ssaspp.SchoolId = sch.SchoolId 
					LEFT OUTER JOIN edfi.Student st ON st.StudentUSI = ssaspp.StudentUSI
					LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = ssaspp.ProgramCategoryDescriptorId
			WHERE eorg.EducationOrganizationId = eorg.EducationOrganizationId AND d.CodeValue LIKE 'Limited%'
		END
		IF @columnIndex = 10
		BEGIN
			INSERT INTO @StudentDetailsTable(IdNumber)
			SELECT DISTINCT st.StudentUniqueId
			FROM 
				edfi.School sch 
					LEFT OUTER JOIN edfi.EducationOrganization eorg ON eorg.EducationOrganizationId = sch.SchoolId 
					LEFT OUTER JOIN edfi.LocalEducationAgency lea ON lea.LocalEducationAgencyId = sch.LocalEducationAgencyId 
					LEFT OUTER JOIN edfi.EducationOrganization eorgdist ON eorgdist.EducationOrganizationId = lea.LocalEducationAgencyId
					LEFT OUTER JOIN extension.StudentSchoolAssociationStudentProgramParticipation ssaspp ON ssaspp.SchoolId = sch.SchoolId 
					LEFT OUTER JOIN edfi.Student st ON st.StudentUSI = ssaspp.StudentUSI
					LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = ssaspp.ProgramCategoryDescriptorId
			WHERE eorg.EducationOrganizationId = eorg.EducationOrganizationId AND d.CodeValue = 'English Learner Served'
		END
		IF @columnIndex = 11
		BEGIN
			INSERT INTO @StudentDetailsTable(IdNumber)
			SELECT DISTINCT st.StudentUniqueId
			FROM 
				edfi.School sch 
					LEFT OUTER JOIN edfi.EducationOrganization eorg ON eorg.EducationOrganizationId = sch.SchoolId 
					LEFT OUTER JOIN edfi.LocalEducationAgency lea ON lea.LocalEducationAgencyId = sch.LocalEducationAgencyId 
					LEFT OUTER JOIN edfi.EducationOrganization eorgdist ON eorgdist.EducationOrganizationId = lea.LocalEducationAgencyId
					LEFT OUTER JOIN extension.StudentSchoolAssociationStudentProgramParticipation ssaspp ON ssaspp.SchoolId = sch.SchoolId 
					LEFT OUTER JOIN edfi.Student st ON st.StudentUSI = ssaspp.StudentUSI
					LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = ssaspp.ProgramCategoryDescriptorId
			WHERE eorg.EducationOrganizationId = eorg.EducationOrganizationId AND d.CodeValue = 'Independent Study'
		END
		IF @columnIndex = 12
		BEGIN
			INSERT INTO @StudentDetailsTable(IdNumber)
			SELECT DISTINCT st.StudentUniqueId
			FROM 
				edfi.School sch 
					LEFT OUTER JOIN edfi.EducationOrganization eorg ON eorg.EducationOrganizationId = sch.SchoolId 
					LEFT OUTER JOIN edfi.LocalEducationAgency lea ON lea.LocalEducationAgencyId = sch.LocalEducationAgencyId 
					LEFT OUTER JOIN edfi.EducationOrganization eorgdist ON eorgdist.EducationOrganizationId = lea.LocalEducationAgencyId
					LEFT OUTER JOIN extension.StudentSchoolAssociationStudentProgramParticipation ssaspp ON ssaspp.SchoolId = sch.SchoolId 
					LEFT OUTER JOIN edfi.Student st ON st.StudentUSI = ssaspp.StudentUSI
					LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = ssaspp.ProgramCategoryDescriptorId
			WHERE eorg.EducationOrganizationId = eorg.EducationOrganizationId AND d.CodeValue = 'Section 504 Placement'
		END
		IF @columnIndex = 13
		BEGIN
			INSERT INTO @StudentDetailsTable(IdNumber)
			SELECT DISTINCT st.StudentUniqueId
			FROM 
				edfi.School sch 
					LEFT OUTER JOIN edfi.EducationOrganization eorg ON eorg.EducationOrganizationId = sch.SchoolId 
					LEFT OUTER JOIN edfi.LocalEducationAgency lea ON lea.LocalEducationAgencyId = sch.LocalEducationAgencyId 
					LEFT OUTER JOIN edfi.EducationOrganization eorgdist ON eorgdist.EducationOrganizationId = lea.LocalEducationAgencyId
					LEFT OUTER JOIN extension.StudentSchoolAssociationStudentProgramParticipation ssaspp ON ssaspp.SchoolId = sch.SchoolId 
					LEFT OUTER JOIN edfi.Student st ON st.StudentUSI = ssaspp.StudentUSI
					LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = ssaspp.ProgramCategoryDescriptorId
			WHERE eorg.EducationOrganizationId = eorg.EducationOrganizationId AND d.CodeValue = 'Title I Part A'
		END
		IF @columnIndex = 14
		BEGIN
			INSERT INTO @StudentDetailsTable(IdNumber)
			SELECT DISTINCT st.StudentUniqueId
			FROM 
				edfi.School sch 
					LEFT OUTER JOIN edfi.EducationOrganization eorg ON eorg.EducationOrganizationId = sch.SchoolId 
					LEFT OUTER JOIN edfi.LocalEducationAgency lea ON lea.LocalEducationAgencyId = sch.LocalEducationAgencyId 
					LEFT OUTER JOIN edfi.EducationOrganization eorgdist ON eorgdist.EducationOrganizationId = lea.LocalEducationAgencyId
					LEFT OUTER JOIN edfi.StudentEducationOrganizationAssociation seoa ON seoa.EducationOrganizationId = sch.SchoolId 
					LEFT OUTER JOIN edfi.Student st ON st.StudentUSI = seoa.StudentUSI
					LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = st.SchoolFoodServicesEligibilityDescriptorId
			WHERE seoa.EducationOrganizationId = eorg.EducationOrganizationId AND (d.ShortDescription LIKE 'reduced' OR d.ShortDescription LIKE 'free')
		END
	END

	If @distid IS NOT NULL
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
			WHERE eorgdist.EducationOrganizationId = @distid
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
			WHERE eorgdist.EducationOrganizationId = @distid
			;
		END
		IF @columnIndex = 2
		BEGIN
			INSERT INTO @StudentDetailsTable(IdNumber)
			SELECT DISTINCT st.StudentUniqueId
			FROM 
				edfi.School sch 
					LEFT OUTER JOIN edfi.EducationOrganization eorg ON eorg.EducationOrganizationId = sch.SchoolId 
					LEFT OUTER JOIN edfi.LocalEducationAgency lea ON lea.LocalEducationAgencyId = sch.LocalEducationAgencyId 
					LEFT OUTER JOIN edfi.EducationOrganization eorgdist ON eorgdist.EducationOrganizationId = lea.LocalEducationAgencyId
					LEFT OUTER JOIN extension.StudentEducationOrganizationAssociationStudentCharacteristic seoasc ON seoasc.EducationOrganizationId = eorg.EducationOrganizationId
					LEFT OUTER JOIN edfi.Student st ON st.StudentUSI = seoasc.StudentUSI
					LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = seoasc.StudentCharacteristicDescriptorId
			WHERE eorgdist.EducationOrganizationId = @distid AND d.CodeValue = 'Active Duty Parent (ADP)'
		END
		IF @columnIndex = 3
		BEGIN
			INSERT INTO @StudentDetailsTable(IdNumber)
			SELECT DISTINCT st.StudentUniqueId
			FROM 
				edfi.School sch 
					LEFT OUTER JOIN edfi.EducationOrganization eorg ON eorg.EducationOrganizationId = sch.SchoolId 
					LEFT OUTER JOIN edfi.LocalEducationAgency lea ON lea.LocalEducationAgencyId = sch.LocalEducationAgencyId 
					LEFT OUTER JOIN edfi.EducationOrganization eorgdist ON eorgdist.EducationOrganizationId = lea.LocalEducationAgencyId
					LEFT OUTER JOIN extension.StudentEducationOrganizationAssociationStudentCharacteristic seoasc ON seoasc.EducationOrganizationId = eorg.EducationOrganizationId
					LEFT OUTER JOIN edfi.Student st ON st.StudentUSI = seoasc.StudentUSI
					LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = seoasc.StudentCharacteristicDescriptorId
			WHERE eorgdist.EducationOrganizationId = @distid AND d.CodeValue = 'American Indian - Alaskan Native (Minnesota)'
		END
		IF @columnIndex = 4
		BEGIN
			INSERT INTO @StudentDetailsTable(IdNumber)
			SELECT DISTINCT st.StudentUniqueId
			FROM 
				edfi.School sch 
					LEFT OUTER JOIN edfi.EducationOrganization eorg ON eorg.EducationOrganizationId = sch.SchoolId 
					LEFT OUTER JOIN edfi.LocalEducationAgency lea ON lea.LocalEducationAgencyId = sch.LocalEducationAgencyId 
					LEFT OUTER JOIN edfi.EducationOrganization eorgdist ON eorgdist.EducationOrganizationId = lea.LocalEducationAgencyId
					LEFT OUTER JOIN extension.StudentEducationOrganizationAssociationStudentCharacteristic seoasc ON seoasc.EducationOrganizationId = eorg.EducationOrganizationId
					LEFT OUTER JOIN edfi.Student st ON st.StudentUSI = seoasc.StudentUSI
					LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = seoasc.StudentCharacteristicDescriptorId
			WHERE eorgdist.EducationOrganizationId = @distid AND d.CodeValue = 'Migrant'
		END
		IF @columnIndex = 5
		BEGIN
			INSERT INTO @StudentDetailsTable(IdNumber)
			SELECT DISTINCT st.StudentUniqueId
			FROM 
				edfi.School sch 
					LEFT OUTER JOIN edfi.EducationOrganization eorg ON eorg.EducationOrganizationId = sch.SchoolId 
					LEFT OUTER JOIN edfi.LocalEducationAgency lea ON lea.LocalEducationAgencyId = sch.LocalEducationAgencyId 
					LEFT OUTER JOIN edfi.EducationOrganization eorgdist ON eorgdist.EducationOrganizationId = lea.LocalEducationAgencyId
					LEFT OUTER JOIN extension.StudentEducationOrganizationAssociationStudentCharacteristic seoasc ON seoasc.EducationOrganizationId = eorg.EducationOrganizationId
					LEFT OUTER JOIN edfi.Student st ON st.StudentUSI = seoasc.StudentUSI
					LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = seoasc.StudentCharacteristicDescriptorId
			WHERE eorgdist.EducationOrganizationId = @distid AND d.CodeValue = 'Homeless'
		END
		IF @columnIndex = 6
		BEGIN
			INSERT INTO @StudentDetailsTable(IdNumber)
			SELECT DISTINCT st.StudentUniqueId
			FROM 
				edfi.School sch 
					LEFT OUTER JOIN edfi.EducationOrganization eorg ON eorg.EducationOrganizationId = sch.SchoolId 
					LEFT OUTER JOIN edfi.LocalEducationAgency lea ON lea.LocalEducationAgencyId = sch.LocalEducationAgencyId 
					LEFT OUTER JOIN edfi.EducationOrganization eorgdist ON eorgdist.EducationOrganizationId = lea.LocalEducationAgencyId
					LEFT OUTER JOIN extension.StudentEducationOrganizationAssociationStudentCharacteristic seoasc ON seoasc.EducationOrganizationId = eorg.EducationOrganizationId
					LEFT OUTER JOIN edfi.Student st ON st.StudentUSI = seoasc.StudentUSI
					LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = seoasc.StudentCharacteristicDescriptorId
			WHERE eorgdist.EducationOrganizationId = @distid AND d.CodeValue = 'Immigrant'
		END
		IF @columnIndex = 7
		BEGIN
			INSERT INTO @StudentDetailsTable(IdNumber)
			SELECT DISTINCT st.StudentUniqueId
			FROM 
				edfi.School sch 
					LEFT OUTER JOIN edfi.EducationOrganization eorg ON eorg.EducationOrganizationId = sch.SchoolId 
					LEFT OUTER JOIN edfi.LocalEducationAgency lea ON lea.LocalEducationAgencyId = sch.LocalEducationAgencyId 
					LEFT OUTER JOIN edfi.EducationOrganization eorgdist ON eorgdist.EducationOrganizationId = lea.LocalEducationAgencyId
					LEFT OUTER JOIN extension.StudentEducationOrganizationAssociationStudentCharacteristic seoasc ON seoasc.EducationOrganizationId = eorg.EducationOrganizationId
					LEFT OUTER JOIN edfi.Student st ON st.StudentUSI = seoasc.StudentUSI
					LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = seoasc.StudentCharacteristicDescriptorId
			WHERE eorgdist.EducationOrganizationId = @distid AND d.CodeValue = 'Recently Arrived English Learner (RAEL)'
		END
		IF @columnIndex = 8
		BEGIN
			INSERT INTO @StudentDetailsTable(IdNumber)
			SELECT DISTINCT st.StudentUniqueId
			FROM 
				edfi.School sch 
					LEFT OUTER JOIN edfi.EducationOrganization eorg ON eorg.EducationOrganizationId = sch.SchoolId 
					LEFT OUTER JOIN edfi.LocalEducationAgency lea ON lea.LocalEducationAgencyId = sch.LocalEducationAgencyId 
					LEFT OUTER JOIN edfi.EducationOrganization eorgdist ON eorgdist.EducationOrganizationId = lea.LocalEducationAgencyId
					LEFT OUTER JOIN extension.StudentEducationOrganizationAssociationStudentCharacteristic seoasc ON seoasc.EducationOrganizationId = eorg.EducationOrganizationId
					LEFT OUTER JOIN edfi.Student st ON st.StudentUSI = seoasc.StudentUSI
					LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = seoasc.StudentCharacteristicDescriptorId
			WHERE eorgdist.EducationOrganizationId = @distid AND d.CodeValue = 'SLIFE'
		END
		IF @columnIndex = 9
		BEGIN
			INSERT INTO @StudentDetailsTable(IdNumber)
			SELECT DISTINCT st.StudentUniqueId
			FROM 
				edfi.School sch 
					LEFT OUTER JOIN edfi.EducationOrganization eorg ON eorg.EducationOrganizationId = sch.SchoolId 
					LEFT OUTER JOIN edfi.LocalEducationAgency lea ON lea.LocalEducationAgencyId = sch.LocalEducationAgencyId 
					LEFT OUTER JOIN edfi.EducationOrganization eorgdist ON eorgdist.EducationOrganizationId = lea.LocalEducationAgencyId
					LEFT OUTER JOIN extension.StudentSchoolAssociationStudentProgramParticipation ssaspp ON ssaspp.SchoolId = sch.SchoolId 
					LEFT OUTER JOIN edfi.Student st ON st.StudentUSI = ssaspp.StudentUSI
					LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = ssaspp.ProgramCategoryDescriptorId
			WHERE eorgdist.EducationOrganizationId = @distid AND d.CodeValue LIKE 'Limited%'
		END
		IF @columnIndex = 10
		BEGIN
			INSERT INTO @StudentDetailsTable(IdNumber)
			SELECT DISTINCT st.StudentUniqueId
			FROM 
				edfi.School sch 
					LEFT OUTER JOIN edfi.EducationOrganization eorg ON eorg.EducationOrganizationId = sch.SchoolId 
					LEFT OUTER JOIN edfi.LocalEducationAgency lea ON lea.LocalEducationAgencyId = sch.LocalEducationAgencyId 
					LEFT OUTER JOIN edfi.EducationOrganization eorgdist ON eorgdist.EducationOrganizationId = lea.LocalEducationAgencyId
					LEFT OUTER JOIN extension.StudentSchoolAssociationStudentProgramParticipation ssaspp ON ssaspp.SchoolId = sch.SchoolId 
					LEFT OUTER JOIN edfi.Student st ON st.StudentUSI = ssaspp.StudentUSI
					LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = ssaspp.ProgramCategoryDescriptorId
			WHERE eorgdist.EducationOrganizationId = @distid AND d.CodeValue = 'English Learner Served'
		END
		IF @columnIndex = 11
		BEGIN
			INSERT INTO @StudentDetailsTable(IdNumber)
			SELECT DISTINCT st.StudentUniqueId
			FROM 
				edfi.School sch 
					LEFT OUTER JOIN edfi.EducationOrganization eorg ON eorg.EducationOrganizationId = sch.SchoolId 
					LEFT OUTER JOIN edfi.LocalEducationAgency lea ON lea.LocalEducationAgencyId = sch.LocalEducationAgencyId 
					LEFT OUTER JOIN edfi.EducationOrganization eorgdist ON eorgdist.EducationOrganizationId = lea.LocalEducationAgencyId
					LEFT OUTER JOIN extension.StudentSchoolAssociationStudentProgramParticipation ssaspp ON ssaspp.SchoolId = sch.SchoolId 
					LEFT OUTER JOIN edfi.Student st ON st.StudentUSI = ssaspp.StudentUSI
					LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = ssaspp.ProgramCategoryDescriptorId
			WHERE eorgdist.EducationOrganizationId = @distid AND d.CodeValue = 'Independent Study'
		END
		IF @columnIndex = 12
		BEGIN
			INSERT INTO @StudentDetailsTable(IdNumber)
			SELECT DISTINCT st.StudentUniqueId
			FROM 
				edfi.School sch 
					LEFT OUTER JOIN edfi.EducationOrganization eorg ON eorg.EducationOrganizationId = sch.SchoolId 
					LEFT OUTER JOIN edfi.LocalEducationAgency lea ON lea.LocalEducationAgencyId = sch.LocalEducationAgencyId 
					LEFT OUTER JOIN edfi.EducationOrganization eorgdist ON eorgdist.EducationOrganizationId = lea.LocalEducationAgencyId
					LEFT OUTER JOIN extension.StudentSchoolAssociationStudentProgramParticipation ssaspp ON ssaspp.SchoolId = sch.SchoolId 
					LEFT OUTER JOIN edfi.Student st ON st.StudentUSI = ssaspp.StudentUSI
					LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = ssaspp.ProgramCategoryDescriptorId
			WHERE eorgdist.EducationOrganizationId = @distid AND d.CodeValue = 'Section 504 Placement'
		END
		IF @columnIndex = 13
		BEGIN
			INSERT INTO @StudentDetailsTable(IdNumber)
			SELECT DISTINCT st.StudentUniqueId
			FROM 
				edfi.School sch 
					LEFT OUTER JOIN edfi.EducationOrganization eorg ON eorg.EducationOrganizationId = sch.SchoolId 
					LEFT OUTER JOIN edfi.LocalEducationAgency lea ON lea.LocalEducationAgencyId = sch.LocalEducationAgencyId 
					LEFT OUTER JOIN edfi.EducationOrganization eorgdist ON eorgdist.EducationOrganizationId = lea.LocalEducationAgencyId
					LEFT OUTER JOIN extension.StudentSchoolAssociationStudentProgramParticipation ssaspp ON ssaspp.SchoolId = sch.SchoolId 
					LEFT OUTER JOIN edfi.Student st ON st.StudentUSI = ssaspp.StudentUSI
					LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = ssaspp.ProgramCategoryDescriptorId
			WHERE eorgdist.EducationOrganizationId = @distid AND d.CodeValue = 'Title I Part A'
		END
		IF @columnIndex = 14
		BEGIN
			INSERT INTO @StudentDetailsTable(IdNumber)
			SELECT DISTINCT st.StudentUniqueId
			FROM 
				edfi.School sch 
					LEFT OUTER JOIN edfi.EducationOrganization eorg ON eorg.EducationOrganizationId = sch.SchoolId 
					LEFT OUTER JOIN edfi.LocalEducationAgency lea ON lea.LocalEducationAgencyId = sch.LocalEducationAgencyId 
					LEFT OUTER JOIN edfi.EducationOrganization eorgdist ON eorgdist.EducationOrganizationId = lea.LocalEducationAgencyId
					LEFT OUTER JOIN edfi.StudentEducationOrganizationAssociation seoa ON seoa.EducationOrganizationId = sch.SchoolId 
					LEFT OUTER JOIN edfi.Student st ON st.StudentUSI = seoa.StudentUSI
					LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = st.SchoolFoodServicesEligibilityDescriptorId
			WHERE eorgdist.EducationOrganizationId = @distid AND (d.ShortDescription LIKE 'reduced' OR d.ShortDescription LIKE 'free')
		END
	END

	IF @distid IS NULL
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
			WHERE eorgdist.EducationOrganizationId = @distid
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
			WHERE eorgdist.EducationOrganizationId = @distid
			;
		END
		IF @columnIndex = 2
		BEGIN
			INSERT INTO @StudentDetailsTable(IdNumber)
			SELECT DISTINCT st.StudentUniqueId
			FROM 
				edfi.School sch 
					LEFT OUTER JOIN edfi.EducationOrganization eorg ON eorg.EducationOrganizationId = sch.SchoolId 
					LEFT OUTER JOIN edfi.LocalEducationAgency lea ON lea.LocalEducationAgencyId = sch.LocalEducationAgencyId 
					LEFT OUTER JOIN edfi.EducationOrganization eorgdist ON eorgdist.EducationOrganizationId = lea.LocalEducationAgencyId
					LEFT OUTER JOIN extension.StudentEducationOrganizationAssociationStudentCharacteristic seoasc ON seoasc.EducationOrganizationId = eorg.EducationOrganizationId
					LEFT OUTER JOIN edfi.Student st ON st.StudentUSI = seoasc.StudentUSI
					LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = seoasc.StudentCharacteristicDescriptorId
			WHERE eorgdist.EducationOrganizationId = @distid AND d.CodeValue = 'Active Duty Parent (ADP)'
		END
		IF @columnIndex = 3
		BEGIN
			INSERT INTO @StudentDetailsTable(IdNumber)
			SELECT DISTINCT st.StudentUniqueId
			FROM 
				edfi.School sch 
					LEFT OUTER JOIN edfi.EducationOrganization eorg ON eorg.EducationOrganizationId = sch.SchoolId 
					LEFT OUTER JOIN edfi.LocalEducationAgency lea ON lea.LocalEducationAgencyId = sch.LocalEducationAgencyId 
					LEFT OUTER JOIN edfi.EducationOrganization eorgdist ON eorgdist.EducationOrganizationId = lea.LocalEducationAgencyId
					LEFT OUTER JOIN extension.StudentEducationOrganizationAssociationStudentCharacteristic seoasc ON seoasc.EducationOrganizationId = eorg.EducationOrganizationId
					LEFT OUTER JOIN edfi.Student st ON st.StudentUSI = seoasc.StudentUSI
					LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = seoasc.StudentCharacteristicDescriptorId
			WHERE eorgdist.EducationOrganizationId = @distid AND d.CodeValue = 'American Indian - Alaskan Native (Minnesota)'
		END
		IF @columnIndex = 4
		BEGIN
			INSERT INTO @StudentDetailsTable(IdNumber)
			SELECT DISTINCT st.StudentUniqueId
			FROM 
				edfi.School sch 
					LEFT OUTER JOIN edfi.EducationOrganization eorg ON eorg.EducationOrganizationId = sch.SchoolId 
					LEFT OUTER JOIN edfi.LocalEducationAgency lea ON lea.LocalEducationAgencyId = sch.LocalEducationAgencyId 
					LEFT OUTER JOIN edfi.EducationOrganization eorgdist ON eorgdist.EducationOrganizationId = lea.LocalEducationAgencyId
					LEFT OUTER JOIN extension.StudentEducationOrganizationAssociationStudentCharacteristic seoasc ON seoasc.EducationOrganizationId = eorg.EducationOrganizationId
					LEFT OUTER JOIN edfi.Student st ON st.StudentUSI = seoasc.StudentUSI
					LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = seoasc.StudentCharacteristicDescriptorId
			WHERE eorgdist.EducationOrganizationId = @distid AND d.CodeValue = 'Migrant'
		END
		IF @columnIndex = 5
		BEGIN
			INSERT INTO @StudentDetailsTable(IdNumber)
			SELECT DISTINCT st.StudentUniqueId
			FROM 
				edfi.School sch 
					LEFT OUTER JOIN edfi.EducationOrganization eorg ON eorg.EducationOrganizationId = sch.SchoolId 
					LEFT OUTER JOIN edfi.LocalEducationAgency lea ON lea.LocalEducationAgencyId = sch.LocalEducationAgencyId 
					LEFT OUTER JOIN edfi.EducationOrganization eorgdist ON eorgdist.EducationOrganizationId = lea.LocalEducationAgencyId
					LEFT OUTER JOIN extension.StudentEducationOrganizationAssociationStudentCharacteristic seoasc ON seoasc.EducationOrganizationId = eorg.EducationOrganizationId
					LEFT OUTER JOIN edfi.Student st ON st.StudentUSI = seoasc.StudentUSI
					LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = seoasc.StudentCharacteristicDescriptorId
			WHERE eorgdist.EducationOrganizationId = @distid AND d.CodeValue = 'Homeless'
		END
		IF @columnIndex = 6
		BEGIN
			INSERT INTO @StudentDetailsTable(IdNumber)
			SELECT DISTINCT st.StudentUniqueId
			FROM 
				edfi.School sch 
					LEFT OUTER JOIN edfi.EducationOrganization eorg ON eorg.EducationOrganizationId = sch.SchoolId 
					LEFT OUTER JOIN edfi.LocalEducationAgency lea ON lea.LocalEducationAgencyId = sch.LocalEducationAgencyId 
					LEFT OUTER JOIN edfi.EducationOrganization eorgdist ON eorgdist.EducationOrganizationId = lea.LocalEducationAgencyId
					LEFT OUTER JOIN extension.StudentEducationOrganizationAssociationStudentCharacteristic seoasc ON seoasc.EducationOrganizationId = eorg.EducationOrganizationId
					LEFT OUTER JOIN edfi.Student st ON st.StudentUSI = seoasc.StudentUSI
					LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = seoasc.StudentCharacteristicDescriptorId
			WHERE eorgdist.EducationOrganizationId = @distid AND d.CodeValue = 'Immigrant'
		END
		IF @columnIndex = 7
		BEGIN
			INSERT INTO @StudentDetailsTable(IdNumber)
			SELECT DISTINCT st.StudentUniqueId
			FROM 
				edfi.School sch 
					LEFT OUTER JOIN edfi.EducationOrganization eorg ON eorg.EducationOrganizationId = sch.SchoolId 
					LEFT OUTER JOIN edfi.LocalEducationAgency lea ON lea.LocalEducationAgencyId = sch.LocalEducationAgencyId 
					LEFT OUTER JOIN edfi.EducationOrganization eorgdist ON eorgdist.EducationOrganizationId = lea.LocalEducationAgencyId
					LEFT OUTER JOIN extension.StudentEducationOrganizationAssociationStudentCharacteristic seoasc ON seoasc.EducationOrganizationId = eorg.EducationOrganizationId
					LEFT OUTER JOIN edfi.Student st ON st.StudentUSI = seoasc.StudentUSI
					LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = seoasc.StudentCharacteristicDescriptorId
			WHERE eorgdist.EducationOrganizationId = @distid AND d.CodeValue = 'Recently Arrived English Learner (RAEL)'
		END
		IF @columnIndex = 8
		BEGIN
			INSERT INTO @StudentDetailsTable(IdNumber)
			SELECT DISTINCT st.StudentUniqueId
			FROM 
				edfi.School sch 
					LEFT OUTER JOIN edfi.EducationOrganization eorg ON eorg.EducationOrganizationId = sch.SchoolId 
					LEFT OUTER JOIN edfi.LocalEducationAgency lea ON lea.LocalEducationAgencyId = sch.LocalEducationAgencyId 
					LEFT OUTER JOIN edfi.EducationOrganization eorgdist ON eorgdist.EducationOrganizationId = lea.LocalEducationAgencyId
					LEFT OUTER JOIN extension.StudentEducationOrganizationAssociationStudentCharacteristic seoasc ON seoasc.EducationOrganizationId = eorg.EducationOrganizationId
					LEFT OUTER JOIN edfi.Student st ON st.StudentUSI = seoasc.StudentUSI
					LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = seoasc.StudentCharacteristicDescriptorId
			WHERE eorgdist.EducationOrganizationId = @distid AND d.CodeValue = 'SLIFE'
		END
		IF @columnIndex = 9
		BEGIN
			INSERT INTO @StudentDetailsTable(IdNumber)
			SELECT DISTINCT st.StudentUniqueId
			FROM 
				edfi.School sch 
					LEFT OUTER JOIN edfi.EducationOrganization eorg ON eorg.EducationOrganizationId = sch.SchoolId 
					LEFT OUTER JOIN edfi.LocalEducationAgency lea ON lea.LocalEducationAgencyId = sch.LocalEducationAgencyId 
					LEFT OUTER JOIN edfi.EducationOrganization eorgdist ON eorgdist.EducationOrganizationId = lea.LocalEducationAgencyId
					LEFT OUTER JOIN extension.StudentSchoolAssociationStudentProgramParticipation ssaspp ON ssaspp.SchoolId = sch.SchoolId 
					LEFT OUTER JOIN edfi.Student st ON st.StudentUSI = ssaspp.StudentUSI
					LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = ssaspp.ProgramCategoryDescriptorId
			WHERE eorgdist.EducationOrganizationId = @distid AND d.CodeValue LIKE 'Limited%'
		END
		IF @columnIndex = 10
		BEGIN
			INSERT INTO @StudentDetailsTable(IdNumber)
			SELECT DISTINCT st.StudentUniqueId
			FROM 
				edfi.School sch 
					LEFT OUTER JOIN edfi.EducationOrganization eorg ON eorg.EducationOrganizationId = sch.SchoolId 
					LEFT OUTER JOIN edfi.LocalEducationAgency lea ON lea.LocalEducationAgencyId = sch.LocalEducationAgencyId 
					LEFT OUTER JOIN edfi.EducationOrganization eorgdist ON eorgdist.EducationOrganizationId = lea.LocalEducationAgencyId
					LEFT OUTER JOIN extension.StudentSchoolAssociationStudentProgramParticipation ssaspp ON ssaspp.SchoolId = sch.SchoolId 
					LEFT OUTER JOIN edfi.Student st ON st.StudentUSI = ssaspp.StudentUSI
					LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = ssaspp.ProgramCategoryDescriptorId
			WHERE eorgdist.EducationOrganizationId = @distid AND d.CodeValue = 'English Learner Served'
		END
		IF @columnIndex = 11
		BEGIN
			INSERT INTO @StudentDetailsTable(IdNumber)
			SELECT DISTINCT st.StudentUniqueId
			FROM 
				edfi.School sch 
					LEFT OUTER JOIN edfi.EducationOrganization eorg ON eorg.EducationOrganizationId = sch.SchoolId 
					LEFT OUTER JOIN edfi.LocalEducationAgency lea ON lea.LocalEducationAgencyId = sch.LocalEducationAgencyId 
					LEFT OUTER JOIN edfi.EducationOrganization eorgdist ON eorgdist.EducationOrganizationId = lea.LocalEducationAgencyId
					LEFT OUTER JOIN extension.StudentSchoolAssociationStudentProgramParticipation ssaspp ON ssaspp.SchoolId = sch.SchoolId 
					LEFT OUTER JOIN edfi.Student st ON st.StudentUSI = ssaspp.StudentUSI
					LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = ssaspp.ProgramCategoryDescriptorId
			WHERE eorgdist.EducationOrganizationId = @distid AND d.CodeValue = 'Independent Study'
		END
		IF @columnIndex = 12
		BEGIN
			INSERT INTO @StudentDetailsTable(IdNumber)
			SELECT DISTINCT st.StudentUniqueId
			FROM 
				edfi.School sch 
					LEFT OUTER JOIN edfi.EducationOrganization eorg ON eorg.EducationOrganizationId = sch.SchoolId 
					LEFT OUTER JOIN edfi.LocalEducationAgency lea ON lea.LocalEducationAgencyId = sch.LocalEducationAgencyId 
					LEFT OUTER JOIN edfi.EducationOrganization eorgdist ON eorgdist.EducationOrganizationId = lea.LocalEducationAgencyId
					LEFT OUTER JOIN extension.StudentSchoolAssociationStudentProgramParticipation ssaspp ON ssaspp.SchoolId = sch.SchoolId 
					LEFT OUTER JOIN edfi.Student st ON st.StudentUSI = ssaspp.StudentUSI
					LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = ssaspp.ProgramCategoryDescriptorId
			WHERE eorgdist.EducationOrganizationId = @distid AND d.CodeValue = 'Section 504 Placement'
		END
		IF @columnIndex = 13
		BEGIN
			INSERT INTO @StudentDetailsTable(IdNumber)
			SELECT DISTINCT st.StudentUniqueId
			FROM 
				edfi.School sch 
					LEFT OUTER JOIN edfi.EducationOrganization eorg ON eorg.EducationOrganizationId = sch.SchoolId 
					LEFT OUTER JOIN edfi.LocalEducationAgency lea ON lea.LocalEducationAgencyId = sch.LocalEducationAgencyId 
					LEFT OUTER JOIN edfi.EducationOrganization eorgdist ON eorgdist.EducationOrganizationId = lea.LocalEducationAgencyId
					LEFT OUTER JOIN extension.StudentSchoolAssociationStudentProgramParticipation ssaspp ON ssaspp.SchoolId = sch.SchoolId 
					LEFT OUTER JOIN edfi.Student st ON st.StudentUSI = ssaspp.StudentUSI
					LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = ssaspp.ProgramCategoryDescriptorId
			WHERE eorgdist.EducationOrganizationId = @distid AND d.CodeValue = 'Title I Part A'
		END
		IF @columnIndex = 14
		BEGIN
			INSERT INTO @StudentDetailsTable(IdNumber)
			SELECT DISTINCT st.StudentUniqueId
			FROM 
				edfi.School sch 
					LEFT OUTER JOIN edfi.EducationOrganization eorg ON eorg.EducationOrganizationId = sch.SchoolId 
					LEFT OUTER JOIN edfi.LocalEducationAgency lea ON lea.LocalEducationAgencyId = sch.LocalEducationAgencyId 
					LEFT OUTER JOIN edfi.EducationOrganization eorgdist ON eorgdist.EducationOrganizationId = lea.LocalEducationAgencyId
					LEFT OUTER JOIN edfi.StudentEducationOrganizationAssociation seoa ON seoa.EducationOrganizationId = sch.SchoolId 
					LEFT OUTER JOIN edfi.Student st ON st.StudentUSI = seoa.StudentUSI
					LEFT OUTER JOIN edfi.Descriptor d ON d.DescriptorId = st.SchoolFoodServicesEligibilityDescriptorId
			WHERE eorgdist.EducationOrganizationId = @distid AND (d.ShortDescription LIKE 'reduced' OR d.ShortDescription LIKE 'free')
		END
	END

	SELECT DISTINCT * FROM rules.GetStudentEnrollmentDetails(@StudentDetailsTable) enr ORDER BY enr.StudentId;
END
