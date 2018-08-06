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

        public AdminController(
            IAppUserService appUserService,
            IEdOrgService edOrgService,
            ISchoolYearService schoolYearService)
        {
            _appUserService = appUserService;
            _edOrgService = edOrgService;
            _schoolYearService = schoolYearService;
        }

        // GET: Admin
        public ActionResult Index()
        {
            var model = new AdminIndexViewModel
            {
                AppUserSession = _appUserService.GetSession(),
                AuthorizedEdOrgs = _edOrgService.GetEdOrgs(),
                FocusedEdOrg = _edOrgService.GetEdOrgById(_appUserService.GetSession().FocusedEdOrgId),
                YearsOpenForDataSubmission = _schoolYearService.GetSubmittableSchoolYears().OrderByDescending(x => x.EndYear)
            };

            return View(model);
        }

        public string UpdateThresholdErrorValue(int id, int thresholdValue)
        {
            //TODO Ali - Finish updating.
            _schoolYearService.UpdateErrorThresholdValue(id, thresholdValue);

            return "";
        }
    }
}