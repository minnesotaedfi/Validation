using System;
using System.Collections.Generic;
using System.Runtime.Caching;

using Validation.DataModels;

using ValidationWeb.Database.Queries;
using ValidationWeb.Services.Implementations;
using ValidationWeb.Services.Interfaces;

namespace ValidationWeb.DataCache
{
    public class CacheManager : ICacheManager
    {
        protected IRuleDefinitionService RuleDefinitionService { get; }

        public CacheManager(IRuleDefinitionService ruleDefinitionService)
        {
            // todo: inject cache factory and let it give us this or whatever else ObjectCache it wants
            Cache = MemoryCache.Default;
            RuleDefinitionService = ruleDefinitionService;
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
                // if (!Cache.Contains(cacheKey))
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
                // if (!Cache.Contains(cacheKey))
                {
                    var results = odsDataService.GetMultipleEnrollmentStudentDrillDown(
                        orgType,
                        schoolId,
                        edOrgId,
                        drillDownColumnIndex,
                        fourDigitSchoolYear);

                    Cache.Add(cacheKey, results, CacheExpirationOffset);
                }

                return Cache.Get(cacheKey) as IEnumerable<StudentDrillDownQuery>;
            }
        }

        public IEnumerable<StudentProgramsCountReportQuery> GetStudentProgramsCounts(
            IOdsDataService odsDataService,
            int? edOrgId,
            string fourDigitSchoolYear)
        {
            var cacheKey = $"GetStudentProgramsCounts_{edOrgId}_{fourDigitSchoolYear}";

            lock (LockObject)
            {
                if (!Cache.Contains(cacheKey))
                {
                    var results = odsDataService.GetStudentProgramsCounts(edOrgId, fourDigitSchoolYear);
                    Cache.Add(cacheKey, results, CacheExpirationOffset);
                }

                return Cache.Get(cacheKey) as IEnumerable<StudentProgramsCountReportQuery>;
            }
        }

        public IEnumerable<StudentDrillDownQuery> GetStudentProgramsStudentDrillDownData(
            IOdsDataService odsDataService,
            OrgType orgType,
            int? schoolId,
            int edOrgId,
            int drillDownColumnIndex,
            string fourDigitSchoolYear)
        {
            var cacheKey = $"GetStudentProgramsStudentDrillDownData_{orgType}_{schoolId}_{edOrgId}_{drillDownColumnIndex}_{fourDigitSchoolYear}";

            lock (LockObject)
            {
                if (!Cache.Contains(cacheKey))
                {
                    var results = odsDataService.GetStudentProgramsStudentDrillDown(
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

        public IEnumerable<ResidentsEnrolledElsewhereReportQuery> GetResidentsEnrolledElsewhereReport(
            IOdsDataService odsDataService,
            int? edOrgId,
            string fourDigitSchoolYear)
        {
            var cacheKey = $"GetResidentsEnrolledElsewhereReport_{edOrgId}_{fourDigitSchoolYear}";

            lock (LockObject)
            {
                if (!Cache.Contains(cacheKey))
                {
                    var results = odsDataService.GetResidentsEnrolledElsewhereReport(
                        edOrgId,
                        fourDigitSchoolYear);

                    Cache.Add(cacheKey, results, CacheExpirationOffset);
                }

                return Cache.Get(cacheKey) as IEnumerable<ResidentsEnrolledElsewhereReportQuery>;
            }
        }

        public IEnumerable<StudentDrillDownQuery> GetResidentsEnrolledElsewhereStudentDrillDown(
            IOdsDataService odsDataService,
            OrgType orgType,
            int? schoolId,
            int edOrgId,
            int drillDownColumnIndex,
            string fourDigitSchoolYear)
        {
            var cacheKey = $"GetResidentsEnrolledElsewhereStudentDrillDown_{orgType}_{schoolId}_{edOrgId}_{drillDownColumnIndex}_{fourDigitSchoolYear}";

            lock (LockObject)
            {
                if (!Cache.Contains(cacheKey))
                {
                    var results = odsDataService.GetResidentsEnrolledElsewhereStudentDrillDown(
                        edOrgId,
                        fourDigitSchoolYear);

                    Cache.Add(cacheKey, results, CacheExpirationOffset);
                }

                return Cache.Get(cacheKey) as IEnumerable<StudentDrillDownQuery>;
            }
        }

        public IEnumerable<ChangeOfEnrollmentReportQuery> GetChangeOfEnrollmentReport(
            IOdsDataService odsDataService,
            int edOrgId,
            string fourDigitSchoolYear)
        {
            var cacheKey = $"GetChangeOfEnrollmentReport_{edOrgId}_{fourDigitSchoolYear}";

            lock (LockObject)
            {
                if (!Cache.Contains(cacheKey))
                {
                    var results = odsDataService.GetChangeOfEnrollmentReport(
                        edOrgId,
                        fourDigitSchoolYear);

                    Cache.Add(cacheKey, results, CacheExpirationOffset);
                }

                return Cache.Get(cacheKey) as IEnumerable<ChangeOfEnrollmentReportQuery>;
            }
        }

        public IEnumerable<RulesetDefinition> GetRulesetDefinitions()
        {
            var cacheKey = "GetRulesetDefinitions";

            lock (LockObject)
            {
                if (!Cache.Contains(cacheKey))
                {
                    var results = RuleDefinitionService.GetRulesetDefinitions();
                    Cache.Add(cacheKey, results, CacheExpirationOffset);
                }

                return Cache.Get(cacheKey) as IEnumerable<RulesetDefinition>;
            }
        }
    }
}