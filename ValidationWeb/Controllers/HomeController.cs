using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ValidationWeb.Services;

namespace ValidationWeb
{
    public class HomeController : Controller
    {
        private readonly IAnnouncementService _announcementService;
        private readonly IAppUserService _appUserService;
        private readonly IEdOrgService _edOrgService;
        private readonly IValidatedDataSubmissionService _validatedDataSubmissionService;

        public HomeController(
            IAnnouncementService announcementService,
            IAppUserService appUserService,
            IEdOrgService edOrgService,
            IValidatedDataSubmissionService validatedDataSubmissionService)
        {
            _announcementService = announcementService;
            _appUserService = appUserService;
            _edOrgService = edOrgService;
            _validatedDataSubmissionService = validatedDataSubmissionService;
        }

        public ActionResult Index()
        {
            var model = new HomeIndexViewModel
            {
                AppUserSession = _appUserService.GetSession(),
                Announcements = _announcementService.GetAnnoucements(),
                YearsOpenForDataSubmission = _validatedDataSubmissionService.GetYearsOpenForDataSubmission(),
                AuthorizedEdOrgs = _edOrgService.GetEdOrgs(),
                FocusedEdOrg = _appUserService.GetSession().FocusedEdOrg
            };
            return View(model);
        }

        public ActionResult Announcements()
        {
            var model = new HomeAnnouncementsViewModel
            {
                Announcements = _announcementService.GetAnnoucements()
            };
            return View(model);
        }
    }
}
