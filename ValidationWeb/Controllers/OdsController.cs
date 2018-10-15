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
            var edOrg = _edOrgService.GetEdOrgById(_appUserService.GetSession().FocusedEdOrgId);
            var edOrgName = (edOrg == null) ? "Invalid Education Organization Selected" : edOrg.OrganizationName;
            var edOrgId = (edOrg == null) ? "Invalid Education Organization Selected" : edOrg.OrganizationName;
            var theUser = _appUserService.GetUser();
            var model = new OdsReportsViewModel
            {
                EdOrgId = edOrgId,
                EdOrgName = edOrgName,
                User = theUser
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
