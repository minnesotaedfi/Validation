using System.Collections.Generic;
using ValidationWeb.Database.Queries;

namespace ValidationWeb.Services.Interfaces
{
    public interface IOdsDataService
    {
        List<DemographicsCountReportQuery> GetDistrictAncestryRaceCounts(
            int? districtEdOrgId,
            string fourDigitOdsDbYear);

        List<StudentDrillDownQuery> GetDistrictAncestryRaceStudentDrillDown(
            OrgType orgType,
            int? schoolEdOrgId,
            int? districtEdOrgId,
            int? columnIndex,
            string fourDigitOdsDbYear);

        List<MultipleEnrollmentsCountReportQuery> GetMultipleEnrollmentCounts(
            int? districtEdOrgId,
            string fourDigitOdsDbYear);

        List<StudentDrillDownQuery> GetMultipleEnrollmentStudentDrillDown(
            OrgType orgType,
            int? schoolEdOrgId,
            int? districtEdOrgId,
            int? columnIndex,
            string fourDigitOdsDbYear);

        List<StudentProgramsCountReportQuery> GetStudentProgramsCounts(int? districtEdOrgId, string fourDigitOdsDbYear);

        List<StudentDrillDownQuery> GetStudentProgramsStudentDrillDown(
            OrgType orgType,
            int? schoolEdOrgId,
            int? districtEdOrgId,
            int? columnIndex,
            string fourDigitOdsDbYear);

        List<ChangeOfEnrollmentReportQuery> GetChangeOfEnrollmentReport(int districtEdOrgId, string fourDigitOdsDbYear);

        List<ResidentsEnrolledElsewhereReportQuery> GetResidentsEnrolledElsewhereReport(
            int? districtEdOrgId,
            string fourDigitOdsDbYear);

        List<StudentDrillDownQuery> GetResidentsEnrolledElsewhereStudentDrillDown(
            int? districtEdOrgId,
            string fourDigitOdsDbYear);
    }
}