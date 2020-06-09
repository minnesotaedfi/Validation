﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;

using DataTables.AspNet.Core;
using DataTables.AspNet.Mvc5;

using Engine.Models;

using Validation.DataModels;

using ValidationWeb.DataCache;
using ValidationWeb.Filters;
using ValidationWeb.Models;
using ValidationWeb.Services.Interfaces;
using ValidationWeb.Utility;
using ValidationWeb.ViewModels;

namespace ValidationWeb.Controllers
{
    [PortalAuthorize(Roles = "DistrictUser,HelpDesk,Admin")]
    public class ValidationController : Controller
    {
        private readonly IAppUserService _appUserService;
        private readonly IEdOrgService _edOrgService;
        private readonly ILoggingService _loggingService;
        private readonly IRulesEngineService _rulesEngineService;
        private readonly ISchoolYearService _schoolYearService;
        private readonly ISubmissionCycleService _submissionCycleService;
        private readonly IValidationResultsService _validationResultsService;
        private readonly Model _engineObjectModel;
        private readonly ICacheManager _cacheManager;
        private readonly IProgramAreaService _programAreaService;

        public ValidationController(
            IAppUserService appUserService,
            IEdOrgService edOrgService,
            ILoggingService loggingService,
            IValidationResultsService validationResultsService,
            IRulesEngineService rulesEngineService,
            ISchoolYearService schoolYearService,
            ISubmissionCycleService submissionCycleService,
            Model engineObjectModel,
            ICacheManager cacheManager,
            IProgramAreaService programAreaService)
        {
            _appUserService = appUserService;
            _edOrgService = edOrgService;
            _engineObjectModel = engineObjectModel;
            _loggingService = loggingService;
            _rulesEngineService = rulesEngineService;
            _schoolYearService = schoolYearService;
            _submissionCycleService = submissionCycleService;
            _validationResultsService = validationResultsService;
            _cacheManager = cacheManager;
            _programAreaService = programAreaService;
        }

        // GET: Validation/Reports
        public ActionResult Reports()
        {
            var session = _appUserService.GetSession();
            var edOrg = _edOrgService.GetEdOrgById(session.FocusedEdOrgId, session.FocusedSchoolYearId);
            var programArea = _programAreaService.GetProgramAreaById(session.FocusedProgramAreaId);
            var rulesCollections = _engineObjectModel.Collections.OrderBy(x => x.CollectionId).ToList();
            var theUser = _appUserService.GetUser();
            var districtName = edOrg == null ? "Invalid Education Organization Selected" : edOrg.OrganizationName;

            var model = new ValidationReportsViewModel
            {
                DistrictName = districtName,
                TheUser = theUser,
                RulesCollections = rulesCollections,
                SchoolYears = _schoolYearService.GetSubmittableSchoolYears().ToList(),
                SubmissionCycles = _submissionCycleService.GetSubmissionCyclesOpenToday(programArea).ToList(),
                FocusedEdOrgId = session.FocusedEdOrgId,
                FocusedSchoolYearId = session.FocusedSchoolYearId,
                FocusedProgramArea = programArea
            };

            return View(model);
        }

        public FileStreamResult DownloadReportSummariesCsv(int edOrgId)
        {
            var session = _appUserService.GetSession();
            var programArea = _programAreaService.GetProgramAreaById(session.FocusedProgramAreaId);
            var edOrg = _edOrgService.GetSingleEdOrg(edOrgId, _appUserService.GetSession().FocusedSchoolYearId);

            var results = _validationResultsService.GetValidationReportSummaries(edOrgId, programArea)
                .OrderByDescending(rs => rs.CompletedWhen)
                .ToList();

            var csvArray = Csv.WriteCsvToMemory(results.Select(
                x => new
                {
                    x.RequestedWhen,
                    Collection = $"{x.SchoolYear.StartYear}-{x.SchoolYear.EndYear} / {x.Collection}",
                    x.InitiatedBy,
                    x.Status,
                    x.CompletedWhen,
                    x.ErrorCount,
                    x.WarningCount
                }));

            var memoryStream = new MemoryStream(csvArray);

            return new FileStreamResult(memoryStream, "text/csv")
            {
                FileDownloadName = $"ValidationSummary_{edOrg.OrganizationName.Replace(' ', '-')}.csv"
            };
        }

