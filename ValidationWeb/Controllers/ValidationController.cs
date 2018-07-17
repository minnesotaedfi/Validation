using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ValidationWeb.Services;

namespace ValidationWeb
{
    public class ValidationController : Controller
    {
        private readonly IAppUserService _appUserService;
        private readonly IEdOrgService _edOrgService;
        private readonly IValidationResultsService _validationResultsService;

        public ValidationController(
            IAppUserService appUserService,
            IEdOrgService edOrgService,
            IValidationResultsService validationResultsService)
        {
            _appUserService = appUserService;
            _edOrgService = edOrgService;
            _validationResultsService = validationResultsService;
        }

        // GET: Validation/Reports
        public ActionResult Reports()
        {
            var edOrg = _appUserService.GetSession().FocusedEdOrg;
            var model = (edOrg == null) ? 
                new ValidationReportsViewModel
                {
                    DistrictName = "Invalid Education Organization Selected",
                    ReportSummaries = Enumerable.Empty<ValidationReportSummary>().ToList()
                } :
                new ValidationReportsViewModel
                {
                    DistrictName = edOrg.OrganizationName,
                    ReportSummaries = _validationResultsService.GetValidationReportSummaries(edOrg.Id)
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


        // GET: Validation/Reports
        public ActionResult Submissions()
        {
            return View();
        }

        // GET: Validation/GetErrorReportTableData
        public String GetValidationErrorSummaryTableData(int validationReportSummaryId)
        {
            var jsonSerialiser = new JavaScriptSerializer();
            List<ValidationErrorSummary> errorSummaryList = _validationResultsService.GetValidationErrorSummaryTableData(validationReportSummaryId);
            var json = jsonSerialiser.Serialize(errorSummaryList);

            return json;
        }

    }
}