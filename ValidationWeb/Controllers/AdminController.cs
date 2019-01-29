using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ValidationWeb.Filters;
using ValidationWeb.Services;

namespace ValidationWeb
{
    using ValidationWeb.Utility;

    [PortalAuthorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private readonly IAppUserService _appUserService;
        private readonly IEdOrgService _edOrgService;
        private readonly ISchoolYearService _schoolYearService;
        private readonly IRulesEngineService _rulesEngineService;
        private readonly ISubmissionCycleService _submissionCycleService;
        private readonly IAnnouncementService _announcementService;

        public AdminController(
            IAppUserService appUserService,
            IEdOrgService edOrgService,
            ISchoolYearService schoolYearService,
            IRulesEngineService rulesEngineService,
            ISubmissionCycleService submissionCycleService,
            IAnnouncementService announcementService)
        {
            _appUserService = appUserService;
            _edOrgService = edOrgService;
            _schoolYearService = schoolYearService;
            _rulesEngineService = rulesEngineService;
            _submissionCycleService = submissionCycleService;
            _announcementService = announcementService;
        }

        // GET: Admin
        public ActionResult Index()
        {
            var model = new AdminIndexViewModel
            {
                AppUserSession = _appUserService.GetSession(),
                FocusedEdOrg = _edOrgService.GetEdOrgById(_appUserService.GetSession().FocusedEdOrgId, _schoolYearService.GetSchoolYearById(_appUserService.GetSession().FocusedSchoolYearId).Id),
                YearsOpenForDataSubmission = _schoolYearService.GetSubmittableSchoolYears().OrderByDescending(x => x.EndYear),
                RuleCollections = _rulesEngineService.GetCollections(),
                SubmissionCycles = _submissionCycleService.GetSubmissionCycles(),
                Announcements = _announcementService.GetAnnouncements()
            };

            // Check user authorization, if user is admin then then return admin page if not return the error page.
            return model.AppUserSession.UserIdentity.GetViewPermissions(model.AppUserSession.UserIdentity.AppRole).CanAccessAdminFeatures 
                       ? View(model) 
                       : View("Error");
        }

        public bool UpdateThresholdErrorValue(int id, decimal thresholdValue)
        {
            return _schoolYearService.UpdateErrorThresholdValue(id, thresholdValue);
        }

        public ActionResult AddNewSchoolYear(FormCollection formCollection)
        {
            int endDate;
            var didParse = Int32.TryParse(formCollection["startYear"], out endDate);

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

        public ActionResult AddSubmissionCycle()
        {
            PopulateDropDownLists();
            var submissionCycle = new SubmissionCycle { StartDate = DateTime.Now, EndDate = DateTime.Now };
            return PartialView("Partials/SubmissionCycleEditModal", submissionCycle);
        }

        public ActionResult EditSubmissionCycle(int Id)
        {
            var submissionCycle = _submissionCycleService.GetSubmissionCycle(Id);
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
                    var submissionCycles = _submissionCycleService.GetSubmissionCycles();
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
                    _announcementService.SaveAnnouncement(announcement.Id, announcement.Priority, announcement.Message,
                        announcement.ContactInfo,
                        announcement.LinkUrl, announcement.Expiration);
                    var announcements = _announcementService.GetAnnouncements();
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

        public ActionResult EditAnnouncement(int Id)
        {
            var announcement = _announcementService.GetAnnouncement(Id);
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
            ViewData["schoolYears"] = schoolYears;
            ViewData["RuleCollections"] = ruleCollections;
        }
    }
}