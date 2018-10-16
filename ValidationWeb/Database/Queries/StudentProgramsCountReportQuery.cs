using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValidationWeb
{
    public class StudentProgramsCountReportQuery
    {
        public static string StudentProgramsCountQuery =
@"SELECT 
	eorg.NameOfInstitution As SchoolName, 
	eorgdist.NameOfInstitution AS DistrictName,
	COUNT(ssa.StudentUSI) AS EnrollmentCount,
	COUNT(srac.StudentUSI) AS DemographicsCount,
	SUM(CASE WHEN sct.CodeValue='Parent in Military' THEN 1 ELSE 0 END) AS ADParentCount,
	SUM(CASE WHEN rt.CodeValue='American Indian - Alaskan Native' THEN 1 ELSE 0 END) AS IndianNativeCount,
	SUM(CASE WHEN sct.CodeValue='Migrant' THEN 1 ELSE 0 END) AS MigrantCount,
	SUM(CASE WHEN sct.CodeValue='Homeless' THEN 1 ELSE 0 END) AS HomelessCount,
	SUM(CASE WHEN sct.CodeValue='Immigrant' THEN 1 ELSE 0 END) AS ImmigrantCount,
	0 AS EnglishLearnerCount,
	0 AS EnglishLearnerServedCount,
	0 AS SLIFECount,
	0 AS IndependentStudyCount,
	SUM(CASE WHEN sct.CodeValue='Section 504 Handicapped' THEN 1 ELSE 0 END) AS Section504Count,
	0 AS Title1PartACount
	
FROM 
	edfi.Student s 
    LEFT OUTER JOIN edfi.StudentRace srac ON srac.StudentUSI = s.StudentUSI
	LEFT OUTER JOIN edfi.RaceType rt ON rt.RaceTypeId = srac.RaceTypeId AND rt.CodeValue != 'Other'
    LEFT OUTER JOIN edfi.StudentSchoolAssociation ssa ON ssa.StudentUSI = s.StudentUSI
	LEFT OUTER JOIN edfi.StudentCharacteristic sc ON sc.StudentUSI = s.StudentUSI
	LEFT OUTER JOIN edfi.StudentCharacteristicDescriptor scd ON scd.StudentCharacteristicDescriptorId = sc.StudentCharacteristicDescriptorId
	LEFT OUTER JOIN edfi.StudentCharacteristicType sct ON sct.StudentCharacteristicTypeId = scd.StudentCharacteristicTypeId
    LEFT OUTER JOIN edfi.School sch ON sch.SchoolId = ssa.SchoolId
    LEFT OUTER JOIN edfi.EducationOrganization eorg ON eorg.EducationOrganizationId = sch.SchoolId 
    LEFT OUTER JOIN edfi.LocalEducationAgency lea ON lea.LocalEducationAgencyId = sch.LocalEducationAgencyId 
    LEFT OUTER JOIN edfi.EducationOrganization eorgdist ON eorgdist.EducationOrganizationId = lea.LocalEducationAgencyId 
	WHERE lea.LocalEducationAgencyId = @distid
	GROUP BY eorg.NameOfInstitution, eorgdist.NameOfInstitution";

        public const string SchoolNameColumnName = "SchoolName";
        public const string DistrictNameColumnName = "DistrictName";
        public const string EnrollmentCountColumnName = "EnrollmentCount";
        public const string DemographicsCountColumnName = "DemographicsCount";
        public const string ADParentCountColumnName = "ADParentCount";
        public const string IndianNativeCountColumnName = "IndianNativeCount";
        public const string MigrantCountColumnName = "MigrantCount";
        public const string HomelessCountColumnName = "HomelessCount";
        public const string ImmigrantCountColumnName = "ImmigrantCount";
        public const string EnglishLearnerCountColumnName = "EnglishLearnerCount";
        public const string EnglishLearnerServedCountColumnName = "EnglishLearnerServedCount";
        public const string SLIFECountColumnName = "SLIFECount";
        public const string IndependentStudyCountColumnName = "IndependentStudyCount";
        public const string Section504CountColumnName = "Section504Count";
        public const string Title1PartACountColumnName = "Title1PartACount";

        public string SchoolName { get; set; }
        public string DistrictName { get; set; }
        public int EnrollmentCount { get; set; }
        public int DemographicsCount { get; set; }
        public int ADParentCount { get; set; }
        public int IndianNativeCount { get; set; }
        public int MigrantCount { get; set; }
        public int HomelessCount { get; set; }
        public int ImmigrantCount { get; set; }
        public int EnglishLearnerCount { get; set; }
        public int EnglishLearnerServedCount { get; set; }
        public int SLIFECount { get; set; }
        public int IndependentStudyCount { get; set; }
        public int Section504Count { get; set; }
        public int Title1PartACount { get; set; }
    }
}
