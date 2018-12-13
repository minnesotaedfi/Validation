namespace ValidationWeb.DataCache
{
    using System.Collections.Generic;

    using ValidationWeb.Services;

    public interface ICacheManager
    {
        IEnumerable<DemographicsCountReportQuery> GetDistrictAncestryRaceCounts(
            IOdsDataService odsDataService, 
            bool isStateMode, 
            int? edOrgId, 
            string fourDigitSchoolYear);

        IEnumerable<StudentDrillDownQuery> GetStudentDrilldownData(
            IOdsDataService odsDataService, 
            OrgType orgType, 
            int? schoolId, 
            int edOrgId, 
            int drillDownColumnIndex, 
            string fourDigitSchoolYear);
    }
}