using Engine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ValidationWeb.Services;

namespace ValidationWeb
{
    using System.Web.Hosting;

    using DataTables.AspNet.Core;
    using DataTables.AspNet.Mvc5;

    public class ValidationController : Controller
    {
        protected readonly IAppUserService _appUserService;
        protected readonly IEdOrgService _edOrgService;
        protected readonly ILoggingService _loggingService;
        protected readonly IRulesEngineService _rulesEngineService;
        protected readonly ISchoolYearService _schoolYearService;
        private readonly ISubmissionCycleService _submissionCycleService;
        protected readonly IValidationResultsService _validationResultsService;
        protected readonly Model _engineObjectModel;

        public ValidationController(
            IAppUserService appUserService,
            IEdOrgService edOrgService,
            ILoggingService loggingService,
            IValidationResultsService validationResultsService,
            IRulesEngineService rulesEngineService,
            ISchoolYearService schoolYearService,
            ISubmissionCycleService submissionCycleService,
            Model engineObjectModel)
        {
            _appUserService = appUserService;
            _edOrgService = edOrgService;
            _engineObjectModel = engineObjectModel;
            _loggingService = loggingService;
            _rulesEngineService = rulesEngineService;
            _schoolYearService = schoolYearService;
            _submissionCycleService = submissionCycleService;
            _validationResultsService = validationResultsService;
        }

        // GET: Validation/Reports
        public ActionResult Reports()
        {
            var edOrg = _edOrgService.GetEdOrgById(_appUserService.GetSession().FocusedEdOrgId, _appUserService.GetSession().FocusedSchoolYearId);
            var rulesCollections = _engineObjectModel.Collections.OrderBy(x => x.CollectionId).ToList();
            var theUser = _appUserService.GetUser();
            var districtName = (edOrg == null) ? "Invalid Education Organization Selected" : edOrg.OrganizationName;

            var model = new ValidationReportsViewModel
            {
                DistrictName = districtName,
                TheUser = theUser,
                RulesCollections = rulesCollections,
                SchoolYears = _schoolYearService.GetSubmittableSchoolYears().ToList(),
                SubmissionCycles = _submissionCycleService.GetSubmissionCyclesOpenToday(),
                FocusedEdOrgId = _appUserService.GetSession().FocusedEdOrgId,
                FocusedSchoolYearId = _appUserService.GetSession().FocusedSchoolYearId
            };
            return View(model);
        }

        public JsonResult ReportSummaries(int edOrgId, IDataTablesRequest request)
        {
            var results = _validationResultsService.GetValidationReportSummaries(edOrgId).OrderByDescending(rs => rs.CompletedWhen).ToList();
            
            IEnumerable<ValidationReportSummary> sortedResults = results;

            var sortColumn = request.Columns.FirstOrDefault(x => x.Sort != null);
            if (sortColumn != null)
            {
                Func<ValidationReportSummary, string> orderingFunctionString = null;
                Func<ValidationReportSummary, int?> orderingFunctionNullableInt = null;
                Func<ValidationReportSummary, DateTime> orderingFunctionDateTime = null;
                Func<ValidationReportSummary, DateTime?> orderingFunctionNullableDateTime = null;

                switch (sortColumn.Field)
                {
                    case "requestedWhen":
                        {
                            orderingFunctionDateTime = x => x.RequestedWhen;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? results.OrderBy(orderingFunctionDateTime)
                                                : results.OrderByDescending(orderingFunctionDateTime);
                            break;
                        }
                    case "collection":
                        {
                            orderingFunctionString = x => x.Collection;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? results.OrderBy(orderingFunctionString)
                                                : results.OrderByDescending(orderingFunctionString);
                            break;
                        }
                    case "initiatedBy":
                        {
                            orderingFunctionString = x => x.InitiatedBy;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? results.OrderBy(orderingFunctionString)
                                                : results.OrderByDescending(orderingFunctionString);
                            break;
                        }
                    case "status":
                        {
                            orderingFunctionString = x => x.Status;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? results.OrderBy(orderingFunctionString)
                                                : results.OrderByDescending(orderingFunctionString);
                            break;
                        }
                    case "completedWhen":
                        {
                            orderingFunctionNullableDateTime = x => x.CompletedWhen;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? results.OrderBy(orderingFunctionNullableDateTime)
                                                : results.OrderByDescending(orderingFunctionNullableDateTime);
                            break;
                        }
                    case "errorCount":
                        {
                            orderingFunctionNullableInt = x => x.ErrorCount;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? results.OrderBy(orderingFunctionNullableInt)
                                                : results.OrderByDescending(orderingFunctionNullableInt);
                            break;
                        }
                    case "warningCount":
                        {
                            orderingFunctionNullableInt = x => x.WarningCount;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? results.OrderBy(orderingFunctionNullableInt)
                                                : results.OrderByDescending(orderingFunctionNullableInt);
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
            var response = DataTablesResponse.Create(request, results.Count, results.Count, pagedResults);
            var jsonResult = new DataTablesJsonResult(response, JsonRequestBehavior.AllowGet);
            return jsonResult;
        }

        public ActionResult Report(int id = 0)
        {
            if (id == 0)
            {
                return RedirectToAction("Reports");
            }

            var model = new ValidationReportDetailsViewModel { Details = _validationResultsService.GetValidationReportDetails(id) };
            return View(model);
        }

        [HttpPost]
        public ActionResult RunEngine(int submissionCycleId)
        {
            SubmissionCycle submissionCycle = _submissionCycleService.GetSubmissionCycle(submissionCycleId);
            if (submissionCycle == null)
            {
                string strMessage = $"Submission cycle with id {submissionCycleId} not found.";
                _loggingService.LogErrorMessage(strMessage);
                throw new InvalidOperationException(strMessage);
            }

            // TODO: Validate the user's access to district, action, school year
            // todo: all security

            // refactor: step 1 set up run, get id. step 2, queue background run engine in thread 
            ValidationReportSummary summary = _rulesEngineService.SetupValidationRun(submissionCycle.StartDate.Year.ToString(), submissionCycle.CollectionId);
            HostingEnvironment.QueueBackgroundWorkItem(cancellationToken => _rulesEngineService.RunValidationAsync(submissionCycle.StartDate.Year.ToString(), summary.Id));
            
            return Json(summary);
        }
    }
}