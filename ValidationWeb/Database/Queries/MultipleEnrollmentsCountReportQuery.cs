using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValidationWeb
{
    public class MultipleEnrollmentsCountReportQuery
    {
        public static string MultipleEnrollmentsCountQuery =
@"SELECT 
	eorg.NameOfInstitution As SchoolName, 
	eorgdist.NameOfInstitution AS DistrictName,
	COUNT(ssa.StudentUSI) AS EnrollmentCount,
	0 AS MultiWithinDistrictCount,
	0 AS MultiOutsideDistrictCount
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
        public const string MultiWithinDistrictCountColumnName = "MultiWithinDistrictCount";
        public const string MultiOutsideDistrictCountColumnName = "MultiOutsideDistrictCount";

        public string SchoolName { get; set; }
        public string DistrictName { get; set; }
        public int EnrollmentCount { get; set; }
        public int MultiWithinDistrictCount { get; set; }
        public int MultiOutsideDistrictCount { get; set; }
    }
}