----------------------------------------------------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------------------------------------------
-- TYPES:  INT and NVARCHAR array types -----------------------------------------------------------
-- FUNCTION:  GET STUDENT DETAILS for Drilling down to Student-Level from Student Counts in other reports -----------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------------------------------------------

IF OBJECT_ID ('rules.GetStudentEnrollmentDetails') IS NOT NULL   
    DROP FUNCTION rules.GetStudentEnrollmentDetails;  

IF EXISTS(SELECT 1 FROM sys.types WHERE name = 'IdStringTable' AND is_table_type = 1 AND SCHEMA_ID('rules') = schema_id)
    DROP TYPE rules.IdStringTable;  
  
IF EXISTS(SELECT 1 FROM sys.types WHERE name = 'IdIntTable' AND is_table_type = 1 AND SCHEMA_ID('rules') = schema_id)
    DROP TYPE rules.IdIntTable;  
  

CREATE TYPE rules.IdIntTable AS TABLE (IdNumber INT);  
 

CREATE TYPE rules.IdStringTable AS TABLE (IdNumber NVARCHAR(48));  
 
 -- Must be in its own batch
 EXEC('
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
	');