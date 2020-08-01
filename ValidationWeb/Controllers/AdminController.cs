using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

using MoreLinq;

using Newtonsoft.Json;

using Validation.DataModels;

using ValidationWeb.Filters;
using ValidationWeb.Models;
using ValidationWeb.Services.Interfaces;
using ValidationWeb.Utility;
using ValidationWeb.ViewModels;

namespace ValidationWeb.Controllers
{
    [PortalAuthorize(Roles = PortalRoleNames.Admin)]
    public class AdminController : Controller
    {
        private readonly IAppUserService _appUserService;
        private readonly IEdOrgService _edOrgService;
        private readonly ISchoolYearService _schoolYearService;
        private readonly IRulesEngineService _rulesEngineService;
        private readonly ISubmissionCycleService _submissionCycleService;
        private readonly IAnnouncementService _announcementService;
        private readonly IDynamicReportingService _dynamicReportingService;
        private readonly IProgramAreaService _programAreaService;

        public AdminController(
            IAppUserService appUserService,
            IEdOrgService edOrgService,
            ISchoolYearService schoolYearService,
            IRulesEngineService rulesEngineService,
            ISubmissionCycleService submissionCycleService,
            IAnnouncementService announcementService,
            IDynamicReportingService dynamicReportingService,
            IProgramAreaService programAreaService)
        {
            _appUserService = appUserService;
            _edOrgService = edOrgService;
            _schoolYearService = schoolYearService;
            _rulesEngineService = rulesEngineService;
            _submissionCycleService = submissionCycleService;
            _announcementService = announcementService;
            _dynamicReportingService = dynamicReportingService;
            _programAreaService = programAreaService;
        }

        // GET: Admin
        public ActionResult Index()
        {
            var schoolYears = _schoolYearService.GetSubmittableSchoolYears();

            var viewsPerYear = new Dictionary<int, IEnumerable<ValidationRulesView>>();
            foreach (var schoolYear in schoolYears)
            {
                viewsPerYear.Add(
                    schoolYear.Id, 
                    _dynamicReportingService.GetRulesViews(schoolYear.Id));
            }

            var model = new AdminIndexViewModel
            {
                AppUserSession = _appUserService.GetSession(),
                FocusedEdOrg = _edOrgService.GetEdOrgById(
                    _appUserService.GetSession().FocusedEdOrgId,
                    _schoolYearService.GetSchoolYearById(_appUserService.GetSession().FocusedSchoolYearId).Id),
                YearsOpenForDataSubmission = schoolYears.OrderByDescending(x => x.EndYear),
                RuleCollections = _rulesEngineService.GetCollections(),
                SubmissionCycles = _submissionCycleService.GetSubmissionCycles(),
                Announcements = _announcementService.GetAnnouncements(),
                RulesViewsPerSchoolYearId = viewsPerYear,
                ReportSchoolYearId = _appUserService.GetSession().FocusedSchoolYearId,
                ProgramAreas = _programAreaService.GetProgramAreas()
            };

            PopulateDropDownLists();

            // Check user authorization, if user is admin then then return admin page if not redirect to home.
            if (model
                .AppUserSession
                .UserIdentity.GetViewPermissions(model.AppUserSession.UserIdentity.AppRole)
                .CanAccessAdminFeatures)
            {
                return View(model);
            }

            return RedirectToAction("Index", "Home");
        }

        public bool UpdateThresholdErrorValue(int id, decimal thresholdValue)
        {
            return _schoolYearService.UpdateErrorThresholdValue(id, thresholdValue);
        }

        public ActionResult AddNewSchoolYear(FormCollection formCollection)
        {
            int endDate;
            var didParse = int.TryParse(formCollection["startYear"], out endDate);

            if (didParse)
            {
                endDate = endDate + 1;
                _schoolYearService.AddNewSchoolYear(formCollection["startYear"], endDate.ToString());
            }

            return RedirectToAction("Index");
        }

        public bool RemoveSchoolYear(int id)
        {
            return _schoolYearService.RemoveSchoolYear(id);
        }

        public ActionResult GetSubmissionCyclesByCollectionId(string collectionId)
        {
            var submissionCycles = _submissionCycleService.GetSubmissionCyclesByCollectionId(collectionId);
            return PartialView("Partials/SubmissionCycleList", submissionCycles);
        }

