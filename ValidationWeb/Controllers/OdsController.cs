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
        protected readonly IOdsDataService _odsDataService;
        protected readonly IRulesEngineService _rulesEngineService;
        protected readonly ISchoolYearService _schoolYearService;
        protected readonly Model _engineObjectModel;

        public OdsController(
            IAppUserService appUserService,
            IEdOrgService edOrgService,
            IOdsDataService odsDataService,
            IRulesEngineService rulesEngineService,
            ISchoolYearService schoolYearService,
            Model engineObjectModel)
        {
            _appUserService = appUserService;
            _edOrgService = edOrgService;
            _odsDataService = odsDataService;
            _engineObjectModel = engineObjectModel;
            _rulesEngineService = rulesEngineService;
            _schoolYearService = schoolYearService;
        }

        // GET: Ods/Reports
        public ActionResult Reports()
        {
            var edOrg = _edOrgService.GetEdOrgById(_appUserService.GetSession().FocusedEdOrgId);
            var edOrgName = (edOrg == null) ? "Invalid Education Organization Selected" : edOrg.OrganizationName;
            var edOrgId = (edOrg == null) ? "Invalid Education Organization Selected" : edOrg.Id;
            var theUser = _appUserService.GetUser();
            var model = new OdsReportsViewModel
            {
                EdOrgId = edOrgId,
                EdOrgName = edOrgName,
                User = theUser
            };
            return View(model);
        }

        // GET: Ods/DemographicsReport
        public ActionResult DemographicsReport()
        {
            var edOrg = _edOrgService.GetEdOrgById(_appUserService.GetSession().FocusedEdOrgId);
            var schoolYear = _schoolYearService.GetSchoolYearById(_appUserService.GetSession().FocusedSchoolYearId);
            var edOrgName = (edOrg == null) ? "Invalid Education Organization Selected" : edOrg.OrganizationName;
            var edOrgId = (edOrg == null) ? "Invalid Education Organization Selected" : edOrg.Id;
            var theUser = _appUserService.GetUser();
            var results = _odsDataService.GetDistrictAncestryRaceCounts(edOrgId, schoolYear.StartYear);
            var model = new OdsDemographicsReportViewModel
            {
                EdOrgId = edOrgId,
                EdOrgName = edOrgName,
                User = theUser,
                Results = results
            };
            return View(model);
        }

        // GET: Ods/MultipleEnrollmentsReport
        public ActionResult MultipleEnrollmentsReport()
        {
            var edOrg = _edOrgService.GetEdOrgById(_appUserService.GetSession().FocusedEdOrgId);
            var schoolYear = _schoolYearService.GetSchoolYearById(_appUserService.GetSession().FocusedSchoolYearId);
            var edOrgName = (edOrg == null) ? "Invalid Education Organization Selected" : edOrg.OrganizationName;
            var edOrgId = (edOrg == null) ? "Invalid Education Organization Selected" : edOrg.Id;
            var theUser = _appUserService.GetUser();
            var results = _odsDataService.GetMultipleEnrollmentCounts(edOrgId, schoolYear.StartYear);
            var model = new OdsMultipleEnrollmentsReportViewModel
            {
                EdOrgId = edOrgId,
                EdOrgName = edOrgName,
                User = theUser,
                Results = results
            };
            return View(model);
        }

        // GET: Ods/StudentProgramsReport
        public ActionResult StudentProgramsReport()
        {
            var edOrg = _edOrgService.GetEdOrgById(_appUserService.GetSession().FocusedEdOrgId);
            var schoolYear = _schoolYearService.GetSchoolYearById(_appUserService.GetSession().FocusedSchoolYearId);
            var edOrgName = (edOrg == null) ? "Invalid Education Organization Selected" : edOrg.OrganizationName;
            var edOrgId = (edOrg == null) ? "Invalid Education Organization Selected" : edOrg.Id;
            var theUser = _appUserService.GetUser();
            var results = _odsDataService.GetStudentProgramsCounts(edOrgId, schoolYear.StartYear);
            var model = new OdsStudentProgramsReportViewModel
            {
                EdOrgId = edOrgId,
                EdOrgName = edOrgName,
                User = theUser,
                Results = results
            };
            return View(model);
        }

        // GET: Ods/ChangeOfEnrollmentReport
        public ActionResult ChangeOfEnrollmentReport()
        {
            var model = new OdsChangeOfEnrollmentReportViewModel
            {

            };
            return View(model);
        }

        // GET: Ods/ResidentsEnrolledElsewhereReport
        public ActionResult ResidentsEnrolledElsewhereReport()
        {
            var edOrg = _edOrgService.GetEdOrgById(_appUserService.GetSession().FocusedEdOrgId);
            var schoolYear = _schoolYearService.GetSchoolYearById(_appUserService.GetSession().FocusedSchoolYearId);
            var edOrgName = (edOrg == null) ? "Invalid Education Organization Selected" : edOrg.OrganizationName;
            var edOrgId = (edOrg == null) ? "Invalid Education Organization Selected" : edOrg.Id;
            var theUser = _appUserService.GetUser();
            var results = _odsDataService.GetMultipleEnrollmentCounts(edOrgId, schoolYear.StartYear);
            var model = new OdsResidentsEnrolledElsewhereReportViewModel
            {
                EdOrgId = edOrgId,
                EdOrgName = edOrgName,
                User = theUser,
                Results = results
            };
            return View(model);
        }

        // GET: Ods/IdentityIssuesReport
        public ActionResult IdentityIssuesReport()
        {
            var model = new OdsIdentityIssuesReportViewModel
            {

            };
            return View(model);
        }

        // GET: Ods/Level1IssuesReport
        public ActionResult Level1IssuesReport()
        {
            var model = new OdsLevel1IssuesReportViewModel
            {

            };
            return View(model);
        }
    }
}
