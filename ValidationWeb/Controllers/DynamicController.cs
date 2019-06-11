using System.CodeDom;
using System.Linq;
using System.Web.Mvc;
using ValidationWeb.Models;
using ValidationWeb.Services.Interfaces;

namespace ValidationWeb.Controllers
{
    public class DynamicController : Controller
    {
        public DynamicController(IDynamicReportingService dynamicReportingService)
        {
            DynamicReportingService = dynamicReportingService;
        }

        protected IDynamicReportingService DynamicReportingService { get; set; }

        // GET: Dynamic
        public ActionResult Index()
        {
            // todo: build a ui
            DynamicReportingService.UpdateViewsAndRulesForSchoolYear(13);
            // var allRules = DynamicReportingService.GetRulesViews(13);

            var rules = DynamicReportingService.GetRulesViews(13);

            var view = rules.Single(x => x.Name == "SSDC"); 

            var report = new DynamicReportDefinition
            {
                Enabled = true,
                Name = "Test Report Name",
                Description = "Test Report Description",
                SchoolYearId = view.SchoolYearId,
                ValidationRulesViewId = view.Id
            };

            foreach (var field in view.RulesFields)
            {
                report.Fields.Add(
                    new DynamicReportField
                    {
                        Description = $"test {field.Name}",
                        Enabled = true,
                        ValidationRulesFieldId = field.Id,
                        Field = field
                    });
            }

            DynamicReportingService.SaveReportDefinition(report);

            return View();
        }
    }
}