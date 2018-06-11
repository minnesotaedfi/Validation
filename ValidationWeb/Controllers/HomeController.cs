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
        private readonly IEdOrgService _edOrgService;
        private readonly IValidatedDataSubmissionService _validatedDataSubmissionService;

        public HomeController(
            IAnnouncementService announcementService, 
            IEdOrgService edOrgService,
            IValidatedDataSubmissionService validatedDataSubmissionService)
        {
            _edOrgService = edOrgService;
            _announcementService = announcementService;
            _validatedDataSubmissionService = validatedDataSubmissionService;
        }

        public ActionResult Index()
        {
            var model = new HomeIndexViewModel
            {
                Announcements = _announcementService.GetAnnoucements(),
                YearsOpenForDataSubmission = _validatedDataSubmissionService.GetYearsOpenForDataSubmission(),
                AuthorizedEdOrgs = _edOrgService.GetEdOrgs(),
                FocusedEdOrg = _edOrgService.GetEdOrgs().FirstOrDefault()
            };
            return View(model);
        }

        public ActionResult Announcements()
        {
            var model = _announcementService.GetAnnoucements();
            return View(model);
        }
    }
}
