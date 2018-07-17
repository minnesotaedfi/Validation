using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ValidationPortalAuthServer.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var returnUrl = System.Net.WebUtility.UrlDecode(Request["returnUrl"]) ?? string.Empty;
            var model = new ChooseUserNameViewModel { ReturnUrl = returnUrl };
            return View(model);
        }
    }
}