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

        public ActionResult AddSubmissionCycle(string collectionId, DateTime startDate, DateTime endDate)
        {
            if (endDate < startDate)
                return null;

            _submissionCycleService.AddSubmissionCycle(collectionId, startDate, endDate);
            var submissionCycles = _submissionCycleService.GetSubmissionCyclesByCollectionId(collectionId);

            return PartialView("Partials/SubmissionCycleList", submissionCycles);
        }

        public ActionResult RemoveSubmissionCycle(int id, string collectionId)
        {
            _submissionCycleService.RemoveSubmissionCycle(id);
            var submissionCycles = _submissionCycleService.GetSubmissionCyclesByCollectionId(collectionId);

            return PartialView("Partials/SubmissionCycleList", submissionCycles);
        }

        /***
        public ActionResult SaveAnnouncement(FormCollection formCollection)
        {
            if (ModelState.IsValid)
            {
                int priority = 0;
                int announcementId = 0;
                if (!string.IsNullOrEmpty(formCollection["priority"]))
                    priority = Convert.ToInt32(formCollection["priority"]);
                if (!string.IsNullOrEmpty(formCollection["announcementId"]))
                    announcementId = Convert.ToInt32(formCollection["announcementId"]);
                _announcementService.SaveAnnouncement(announcementId, priority, formCollection["message"], formCollection["contactInfo"],
                    formCollection["linkUrl"], formCollection["expiration"]);

            }
            var announcements = _announcementService.GetAnnoucements(true);
            return RedirectToAction("Index", new { tab = "announcements" });
        }
        ***/

        [HttpPost]
        public ActionResult SaveAnnouncement(Announcement announcement)
        {
            if (ModelState.IsValid)
            {
                /***
                int priority = 0;
                int announcementId = 0;
                if (!string.IsNullOrEmpty(announcement.Priority))
                    priority = Convert.ToInt32(formCollection["priority"]);
                //if (!string.IsNullOrEmpty(formCollection["announcementId"]))
                   // announcementId = Convert.ToInt32(formCollection["announcementId"]);
                   ***/
                _announcementService.SaveAnnouncement(announcement.Id, announcement.Priority, announcement.Message,
                    announcement.ContactInfo,
                    announcement.LinkUrl, announcement.Expiration);
                var announcements = _announcementService.GetAnnoucements(true);
                return RedirectToAction("Index", new { tab = "announcements" });
            }

            return PartialView("Partials/AnnouncementEditModal", announcement);
        }

        public ActionResult AnnouncementAdd()
        {
            var announcement = new Announcement { Expiration = DateTime.Now };
            return PartialView("Partials/AnnouncementEditModal", announcement);
        }

        public ActionResult AnnouncementEdit(int Id)
        {
            var announcement = _announcementService.GetAnnoucement(Id);
            //return View("AnnouncementEditModal", announcement);
            return PartialView("Partials/AnnouncementEditModal", announcement);
        }

    }
}