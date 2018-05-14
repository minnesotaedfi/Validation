using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MDE.ValidationPortal
{
    [AllowAnonymous]
    public class ValidationController : Controller
    {
        // GET: Validation/Reports
        [Route("Validation/Reports")]
        public ActionResult Reports(int id = 0)
        {
            return View();
        }
    }
}