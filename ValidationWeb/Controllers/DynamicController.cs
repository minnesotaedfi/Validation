using ValidationWeb.Services.Interfaces;

namespace ValidationWeb.Controllers
{
    using System.Web.Mvc;

    public class DynamicController : Controller
    {
        public DynamicController(IValidationRulesViewService validationRulesViewService)
        {
            ValidationRulesViewService = validationRulesViewService;
        }

        protected IValidationRulesViewService ValidationRulesViewService { get; set; }

        // GET: Dynamic
        public ActionResult Index()
        {
            // todo: build a ui
            ValidationRulesViewService.UpdateRulesForSchoolYear(13);

            var allRules = ValidationRulesViewService.GetRulesViews(13);
            
            return View();
        }
    }
}