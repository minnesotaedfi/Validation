using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValidationWeb.DataCache
{
    using System.Collections;
    using System.Runtime.Caching;

    using ValidationWeb.Services;
    using ValidationWeb.Utility;

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
                var results = odsDataService.GetDistrictAncestryRaceCounts(
                    isStateMode ? null : edOrgId,
                    fourDigitSchoolYear);

                if (!Cache.Contains(cacheKey))
                {
                    Cache.Add(cacheKey, results, CacheExpirationOffset);
                }

                return Cache.Get(cacheKey) as IEnumerable<DemographicsCountReportQuery>;
            }
        }

        public IEnumerable<StudentDrillDownQuery> GetStudentDrilldownData(
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
                var results = odsDataService.GetDistrictAncestryRaceStudentDrillDown(
                    orgType,
                    schoolId,
                    schoolId ?? edOrgId,
                    drillDownColumnIndex,
                    fourDigitSchoolYear);

                if (!Cache.Contains(cacheKey))
                {
                    Cache.Add(cacheKey, results, CacheExpirationOffset);
                }

                return Cache.Get(cacheKey) as IEnumerable<StudentDrillDownQuery>;
            }

        }
    }
}