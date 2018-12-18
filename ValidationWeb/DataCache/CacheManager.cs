namespace ValidationWeb.DataCache
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Caching;

    using ValidationWeb.Services;

    public class CacheManager : ICacheManager
    {
        public CacheManager()
        {
            // todo: inject cache factory and let it give us this or whatever else ObjectCache it wants
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
            int edOrgId,
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

#warning faked data - don't ship this!! - remove before flight
                    // TODO: remove before flight
                    if (!results.Any())
                    {
                        var fakeResults = new List<ChangeOfEnrollmentReportQuery>();
                        var oldDistrict = 12345;
                        var oldSchool = 2345; 
                        Random random = new Random(); // not cryptographically strong
                        for (var i = 0; i < 100; i++)
                        {
                            fakeResults.Add(
                                new ChangeOfEnrollmentReportQuery
                                {
                                    CurrentDistEdOrgId = edOrgId,
                                    CurrentGrade = ((i % 12) + 1).ToString(),
                                    CurrentDistrictName = "incoming district",
                                    CurrentEdOrgEnrollmentDate = DateTime.Now.Subtract(new TimeSpan(random.Next(1, 365), 0, 0, 0)),
                                    CurrentEdOrgExitDate = DateTime.Now.Subtract(new TimeSpan(random.Next(91, 180), 0, 0, 0)),
                                    IsCurrentDistrict = true,
                                    PastDistEdOrgId = oldDistrict,
                                    PastDistrictName = "Old District",
                                    PastSchoolEdOrgId = oldSchool,
                                    PastSchoolName = "Old School",
                                    PastEdOrgEnrollmentDate = DateTime.Now.Subtract(new TimeSpan(random.Next(365, 600), 0, 0, 0)),
                                    PastEdOrgExitDate = DateTime.Now.Subtract(new TimeSpan(random.Next(180, 364), 0, 0, 0)),
                                    StudentID = (1000 + i).ToString(),
                                    StudentBirthDate = new DateTime( random.Next(1990, 2010), random.Next(1, 12), random.Next(1, 28)),
                                    StudentFirstName = $"First_{i}",
                                    StudentMiddleName = "Q.",
                                    StudentLastName = $"Last_{i}",
                                    
                                });

                            fakeResults.Add(
                                new ChangeOfEnrollmentReportQuery
                                {
                                    CurrentDistEdOrgId = oldDistrict,
                                    PastGrade = ((i % 12) + 1).ToString(),
                                    PastDistrictName = "Old District",
                                    CurrentDistrictName = "incoming district",                                                                                          
                                    CurrentSchoolName = "New School",
                                    CurrentEdOrgEnrollmentDate = DateTime.Now.Subtract(new TimeSpan(random.Next(1, 365), 0, 0, 0)),
                                    PastEdOrgEnrollmentDate = DateTime.Now.Subtract(new TimeSpan(random.Next(180, 365), 0, 0, 0)),
                                    PastEdOrgExitDate = random.Next(0, 100) % 10 != 0 ? (DateTime?)DateTime.Now.Subtract(new TimeSpan(random.Next(1, 180), 0, 0, 0)) : null,
                                    IsCurrentDistrict = false,
                                    StudentID = (2000 + i).ToString(),
                                    StudentBirthDate = new DateTime(random.Next(1990, 2010), random.Next(1, 12), random.Next(1, 28)),
                                    StudentFirstName = $"First_{i+2000}",
                                    StudentMiddleName = "P.",
                                    StudentLastName = $"Last_{i + 2000}"
                                });
                        }

                        var blah = fakeResults.Where(x => !x.IsCurrentDistrict)
                            .OrderByDescending(x => x.PastEdOrgExitDate);

                        results = fakeResults; 
                    }

                    Cache.Add(cacheKey, results, CacheExpirationOffset);
                }

                return Cache.Get(cacheKey) as IEnumerable<ChangeOfEnrollmentReportQuery>;
            }
        }
    }
}