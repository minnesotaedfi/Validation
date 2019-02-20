using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ValidationWeb.Filters;
using ValidationWeb.Services;

namespace ValidationWeb
{
    using ValidationWeb.Services.Interfaces;
    using ValidationWeb.ViewModels;

    [PortalAuthorize(Roles = "DataOwner,DistrictUser,HelpDesk")]
    public class HomeController : Controller
    {
        public HomeController(
            IAnnouncementService announcementService,
            IAppUserService appUserService,
            IEdOrgService edOrgService,
            ISchoolYearService schoolYearService,
            IOdsDataService odsDataService,
            IValidatedDataSubmissionService validatedDataSubmissionService,
            ISubmissionCycleService submissionCycleService,
            IRecordsRequestService recordsRequestService, 
            IConfigurationValues configurationValues)
        {
            AnnouncementService = announcementService;
            AppUserService = appUserService;
            EdOrgService = edOrgService;
            SchoolYearService = schoolYearService;
            OdsDataService = odsDataService;
            ValidatedDataSubmissionService = validatedDataSubmissionService;
            SubmissionCycleService = submissionCycleService;
            RecordsRequestService = recordsRequestService;
            ConfigurationValues = configurationValues;
        }

        protected IAnnouncementService AnnouncementService { get; set; }
        
        protected IAppUserService AppUserService { get; set; }
        
        protected IEdOrgService EdOrgService { get; set; }
       
        protected ISchoolYearService SchoolYearService { get; set; }
        
        protected IOdsDataService OdsDataService { get; set; }
        
        protected IValidatedDataSubmissionService ValidatedDataSubmissionService { get; set; }

        protected ISubmissionCycleService SubmissionCycleService { get; set; }

        protected IRecordsRequestService RecordsRequestService { get; set; }
        
        protected IConfigurationValues ConfigurationValues { get; set; }
        
        public ActionResult Index()
        {
            var focusedSchoolYearId = AppUserService.GetSession().FocusedSchoolYearId;

            var focusedEdOrg = EdOrgService.GetEdOrgById(
                AppUserService.GetSession().FocusedEdOrgId,
                SchoolYearService.GetSchoolYearById(focusedSchoolYearId).Id);

            var recordsRequests = RecordsRequestService.GetAllRecordsRequests()
                .Where(x => 
                    x.RespondingDistrict == focusedEdOrg.Id && 
                    x.SchoolYearId == focusedSchoolYearId &&
                    (x.Status == RecordsRequestStatus.PartialResponse || x.Status == RecordsRequestStatus.Requested)).ToList();

            foreach (var recordRequest in recordsRequests)
            {
                recordRequest.RequestingDistrictName = EdOrgService.GetEdOrgById(recordRequest.RequestingDistrict, focusedSchoolYearId).OrganizationName;
            }

            var model = new HomeIndexViewModel
            {
                AppUserSession = AppUserService.GetSession(),
                Announcements = AnnouncementService.GetAnnouncements(),
                YearsOpenForDataSubmission = ValidatedDataSubmissionService.GetYearsOpenForDataSubmission(),
                AuthorizedEdOrgs = EdOrgService.GetAuthorizedEdOrgs(),
                FocusedEdOrg = focusedEdOrg,
                RecordsRequests = recordsRequests,
                SubmissionCycles = SubmissionCycleService.GetSubmissionCyclesOpenToday()
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

        public PartialViewResult Environment()
        {
            var viewModel = new EnvironmentViewModel { EnvironmentName = ConfigurationValues.EnvironmentName };
            return PartialView("~/Views/Shared/_Environment.cshtml", viewModel);
        }
    }
}
