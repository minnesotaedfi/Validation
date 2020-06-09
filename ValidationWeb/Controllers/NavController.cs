using System.Linq;
using System.Net.Configuration;
using System.Web;
using System.Web.Mvc;

using ValidationWeb.Services.Interfaces;
using ValidationWeb.ViewModels;

namespace ValidationWeb.Controllers
{
    [AllowAnonymous]
    public class NavController : Controller
    {
        private static readonly string _version;
        private readonly IAppUserService _appUserService;
        private readonly IEdOrgService _edOrgService;
        private readonly ISchoolYearService _schoolYearService;
        private readonly IProgramAreaService _programAreaService;
        private readonly IConfigurationValues _configurationValues;

        static NavController()
        {
            _version = "3.1.3";
        }

        public NavController(
            IAppUserService appUserService,
            IEdOrgService edOrgService,
            ISchoolYearService schoolYearService,
            IProgramAreaService programAreaService,
            IConfigurationValues configurationValues)
        {
            _appUserService = appUserService;
            _edOrgService = edOrgService;
            _schoolYearService = schoolYearService;
            _programAreaService = programAreaService;
            _configurationValues = configurationValues;
        }

        public ActionResult NavDropDowns()
        {
            var model = new NavMenusViewModel
            {
                AppUserSession = _appUserService.GetSession(),
                EdOrgs = _edOrgService.GetAuthorizedEdOrgs(),
                SchoolYears = _schoolYearService.GetSubmittableSchoolYears().OrderByDescending(x => x.EndYear),
                EdiamProfileLink = _configurationValues.EdiamProfileLink,
                ProgramAreas = _programAreaService.GetProgramAreas(),
                EdiamLogoutLink = _configurationValues.EdiamLogoutLink.StartsWith("~") ? 
                                      VirtualPathUtility.ToAbsolute(_configurationValues.EdiamLogoutLink) : 
                                      _configurationValues.EdiamLogoutLink
            };

            var session =  _appUserService.GetSession();

            // Set focused information after using the service to initialize the model.
            model.FocusedEdOrg = model.EdOrgs.FirstOrDefault(edOrg => edOrg.Id == session.FocusedEdOrgId);

            // If the user's School Year wasn't available any more, then select the first School Year whose data can be submitted.
            model.FocusedSchoolYear = model.SchoolYears
                                        .FirstOrDefault(sy => sy.Id == session.FocusedSchoolYearId) ??
                                        _schoolYearService.GetSubmittableSchoolYears().First();

            model.FocusedProgramArea = model.ProgramAreas.FirstOrDefault(pa => pa.Id == session.FocusedProgramAreaId)
                                       ?? model.ProgramAreas.FirstOrDefault();
            
            return PartialView("_NavDropDowns", model);
        }

        public string ProductVersion()
        {
            return _version;
        }

        public PartialViewResult Environment()
        {
            var viewModel = new EnvironmentViewModel { EnvironmentName = _configurationValues.EnvironmentName };
            return PartialView("~/Views/Shared/_Environment.cshtml", viewModel);
        }
    }
}
