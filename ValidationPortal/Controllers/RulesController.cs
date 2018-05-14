using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MDE.ValidationPortal
{
    [AllowAnonymous]
    public class RulesController : Controller
    {
        // GET: Rules/Collections
        [Route("Rules/Collections")]
        public ActionResult Collections()
        {
            return View();
        }

        // GET: Rules/Detail/id
        [Route("Detail/{id}")]
        public ActionResult Detail()
        {
            return View();
        }

        // GET: Rules/Editor/{id}
        [Route("Rule/{id}")]
        public ActionResult Rule()
        {
            return View();
        }

        // GET: Rules/Editor/{id}
        [Route("Editor/{id}")]
        public ActionResult Editor()
        {
            return View();
        }
    }
}