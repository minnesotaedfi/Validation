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
            bool readOnly = (theUser.AppRole.Name == AppRole.HelpDesk.Name || theUser.AppRole.Name == AppRole.DataOwner.Name);
            var districtName = (edOrg == null) ? "Invalid Education Organization Selected" : edOrg.OrganizationName;
            // Display the latest summary first (by default)
            var reportSummaries = (edOrg == null) ? Enumerable.Empty<ValidationReportSummary>().ToList() : _validationResultsService.GetValidationReportSummaries(edOrg.Id).OrderByDescending(rs => rs.CompletedWhen).ToList();
            var model = new ValidationReportsViewModel
            {
                DistrictName = districtName,
                TheUser = theUser,
                ReportSummaries = reportSummaries,
                RulesCollections = rulesCollections,
                SchoolYears = _schoolYearService.GetSubmittableSchoolYears().ToList(),
                SubmissionCycles = _submissionCycleService.GetSubmissionCyclesOpenToday(),
            FocusedEdOrgId = _appUserService.GetSession().FocusedEdOrgId,
                FocusedSchoolYearId = _appUserService.GetSession().FocusedSchoolYearId,
                ReadOnly = readOnly
            };
            return View(model);
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
                throw new Exception(strMessage);
            }
            // TODO: Validate the user's access to district, action, school year
            // Kick off Validation
            ValidationReportSummary summary = _rulesEngineService.RunEngine(submissionCycle.StartDate.Year.ToString(), submissionCycle.CollectionId);
            return Json(summary);
        }


        /*
         * 
         * While currently not used, expected to be reinstated next year.
            // GET: Validation/Reports
            public ActionResult Submissions()
            {
                return View();
            }
        */
    }
}