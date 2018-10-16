using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValidationWeb
{
    public class DemographicsCountReportQuery
    {
        public static string DistrictAncestryRaceCountsQuery =
@"SELECT 
	eorg.NameOfInstitution As SchoolName, 
	eorgdist.NameOfInstitution AS DistrictName,
	COUNT(ssa.StudentUSI) AS EnrollmentCount,
	COUNT(srac.StudentUSI) AS DemographicsCount,
	COUNT(rt.RaceTypeId) AS RaceGivenCount,
	SUM(CASE WHEN s.HispanicLatinoEthnicity = 1 THEN 1 ELSE 0 END) AS AncestryGivenCount
FROM 
	edfi.Student s 
    LEFT OUTER JOIN edfi.StudentRace srac ON srac.StudentUSI = s.StudentUSI
	LEFT OUTER JOIN edfi.RaceType rt ON rt.RaceTypeId = srac.RaceTypeId AND rt.CodeValue != 'Other'
    LEFT OUTER JOIN edfi.StudentSchoolAssociation ssa ON ssa.StudentUSI = s.StudentUSI
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
        public const string RaceGivenCountColumnName = "RaceGivenCount";
        public const string AncestryGivenCountColumnName = "AncestryGivenCount";

        public string SchoolName { get; set; }
        public string DistrictName { get; set; }
        public int EnrollmentCount { get; set; }
        public int DemographicsCount { get; set; }
        public int RaceGivenCount { get; set; }
        public int AncestryGivenCount { get; set; }
    }
}