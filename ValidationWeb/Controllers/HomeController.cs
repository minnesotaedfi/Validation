using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ValidationWeb.Filters;
using ValidationWeb.Services;

namespace ValidationWeb
{
    [PortalAuthorize(Roles = "DataOwner,DistrictUser,HelpDesk")]
    public class HomeController : Controller
    {
        private readonly IAnnouncementService _announcementService;
        private readonly IAppUserService _appUserService;
        private readonly IEdOrgService _edOrgService;
        private readonly ISchoolYearService _schoolYearService;
        private readonly IValidatedDataSubmissionService _validatedDataSubmissionService;

        public HomeController(
            IAnnouncementService announcementService,
            IAppUserService appUserService,
            IEdOrgService edOrgService,
            ISchoolYearService schoolYearService,
            IValidatedDataSubmissionService validatedDataSubmissionService)
        {
            _announcementService = announcementService;
            _appUserService = appUserService;
            _edOrgService = edOrgService;
            _schoolYearService = schoolYearService;
            _validatedDataSubmissionService = validatedDataSubmissionService;
        }

        public ActionResult Index()
        {
            var model = new HomeIndexViewModel
            {
                AppUserSession = _appUserService.GetSession(),
                Announcements = _announcementService.GetAnnouncements(),
                YearsOpenForDataSubmission = _validatedDataSubmissionService.GetYearsOpenForDataSubmission(),
                AuthorizedEdOrgs = _edOrgService.GetEdOrgs(),
                FocusedEdOrg = _edOrgService.GetEdOrgById(_appUserService.GetSession().FocusedEdOrgId, _schoolYearService.GetSchoolYearById(_appUserService.GetSession().FocusedSchoolYearId).Id)
            };
            if (model.AuthorizedEdOrgs.Count() == 0)
            {
                return new HttpUnauthorizedResult("Unauthorized - no educational organizations assigned to user.");
            }
            return View(model);
        }

        public ActionResult SelectOrg(string selectedEdOrgId)
        {
            _appUserService.UpdateFocusedEdOrg(selectedEdOrgId);
            return RedirectToAction("Index");
        }

        public ActionResult SelectSchoolYear(int selectedSchoolYearId)
        {
            _appUserService.UpdateFocusedSchoolYear(selectedSchoolYearId);
            return RedirectToAction("Index");
        }

        public ActionResult Announcements()
        {
            var model = new HomeAnnouncementsViewModel
            {
                Announcements = _announcementService.GetAnnouncements()
            };
            return View(model);
        }

        public string DismissAnnouncement(int announcement)
        {
            _appUserService.DismissAnnouncement(announcement);
            return string.Empty;
        }
    }
}
