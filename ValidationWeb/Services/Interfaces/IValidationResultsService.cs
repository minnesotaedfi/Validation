using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValidationWeb.Services
{
    public interface IValidationResultsService
    {
        List<ValidationReportSummary> GetValidationReportSummaries(int edOrgId);
        ValidationReportDetails GetValidationReportDetails(int validationReportId);
        List<string> AutocompleteErrorFilter(ValidationErrorFilter filterSpecification);
        FilteredValidationErrors GetFilteredValidationErrorTableData(ValidationErrorFilter filterSpecification);

        IList<ValidationErrorSummary> GetValidationErrors(int reportDetailsId);
    }
}
