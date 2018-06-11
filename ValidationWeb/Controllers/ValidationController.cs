using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ValidationWeb.Services;

namespace ValidationWeb
{
    public class ValidationController : Controller
    {
        private readonly IEdOrgService _edOrgService;
        private readonly IValidationResultsService _validationResultsService;

        public ValidationController(
            IEdOrgService edOrgService,
            IValidationResultsService validationResultsService)
        {
            _edOrgService = edOrgService;
            _validationResultsService = validationResultsService;
        }

        // GET: Validation/Reports
        public ActionResult Reports(string edOrgId)
        {
            var edOrg = _edOrgService.GetEdOrgById(edOrgId);
            var model = (edOrg == null) ? 
                new ValidationReportsViewModel
                {
                    DistrictName = "Invalid Education Organization Selected",
                    ReportSummaries = Enumerable.Empty<ValidationReportSummary>().ToList()
                } :
                new ValidationReportsViewModel
                {
                    DistrictName = edOrg.Name,
                    ReportSummaries = _validationResultsService.GetValidationReportSummaries(edOrgId)
                };
            return View(model);
        }
        public ActionResult Report(int id = 0)
        {
            if (id == 0)
            {
                return RedirectToAction("Reports");
            }

            var model = new ValidationReportDetailsViewModel { Details = _validationResultsService.GetValidationReportDetails(id) };
            return View(model);
        }
    }
}