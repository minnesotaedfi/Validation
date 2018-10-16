using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValidationWeb.Services
{
    public interface IOdsDataService
    {
        List<DemographicsCountReportQuery> GetDistrictAncestryRaceCounts(string districtEdOrgId, string fourDigitOdsDbYear);
        List<MultipleEnrollmentsCountReportQuery> GetMultipleEnrollmentCounts(string districtEdOrgId, string fourDigitOdsDbYear);
        List<StudentProgramsCountReportQuery> GetStudentProgramsCounts(string districtEdOrgId, string fourDigitOdsDbYear);
    }
}