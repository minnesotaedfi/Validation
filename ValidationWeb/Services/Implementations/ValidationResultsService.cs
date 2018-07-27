using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValidationWeb.Services
{
    public class ValidationResultsService : IValidationResultsService
    {
        private ErrorSeverityLookup anError = ValidationPortalDbMigrationConfiguration.ErrorSeverityLookups.First(sev => sev.CodeValue == ErrorSeverity.Error.ToString());
        private ErrorSeverityLookup aWarning = ValidationPortalDbMigrationConfiguration.ErrorSeverityLookups.First(sev => sev.CodeValue == ErrorSeverity.Warning.ToString());
        protected readonly ValidationPortalDbContext _portalDbContext;
        public ValidationResultsService(ValidationPortalDbContext portalDbContext)
        {
            _portalDbContext = portalDbContext;
        }

        public ValidationReportDetails GetValidationReportDetails(int validationReportId)
        {
            return _portalDbContext.ValidationReportDetails.FirstOrDefault(vrd => vrd.Id == validationReportId);
        }

        public List<ValidationReportSummary> GetValidationReportSummaries(string edOrgId)
        {
            return _portalDbContext.ValidationReportSummaries.Where(vrs => vrs.EdOrgId == edOrgId).ToList();
        }

        public List<ValidationErrorSummary> GetValidationErrorSummaryTableData(int validationReportSummaryId)
        {
            return _portalDbContext.ValidationErrorSummaries.Where(ves => ves.Id == validationReportSummaryId).ToList();
        }
    }
}