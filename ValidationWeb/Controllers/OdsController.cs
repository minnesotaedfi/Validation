using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Engine.Models;
using ValidationWeb.Services;

namespace ValidationWeb
{
    using System;
    using System.Collections;

    using DataTables.AspNet.Core;
    using DataTables.AspNet.Mvc5;

    public class OdsController : Controller
    {
        protected readonly IAppUserService _appUserService;
        protected readonly IEdOrgService _edOrgService;
        protected readonly IOdsDataService _odsDataService;
        protected readonly IRulesEngineService _rulesEngineService;
        protected readonly ISchoolYearService _schoolyearService;
        protected readonly Model _engineObjectModel;

        public OdsController(
            IAppUserService appUserService,
            IEdOrgService edOrgService,
            IOdsDataService odsDataService,
            IRulesEngineService rulesEngineService,
            ISchoolYearService schoolyearService,
            Model engineObjectModel)
        {
            _appUserService = appUserService;
            _edOrgService = edOrgService;
            _odsDataService = odsDataService;
            _engineObjectModel = engineObjectModel;
            _rulesEngineService = rulesEngineService;
            _schoolyearService = schoolyearService;
        }

        // GET: Ods/Reports
        public ActionResult Reports()
        {
            var session = _appUserService.GetSession();
            var edOrg = _edOrgService.GetEdOrgById(session.FocusedEdOrgId, session.FocusedSchoolYearId);
            var edOrgName = (edOrg == null) ? "Invalid Education Organization Selected" : edOrg.OrganizationName;
            var edOrgId = edOrg.Id;
            var theUser = _appUserService.GetUser();
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
            var session = _appUserService.GetSession();
            var edOrg = _edOrgService.GetEdOrgById(session.FocusedEdOrgId, session.FocusedSchoolYearId);
            var edOrgName = (edOrg == null) ? "Invalid Education Organization Selected" : edOrg.OrganizationName;
            var edOrgId = edOrg.Id;

            // A state user can look at any district via a link, without changing the default district.
            if (districtToDisplay.HasValue && session.UserIdentity.AuthorizedEdOrgs.Select(eorg => eorg.Id).Contains(districtToDisplay.Value))
            {
                edOrgId = districtToDisplay.Value;
                edOrg = _edOrgService.GetEdOrgById(edOrgId, session.FocusedSchoolYearId);
                edOrgName = (edOrg == null) ? "Invalid Education Organization Selected" : edOrg.OrganizationName;
            }

            var schoolName = schoolId.HasValue
                                 ? "placeholder" // _edOrgService.GetEdOrgById(schoolId.Value, session.FocusedSchoolYearId)?.OrganizationName
                                 : string.Empty;

            var fourDigitSchoolYear = _schoolyearService.GetSchoolYearById(session.FocusedSchoolYearId).EndYear;
            var theUser = _appUserService.GetUser();

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
                    OrgType = orgType
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
            IDataTablesRequest request)
        {
            IEnumerable<StudentDrillDownQuery> results = _odsDataService.GetDistrictAncestryRaceStudentDrillDown(
                orgType,
                schoolId,
                schoolId ?? edOrgId,
                drillDownColumnIndex,
                fourDigitSchoolYear);

            IEnumerable<StudentDrillDownQuery> sortedResults = results;

            var sortColumn = request.Columns.FirstOrDefault(x => x.Sort != null);
            if (sortColumn != null)
            {
                Func<StudentDrillDownQuery, string> orderingFunctionString = null;
                Func<StudentDrillDownQuery, int?> orderingFunctionNullableInt = null;
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
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? results.OrderBy(orderingFunctionString)
                                                : results.OrderByDescending(orderingFunctionString);
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
            IEnumerable<DemographicsCountReportQuery> results = _odsDataService.GetDistrictAncestryRaceCounts(
                isStateMode ? (int?)null : edOrgId,
                fourDigitSchoolYear);

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
        public ActionResult MultipleEnrollmentsReport(bool isStateMode = false, int? districtToDisplay = null, bool isStudentDrillDown = false, int? schoolId = null, int? drillDownColumnIndex = null, OrgType orgType = OrgType.District)
        {
            var session = _appUserService.GetSession();
            var edOrg = _edOrgService.GetEdOrgById(session.FocusedEdOrgId, session.FocusedSchoolYearId);
            var edOrgName = (edOrg == null) ? "Invalid Education Organization Selected" : edOrg.OrganizationName;
            var edOrgId = edOrg.Id;

            // A state user can look at any district via a link, without changing the default district.
            if (districtToDisplay.HasValue && session.UserIdentity.AuthorizedEdOrgs.Select(eorg => eorg.Id).Contains(districtToDisplay.Value))
            {
                edOrgId = districtToDisplay.Value;
                edOrg = _edOrgService.GetEdOrgById(edOrgId, session.FocusedSchoolYearId);
                edOrgName = (edOrg == null) ? "Invalid Education Organization Selected" : edOrg.OrganizationName;
            }
            var fourDigitSchoolYear = _schoolyearService.GetSchoolYearById(session.FocusedSchoolYearId).EndYear;
            var theUser = _appUserService.GetUser();
            if (isStudentDrillDown)
            {
                var studentDrillDownResults = _odsDataService.GetMultipleEnrollmentStudentDrillDown(orgType, schoolId, schoolId ?? edOrgId, drillDownColumnIndex, fourDigitSchoolYear);
                var studentDrillDownModel = new StudentDrillDownViewModel
                {
                    ReportName = "Multiple Enrollment",
                    EdOrgId = edOrgId,
                    EdOrgName = edOrgName,
                    User = theUser,
                    Results = studentDrillDownResults,
                    IsStateMode = isStateMode
                };
                return View("StudentDrillDown", studentDrillDownModel);
            }
            var results = _odsDataService.GetMultipleEnrollmentCounts(isStateMode ? (int?)null : edOrgId, fourDigitSchoolYear);
            var model = new OdsMultipleEnrollmentsReportViewModel
            {
                EdOrgId = edOrgId,
                EdOrgName = edOrgName,
                User = theUser,
                Results = results,
                IsStateMode = isStateMode
            };
            return View(model);
        }

        // GET: Ods/StudentProgramsReport
        public ActionResult StudentProgramsReport(bool isStateMode = false, int? districtToDisplay = null, bool isStudentDrillDown = false, int? schoolId = null, int? drillDownColumnIndex = null, OrgType orgType = OrgType.District)
        {
            var session = _appUserService.GetSession();
            var edOrg = _edOrgService.GetEdOrgById(session.FocusedEdOrgId, session.FocusedSchoolYearId);
            var edOrgName = (edOrg == null) ? "Invalid Education Organization Selected" : edOrg.OrganizationName;
            var edOrgId = edOrg.Id;
            // A state user can look at any district via a link, without changing the default district.
            if (districtToDisplay.HasValue && session.UserIdentity.AuthorizedEdOrgs.Select(eorg => eorg.Id).Contains(districtToDisplay.Value))
            {
                edOrgId = districtToDisplay.Value;
                edOrg = _edOrgService.GetEdOrgById(edOrgId, session.FocusedSchoolYearId);
                edOrgName = (edOrg == null) ? "Invalid Education Organization Selected" : edOrg.OrganizationName;
            }
            var fourDigitSchoolYear = _schoolyearService.GetSchoolYearById(session.FocusedSchoolYearId).EndYear;
            var theUser = _appUserService.GetUser();
            if (isStudentDrillDown)
            {
                var studentDrillDownResults = _odsDataService.GetStudentProgramsStudentDrillDown(orgType, schoolId, schoolId ?? edOrgId, drillDownColumnIndex, fourDigitSchoolYear);
                var studentDrillDownModel = new StudentDrillDownViewModel
                {
                    ReportName = "Student Characteristics and Program Participation",
                    EdOrgId = edOrgId,
                    EdOrgName = edOrgName,
                    User = theUser,
                    Results = studentDrillDownResults,
                    IsStateMode = isStateMode
                };
                return View("StudentDrillDown", studentDrillDownModel);
            }
            var results = _odsDataService.GetStudentProgramsCounts(isStateMode ? (int?)null : edOrgId, fourDigitSchoolYear);
            var model = new OdsStudentProgramsReportViewModel
            {
                EdOrgId = edOrgId,
                EdOrgName = edOrgName,
                User = theUser,
                Results = results,
                IsStateMode = isStateMode
            };
            return View(model);
        }

        // GET: Ods/ChangeOfEnrollmentReport
        public ActionResult ChangeOfEnrollmentReport()
        {
            var session = _appUserService.GetSession();
            var edOrg = _edOrgService.GetEdOrgById(session.FocusedEdOrgId, session.FocusedSchoolYearId);
            var edOrgName = (edOrg == null) ? "Invalid Education Organization Selected" : edOrg.OrganizationName;
            var edOrgId = edOrg.Id;
            var fourDigitSchoolYear = _schoolyearService.GetSchoolYearById(session.FocusedSchoolYearId).EndYear;
            var theUser = _appUserService.GetUser();
            var results = _odsDataService.GetChangeOfEnrollmentReport(edOrgId, fourDigitSchoolYear);
            var model = new OdsChangeOfEnrollmentReportViewModel
            {
                EdOrgId = edOrgId,
                EdOrgName = edOrgName,
                User = theUser,
                Results = results
            };
            return View(model);
        }

        // GET: Ods/ResidentsEnrolledElsewhereReport
        public ActionResult ResidentsEnrolledElsewhereReport(bool isStateMode = false, int? districtToDisplay = null, bool isStudentDrillDown = false)
        {
            var session = _appUserService.GetSession();
            var edOrg = _edOrgService.GetEdOrgById(session.FocusedEdOrgId, session.FocusedSchoolYearId);
            var edOrgName = (edOrg == null) ? "Invalid Education Organization Selected" : edOrg.OrganizationName;
            var edOrgId = edOrg.Id;
            // A state user can look at any district via a link, without changing the default district.
            if (districtToDisplay.HasValue && session.UserIdentity.AuthorizedEdOrgs.Select(eorg => eorg.Id).Contains(districtToDisplay.Value))
            {
                edOrgId = districtToDisplay.Value;
                edOrg = _edOrgService.GetEdOrgById(edOrgId, session.FocusedSchoolYearId);
                edOrgName = (edOrg == null) ? "Invalid Education Organization Selected" : edOrg.OrganizationName;
            }
            var fourDigitSchoolYear = _schoolyearService.GetSchoolYearById(session.FocusedSchoolYearId).EndYear;
            var theUser = _appUserService.GetUser();
            if (isStudentDrillDown)
            {
                var studentDrillDownResults = _odsDataService.GetResidentsEnrolledElsewhereStudentDrillDown(isStateMode ? (int?)null : edOrgId, fourDigitSchoolYear);
                var studentDrillDownModel = new StudentDrillDownViewModel
                {
                    ReportName = "Residents Enrolled Elsewhere",
                    EdOrgId = edOrgId,
                    EdOrgName = edOrgName,
                    User = theUser,
                    Results = studentDrillDownResults,
                    IsStateMode = isStateMode
                };
                return View("StudentDrillDown", studentDrillDownModel);
            }
            var results = _odsDataService.GetResidentsEnrolledElsewhereReport(isStateMode ? (int?)null : edOrgId, fourDigitSchoolYear);
            var model = new OdsResidentsEnrolledElsewhereReportViewModel
            {
                EdOrgId = edOrgId,
                EdOrgName = edOrgName,
                User = theUser,
                Results = results,
                IsStateMode = isStateMode
            };
            return View(model);
        }

        // GET: Ods/IdentityIssuesReport
        public ActionResult IdentityIssuesReport()
        {
            var model = new OdsIdentityIssuesReportViewModel
            {

            };
            return View(model);
        }

        // GET: Ods/Level1IssuesReport
        public ActionResult Level1IssuesReport()
        {
            var model = new OdsLevel1IssuesReportViewModel
            {

            };
            return View(model);
        }
    }
}
