using System.IO;
using System.Linq;
using System.Web.Mvc;

using Newtonsoft.Json;

using ValidationWeb.Filters;
using ValidationWeb.Models;
using ValidationWeb.Services.Interfaces;
using ValidationWeb.Utility;
using ValidationWeb.ViewModels;

namespace ValidationWeb.Controllers
{
    [PortalAuthorize(Roles = "Admin,DataOwner,DistrictUser,HelpDesk")]
    public class DynamicController : Controller
    {
        public DynamicController(
            IDynamicReportingService dynamicReportingService,
            IAppUserService appUserService,
            IEdOrgService edOrgService,
            ISchoolYearService schoolYearService)
        {
            DynamicReportingService = dynamicReportingService;
            AppUserService = appUserService;
            EdOrgService = edOrgService;
            SchoolYearService = schoolYearService;
        }

        private IDynamicReportingService DynamicReportingService { get; }
        private IAppUserService AppUserService { get; }
        private IEdOrgService EdOrgService { get; }
        private ISchoolYearService SchoolYearService { get; }

        // GET: Dynamic
        public ActionResult Index()
        {
            var focusedSchoolYearId = AppUserService.GetSession().FocusedSchoolYearId;
            var focusedSchoolYear = SchoolYearService.GetSchoolYearById(focusedSchoolYearId);

            var focusedEdOrg = EdOrgService.GetEdOrgById(
                AppUserService.GetSession().FocusedEdOrgId,
                focusedSchoolYear.Id);

            var viewModel = new DynamicReportViewModel
            {
                FocusedEdOrg = focusedEdOrg,
                SchoolYear = focusedSchoolYear,
                User = AppUserService.GetUser()
            };

            return View(viewModel);
        }

        public ActionResult GetReportDefinitions(int schoolYearId)
        {
            var identity = (ValidationPortalIdentity)User.Identity;
            var permissions = User.Identity.GetViewPermissions(identity.AppRole);

            var result = DynamicReportingService.GetReportDefinitions()
                .Where(x => x.SchoolYearId == schoolYearId)
                .Where(x => x.Enabled);

            if (!permissions.CanViewStudentLevelReports)
            {
                result = result.Where(x => x.IsOrgLevelReport);
            }

            var serializedResult = JsonConvert.SerializeObject(
                result.OrderBy(x => x.Name),
                Formatting.None,
                new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            return Content(serializedResult, "application/json");
        }

        public ActionResult GenerateReport(DynamicReportRequest request, int districtId)
        {
            var user = AppUserService.GetUser();
            var filterByDistrict = user.AppRole.Name != PortalRoleNames.DataOwner;
            var report = DynamicReportingService.GetReportData(request, filterByDistrict ? districtId : (int?)null);
            var csvArray = Csv.WriteCsvToMemory(report);
            var memoryStream = new MemoryStream(csvArray);

            return new FileStreamResult(memoryStream, "text/csv");
        }
    }
}