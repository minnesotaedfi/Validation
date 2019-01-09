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
        public HomeController(
            IAnnouncementService announcementService,
            IAppUserService appUserService,
            IEdOrgService edOrgService,
            ISchoolYearService schoolYearService,
            IOdsDataService odsDataService,
            IValidatedDataSubmissionService validatedDataSubmissionService)
        {
            AnnouncementService = announcementService;
            AppUserService = appUserService;
            EdOrgService = edOrgService;
            SchoolYearService = schoolYearService;
            OdsDataService = odsDataService;
            ValidatedDataSubmissionService = validatedDataSubmissionService;
        }

        protected IAnnouncementService AnnouncementService { get; set; }
        
        protected IAppUserService AppUserService { get; set; }
        
        protected IEdOrgService EdOrgService { get; set; }
       
        protected ISchoolYearService SchoolYearService { get; set; }
        
        protected IOdsDataService OdsDataService { get; set; }
        
        protected IValidatedDataSubmissionService ValidatedDataSubmissionService { get; set; }

        public ActionResult Index()
        {
            var focusedEdOrg = EdOrgService.GetEdOrgById(
                AppUserService.GetSession().FocusedEdOrgId,
                SchoolYearService.GetSchoolYearById(AppUserService.GetSession().FocusedSchoolYearId).Id);

            var recordsRequests = OdsDataService.GetAllRecordsRequests()
                .Where(x => x.RespondingDistrict == focusedEdOrg.Id && string.IsNullOrEmpty(x.RespondingUser));

            var model = new HomeIndexViewModel
            {
                AppUserSession = AppUserService.GetSession(),
                Announcements = AnnouncementService.GetAnnouncements(),
                YearsOpenForDataSubmission = ValidatedDataSubmissionService.GetYearsOpenForDataSubmission(),
                AuthorizedEdOrgs = EdOrgService.GetEdOrgs(),
                FocusedEdOrg = focusedEdOrg,
                RecordsRequests = recordsRequests
            };

            if (!model.AuthorizedEdOrgs.Any())
            {
                return new HttpUnauthorizedResult("Unauthorized - no educational organizations assigned to user.");
            }

            return View(model);
        }

        public ActionResult SelectOrg(string selectedEdOrgId)
        {
            AppUserService.UpdateFocusedEdOrg(selectedEdOrgId);
            return RedirectToAction("Index");
        }

        public ActionResult SelectSchoolYear(int selectedSchoolYearId)
        {
            AppUserService.UpdateFocusedSchoolYear(selectedSchoolYearId);
            return RedirectToAction("Index");
        }

        public ActionResult Announcements()
        {
            var model = new HomeAnnouncementsViewModel
            {
                Announcements = AnnouncementService.GetAnnouncements()
            };
            return View(model);
        }

        public string DismissAnnouncement(int announcement)
        {
            AppUserService.DismissAnnouncement(announcement);
            return string.Empty;
        }
    }
}
