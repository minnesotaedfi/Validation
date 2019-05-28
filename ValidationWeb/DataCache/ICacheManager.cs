using System.Collections.Generic;
using ValidationWeb.Database.Queries;
using ValidationWeb.Services.Interfaces;

namespace ValidationWeb.DataCache
{
    public interface ICacheManager
    {
        IEnumerable<DemographicsCountReportQuery> GetDistrictAncestryRaceCounts(
            IOdsDataService odsDataService, 
            bool isStateMode, 
            int? edOrgId, 
            string fourDigitSchoolYear);

        IEnumerable<StudentDrillDownQuery> GetStudentDemographicsDrilldownData(
            IOdsDataService odsDataService, 
            OrgType orgType, 
            int? schoolId, 
            int edOrgId, 
            int drillDownColumnIndex, 
            string fourDigitSchoolYear);

        IEnumerable<MultipleEnrollmentsCountReportQuery> GetMultipleEnrollmentCounts(
            IOdsDataService odsDataService,
            int? edOrgId, 
            string fourDigitSchoolYear);

        IEnumerable<StudentDrillDownQuery> GetStudentMultipleEnrollmentsDrilldownData(
            IOdsDataService odsDataService,
            OrgType orgType,
            int? schoolId,
            int edOrgId,
            int drillDownColumnIndex,
            string fourDigitSchoolYear);

        IEnumerable<StudentProgramsCountReportQuery> GetStudentProgramsCounts(
            IOdsDataService odsDataService,
            int? edOrgId,
            string fourDigitSchoolYear);

        IEnumerable<StudentDrillDownQuery> GetStudentProgramsStudentDrillDownData(
            IOdsDataService odsDataService,
            OrgType orgType,
            int? schoolId,
            int edOrgId,
            int drillDownColumnIndex,
            string fourDigitSchoolYear);

        IEnumerable<ResidentsEnrolledElsewhereReportQuery> GetResidentsEnrolledElsewhereReport(
            IOdsDataService odsDataService,
            int? edOrgId,
            string fourDigitSchoolYear);

        IEnumerable<StudentDrillDownQuery> GetResidentsEnrolledElsewhereStudentDrillDown(
            IOdsDataService odsDataService,
            OrgType orgType,
            int? schoolId,
            int edOrgId,
            int drillDownColumnIndex,
            string fourDigitSchoolYear);

        IEnumerable<ChangeOfEnrollmentReportQuery> GetChangeOfEnrollmentReport(
            IOdsDataService odsDataService,
            int edOrgId,
            string fourDigitSchoolYear);
    }
}