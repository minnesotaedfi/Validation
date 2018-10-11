using Engine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ValidationWeb.Services;

namespace ValidationWeb
{
    public class OdsController : Controller
    {
        protected readonly IAppUserService _appUserService;
        protected readonly IEdOrgService _edOrgService;
        protected readonly IRulesEngineService _rulesEngineService;
        protected readonly Model _engineObjectModel;

        public OdsController(
            IAppUserService appUserService,
            IEdOrgService edOrgService,
            IRulesEngineService rulesEngineService,
            Model engineObjectModel)
        {
            _appUserService = appUserService;
            _edOrgService = edOrgService;
            _engineObjectModel = engineObjectModel;
            _rulesEngineService = rulesEngineService;
        }

        // GET: Ods/Reports
        public ActionResult Reports()
        {
            //var edOrg = _edOrgService.GetEdOrgById(_appUserService.GetSession().FocusedEdOrgId);
            //var rulesCollections = _engineObjectModel.Collections.OrderBy(x => x.CollectionId).ToList();
            //var theUser = _appUserService.GetUser();
            //var districtName = (edOrg == null) ? "Invalid Education Organization Selected" : edOrg.OrganizationName;
            //// Display the latest summary first (by default)
            //var reportSummaries = (edOrg == null) ? Enumerable.Empty<ValidationReportSummary>().ToList() : _validationResultsService.GetValidationReportSummaries(edOrg.Id).OrderByDescending(rs => rs.CompletedWhen).ToList();
            //var model = new ValidationReportsViewModel
            //{
            //    DistrictName = districtName,
            //    TheUser = theUser,
            //    ReportSummaries = reportSummaries,
            //    RulesCollections = rulesCollections,
            //    SchoolYears = _schoolYearService.GetSubmittableSchoolYears().ToList(),
            //    FocusedEdOrgId = _appUserService.GetSession().FocusedEdOrgId,
            //    FocusedSchoolYearId = _appUserService.GetSession().FocusedSchoolYearId
            //};
            var model = new OdsReportsViewModel
            {

            };
            return View(model);
        }

        // GET: Ods/Report/{id}
        public ActionResult Report(int id = 0)
        {
            if (id == 0)
            {
                return RedirectToAction("Reports");
            }

            var model = new OdsReportViewModel
            {

            };
            return View(model);
        }
    }
}
