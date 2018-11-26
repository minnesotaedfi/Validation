IF OBJECT_ID('StudentDemographic','V') IS NOT NULL
	DROP VIEW StudentDemographic

GO

CREATE VIEW StudentDemographic 
AS

SELECT S.StudentUniqueId AS [MARSSNumber]
	 , SEOA.EducationOrganizationId AS [SchoolId]
	 , LTRIM(RTRIM(SEOAE.FirstName)) AS [FirstName]
	 , LTRIM(RTRIM(SEOAE.MiddleName)) AS [MiddleName]
	 , LTRIM(RTRIM(SEOAE.LastSurname)) AS [LastName]
	 , SEOAE.GenerationCodeSuffix AS [Suffix]
	 , SEOAE.BirthDate
	 , ST.ShortDescription AS [Gender]
	 , SEOAE.HispanicLatinoEthnicity AS [HispanicIndicator]
	 , AI.AsianIndicator
	 , BI.BlackIndicator
	 , II.IndianIndicator
	 , PII.PacificIslanderIndicator
	 , WI.WhiteIndicator
	 , EC.EthnicCode
	 , DAEO.CodeValue AS [AncestryEthnicOrigin]
	 , HPL.HomePrimaryLanguage
	 , DLEP.CodeValue AS [EnglishLearner]
	 , LUD.LocalUseData
FROM edfi.StudentEducationOrganizationAssociation SEOA
JOIN extension.StudentEducationOrganizationAssociationExtension SEOAE
	ON SEOAE.StudentUSI = SEOA.StudentUSI
	AND SEOAE.ResponsibilityDescriptorId = SEOA.ResponsibilityDescriptorId
	AND SEOAE.EducationOrganizationId = SEOA.EducationOrganizationId
JOIN edfi.Student S ON S.StudentUSI = SEOA.StudentUSI
JOIN edfi.ResponsibilityDescriptor RD ON RD.ResponsibilityDescriptorId = SEOA.ResponsibilityDescriptorId
JOIN edfi.Descriptor DR ON DR.DescriptorId = RD.ResponsibilityDescriptorId
	AND DR.CodeValue = 'Demographic'
	AND DR.Namespace LIKE 'http://education.mn.gov%'
LEFT JOIN edfi.SexType ST ON ST.SexTypeId = SEOAE.SexTypeId
LEFT JOIN (
	SELECT SEOAR.StudentUSI
		 , SEOAR.ResponsibilityDescriptorId
		 , SEOAR.EducationOrganizationId
		 , 1 AS [AsianIndicator]
	FROM extension.StudentEducationOrganizationAssociationRace SEOAR
	JOIN edfi.RaceType RT ON RT.RaceTypeId = SEOAR.RaceTypeId
		 AND RT.ShortDescription = 'Asian'
	) AI ON AI.StudentUSI = SEOAE.StudentUSI
		 AND AI.ResponsibilityDescriptorId = SEOAE.ResponsibilityDescriptorId
		 AND AI.EducationOrganizationId = SEOAE.EducationOrganizationId
LEFT JOIN (
	SELECT SEOAR.StudentUSI
		 , SEOAR.ResponsibilityDescriptorId
		 , SEOAR.EducationOrganizationId
		 , 1 AS [BlackIndicator]
	FROM extension.StudentEducationOrganizationAssociationRace SEOAR
	JOIN edfi.RaceType RT ON RT.RaceTypeId = SEOAR.RaceTypeId
		 AND RT.ShortDescription = 'Black - African American'
	) BI ON BI.StudentUSI = SEOAE.StudentUSI
		 AND BI.ResponsibilityDescriptorId = SEOAE.ResponsibilityDescriptorId
		 AND BI.EducationOrganizationId = SEOAE.EducationOrganizationId
LEFT JOIN (
	SELECT SEOAR.StudentUSI
		 , SEOAR.ResponsibilityDescriptorId
		 , SEOAR.EducationOrganizationId
		 , 1 AS [IndianIndicator]
	FROM extension.StudentEducationOrganizationAssociationRace SEOAR
	JOIN edfi.RaceType RT ON RT.RaceTypeId = SEOAR.RaceTypeId
		 AND RT.ShortDescription = 'American Indian - Alaskan Native'
	) II ON II.StudentUSI = SEOAE.StudentUSI
		 AND II.ResponsibilityDescriptorId = SEOAE.ResponsibilityDescriptorId
		 AND II.EducationOrganizationId = SEOAE.EducationOrganizationId
LEFT JOIN (
	SELECT SEOAR.StudentUSI
		 , SEOAR.ResponsibilityDescriptorId
		 , SEOAR.EducationOrganizationId
		 , 1 AS [PacificIslanderIndicator]
	FROM extension.StudentEducationOrganizationAssociationRace SEOAR
	JOIN edfi.RaceType RT ON RT.RaceTypeId = SEOAR.RaceTypeId
		 AND RT.ShortDescription = 'Native Hawaiian - Pacific Islander'
	) PII ON PII.StudentUSI = SEOAE.StudentUSI
		 AND PII.ResponsibilityDescriptorId = SEOAE.ResponsibilityDescriptorId
		 AND PII.EducationOrganizationId = SEOAE.EducationOrganizationId