        // todo: separate these out from being mixed in with submission cycle stuff. follow same model 

        public ActionResult GetRulesViewsPerSchoolYearId(int schoolYearId)
        {
            var result = _dynamicReportingService
                .GetRulesViews(schoolYearId)
                .OrderBy(x => x.Name);

            var serializedResult = JsonConvert.SerializeObject(
                result,
                Formatting.None,
                new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            return Content(serializedResult, "application/json");
        }

        public ActionResult GetReportDefinitionsPerSchoolYearId(int schoolYearId)
        {
            var result = _dynamicReportingService.GetReportDefinitions()
                .Where(x => x.SchoolYearId == schoolYearId)
                .OrderBy(x => x.Name)
                .ToList();

            var serializedResult = JsonConvert.SerializeObject(
                new { data = result },
                Formatting.None,
                new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            return Content(serializedResult, "application/json");
        }

        public ActionResult AddDynamicReportDefinition(int schoolYearId)
        {
            var schoolYear = _schoolYearService.GetSchoolYearById(schoolYearId);

            var dynamicReport = new DynamicReportDefinition
            {
                Enabled = true,
                SchoolYearId = schoolYearId,
                SchoolYear = schoolYear,
            };

            var rulesViews = _dynamicReportingService.GetRulesViews(schoolYearId);

            var viewModel = new AdminDynamicReportViewModel
            {
                DynamicReportDefinition = dynamicReport,
                ReportSchoolYearId = schoolYearId,
                ValidationRulesViews = rulesViews.ToList()
            };

            return PartialView("Partials/DynamicReportAddModal", viewModel);
        }

        public ActionResult SaveDynamicReportDefinition(DynamicReportDefinition reportDefinition)
        {
            _dynamicReportingService.SaveReportDefinition(reportDefinition);
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public ActionResult EditDynamicReportDefinition(int id)
        {
            var dynamicReport = _dynamicReportingService.GetReportDefinition(id);
            return PartialView("Partials/DynamicReportEditModal", dynamicReport);
        }

        public ActionResult UpdateDynamicReportDefinition(DynamicReportDefinition formResponse)
        {
            _dynamicReportingService.UpdateReportDefinition(formResponse);
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [HttpGet]
        public ActionResult DisableDynamicReportDefinition(int id)
        {
            _dynamicReportingService.DisableReportDefinition(id);
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [HttpGet]
        public ActionResult EnableDynamicReportDefinition(int id)
        {
            _dynamicReportingService.EnableReportDefinition(id);
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [HttpDelete]
        public ActionResult DeleteDynamicReportDefinition(int id)
        {
            _dynamicReportingService.DeleteReportDefinition(id);
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [HttpGet]
        public ActionResult RefreshRulesViews(int schoolYearId)
        {
            _dynamicReportingService.DeleteViewsAndRulesForSchoolYear(schoolYearId);
            _dynamicReportingService.UpdateViewsAndRulesForSchoolYear(schoolYearId);
            return new HttpStatusCodeResult(HttpStatusCode.OK); 
        }

        public ActionResult AddSubmissionCycle()
        {
            PopulateDropDownLists();
            var submissionCycle = new SubmissionCycle { StartDate = DateTime.Now, EndDate = DateTime.Now };
            return PartialView("Partials/SubmissionCycleEditModal", submissionCycle);
        }

        public ActionResult EditSubmissionCycle(int id)
        {
            var submissionCycle = _submissionCycleService.GetSubmissionCycle(id);
            PopulateDropDownLists(submissionCycle);
            return PartialView("Partials/SubmissionCycleEditModal", submissionCycle);
        }

        public ActionResult SaveSubmissionCycle(SubmissionCycle submissionCycle)
        {
            if (submissionCycle.EndDate < submissionCycle.StartDate)
            {
                ModelState.AddModelError("EndDate", "End Date needs to be later than Start Date");
            }

            // We should not let the user add a collection cycle with the same school year and collectionId combination 
            // that already exists in the database, or set the school year and collectionId of an existing cycle
            // to ones of another existing collection cycle.
            SubmissionCycle duplicate = _submissionCycleService.SchoolYearCollectionAlreadyExists(submissionCycle);
            if (duplicate != null && (submissionCycle.Id == 0 || submissionCycle.Id != duplicate.Id))
            {
                ModelState.AddModelError("CollectionId", "A collection cycle with this School Year and Collection already exists.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _submissionCycleService.SaveSubmissionCycle(submissionCycle);
                    return RedirectToAction("Index", new { tab = "submissioncycles" });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("General Error", ex.Message);
                    PopulateDropDownLists(submissionCycle);
                    return PartialView("Partials/SubmissionCycleEditModal", submissionCycle);
                }
            }
            else
            {
                PopulateDropDownLists(submissionCycle);
                return PartialView("Partials/SubmissionCycleEditModal", submissionCycle);
            }
        }

        [HttpDelete]
        public ActionResult DeleteSubmissionCycle(int id)
        {
            _submissionCycleService.DeleteSubmissionCycle(id);
            return RedirectToAction("Index", new { tab = "submissioncycles" });
        }

        [HttpPost]
        public ActionResult SaveAnnouncement(Announcement announcement)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _announcementService.SaveAnnouncement(
                        announcement.Id,
                        announcement.Priority,
                        announcement.Message,
                        announcement.ContactInfo,
                        announcement.LinkUrl,
                        announcement.Expiration);

                    return RedirectToAction("Index", new { tab = "announcements" });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("General Error", ex.Message);
                    return PartialView("Partials/AnnouncementEditModal", announcement);
                }
            }
            else
            {
                return PartialView("Partials/AnnouncementEditModal", announcement);
            }
        }

        public ActionResult AddAnnouncement()
        {
            var announcement = new Announcement { Expiration = DateTime.Now };
            return PartialView("Partials/AnnouncementEditModal", announcement);
        }

        public ActionResult EditAnnouncement(int id)
        {
            var announcement = _announcementService.GetAnnouncement(id);
            return PartialView("Partials/AnnouncementEditModal", announcement);
        }
        
        [HttpDelete]
        public ActionResult DeleteAnnouncement(int id)
        {
            _announcementService.DeleteAnnouncement(id);
            return RedirectToAction("Index", new { tab = "announcements" });
        }

        private void PopulateDropDownLists(SubmissionCycle submissionCycle = null)
        {
            List<SelectListItem> schoolYears = _submissionCycleService.GetSchoolYearsSelectList(submissionCycle);
            var ruleCollections = _rulesEngineService.GetCollections().Select(c => c.CollectionId);
            var programAreas = _programAreaService.GetProgramAreas();
            ViewData["schoolYears"] = schoolYears;
            ViewData["RuleCollections"] = ruleCollections;
            ViewData["ProgramAreas"] = programAreas;
        }

        public ActionResult AddProgramArea()
        {
            PopulateDropDownLists();
            var programArea = new ProgramArea(); 
            return PartialView("Partials/ProgramAreaEditModal", programArea);
        }

        public ActionResult EditProgramArea(int id)
        {
            var programArea = _programAreaService.GetProgramAreaById(id);
            PopulateDropDownLists();
            return PartialView("Partials/ProgramAreaEditModal", programArea);
        }

        public ActionResult SaveProgramArea(ProgramArea programArea)
        {
            var duplicate = _programAreaService.GetProgramAreas()
                .FirstOrDefault(x => x.Description == programArea.Description);

            if (duplicate != null && (programArea.Id == 0 || programArea.Id != duplicate.Id))
            {
                ModelState.AddModelError("ProgramAreaId", "A Program Area with this name already exists.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _programAreaService.SaveProgramArea(programArea);
                    return RedirectToAction("Index", new { tab = "programareas" });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("General Error", ex.Message);
                    PopulateDropDownLists();
                    return PartialView("Partials/ProgramAreaEditModal", programArea);
                }
            }
            else
            {
                PopulateDropDownLists();
                return PartialView("Partials/ProgramAreaEditModal", programArea);
            }
        }

        [HttpDelete]
        public ActionResult DeleteProgramArea(int id)
        {
            _programAreaService.DeleteProgramArea(id);
            return RedirectToAction("Index", new { tab = "programareas" });
        }
    }
}