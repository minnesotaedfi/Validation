IF OBJECT_ID('SSDC','V') IS NOT NULL
	DROP VIEW SSDC

GO

CREATE VIEW SSDC

AS

SELECT S.StudentUniqueId AS [MARSSNumber]
	 , SEOA.EducationOrganizationId AS [SchoolId]
	 , ADP.ActiveDutyParentIndicator
	 , II.ImmigrantIndicator
	 , RAEL.RecentlyArrivedEnglishLearner
	 , SLIFE.SLIFE
FROM edfi.Student S
JOIN edfi.StudentEducationOrganizationAssociation SEOA ON SEOA.StudentUSI = S.StudentUSI
JOIN extension.StudentEducationOrganizationAssociationExtension SEOAE ON SEOAE.StudentUSI = SEOA.StudentUSI
	AND SEOAE.EducationOrganizationId = SEOA.EducationOrganizationId
	AND SEOAE.ResponsibilityDescriptorId = SEOA.ResponsibilityDescriptorId
LEFT JOIN (
	SELECT SEOASC.StudentUSI
		 , SEOASC.EducationOrganizationId
		 , SEOASC.ResponsibilityDescriptorId
		 , 1 AS [ActiveDutyParentIndicator]
	FROM extension.StudentEducationOrganizationAssociationStudentCharacteristic SEOASC
	JOIN edfi.StudentCharacteristicDescriptor SCD ON SCD.StudentCharacteristicDescriptorId = SEOASC.StudentCharacteristicDescriptorId
	JOIN edfi.Descriptor DSC ON DSC.DescriptorId = SCD.StudentCharacteristicDescriptorId
		AND DSC.CodeValue = 'Active Duty Parent (ADP)'
		AND DSC.Namespace LIKE 'http://education.mn.gov%'
	) ADP ON ADP.StudentUSI = SEOAE.StudentUSI
		  AND ADP.EducationOrganizationId = SEOAE.EducationOrganizationId
		  AND ADP.ResponsibilityDescriptorId = SEOAE.ResponsibilityDescriptorId
LEFT JOIN (
	SELECT SEOASC.StudentUSI
		 , SEOASC.EducationOrganizationId
		 , SEOASC.ResponsibilityDescriptorId
		 , 1 AS [ImmigrantIndicator]
	FROM extension.StudentEducationOrganizationAssociationStudentCharacteristic SEOASC
	JOIN edfi.StudentCharacteristicDescriptor SCD ON SCD.StudentCharacteristicDescriptorId = SEOASC.StudentCharacteristicDescriptorId
	JOIN edfi.Descriptor DSC ON DSC.DescriptorId = SCD.StudentCharacteristicDescriptorId
		AND DSC.CodeValue = 'Immigrant'
		AND DSC.Namespace LIKE 'http://education.mn.gov%'
	) II ON II.StudentUSI = SEOAE.StudentUSI
		  AND II.EducationOrganizationId = SEOAE.EducationOrganizationId
		  AND II.ResponsibilityDescriptorId = SEOAE.ResponsibilityDescriptorId
LEFT JOIN (
	SELECT SEOASC.StudentUSI
		 , SEOASC.EducationOrganizationId
		 , SEOASC.ResponsibilityDescriptorId
		 , 1 AS [RecentlyArrivedEnglishLearner]
	FROM extension.StudentEducationOrganizationAssociationStudentCharacteristic SEOASC
	JOIN edfi.StudentCharacteristicDescriptor SCD ON SCD.StudentCharacteristicDescriptorId = SEOASC.StudentCharacteristicDescriptorId
	JOIN edfi.Descriptor DSC ON DSC.DescriptorId = SCD.StudentCharacteristicDescriptorId
		AND DSC.CodeValue = 'Recently Arrived English Learner (RAEL)'
		AND DSC.Namespace LIKE 'http://education.mn.gov%'
	) RAEL ON RAEL.StudentUSI = SEOAE.StudentUSI
		  AND RAEL.EducationOrganizationId = SEOAE.EducationOrganizationId
		  AND RAEL.ResponsibilityDescriptorId = SEOAE.ResponsibilityDescriptorId
LEFT JOIN (
	SELECT SEOASC.StudentUSI
		 , SEOASC.EducationOrganizationId
		 , SEOASC.ResponsibilityDescriptorId
		 , 1 AS [SLIFE]
	FROM extension.StudentEducationOrganizationAssociationStudentCharacteristic SEOASC
	JOIN edfi.StudentCharacteristicDescriptor SCD ON SCD.StudentCharacteristicDescriptorId = SEOASC.StudentCharacteristicDescriptorId
	JOIN edfi.Descriptor DSC ON DSC.DescriptorId = SCD.StudentCharacteristicDescriptorId
		AND DSC.CodeValue = 'SLIFE'
		AND DSC.Namespace LIKE 'http://education.mn.gov%'
	) SLIFE ON SLIFE.StudentUSI = SEOAE.StudentUSI
		  AND SLIFE.EducationOrganizationId = SEOAE.EducationOrganizationId
		  AND SLIFE.ResponsibilityDescriptorId = SEOAE.ResponsibilityDescriptorId
;