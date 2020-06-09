using System.Collections.Generic;

using Validation.DataModels;

using ValidationWeb.Models;

namespace ValidationWeb.Services.Interfaces
{
    public interface IValidationResultsService
    {
        IEnumerable<ValidationReportSummary> GetValidationReportSummaries(int edOrgId, ProgramAreaLookup programArea = null);
        
        ValidationReportDetails GetValidationReportDetails(int validationReportId);

        List<string> AutocompleteErrorFilter(ValidationErrorFilter filterSpecification);
        
        FilteredValidationErrors GetFilteredValidationErrorTableData(ValidationErrorFilter filterSpecification);

        IList<ValidationErrorSummary> GetValidationErrors(int reportDetailsId);
    }
}