LEFT JOIN (
	SELECT SEOAR.StudentUSI
		 , SEOAR.ResponsibilityDescriptorId
		 , SEOAR.EducationOrganizationId
		 , 1 AS [WhiteIndicator]
	FROM extension.StudentEducationOrganizationAssociationRace SEOAR
	JOIN edfi.RaceType RT ON RT.RaceTypeId = SEOAR.RaceTypeId
		 AND RT.ShortDescription = 'White'
	) WI ON WI.StudentUSI = SEOAE.StudentUSI
		 AND WI.ResponsibilityDescriptorId = SEOAE.ResponsibilityDescriptorId
		 AND WI.EducationOrganizationId = SEOAE.EducationOrganizationId
LEFT JOIN (
	SELECT SEOASC.StudentUSI
		 , SEOASC.ResponsibilityDescriptorId
		 , SEOASC.EducationOrganizationId
		 , DSC.CodeValue AS [EthnicCode]
	FROM extension.StudentEducationOrganizationAssociationStudentCharacteristic SEOASC
	JOIN edfi.StudentCharacteristicDescriptor SCD ON SCD.StudentCharacteristicDescriptorId = SEOASC.StudentCharacteristicDescriptorId
	JOIN edfi.Descriptor DSC ON DSC.DescriptorId = SCD.StudentCharacteristicDescriptorId
			AND DSC.CodeValue = 'American Indian - Alaskan Native (Minnesota)'
			AND DSC.Namespace LIKE 'http://education.mn.gov%'
	) EC ON EC.StudentUSI = SEOAE.StudentUSI
		 AND EC.ResponsibilityDescriptorId = SEOAE.ResponsibilityDescriptorId
		 AND EC.EducationOrganizationId = SEOAE.EducationOrganizationId
LEFT JOIN extension.StudentEducationOrganizationAssociationAncestryEthnicOrigin SEOAAEO
	ON SEOAAEO.StudentUSI = SEOAE.StudentUSI
	AND SEOAAEO.ResponsibilityDescriptorId = SEOAE.ResponsibilityDescriptorId
	AND SEOAAEO.EducationOrganizationId = SEOAE.EducationOrganizationId
LEFT JOIN extension.AncestryEthnicOriginDescriptor AEOD ON AEOD.AncestryEthnicOriginDescriptorId = SEOAAEO.AncestryEthnicOriginDescriptorId
LEFT JOIN edfi.Descriptor DAEO ON DAEO.DescriptorId = AEOD.AncestryEthnicOriginDescriptorId
	AND DAEO.Namespace LIKE 'http://education.mn.gov%'
LEFT JOIN (
	SELECT SEOAL.StudentUSI
		 , SEOAL.EducationOrganizationId
		 , SEOAL.ResponsibilityDescriptorId
		 , DL.CodeValue AS [HomePrimaryLanguage]
	FROM extension.StudentEducationOrganizationAssociationLanguage SEOAL
	JOIN edfi.LanguageDescriptor LD ON LD.LanguageDescriptorId = SEOAL.LanguageDescriptorId
	JOIN edfi.Descriptor DL ON DL.DescriptorId = SEOAL.LanguageDescriptorId
		AND DL.Namespace LIKE 'http://education.mn.gov%'
	JOIN extension.StudentEducationOrganizationAssociationLanguageUse SEOALU 
		ON SEOALU.EducationOrganizationId = SEOAL.EducationOrganizationId
		AND SEOALU.StudentUSI = SEOAL.StudentUSI
		AND SEOALU.ResponsibilityDescriptorId = SEOAL.ResponsibilityDescriptorId
		AND SEOALU.LanguageDescriptorId = SEOAL.LanguageDescriptorId
	JOIN edfi.LanguageUseType LUT ON LUT.LanguageUseTypeId = SEOALU.LanguageUseTypeId
		AND LUT.ShortDescription = 'Home language'
	) HPL ON HPL.StudentUSI = SEOAE.StudentUSI
	AND HPL.EducationOrganizationId = SEOAE.EducationOrganizationId
	AND HPL.ResponsibilityDescriptorId = SEOAE.ResponsibilityDescriptorId
LEFT JOIN edfi.LimitedEnglishProficiencyDescriptor LEPD ON LEPD.LimitedEnglishProficiencyDescriptorId = SEOAE.LimitedEnglishProficiencyDescriptorId
LEFT JOIN edfi.Descriptor DLEP ON DLEP.DescriptorId = LEPD.LimitedEnglishProficiencyDescriptorId
	AND DLEP.Namespace LIKE 'http://education.mn.gov%'
LEFT JOIN (
	SELECT IC.StudentUSI
		 , IC.EducationOrganizationId
		 , IC.ResponsibilityDescriptorId
		 , IC.IdentificationCode AS [LocalUseData]
	FROM extension.StudentEducationOrganizationAssociationStudentIdentificationCode IC
	JOIN edfi.StudentIdentificationSystemDescriptor SISD ON SISD.StudentIdentificationSystemDescriptorId = IC.StudentIdentificationSystemDescriptorId
	JOIN edfi.Descriptor DSIS ON DSIS.DescriptorId = SISD.StudentIdentificationSystemDescriptorId
		AND DSIS.CodeValue = 'Local'
		AND DSIS.Namespace LIKE 'http://education.mn.gov%'
	) LUD ON LUD.StudentUSI = SEOAE.StudentUSI
		  AND LUD.EducationOrganizationId = SEOAE.EducationOrganizationId
		  AND LUD.ResponsibilityDescriptorId = SEOAE.ResponsibilityDescriptorId;