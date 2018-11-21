-- Using the StudentUniqueID (NOT the StudentUSI)
CREATE FUNCTION rules.GetStudentEnrollmentDetails(@StudentIdList rules.IdStringTable READONLY)
	RETURNS TABLE
AS
RETURN
	SELECT 
		StudentId = st.StudentUniqueId,
        StudentFirstName = st.FirstName,
        StudentMiddleName = st.MiddleName,
        StudentLastName = st.LastSurname,
        DistrictName = distedorg.NameOfInstitution,
        DistrictId = distedorg.EducationOrganizationId,
        SchoolName = schedorg.NameOfInstitution,
        SchoolId = schedorg.EducationOrganizationId,
        EnrolledDate = ssa.EntryDate,
        WithdrawDate = ssa.ExitWithdrawDate,
		Grade = gradedesc.CodeValue,
		SpecialEdStatus = speddesc.ShortDescription
	FROM 
		edfi.student st
	INNER JOIN  @StudentIdList sil ON sil.IdNumber = st.StudentUniqueId
	INNER JOIN  edfi.StudentSchoolAssociation ssa ON ssa.StudentUSI = st.StudentUSI
	INNER JOIN  edfi.School sch ON sch.SchoolId = ssa.SchoolId
	LEFT OUTER JOIN edfi.EducationOrganization schedorg ON schedorg.EducationOrganizationId = sch.SchoolId
	LEFT OUTER JOIN edfi.EducationOrganization distedorg ON distedorg.EducationOrganizationId = sch.LocalEducationAgencyId
	LEFT OUTER JOIN edfi.Descriptor gradedesc ON gradedesc.DescriptorId = ssa.EntryGradeLevelDescriptorId
	LEFT OUTER JOIN extension.StudentSchoolAssociationExtension ssae ON ssae.StudentUSI = st.StudentUSI
	LEFT OUTER JOIN edfi.Descriptor speddesc ON speddesc.DescriptorId = ssae.SpecialEducationEvaluationStatusDescriptorId
