using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ValidationWeb.Services;

namespace ValidationWeb
{
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
                AuthorizedEdOrgs = _edOrgService.GetEdOrgs(),
                FocusedEdOrg = _edOrgService.GetEdOrgById(_appUserService.GetSession().FocusedEdOrgId, _schoolYearService.GetSchoolYearById(_appUserService.GetSession().FocusedSchoolYearId).Id),
                YearsOpenForDataSubmission = _schoolYearService.GetSubmittableSchoolYears().OrderByDescending(x => x.EndYear),
                RuleCollections = _rulesEngineService.GetCollections(),
                SubmissionCycles = _submissionCycleService.GetSubmissionCycles(),
                Announcements = _announcementService.GetAnnoucements(true)
            };

            // Check user authorization, if user is admin then then return admin page if not return the error page.
            return model.AppUserSession.UserIdentity.AppRole.Name == "Administrator" ? View(model) : View("Error");
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
            var schoolYearsEnumerable = _schoolYearService.GetSubmittableSchoolYearsDictionary().OrderByDescending(x => x.Value);
            var schoolYears = new List<SelectListItem>();
            foreach (var kvPair in schoolYearsEnumerable)
            {
                schoolYears.Add(new SelectListItem { Value = kvPair.Key.ToString(), Text = kvPair.Value });
            }
            schoolYears[0].Selected = true;
            var ruleCollections = _rulesEngineService.GetCollections().Select(c => c.CollectionId);
            ViewData["schoolYears"] = schoolYears;
            ViewData["RuleCollections"] = ruleCollections;
            var submissionCycle = new SubmissionCycle { StartDate = DateTime.Now, EndDate = DateTime.Now };
            return PartialView("Partials/SubmissionCycleEditModal", submissionCycle);
        }

        public ActionResult EditSubmissionCycle(int Id)
        {
            var submissionCycle = _submissionCycleService.GetSubmissionCycle(Id);

            var schoolYearsEnumerable = _schoolYearService.GetSubmittableSchoolYearsDictionary().OrderByDescending(x => x.Value);
            var schoolYears = new List<SelectListItem>();

            foreach (var kvPair in schoolYearsEnumerable)
            {
                SelectListItem selectListItem = new SelectListItem { Value = kvPair.Key.ToString(), Text = kvPair.Value };
                if (kvPair.Key == submissionCycle.SchoolYearId)
                    selectListItem.Selected = true;
                schoolYears.Add(selectListItem);
            }
            var ruleCollections = _rulesEngineService.GetCollections().Select(c => c.CollectionId);
            ViewData["schoolYears"] = schoolYears;
            ViewData["RuleCollections"] = ruleCollections;

            return PartialView("Partials/SubmissionCycleEditModal", submissionCycle);
        }

        public ActionResult SaveSubmissionCycle(SubmissionCycle submissionCycle)
        {
            if (submissionCycle.EndDate < submissionCycle.StartDate)
            {
                ModelState.AddModelError("EndDate", "End Date needs to be later than Start Date");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _submissionCycleService.SaveSubmissionCycle(submissionCycle);
                    var submissionCycles = _submissionCycleService.GetSubmissionCycles();
                    return RedirectToAction("Index", new { tab = "submissionCycles" });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("General Error", ex.Message);
                    return PartialView("Partials/SubmissionCycleEditModal", submissionCycle);
                }
            }
            else
            {
                return PartialView("Partials/SubmissionCycleEditModal", submissionCycle);
            }

        }

        public ActionResult RemoveSubmissionCycle(int id, string collectionId)
        {
            _submissionCycleService.RemoveSubmissionCycle(id);
            var submissionCycles = _submissionCycleService.GetSubmissionCyclesByCollectionId(collectionId);

            return PartialView("Partials/SubmissionCycleList", submissionCycles);
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
                    var announcements = _announcementService.GetAnnoucements(true);
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

        public ActionResult AnnouncementAdd()
        {
            var announcement = new Announcement { Expiration = DateTime.Now };
            return PartialView("Partials/AnnouncementEditModal", announcement);
        }

        public ActionResult AnnouncementEdit(int Id)
        {
            var announcement = _announcementService.GetAnnoucement(Id);
            return PartialView("Partials/AnnouncementEditModal", announcement);
        }


        [HttpDelete]
        public ActionResult DeleteAnnouncement(int id)
        {
            _announcementService.DeleteAnnoucement(id);
            return RedirectToAction("Index", new { tab = "announcements" });
        }
    }
}