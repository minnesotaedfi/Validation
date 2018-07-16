using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValidationWeb.Services
{
    public interface IValidationResultsService
    {
        List<ValidationReportSummary> GetValidationReportSummaries(string edOrgId);
        ValidationReportDetails GetValidationReportDetails(int validationReportId);
        List<ValidationErrorSummary> GetValidationErrorSummaryTableData(int validationReportSummaryId);
    }
}
