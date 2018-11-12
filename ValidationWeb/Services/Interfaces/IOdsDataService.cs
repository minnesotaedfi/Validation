using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValidationWeb.Services
{
    public interface IOdsDataService
    {
        List<DemographicsCountReportQuery> GetDistrictAncestryRaceCounts(int? districtEdOrgId, string fourDigitOdsDbYear);
        List<MultipleEnrollmentsCountReportQuery> GetMultipleEnrollmentCounts(int? districtEdOrgId, string fourDigitOdsDbYear);
        List<StudentProgramsCountReportQuery> GetStudentProgramsCounts(int? districtEdOrgId, string fourDigitOdsDbYear);
        List<ChangeOfEnrollmentReportQuery> GetChangeOfEnrollmentReport(int districtEdOrgId, string fourDigitOdsDbYear);
        List<StudentDrillDownViewModel> GetResidentsEnrolledElsewhereStudentDrillDown(int? districtEdOrgId, string fourDigitOdsDbYear, string uniqueStudentId);
    }
}