using ValidationWeb.Filters;

namespace ValidationWeb
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    using DataTables.AspNet.Core;
    using DataTables.AspNet.Mvc5;

    using Engine.Models;

    using MoreLinq;

    using ValidationWeb.ApiControllers.ModelBinders;
    using ValidationWeb.DataCache;
    using ValidationWeb.Models;
    using ValidationWeb.Services;
    using ValidationWeb.Services.Implementations;
    using ValidationWeb.Utility;
    using ValidationWeb.ViewModels;

    [PortalAuthorize(Roles = "DataOwner,DistrictUser,HelpDesk")]
    // TODO: refactor repeated code. move cache manager and serialization calls into a separate layer. -pocky
    public class OdsController : Controller
    {
        public OdsController(
            IAppUserService appUserService,
            IEdOrgService edOrgService,
            IOdsDataService odsDataService,
            IRulesEngineService rulesEngineService,
            ISchoolYearService schoolYearService,
            ICacheManager cacheManager,
            IEdFiApiLogService apiLogService,
            Model engineObjectModel)
        {
            AppUserService = appUserService;
            EdOrgService = edOrgService;
            OdsDataService = odsDataService;
            EngineObjectModel = engineObjectModel;
            RulesEngineService = rulesEngineService;
            SchoolYearService = schoolYearService;
            CacheManager = cacheManager;
            ApiLogService = apiLogService;
        }

        protected IAppUserService AppUserService { get; set; }

        protected IEdOrgService EdOrgService { get; set; }

        protected IOdsDataService OdsDataService { get; set; }

        protected IRulesEngineService RulesEngineService { get; set; }

        protected ISchoolYearService SchoolYearService { get; set; }

        protected ICacheManager CacheManager { get; set; }

        protected IEdFiApiLogService ApiLogService { get; set; }

        protected Model EngineObjectModel { get; set; }

        // GET: Ods/Reports
        public ActionResult Reports()
        {
            var session = AppUserService.GetSession();
            var edOrg = EdOrgService.GetEdOrgById(session.FocusedEdOrgId, session.FocusedSchoolYearId);
            var edOrgName = (edOrg == null) ? "Invalid Education Organization Selected" : edOrg.OrganizationName;
            var edOrgId = edOrg.Id;
            var theUser = AppUserService.GetUser();
            var model = new OdsReportsViewModel
            {
                EdOrgId = edOrgId,
                EdOrgName = edOrgName,
                User = theUser
            };
            return View(model);
        }

        // GET: Ods/DemographicsReport
        public ActionResult DemographicsReport(
            bool isStateMode = false,
            int? districtToDisplay = null,
            bool isStudentDrillDown = false,
            int? schoolId = null,
            int? drillDownColumnIndex = null,
            OrgType orgType = OrgType.District)
        {
            var session = AppUserService.GetSession();
            var edOrg = EdOrgService.GetEdOrgById(session.FocusedEdOrgId, session.FocusedSchoolYearId);
            var edOrgName = (edOrg == null) ? "Invalid Education Organization Selected" : edOrg.OrganizationName;
            var edOrgId = edOrg.Id;

            // A state user can look at any district via a link, without changing the default district.
            if (districtToDisplay.HasValue && session.UserIdentity.AuthorizedEdOrgs.Select(eorg => eorg.Id).Contains(districtToDisplay.Value))
            {
                edOrgId = districtToDisplay.Value;
                edOrg = EdOrgService.GetEdOrgById(edOrgId, session.FocusedSchoolYearId);
                edOrgName = (edOrg == null) ? "Invalid Education Organization Selected" : edOrg.OrganizationName;
            }

            string schoolName = string.Empty;

            if (orgType == OrgType.School && schoolId.HasValue)
            {
                schoolName = EdOrgService.GetSingleEdOrg(schoolId.Value, session.FocusedSchoolYearId)?.OrganizationName;
            }

            var fourDigitSchoolYear = SchoolYearService.GetSchoolYearById(session.FocusedSchoolYearId).EndYear;
            var theUser = AppUserService.GetUser();

            if (isStudentDrillDown)
            {
                var studentDrillDownModel = new StudentDrillDownViewModel
                {
                    ReportName = "Race and Ancestry Ethnic Origin",
                    EdOrgId = edOrgId,
                    EdOrgName = edOrgName,
                    User = theUser,
                    FourDigitSchoolYear = fourDigitSchoolYear,
                    DrillDownColumnIndex = drillDownColumnIndex,
                    IsStateMode = isStateMode,
                    SchoolId = schoolId,
                    OrgType = orgType,
                    SchoolName = schoolName,
                    ReportType = StudentDrillDownReportType.Demographics
                };

                return View("StudentDrillDown", studentDrillDownModel);
            }

            var model = new OdsDemographicsReportViewModel
            {
                EdOrgId = edOrgId,
                FourDigitSchoolYear = fourDigitSchoolYear,
                EdOrgName = edOrgName,
                User = theUser,
                DistrictToDisplay = districtToDisplay,
                IsStudentDrillDown = isStudentDrillDown,
                IsStateMode = isStateMode,
                SchoolId = schoolId,
                SchoolName = schoolName
            };

            return View(model);
        }

        public JsonResult GetStudentDrilldownData(
            OrgType orgType,
            int? schoolId,
            int edOrgId,
            int drillDownColumnIndex,
            string fourDigitSchoolYear,
            StudentDrillDownReportType reportType,
            IDataTablesRequest request)
        {
#if DEBUG
            var startTime = DateTime.Now;
#endif
            IEnumerable<StudentDrillDownQuery> results;
            Func<IOdsDataService, OrgType, int?, int, int, string, IEnumerable<StudentDrillDownQuery>> queryFunction;

            switch (reportType)
            {
                case StudentDrillDownReportType.Demographics:
                    queryFunction = CacheManager.GetStudentDemographicsDrilldownData;
                    break;
                case StudentDrillDownReportType.MultipleEnrollment:
                    queryFunction = CacheManager.GetStudentMultipleEnrollmentsDrilldownData;
                    break;
                case StudentDrillDownReportType.Programs:
                    queryFunction = CacheManager.GetStudentProgramsStudentDrillDownData;
                    break;
                case StudentDrillDownReportType.EnrolledElsewhere:
                    queryFunction = CacheManager.GetResidentsEnrolledElsewhereStudentDrillDown;
                    break;
                default:
                    throw new InvalidOperationException($"reportType of {reportType} not supported");
            }

            results = queryFunction(
                OdsDataService,
                orgType,
                schoolId,
                edOrgId,
                drillDownColumnIndex,
                fourDigitSchoolYear);

            // the underlying sql query is returning for all schools even though we're trying to view for one school 
            if (reportType != StudentDrillDownReportType.MultipleEnrollment && (orgType == OrgType.School && schoolId.HasValue))
            {
                results = results.Where(x => x.SchoolId == schoolId);
            }

            if (reportType == StudentDrillDownReportType.EnrolledElsewhere)
            {
                results = results.Where(x => x.DistrictId == schoolId);
            }

            // some drilldown reports show apparent dupes, but they're only distinct by undisplayed fields
            // distinct them by basically everything but SpecialEdStatus
            if (reportType == StudentDrillDownReportType.Demographics || reportType == StudentDrillDownReportType.Programs)
            {
                results = results.DistinctBy(x =>
                    new
                    {
                        x.StudentId,
                        StudentName = $"{x.StudentLastName}, {x.StudentFirstName} {x.StudentMiddleName}",
                    });
            }
            else
            {
                results = results.DistinctBy(x =>
                    new
                    {
                        x.StudentId,
                        StudentName = $"{x.StudentLastName}, {x.StudentFirstName} {x.StudentMiddleName}",
                        x.DistrictName,
                        x.SchoolName,
                        x.Grade,
                        x.DistrictId,
                        x.EnrolledDate,
                        x.WithdrawDate
                    });
            }

#if DEBUG
            System.Diagnostics.Debug.WriteLine($"GetStudentDrillDownData ({reportType}): {(DateTime.Now - startTime).Milliseconds}ms");
#endif 

            var sortedResults = results;

            var sortColumn = request.Columns.FirstOrDefault(x => x.Sort != null);
            if (sortColumn != null)
            {
                Func<StudentDrillDownQuery, string> orderingFunctionString = null;
                Func<StudentDrillDownQuery, int?> orderingFunctionNullableInt = null;
                Func<StudentDrillDownQuery, int> orderingFunctionInt = null;
                Func<StudentDrillDownQuery, DateTime?> orderingFunctionNullableDateTime = null;

                switch (sortColumn.Name)
                {
                    case "studentId":
                        {
                            orderingFunctionString = x => x.StudentId;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? results.OrderBy(orderingFunctionString)
                                                : results.OrderByDescending(orderingFunctionString);
                            break;
                        }
                    case "studentName":
                        {
                            // watch this implementation detail! name is being stitched together in javascript now --pocky
                            orderingFunctionString = x => $"{x.StudentLastName}, {x.StudentFirstName} {x.StudentMiddleName}";
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? results.OrderBy(orderingFunctionString)
                                                : results.OrderByDescending(orderingFunctionString);
                            break;
                        }
                    case "schoolId":
                        {
                            orderingFunctionNullableInt = x => x.SchoolId;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? results.OrderBy(orderingFunctionNullableInt)
                                                : results.OrderByDescending(orderingFunctionNullableInt);
                            break;
                        }
                    case "districtId":
                        {
                            orderingFunctionNullableInt = x => x.DistrictId;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? results.OrderBy(orderingFunctionNullableInt)
                                                : results.OrderByDescending(orderingFunctionNullableInt);
                            break;
                        }
                    case "schoolName":
                        {
                            orderingFunctionString = x => x.SchoolName;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? results.OrderBy(orderingFunctionString)
                                                : results.OrderByDescending(orderingFunctionString);
                            break;
                        }
                    case "enrolledDate":
                        {
                            orderingFunctionNullableDateTime = x => x.EnrolledDate;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? results.OrderBy(orderingFunctionNullableDateTime)
                                                : results.OrderByDescending(orderingFunctionNullableDateTime);
                            break;
                        }
                    case "withdrawDate":
                        {
                            orderingFunctionNullableDateTime = x => x.WithdrawDate;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? results.OrderBy(orderingFunctionNullableDateTime)
                                                : results.OrderByDescending(orderingFunctionNullableDateTime);
                            break;
                        }
                    case "grade":
                        {
                            orderingFunctionString = x => x.Grade;
                            var comparer = new GradeLevelComparer();
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? sortedResults.OrderBy(orderingFunctionString, comparer)
                                                : sortedResults.OrderByDescending(orderingFunctionString, comparer);
                            break;
                        }
                    case "specialEdStatus":
                        {
                            orderingFunctionString = x => x.SpecialEdStatus;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? sortedResults.OrderBy(orderingFunctionString)
                                                : sortedResults.OrderByDescending(orderingFunctionString);
                            break;
                        }
                    default:
                        {
                            sortedResults = results;
                            break;
                        }
                }
            }

            var pagedResults = sortedResults.Skip(request.Start).Take(request.Length);
            var response = DataTablesResponse.Create(request, results.Count(), results.Count(), pagedResults);
            var jsonResult = new DataTablesJsonResult(response, JsonRequestBehavior.AllowGet);
            return jsonResult;
        }

        public JsonResult GetDemographicsReportData(
            int edOrgId,
            string fourDigitSchoolYear,
            bool isStateMode,
            IDataTablesRequest request)
        {
#if DEBUG
            var startTime = DateTime.Now;
#endif
            var results = CacheManager.GetDistrictAncestryRaceCounts(
                OdsDataService,
                isStateMode,
                edOrgId,
                fourDigitSchoolYear);
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"GetDistrictAncestryRaceCounts: {(DateTime.Now - startTime).Milliseconds}ms");
#endif 
            IEnumerable<DemographicsCountReportQuery> sortedResults = results;

            var sortColumn = request.Columns.FirstOrDefault(x => x.Sort != null);
            if (sortColumn != null)
            {
                Func<DemographicsCountReportQuery, string> orderingFunctionString = null;
                Func<DemographicsCountReportQuery, int?> orderingFunctionNullableInt = null;
                Func<DemographicsCountReportQuery, int> orderingFunctionInt = null;

                switch (sortColumn.Field)
                {
                    case "edOrgId":
                        {
                            orderingFunctionNullableInt = x => x.EdOrgId;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? results.OrderBy(orderingFunctionNullableInt)
                                                : results.OrderByDescending(orderingFunctionNullableInt);
                            break;
                        }
                    case "leaSchool":
                        {
                            orderingFunctionString = x => x.LEASchool;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? results.OrderBy(orderingFunctionString)
                                                : results.OrderByDescending(orderingFunctionString);
                            break;
                        }
                    case "enrollmentCount":
                        {
                            orderingFunctionInt = x => x.EnrollmentCount;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? results.OrderBy(orderingFunctionInt)
                                                : results.OrderByDescending(orderingFunctionInt);
                            break;
                        }
                    case "demographicsCount":
                        {
                            orderingFunctionInt = x => x.DemographicsCount;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? results.OrderBy(orderingFunctionInt)
                                                : results.OrderByDescending(orderingFunctionInt);
                            break;
                        }
                    case "raceGivenCount":
                        {
                            orderingFunctionInt = x => x.RaceGivenCount;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? results.OrderBy(orderingFunctionInt)
                                                : results.OrderByDescending(orderingFunctionInt);
                            break;
                        }
                    case "ancestryGivenCount":
                        {
                            orderingFunctionInt = x => x.AncestryGivenCount;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? results.OrderBy(orderingFunctionInt)
                                                : results.OrderByDescending(orderingFunctionInt);
                            break;
                        }
                    default:
                        {
                            sortedResults = results;
                            break;
                        }
                }
            }

            var pagedResults = sortedResults.Skip(request.Start).Take(request.Length);
            var response = DataTablesResponse.Create(request, results.Count(), results.Count(), pagedResults);
            var jsonResult = new DataTablesJsonResult(response, JsonRequestBehavior.AllowGet);
            return jsonResult;
        }

        // GET: Ods/MultipleEnrollmentsReport
        public ActionResult MultipleEnrollmentsReport(
            bool isStateMode = false,
            int? districtToDisplay = null,
            bool isStudentDrillDown = false,
            int? schoolId = null,
            int? drillDownColumnIndex = null,
            OrgType orgType = OrgType.District)
        {
            var session = AppUserService.GetSession();
            var edOrg = EdOrgService.GetEdOrgById(session.FocusedEdOrgId, session.FocusedSchoolYearId);
            var edOrgName = (edOrg == null) ? "Invalid Education Organization Selected" : edOrg.OrganizationName;
            var edOrgId = edOrg.Id;

            // A state user can look at any district via a link, without changing the default district.
            if (districtToDisplay.HasValue && session.UserIdentity.AuthorizedEdOrgs.Select(eorg => eorg.Id).Contains(districtToDisplay.Value))
            {
                edOrgId = districtToDisplay.Value;
                edOrg = EdOrgService.GetEdOrgById(edOrgId, session.FocusedSchoolYearId);
                edOrgName = (edOrg == null) ? "Invalid Education Organization Selected" : edOrg.OrganizationName;
            }

            string schoolName = string.Empty;

            if (orgType == OrgType.School && schoolId.HasValue)
            {
                schoolName = EdOrgService.GetSingleEdOrg(schoolId.Value, session.FocusedSchoolYearId)?.OrganizationName;
            }

            var fourDigitSchoolYear = SchoolYearService.GetSchoolYearById(session.FocusedSchoolYearId).EndYear;
            var theUser = AppUserService.GetUser();
            if (isStudentDrillDown)
            {
                var studentDrillDownModel = new StudentDrillDownViewModel
                {
                    ReportName = "Multiple Enrollment",
                    EdOrgId = edOrgId,
                    EdOrgName = edOrgName,
                    User = theUser,
                    FourDigitSchoolYear = fourDigitSchoolYear,
                    DrillDownColumnIndex = drillDownColumnIndex,
                    IsStateMode = isStateMode,
                    SchoolId = schoolId,
                    OrgType = orgType,
                    SchoolName = schoolName,
                    ReportType = StudentDrillDownReportType.MultipleEnrollment
                };
                return View("StudentDrillDown", studentDrillDownModel);
            }

            var model = new OdsMultipleEnrollmentsReportViewModel
            {
                EdOrgId = edOrgId,
                EdOrgName = edOrgName,
                User = theUser,
                IsStateMode = isStateMode,
                SchoolName = schoolName,
                SchoolId = schoolId,
                FourDigitSchoolYear = fourDigitSchoolYear
            };
            return View(model);
        }

        public JsonResult GetMultipleEnrollmentCountsData(
            bool isStateMode,
            int edOrgId,
            string fourDigitSchoolYear,
            IDataTablesRequest request)
        {
#if DEBUG
            var startTime = DateTime.Now;
#endif
            var results = CacheManager.GetMultipleEnrollmentCounts(
                OdsDataService,
                isStateMode ? (int?)null : edOrgId,
                fourDigitSchoolYear);
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"GetMultipleEnrollmentCounts: {(DateTime.Now - startTime).Milliseconds}ms");
#endif 
            IEnumerable<MultipleEnrollmentsCountReportQuery> sortedResults = results;

            var sortColumn = request.Columns.FirstOrDefault(x => x.Sort != null);
            if (sortColumn != null)
            {
                Func<MultipleEnrollmentsCountReportQuery, string> orderingFunctionString = null;
                Func<MultipleEnrollmentsCountReportQuery, int?> orderingFunctionNullableInt = null;
                Func<MultipleEnrollmentsCountReportQuery, int> orderingFunctionInt = null;

                switch (sortColumn.Field)
                {
                    case "edOrgId":
                        {
                            orderingFunctionNullableInt = x => x.EdOrgId;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? results.OrderBy(orderingFunctionNullableInt)
                                                : results.OrderByDescending(orderingFunctionNullableInt);
                            break;
                        }
                    case "leaSchool":
                        {
                            orderingFunctionString = x => x.LEASchool;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? results.OrderBy(orderingFunctionString)
                                                : results.OrderByDescending(orderingFunctionString);
                            break;
                        }
                    case "distinctEnrollmentCount":
                        {
                            orderingFunctionInt = x => x.DistinctEnrollmentCount;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? results.OrderBy(orderingFunctionInt)
                                                : results.OrderByDescending(orderingFunctionInt);
                            break;
                        }
                    case "enrolledInOtherSchoolsCount":
                        {
                            orderingFunctionInt = x => x.EnrolledInOtherSchoolsCount;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? results.OrderBy(orderingFunctionInt)
                                                : results.OrderByDescending(orderingFunctionInt);
                            break;
                        }
                    case "enrolledInOtherDistrictsCount":
                        {
                            orderingFunctionInt = x => x.EnrolledInOtherDistrictsCount;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? results.OrderBy(orderingFunctionInt)
                                                : results.OrderByDescending(orderingFunctionInt);
                            break;
                        }
                    default:
                        {
                            sortedResults = results;
                            break;
                        }
                }
            }

            var pagedResults = sortedResults.Skip(request.Start).Take(request.Length);
            var response = DataTablesResponse.Create(request, results.Count(), results.Count(), pagedResults);
            var jsonResult = new DataTablesJsonResult(response, JsonRequestBehavior.AllowGet);
            return jsonResult;
        }

        // GET: Ods/StudentProgramsReport
        public ActionResult StudentProgramsReport(
            bool isStateMode = false,
            int? districtToDisplay = null,
            bool isStudentDrillDown = false,
            int? schoolId = null,
            int? drillDownColumnIndex = null,
            OrgType orgType = OrgType.District)
        {
            var session = AppUserService.GetSession();
            var edOrg = EdOrgService.GetEdOrgById(session.FocusedEdOrgId, session.FocusedSchoolYearId);
            var edOrgName = (edOrg == null) ? "Invalid Education Organization Selected" : edOrg.OrganizationName;
            var edOrgId = edOrg.Id;

            // A state user can look at any district via a link, without changing the default district.
            if (districtToDisplay.HasValue && session.UserIdentity.AuthorizedEdOrgs.Select(eorg => eorg.Id).Contains(districtToDisplay.Value))
            {
                edOrgId = districtToDisplay.Value;
                edOrg = EdOrgService.GetEdOrgById(edOrgId, session.FocusedSchoolYearId);
                edOrgName = (edOrg == null) ? "Invalid Education Organization Selected" : edOrg.OrganizationName;
            }

            string schoolName = string.Empty;

            if (orgType == OrgType.School && schoolId.HasValue)
            {
                schoolName = EdOrgService.GetSingleEdOrg(schoolId.Value, session.FocusedSchoolYearId)?.OrganizationName;
            }

            var fourDigitSchoolYear = SchoolYearService.GetSchoolYearById(session.FocusedSchoolYearId).EndYear;
            var theUser = AppUserService.GetUser();
            if (isStudentDrillDown)
            {
                var studentDrillDownModel = new StudentDrillDownViewModel
                {
                    ReportName = "Student Characteristics and Program Participation",
                    EdOrgId = edOrgId,
                    EdOrgName = edOrgName,
                    User = theUser,
                    FourDigitSchoolYear = fourDigitSchoolYear,
                    DrillDownColumnIndex = drillDownColumnIndex,
                    IsStateMode = isStateMode,
                    SchoolId = schoolId,
                    OrgType = orgType,
                    SchoolName = schoolName,
                    ReportType = StudentDrillDownReportType.Programs
                };
                return View("StudentDrillDown", studentDrillDownModel);
            }

            var model = new OdsStudentProgramsReportViewModel
            {
                EdOrgId = edOrgId,
                EdOrgName = edOrgName,
                User = theUser,
                FourDigitSchoolYear = fourDigitSchoolYear,
                SchoolName = schoolName,
                SchoolId = schoolId,
                IsStateMode = isStateMode
            };
            return View(model);
        }

        public JsonResult GetStudentProgramsReportData(
            int edOrgId,
            string fourDigitSchoolYear,
            bool isStateMode,
            IDataTablesRequest request)
        {
#if DEBUG
            var startTime = DateTime.Now;
#endif
            var results = CacheManager.GetStudentProgramsCounts(
                OdsDataService,
                isStateMode ? (int?)null : edOrgId,
                fourDigitSchoolYear);
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"GetStudentProgramsCounts: {(DateTime.Now - startTime).Milliseconds}ms");
#endif 
            IEnumerable<StudentProgramsCountReportQuery> sortedResults = results;

            var sortColumn = request.Columns.FirstOrDefault(x => x.Sort != null);
            if (sortColumn != null)
            {
                Func<StudentProgramsCountReportQuery, string> orderingFunctionString;
                Func<StudentProgramsCountReportQuery, int?> orderingFunctionNullableInt;
                Func<StudentProgramsCountReportQuery, int> orderingFunctionInt;

                switch (sortColumn.Field)
                {
                    case "edOrgId":
                        {
                            orderingFunctionNullableInt = x => x.EdOrgId;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? results.OrderBy(orderingFunctionNullableInt)
                                                : results.OrderByDescending(orderingFunctionNullableInt);
                            break;
                        }
                    case "leaSchool":
                        {
                            orderingFunctionString = x => x.LEASchool;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? results.OrderBy(orderingFunctionString)
                                                : results.OrderByDescending(orderingFunctionString);
                            break;
                        }
                    case "distinctEnrollmentCount":
                        {
                            orderingFunctionInt = x => x.DistinctEnrollmentCount;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? results.OrderBy(orderingFunctionInt)
                                                : results.OrderByDescending(orderingFunctionInt);
                            break;
                        }
                    case "distinctDemographicsCount":
                        {
                            orderingFunctionInt = x => x.DistinctDemographicsCount;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? results.OrderBy(orderingFunctionInt)
                                                : results.OrderByDescending(orderingFunctionInt);
                            break;
                        }
                    case "adParentCount":
                        {
                            orderingFunctionInt = x => x.ADParentCount;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? results.OrderBy(orderingFunctionInt)
                                                : results.OrderByDescending(orderingFunctionInt);
                            break;
                        }
                    case "indianNativeCount":
                        {
                            orderingFunctionInt = x => x.IndianNativeCount;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? results.OrderBy(orderingFunctionInt)
                                                : results.OrderByDescending(orderingFunctionInt);
                            break;
                        }
                    case "migrantCount":
                        {
                            orderingFunctionInt = x => x.MigrantCount;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? results.OrderBy(orderingFunctionInt)
                                                : results.OrderByDescending(orderingFunctionInt);
                            break;
                        }
                    case "homelessCount":
                        {
                            orderingFunctionInt = x => x.HomelessCount;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? results.OrderBy(orderingFunctionInt)
                                                : results.OrderByDescending(orderingFunctionInt);
                            break;
                        }
                    case "immigrantCount":
                        {
                            orderingFunctionInt = x => x.ImmigrantCount;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? results.OrderBy(orderingFunctionInt)
                                                : results.OrderByDescending(orderingFunctionInt);
                            break;
                        }
                    case "recentEnglishCount":
                        {
                            orderingFunctionInt = x => x.RecentEnglishCount;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? results.OrderBy(orderingFunctionInt)
                                                : results.OrderByDescending(orderingFunctionInt);
                            break;
                        }
                    case "slifeCount":
                        {
                            orderingFunctionInt = x => x.SLIFECount;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? results.OrderBy(orderingFunctionInt)
                                                : results.OrderByDescending(orderingFunctionInt);
                            break;
                        }
                    case "englishLearnerIdentifiedCount":
                        {
                            orderingFunctionInt = x => x.EnglishLearnerIdentifiedCount;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? results.OrderBy(orderingFunctionInt)
                                                : results.OrderByDescending(orderingFunctionInt);
                            break;
                        }
                    case "englishLearnerServedCount":
                        {
                            orderingFunctionInt = x => x.EnglishLearnerServedCount;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? results.OrderBy(orderingFunctionInt)
                                                : results.OrderByDescending(orderingFunctionInt);
                            break;
                        }
                    case "independentStudyCount":
                        {
                            orderingFunctionInt = x => x.IndependentStudyCount;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? results.OrderBy(orderingFunctionInt)
                                                : results.OrderByDescending(orderingFunctionInt);
                            break;
                        }
                    case "section504Count":
                        {
                            orderingFunctionInt = x => x.Section504Count;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? results.OrderBy(orderingFunctionInt)
                                                : results.OrderByDescending(orderingFunctionInt);
                            break;
                        }
                    case "title1PartACount":
                        {
                            orderingFunctionInt = x => x.Title1PartACount;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? results.OrderBy(orderingFunctionInt)
                                                : results.OrderByDescending(orderingFunctionInt);
                            break;
                        }
                    case "freeReducedCount":
                        {
                            orderingFunctionInt = x => x.FreeReducedCount;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? results.OrderBy(orderingFunctionInt)
                                                : results.OrderByDescending(orderingFunctionInt);
                            break;
                        }
                    default:
                        {
                            throw new InvalidOperationException($"Unknown field {sortColumn.Field}");
                        }
                }
            }

            var pagedResults = sortedResults.Skip(request.Start).Take(request.Length);
            var response = DataTablesResponse.Create(request, results.Count(), results.Count(), pagedResults);
            var jsonResult = new DataTablesJsonResult(response, JsonRequestBehavior.AllowGet);
            return jsonResult;
        }

        // GET: Ods/ChangeOfEnrollmentReport
        public ActionResult ChangeOfEnrollmentReport()
        {
            var session = AppUserService.GetSession();
            var edOrg = EdOrgService.GetEdOrgById(session.FocusedEdOrgId, session.FocusedSchoolYearId);
            var edOrgName = (edOrg == null) ? "Invalid Education Organization Selected" : edOrg.OrganizationName;
            var edOrgId = edOrg.Id;
            var fourDigitSchoolYear = SchoolYearService.GetSchoolYearById(session.FocusedSchoolYearId).EndYear;
            var theUser = AppUserService.GetUser();
            var model = new OdsChangeOfEnrollmentReportViewModel
            {
                EdOrgId = edOrgId,
                EdOrgName = edOrgName,
                User = theUser,
                FourDigitSchoolYear = fourDigitSchoolYear
            };
            return View(model);
        }

        public JsonResult GetChangeOfEnrollmentReportData(
            int edOrgId,
            string fourDigitSchoolYear,
            bool isCurrentDistrict,
            IDataTablesRequest request)
        {
#if DEBUG
            var startTime = DateTime.Now;
#endif
            var results = CacheManager.GetChangeOfEnrollmentReport(
                OdsDataService,
                edOrgId,
                fourDigitSchoolYear);

            var recordsRequestStudentIds = OdsDataService.GetAllRecordsRequests().Select(x => x.StudentId);
            results.Where(x => recordsRequestStudentIds.Contains(x.StudentID)).ForEach(y => y.HasRecordsRequest = true);

#if DEBUG
            System.Diagnostics.Debug.WriteLine($"GetChangeOfEnrollmentReport: {(DateTime.Now - startTime).Milliseconds}ms");
#endif
            results = results.Where(x => x.IsCurrentDistrict == isCurrentDistrict);

            var filteredResults = results;

            var filterColumn = request.Columns.FirstOrDefault(x => x.Search != null && !string.IsNullOrWhiteSpace(x.Search.Value));
            if (filterColumn != null)
            {
                DateTime minDate = DateTime.MinValue;

                switch (filterColumn.Search.Value)
                {
                    case "30days":
                        {
                            minDate = DateTime.Now.AddDays(-30).Date;
                            break;
                        }
                    case "90days":
                        {
                            minDate = DateTime.Now.AddDays(-90).Date;
                            break;
                        }
                    case "1year":
                        {
                            minDate = DateTime.Now.AddYears(-1).Date;
                            break;
                        }
                    default:
                        throw new InvalidOperationException($"Unknown date option {filterColumn.Search.Value}");
                }

                switch (filterColumn.Name)
                {
                    case "currentEdOrgEnrollmentDate":
                        filteredResults = filteredResults.Where(x => x.CurrentEdOrgEnrollmentDate != null && x.CurrentEdOrgEnrollmentDate >= minDate);
                        break;

                        // default:
                        //    throw new InvalidOperationException($"Unknown search field {filterColumn.Name}");
                }
            }

            IEnumerable<ChangeOfEnrollmentReportQuery> sortedResults = filteredResults;

            var sortColumn = request.Columns.FirstOrDefault(x => x.Sort != null);
            if (sortColumn != null)
            {
                Func<ChangeOfEnrollmentReportQuery, string> orderingFunctionString = null;
                Func<ChangeOfEnrollmentReportQuery, bool> orderingFunctionBool = null;
                Func<ChangeOfEnrollmentReportQuery, DateTime?> orderingFunctionNullableDateTime = null;
                Func<ChangeOfEnrollmentReportQuery, int> orderingFunctionInt = null;

                switch (sortColumn.Name)
                {
                    case "studentID":
                        {
                            orderingFunctionInt = x => int.Parse(x.StudentID);
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? sortedResults.OrderBy(orderingFunctionInt)
                                                : sortedResults.OrderByDescending(orderingFunctionInt);
                            break;
                        }
                    case "studentName":
                        {
                            orderingFunctionString = x => $"{x.StudentLastName}, {x.StudentFirstName} {x.StudentMiddleName}";
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? sortedResults.OrderBy(orderingFunctionString)
                                                : sortedResults.OrderByDescending(orderingFunctionString);
                            break;
                        }
                    case "studentBirthDate":
                        {
                            orderingFunctionNullableDateTime = x => x.StudentBirthDate;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? sortedResults.OrderBy(orderingFunctionNullableDateTime)
                                                : sortedResults.OrderByDescending(orderingFunctionNullableDateTime);
                            break;
                        }
                    case "currentGrade":
                        {
                            orderingFunctionString = x => x.CurrentGrade;
                            var comparer = new GradeLevelComparer();
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? sortedResults.OrderBy(orderingFunctionString, comparer)
                                                : sortedResults.OrderByDescending(orderingFunctionString, comparer);
                            break;
                        }
                    case "pastGrade":
                        {
                            orderingFunctionString = x => x.PastGrade;
                            var comparer = new GradeLevelComparer();
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? sortedResults.OrderBy(orderingFunctionString, comparer)
                                                : sortedResults.OrderByDescending(orderingFunctionString, comparer);
                            break;
                        }
                    case "pastEdOrgEnrollmentDate":
                        {
                            orderingFunctionNullableDateTime = x => x.PastEdOrgEnrollmentDate;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? sortedResults.OrderBy(orderingFunctionNullableDateTime)
                                                : sortedResults.OrderByDescending(orderingFunctionNullableDateTime);
                            break;
                        }
                    case "pastEdOrgExitDate":
                        {
                            orderingFunctionNullableDateTime = x => x.PastEdOrgExitDate;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? sortedResults.OrderBy(orderingFunctionNullableDateTime)
                                                : sortedResults.OrderByDescending(orderingFunctionNullableDateTime);
                            break;
                        }
                    case "currentDistEdOrgId":
                        {
                            orderingFunctionInt = x => x.CurrentDistEdOrgId;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? sortedResults.OrderBy(orderingFunctionInt)
                                                : sortedResults.OrderByDescending(orderingFunctionInt);
                            break;
                        }
                    case "currentEdOrgEnrollmentDate":
                        {
                            orderingFunctionNullableDateTime = x => x.CurrentEdOrgEnrollmentDate;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? sortedResults.OrderBy(orderingFunctionNullableDateTime)
                                                : sortedResults.OrderByDescending(orderingFunctionNullableDateTime);
                            break;
                        }
                    case "currentEdOrgExitDate":
                        {
                            orderingFunctionNullableDateTime = x => x.CurrentEdOrgExitDate;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? sortedResults.OrderBy(orderingFunctionNullableDateTime)
                                                : sortedResults.OrderByDescending(orderingFunctionNullableDateTime);
                            break;
                        }
                    case "currentDistrictName":
                        {
                            orderingFunctionString = x => x.CurrentDistrictName;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? sortedResults.OrderBy(orderingFunctionString)
                                                : sortedResults.OrderByDescending(orderingFunctionString);
                            break;
                        }
                    case "pastDistrictName":
                        {
                            orderingFunctionString = x => x.PastDistrictName;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? sortedResults.OrderBy(orderingFunctionString)
                                                : sortedResults.OrderByDescending(orderingFunctionString);
                            break;
                        }
                    case "currentSchoolName":
                        {
                            orderingFunctionString = x => x.CurrentSchoolName;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? sortedResults.OrderBy(orderingFunctionString)
                                                : sortedResults.OrderByDescending(orderingFunctionString);
                            break;
                        }
                    case "recordsRequested":
                        {
                            orderingFunctionBool = x => x.HasRecordsRequest;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? sortedResults.OrderBy(orderingFunctionBool)
                                                : sortedResults.OrderByDescending(orderingFunctionBool);
                            break;
                        }
                    default:
                        {
                            throw new InvalidOperationException($"Unknown field {sortColumn.Field}");
                        }
                }
            }

            var pagedResults = sortedResults.Skip(request.Start).Take(request.Length);
            var response = DataTablesResponse.Create(request, results.Count(), filteredResults.Count(), pagedResults);
            var jsonResult = new DataTablesJsonResult(response, JsonRequestBehavior.AllowGet);
            return jsonResult;
        }

        // GET: Ods/ResidentsEnrolledElsewhereReport
        public ActionResult ResidentsEnrolledElsewhereReport(
            bool isStateMode = false,
            int? districtToDisplay = null,
            bool isStudentDrillDown = false,
            int? schoolId = null)
        {
            var session = AppUserService.GetSession();
            var edOrg = EdOrgService.GetEdOrgById(session.FocusedEdOrgId, session.FocusedSchoolYearId);
            var edOrgName = (edOrg == null) ? "Invalid Education Organization Selected" : edOrg.OrganizationName;
            var edOrgId = edOrg.Id;

            // A state user can look at any district via a link, without changing the default district.
            if (districtToDisplay.HasValue && session.UserIdentity.AuthorizedEdOrgs.Select(eorg => eorg.Id).Contains(districtToDisplay.Value))
            {
                edOrgId = districtToDisplay.Value;
                edOrg = EdOrgService.GetEdOrgById(edOrgId, session.FocusedSchoolYearId);
                edOrgName = (edOrg == null) ? "Invalid Education Organization Selected" : edOrg.OrganizationName;
            }

            var fourDigitSchoolYear = SchoolYearService.GetSchoolYearById(session.FocusedSchoolYearId).EndYear;
            var theUser = AppUserService.GetUser();
            if (isStudentDrillDown)
            {
                var studentDrillDownModel = new StudentDrillDownViewModel
                {
                    ReportName = "Residents Enrolled Elsewhere",
                    EdOrgId = edOrgId,
                    SchoolId = schoolId,
                    DrillDownColumnIndex = 0,
                    EdOrgName = edOrgName,
                    User = theUser,
                    IsStateMode = isStateMode,
                    FourDigitSchoolYear = fourDigitSchoolYear,
                    ReportType = StudentDrillDownReportType.EnrolledElsewhere,
                };
                return View("StudentDrillDown", studentDrillDownModel);
            }

            var model = new OdsResidentsEnrolledElsewhereReportViewModel
            {
                EdOrgId = edOrgId,
                EdOrgName = edOrgName,
                User = theUser,
                IsStateMode = isStateMode,
                FourDigitSchoolYear = fourDigitSchoolYear
            };

            return View(model);
        }

        public JsonResult GetResidentsEnrolledElsewhereData(
            int edOrgId,
            string fourDigitSchoolYear,
            bool isStateMode,
            IDataTablesRequest request)
        {
#if DEBUG
            var startTime = DateTime.Now;
#endif
            var results = CacheManager.GetResidentsEnrolledElsewhereReport(
                OdsDataService,
                isStateMode ? (int?)null : edOrgId,
                fourDigitSchoolYear);
#if DEBUG
            System.Diagnostics.Debug.WriteLine(
                $"GetResidentsEnrolledElsewhere: {(DateTime.Now - startTime).Milliseconds}ms");
#endif
            IEnumerable<ResidentsEnrolledElsewhereReportQuery> sortedResults = results.Distinct();

            var sortColumn = request.Columns.FirstOrDefault(x => x.Sort != null);
            if (sortColumn != null)
            {
                Func<ResidentsEnrolledElsewhereReportQuery, string> orderingFunctionString = null;
                Func<ResidentsEnrolledElsewhereReportQuery, int?> orderingFunctionNullableInt = null;
                Func<ResidentsEnrolledElsewhereReportQuery, int> orderingFunctionInt = null;

                switch (sortColumn.Field)
                {
                    case "edOrgId":
                        {
                            orderingFunctionNullableInt = x => x.EdOrgId;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? results.OrderBy(orderingFunctionNullableInt)
                                                : results.OrderByDescending(orderingFunctionNullableInt);
                            break;
                        }
                    case "edOrgName":
                        {
                            orderingFunctionString = x => x.EdOrgName;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? results.OrderBy(orderingFunctionString)
                                                : results.OrderByDescending(orderingFunctionString);
                            break;
                        }
                    case "districtOfEnrollmentId":
                        {
                            orderingFunctionInt = x => x.DistrictOfEnrollmentId;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? results.OrderBy(orderingFunctionInt)
                                                : results.OrderByDescending(orderingFunctionInt);
                            break;
                        }
                    case "districtOfEnrollmentName":
                        {
                            orderingFunctionString = x => x.DistrictOfEnrollmentName;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? results.OrderBy(orderingFunctionString)
                                                : results.OrderByDescending(orderingFunctionString);
                            break;
                        }
                    case "residentsEnrolled":
                        {
                            orderingFunctionInt = x => x.ResidentsEnrolled;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? results.OrderBy(orderingFunctionInt)
                                                : results.OrderByDescending(orderingFunctionInt);
                            break;
                        }
                }
            }

            var pagedResults = sortedResults.Skip(request.Start).Take(request.Length);
            var response = DataTablesResponse.Create(request, results.Count(), results.Count(), pagedResults);
            var jsonResult = new DataTablesJsonResult(response, JsonRequestBehavior.AllowGet);
            return jsonResult;
        }


        // GET: Ods/IdentityIssuesReport
        public ActionResult IdentityIssuesReport()
        {
            var model = new OdsIdentityIssuesReportViewModel
            {
                // todo: implement
            };
            return View(model);
        }

        public JsonResult GetIdentityIssuesReportData(IDataTablesRequest request)
        {
            var results = ApiLogService.GetIdentityIssues();
            return SortAndFilterApiReportData(results, request);
        }

        public JsonResult GetApiErrorsReportData(IDataTablesRequest request)
        {
            var results = ApiLogService.GetApiErrors();
            return SortAndFilterApiReportData(results, request);
        }

        protected JsonResult SortAndFilterApiReportData(IEnumerable<Log> results, IDataTablesRequest request)
        {
            var sortedResults = results.Distinct();

            var sortColumn = request.Columns.FirstOrDefault(x => x.Sort != null);
            if (sortColumn != null)
            {
                Func<Log, string> orderingFunctionString = null;
                Func<Log, DateTime?> orderingFunctionNullableDateTime = null;
                Func<Log, int> orderingFunctionInt = null;

                switch (sortColumn.Field)
                {
                    case "id":
                        {
                            orderingFunctionInt = x => x.Id;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? results.OrderBy(orderingFunctionInt)
                                                : results.OrderByDescending(orderingFunctionInt);
                            break;
                        }
                    case "date":
                        {
                            orderingFunctionNullableDateTime = x => x.Date;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? results.OrderBy(orderingFunctionNullableDateTime)
                                                : results.OrderByDescending(orderingFunctionNullableDateTime);
                            break;
                        }
                    case "thread":
                        {
                            orderingFunctionString = x => x.Thread;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? results.OrderBy(orderingFunctionString)
                                                : results.OrderByDescending(orderingFunctionString);
                            break;
                        }
                    case "level":
                        {
                            orderingFunctionString = x => x.Level;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? results.OrderBy(orderingFunctionString)
                                                : results.OrderByDescending(orderingFunctionString);
                            break;
                        }
                    case "logger":
                        {
                            orderingFunctionString = x => x.Logger;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? results.OrderBy(orderingFunctionString)
                                                : results.OrderByDescending(orderingFunctionString);
                            break;
                        }
                    case "year":
                        {
                            orderingFunctionString = x => x.Year;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? results.OrderBy(orderingFunctionString)
                                                : results.OrderByDescending(orderingFunctionString);
                            break;
                        }

                    case "district":
                        {
                            orderingFunctionString = x => x.District;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? results.OrderBy(orderingFunctionString)
                                                : results.OrderByDescending(orderingFunctionString);
                            break;
                        }
                    case "method":
                        {
                            orderingFunctionString = x => x.Method;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? results.OrderBy(orderingFunctionString)
                                                : results.OrderByDescending(orderingFunctionString);
                            break;
                        }
                    case "url":
                        {
                            orderingFunctionString = x => x.Url;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? results.OrderBy(orderingFunctionString)
                                                : results.OrderByDescending(orderingFunctionString);
                            break;
                        }
                    case "responseCode":
                        {
                            orderingFunctionString = x => x.ResponseCode;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? results.OrderBy(orderingFunctionString)
                                                : results.OrderByDescending(orderingFunctionString);
                            break;
                        }
                    case "responsePhrase":
                        {
                            orderingFunctionString = x => x.ResponsePhrase;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? results.OrderBy(orderingFunctionString)
                                                : results.OrderByDescending(orderingFunctionString);
                            break;
                        }
                    case "exception":
                        {
                            orderingFunctionString = x => x.Exception;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? results.OrderBy(orderingFunctionString)
                                                : results.OrderByDescending(orderingFunctionString);
                            break;
                        }
                }
            }

            var pagedResults = sortedResults.Skip(request.Start).Take(request.Length);
            var response = DataTablesResponse.Create(request, results.Count(), results.Count(), pagedResults);
            var jsonResult = new DataTablesJsonResult(response, JsonRequestBehavior.AllowGet);
            return jsonResult;
        }

        // GET: Ods/Level1IssuesReport
        public ActionResult Level1IssuesReport()
        {
            var model = new OdsLevel1IssuesReportViewModel
            {
                // todo: implement
            };
            return View(model);
        }

        public JsonResult GetRecordsRequestData(int edOrgId, string studentId)
        {
            var session = AppUserService.GetSession();
            var result = OdsDataService.GetRecordsRequestData(session.FocusedSchoolYearId, edOrgId, studentId);

            if (result.RequestingUser == null)
            {
                result.RequestingUser = session.UserIdentity.UserId;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SendRecordsRequest([ModelBinder(typeof(JsonNetModelBinder))]RecordsRequestFormData request)
        {
            var session = AppUserService.GetSession();
            OdsDataService.SaveRecordsRequest(session.FocusedSchoolYearId, request);
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SendRecordsResponse([ModelBinder(typeof(JsonNetModelBinder))]RecordsResponseFormData response)
        {
            var session = AppUserService.GetSession();
            OdsDataService.SaveRecordsResponse(session.FocusedSchoolYearId, response);
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }
    }
}
