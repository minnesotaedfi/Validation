using System.IO;
using System.Linq;
using System.Web.Mvc;

using Newtonsoft.Json;

using Validation.DataModels;

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
        private readonly IDynamicReportingService _dynamicReportingService;
        private readonly IAppUserService _appUserService;
        private readonly IEdOrgService _edOrgService;
        private readonly ISchoolYearService _schoolYearService ;
        private readonly IProgramAreaService _programAreaService;

        public DynamicController(
            IDynamicReportingService dynamicReportingService,
            IAppUserService appUserService,
            IEdOrgService edOrgService,
            ISchoolYearService schoolYearService,
            IProgramAreaService programAreaService)
        {
            _dynamicReportingService = dynamicReportingService;
            _appUserService = appUserService;
            _edOrgService = edOrgService;
            _schoolYearService = schoolYearService;
            _programAreaService = programAreaService;
        }

        // GET: Dynamic
        public ActionResult Index()
        {
            var session = _appUserService.GetSession();

            ProgramArea programArea;

            if (session.FocusedProgramAreaId != null)
            {
                programArea = _programAreaService.GetProgramAreaById(session.FocusedProgramAreaId.Value);
            }
            else
            {
                programArea = _programAreaService.GetProgramAreas().FirstOrDefault();
            }
            
            var focusedSchoolYearId = session.FocusedSchoolYearId;
            var focusedSchoolYear = _schoolYearService.GetSchoolYearById(focusedSchoolYearId);

            var focusedEdOrg = _edOrgService.GetEdOrgById(
                _appUserService.GetSession().FocusedEdOrgId,
                focusedSchoolYear.Id);

            var viewModel = new DynamicReportViewModel
            {
                FocusedEdOrg = focusedEdOrg,
                SchoolYear = focusedSchoolYear,
                FocusedProgramArea = programArea,
                User = _appUserService.GetUser()
            };

            return View(viewModel);
        }

        public ActionResult GetReportDefinitions(int schoolYearId)
        {
            var identity = (ValidationPortalIdentity)User.Identity;
            var permissions = User.Identity.GetViewPermissions(identity.AppRole);

            var session = _appUserService.GetSession();

            ProgramArea programArea;

            if (session.FocusedProgramAreaId != null)
            {
                programArea = _programAreaService.GetProgramAreaById(session.FocusedProgramAreaId.Value);
            }
            else
            {
                programArea = _programAreaService.GetProgramAreas().FirstOrDefault();
            }

            var result = _dynamicReportingService.GetReportDefinitions(programArea)
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
            var user = _appUserService.GetUser();
            var filterByDistrict = user.AppRole.Name != PortalRoleNames.DataOwner;
            var report = _dynamicReportingService.GetReportData(request, filterByDistrict ? districtId : (int?)null);
            var csvArray = Csv.WriteCsvToMemory(report);
            var memoryStream = new MemoryStream(csvArray);

            return new FileStreamResult(memoryStream, "text/csv");
        }
    }
}