        public JsonResult ReportSummaries(int edOrgId, IDataTablesRequest request)
        {
            var session = _appUserService.GetSession();
            var programArea = _programAreaService.GetProgramAreaById(session.FocusedProgramAreaId);

            var results = _validationResultsService.GetValidationReportSummaries(edOrgId, programArea)
                .OrderByDescending(rs => rs.CompletedWhen)
                .ToList();

            IEnumerable<ValidationReportSummary> sortedResults = results;

            var sortColumn = request.Columns.FirstOrDefault(x => x.Sort != null);
            if (sortColumn != null)
            {
                Func<ValidationReportSummary, string> orderingFunctionString;
                Func<ValidationReportSummary, int?> orderingFunctionNullableInt;
                Func<ValidationReportSummary, DateTime> orderingFunctionDateTime;
                Func<ValidationReportSummary, DateTime?> orderingFunctionNullableDateTime;

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
                            orderingFunctionString = x => $"{x.SchoolYear.StartYear}-{x.SchoolYear.EndYear} / {x.Collection}";
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

        public FileStreamResult DownloadReportCsv(int id, int reportSummaryId)
        {
            var results = _validationResultsService.GetValidationErrors(reportSummaryId);

            var groupedByStudent = new List<dynamic>();

            foreach (var detailRow in results)
            {
                groupedByStudent.Add(
                    new
                    {
                        StudentId = detailRow.StudentUniqueId,
                        Student = detailRow.StudentFullName,
                        School = CombineDetailField(detailRow.ErrorEnrollmentDetails, x => x.School),
                        SchoolId = CombineDetailField(detailRow.ErrorEnrollmentDetails, x => x.SchoolId),
                        DateEnrolled = CombineDetailField(
                                 detailRow.ErrorEnrollmentDetails,
                                 x => x.DateEnrolled?.ToShortDateString()),
                        DateWithdrawn = CombineDetailField(
                                 detailRow.ErrorEnrollmentDetails,
                                 x => x.DateWithdrawn == null
                                          ? "Present"
                                          : x.DateWithdrawn.Value.ToShortDateString()),
                        Grade = CombineDetailField(detailRow.ErrorEnrollmentDetails, x => x.Grade),
                        detailRow.Severity.CodeValue,
                        detailRow.ErrorCode,
                        detailRow.ErrorText
                    });
            }

            var csvArray = Csv.WriteCsvToMemory(groupedByStudent);
            var memoryStream = new MemoryStream(csvArray);

            var reportSummary = _validationResultsService.GetValidationReportDetails(reportSummaryId);

            return new FileStreamResult(memoryStream, "text/csv")
            {
                FileDownloadName = $"ValidationErrors_{reportSummary.DistrictName.Replace(' ', '-')}_{reportSummary.CollectionName}_{reportSummary.CompletedWhen?.ToShortDateString()}.csv"
            };
        }

        [PortalAuthorize(Roles = "DistrictUser")]
        [HttpPost]
        public ActionResult RunEngine(int submissionCycleId)
        {
            SubmissionCycle submissionCycle = _submissionCycleService.GetSubmissionCycle(submissionCycleId);
            if (submissionCycle == null)
            {
                string strMessage = $"Collection cycle with id {submissionCycleId} not found.";
                _loggingService.LogErrorMessage(strMessage);
                throw new InvalidOperationException(strMessage);
            }

            var session = _appUserService.GetSession();
            var edOrg = _edOrgService.GetEdOrgById(session.FocusedEdOrgId, session.FocusedSchoolYearId);

            _rulesEngineService.DeleteOldValidationRuns(submissionCycle, edOrg.Id);

            // TODO: Validate the user's access to district, action, school year
            // todo: all security

            ValidationReportSummary summary = _rulesEngineService.SetupValidationRun(
                submissionCycle,
                submissionCycle.CollectionId);

            HostingEnvironment.QueueBackgroundWorkItem(
                cancellationToken => _rulesEngineService.RunValidationAsync(
                    submissionCycle,
                    summary.ValidationReportSummaryId));

            return Json(summary);
        }

        protected string CombineDetailField(
            ICollection<ValidationErrorEnrollmentDetail> details,
            Func<ValidationErrorEnrollmentDetail, string> func)
        {
            return string.Join(Environment.NewLine, details.Select(func));
        }

        public ActionResult ValidationRulesReport()
        {
            return View();
        }

        public JsonResult GetValidationRulesReportData(IDataTablesRequest request)
        {
            var result = _cacheManager.GetRulesetDefinitions();
            var sortedResults = result.SelectMany(
                x =>
                    x.RuleDefinitions.Select(
                        y =>
                            new RulesetReportDetail
                            {
                                Ruleset = x.Name,
                                Id = y.Id,
                                ValidationType = y.ValidationType,
                                Message = y.Message
                            }));

            var count = sortedResults.Count();

            var sortColumn = request.Columns.FirstOrDefault(x => x.Sort != null);
            if (sortColumn != null)
            {
                Func<RulesetReportDetail, string> orderingFunctionString = null;

                switch (sortColumn.Field)
                {
                    case "name":
                        {
                            orderingFunctionString = x => x.Ruleset;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? sortedResults.OrderBy(orderingFunctionString)
                                                : sortedResults.OrderByDescending(orderingFunctionString);
                            break;
                        }
                    case "id":
                        {
                            orderingFunctionString = x => x.Id;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? sortedResults.OrderBy(orderingFunctionString)
                                                : sortedResults.OrderByDescending(orderingFunctionString);
                            break;
                        }
                    case "validationType":
                        {
                            orderingFunctionString = x => x.ValidationType;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? sortedResults.OrderBy(orderingFunctionString)
                                                : sortedResults.OrderByDescending(orderingFunctionString);
                            break;
                        }
                    case "message":
                        {
                            orderingFunctionString = x => x.Message;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? sortedResults.OrderBy(orderingFunctionString)
                                                : sortedResults.OrderByDescending(orderingFunctionString);
                            break;
                        }
                    default:
                        break;
                }
            }

            if (request.Search != null)
            {
                sortedResults = sortedResults.Where(x =>
                    x.Ruleset.IndexOf(request.Search.Value, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    x.Id.IndexOf(request.Search.Value, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    x.Message.IndexOf(request.Search.Value, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            var pagedResults = sortedResults.Skip(request.Start).Take(request.Length);
            var response = DataTablesResponse.Create(request, count, sortedResults.Count(), pagedResults);
            var jsonResult = new DataTablesJsonResult(response, JsonRequestBehavior.AllowGet);
            return jsonResult;
        }
    }
}