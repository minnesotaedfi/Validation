using System.Collections.Generic;
using ValidationWeb.Models;

namespace ValidationWeb.Services.Interfaces
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
