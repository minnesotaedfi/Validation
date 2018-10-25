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
        protected readonly ISchoolYearService _schoolyearService;
        protected readonly Model _engineObjectModel;

        public OdsController(
            IAppUserService appUserService,
            IEdOrgService edOrgService,
            IOdsDataService odsDataService,
            IRulesEngineService rulesEngineService,
            ISchoolYearService schoolyearService,
            Model engineObjectModel)
        {
            _appUserService = appUserService;
            _edOrgService = edOrgService;
            _odsDataService = odsDataService;
            _engineObjectModel = engineObjectModel;
            _rulesEngineService = rulesEngineService;
            _schoolyearService = schoolyearService;
        }

        // GET: Ods/Reports
        public ActionResult Reports()
        {
            var session = _appUserService.GetSession();
            var edOrg = _edOrgService.GetEdOrgById(session.FocusedEdOrgId, session.FocusedSchoolYearId);
            var edOrgName = (edOrg == null) ? "Invalid Education Organization Selected" : edOrg.OrganizationName;
            var edOrgId = edOrg.Id;
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
        public ActionResult DemographicsReport(bool isStateMode = false, int? districtToDisplay = null)
        {
            var session = _appUserService.GetSession();
            var edOrg = _edOrgService.GetEdOrgById(session.FocusedEdOrgId, session.FocusedSchoolYearId);
            var edOrgName = (edOrg == null) ? "Invalid Education Organization Selected" : edOrg.OrganizationName;
            var edOrgId = edOrg.Id;
            // A state user can look at any district via a link, without changing the default district.
            if (districtToDisplay.HasValue && session.UserIdentity.AuthorizedEdOrgs.Select(eorg => eorg.Id).Contains(districtToDisplay.Value))
            {
                edOrgId = districtToDisplay.Value;
            }
            var fourDigitSchoolYear = _schoolyearService.GetSchoolYearById(session.FocusedSchoolYearId).StartYear;
            var theUser = _appUserService.GetUser();
            var results = _odsDataService.GetDistrictAncestryRaceCounts(isStateMode ? (int?)null : edOrgId, fourDigitSchoolYear);
            var model = new OdsDemographicsReportViewModel
            {
                EdOrgId = edOrgId,
                EdOrgName = edOrgName,
                User = theUser,
                Results = results,
                IsStateMode = isStateMode
            };
            return View(model);
        }

        // GET: Ods/MultipleEnrollmentsReport
        public ActionResult MultipleEnrollmentsReport(bool isStateMode = false)
        {
            var session = _appUserService.GetSession();
            var edOrg = _edOrgService.GetEdOrgById(session.FocusedEdOrgId, session.FocusedSchoolYearId);
            var edOrgName = (edOrg == null) ? "Invalid Education Organization Selected" : edOrg.OrganizationName;
            var edOrgId = edOrg.Id;
            var fourDigitSchoolYear = _schoolyearService.GetSchoolYearById(session.FocusedSchoolYearId).StartYear;
            var theUser = _appUserService.GetUser();
            var results = _odsDataService.GetMultipleEnrollmentCounts(edOrgId, fourDigitSchoolYear);
            var model = new OdsMultipleEnrollmentsReportViewModel
            {
                EdOrgId = edOrgId,
                EdOrgName = edOrgName,
                User = theUser,
                Results = results,
                IsStateMode = isStateMode
            };
            return View(model);
        }

        // GET: Ods/StudentProgramsReport
        public ActionResult StudentProgramsReport()
        {
            var session = _appUserService.GetSession();
            var edOrg = _edOrgService.GetEdOrgById(session.FocusedEdOrgId, session.FocusedSchoolYearId);
            var edOrgName = (edOrg == null) ? "Invalid Education Organization Selected" : edOrg.OrganizationName;
            var edOrgId = edOrg.Id;
            var fourDigitSchoolYear = _schoolyearService.GetSchoolYearById(session.FocusedSchoolYearId).StartYear;
            var theUser = _appUserService.GetUser();
            var results = _odsDataService.GetStudentProgramsCounts(edOrgId, fourDigitSchoolYear);
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
            var session = _appUserService.GetSession();
            var edOrg = _edOrgService.GetEdOrgById(session.FocusedEdOrgId, session.FocusedSchoolYearId);
            var edOrgName = (edOrg == null) ? "Invalid Education Organization Selected" : edOrg.OrganizationName;
            var edOrgId = edOrg.Id;
            var fourDigitSchoolYear = _schoolyearService.GetSchoolYearById(session.FocusedSchoolYearId).StartYear;
            var theUser = _appUserService.GetUser();
            var results = _odsDataService.GetMultipleEnrollmentCounts(edOrgId, fourDigitSchoolYear);
            var model = new OdsChangeOfEnrollmentReportViewModel
            {
                EdOrgId = edOrgId,
                EdOrgName = edOrgName,
                User = theUser,
                Results = new List<DemographicsCountReportQuery>()
            };
            return View(model);
        }

        // GET: Ods/ResidentsEnrolledElsewhereReport
        public ActionResult ResidentsEnrolledElsewhereReport()
        {
            var session = _appUserService.GetSession();
            var edOrg = _edOrgService.GetEdOrgById(session.FocusedEdOrgId, session.FocusedSchoolYearId);
            var edOrgName = (edOrg == null) ? "Invalid Education Organization Selected" : edOrg.OrganizationName;
            var edOrgId = edOrg.Id;
            var fourDigitSchoolYear = _schoolyearService.GetSchoolYearById(session.FocusedSchoolYearId).StartYear;
            var theUser = _appUserService.GetUser();
            var results = _odsDataService.GetMultipleEnrollmentCounts(edOrgId, fourDigitSchoolYear);
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
