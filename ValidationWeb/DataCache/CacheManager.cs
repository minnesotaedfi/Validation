namespace ValidationWeb.DataCache
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Caching;

    using ValidationWeb.Services;

    public class CacheManager : ICacheManager
    {
        public CacheManager()
        {
            Cache = MemoryCache.Default;
        }

        protected ObjectCache Cache { get; }

        protected DateTimeOffset CacheExpirationOffset => DateTimeOffset.Now.AddMinutes(30);

        private static object LockObject => new object(); 

        public IEnumerable<DemographicsCountReportQuery> GetDistrictAncestryRaceCounts(
            IOdsDataService odsDataService,
            bool isStateMode,
            int? edOrgId,
            string fourDigitSchoolYear)
        {
            var cacheKey = $"DistrictAncestryRaceCounts_{isStateMode}_{edOrgId}_{fourDigitSchoolYear}";

            lock (LockObject)
            {
                if (!Cache.Contains(cacheKey))
                {
                    var results = odsDataService.GetDistrictAncestryRaceCounts(
                        isStateMode ? null : edOrgId,
                        fourDigitSchoolYear);

                    Cache.Add(cacheKey, results, CacheExpirationOffset);
                }

                return Cache.Get(cacheKey) as IEnumerable<DemographicsCountReportQuery>;
            }
        }

        public IEnumerable<StudentDrillDownQuery> GetStudentDemographicsDrilldownData(
            IOdsDataService odsDataService,
            OrgType orgType,
            int? schoolId,
            int edOrgId,
            int drillDownColumnIndex,
            string fourDigitSchoolYear)
        {
            var cacheKey = $"DistrictAncestryRaceStudentDrillDownData_{orgType}_{schoolId}_{edOrgId}_{drillDownColumnIndex}_{fourDigitSchoolYear}";

            lock (LockObject)
            {
                if (!Cache.Contains(cacheKey))
                {
                    var results = odsDataService.GetDistrictAncestryRaceStudentDrillDown(
                        orgType,
                        schoolId,
                        schoolId ?? edOrgId,
                        drillDownColumnIndex,
                        fourDigitSchoolYear);

                    Cache.Add(cacheKey, results, CacheExpirationOffset);
                }

                return Cache.Get(cacheKey) as IEnumerable<StudentDrillDownQuery>;
            }
        }

        public IEnumerable<MultipleEnrollmentsCountReportQuery> GetMultipleEnrollmentCounts(
            IOdsDataService odsDataService,
            int? edOrgId,
            string fourDigitSchoolYear)
        {
            var cacheKey = $"MultipleEnrollmentCounts_{edOrgId}_{fourDigitSchoolYear}";

            lock (LockObject)
            {
                if (!Cache.Contains(cacheKey))
                {
                    var results = odsDataService.GetMultipleEnrollmentCounts(edOrgId, fourDigitSchoolYear);

                    Cache.Add(cacheKey, results, CacheExpirationOffset);
                }

                return Cache.Get(cacheKey) as IEnumerable<MultipleEnrollmentsCountReportQuery>;
            }
        }

        public IEnumerable<StudentDrillDownQuery> GetStudentMultipleEnrollmentsDrilldownData(
            IOdsDataService odsDataService,
            OrgType orgType,
            int? schoolId,
            int edOrgId,
            int drillDownColumnIndex,
            string fourDigitSchoolYear)
        {
            var cacheKey = $"GetStudentMultipleEnrollmentsDrilldownData_{orgType}_{schoolId}_{edOrgId}_{drillDownColumnIndex}_{fourDigitSchoolYear}";

            lock (LockObject)
            {
                if (!Cache.Contains(cacheKey))
                {
                    var results = odsDataService.GetMultipleEnrollmentStudentDrillDown(
                        orgType,
                        schoolId,
                        schoolId ?? edOrgId,
                        drillDownColumnIndex,
                        fourDigitSchoolYear);

                    Cache.Add(cacheKey, results, CacheExpirationOffset);
                }

                return Cache.Get(cacheKey) as IEnumerable<StudentDrillDownQuery>;
            }
        }
    }
}