using System.Net;
using System.Web.Mvc;

namespace ValidationWeb.Controllers
{
    [AllowAnonymous]
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Index()
        {
            Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return View();
        }

        public ActionResult Unauthorized()
        {
            Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return View();
        }
    }
}