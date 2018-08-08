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

        public bool UpdateThresholdErrorValue(int id, decimal thresholdValue)
        {
            return _schoolYearService.UpdateErrorThresholdValue(id, thresholdValue);
        }

        public bool AddNewSchoolYear(string startYear, string endYear)
        {
            //testing
            return false;
            return _schoolYearService.AddNewSchoolYear(startYear, endYear);
        }

        public bool RemoveSchoolYear(int id)
        {
            return _schoolYearService.RemoveSchoolYear(id);
        }
    }
}