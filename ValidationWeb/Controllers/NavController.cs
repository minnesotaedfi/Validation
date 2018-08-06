using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using ValidationWeb.Services;

namespace ValidationWeb
{
    public class NavController : Controller
    {
        private static string _version;
        private readonly IAppUserService _appUserService;
        private readonly IEdOrgService _edOrgService;
        private readonly ISchoolYearService _schoolYearService;
        private readonly IValidatedDataSubmissionService _validatedDataSubmissionService;

        static NavController()
        {
            _version = new Version(FileVersionInfo.GetVersionInfo(Assembly.GetCallingAssembly().Location).ProductVersion).ToString();
        }

        public NavController(
            IAppUserService appUserService,
            IEdOrgService edOrgService,
            ISchoolYearService schoolYearService,
            IValidatedDataSubmissionService validatedDataSubmissionService)
        {
            _appUserService = appUserService;
            _edOrgService = edOrgService;
            _schoolYearService = schoolYearService;
            _validatedDataSubmissionService = validatedDataSubmissionService;
        }

        public ActionResult NavDropDowns()
        {
            var model = new NavMenusViewModel
            {
                AppUserSession = _appUserService.GetSession(),
                EdOrgs = _edOrgService.GetEdOrgs(),
                SchoolYears = _schoolYearService.GetSubmittableSchoolYears().OrderByDescending(x => x.EndYear)
            };
            
            // Set focused information after using the service to initailize the model.
            model.FocusedEdOrg = model.EdOrgs.FirstOrDefault(edOrg => edOrg.Id == _appUserService.GetSession().FocusedEdOrgId);
            // If the user's School Year wasn't available any more, then select the first School Year whose data can be submitted.
            model.FocusedSchoolYear = model.SchoolYears.FirstOrDefault(sy => sy.Id == _appUserService.GetSession().FocusedSchoolYearId) ??
                                      _schoolYearService.GetSubmittableSchoolYears().First();

            return PartialView("_NavDropDowns", model);
        }

        public string ProductVersion()
        {
            return _version;
        }
    }
}
