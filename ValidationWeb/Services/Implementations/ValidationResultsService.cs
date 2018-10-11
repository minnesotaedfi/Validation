using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ValidationWeb.Services
{
    public class ValidationResultsService : IValidationResultsService
    {
        private ErrorSeverityLookup anError = ValidationPortalDbMigrationConfiguration.ErrorSeverityLookups.First(sev => sev.CodeValue == ErrorSeverity.Error.ToString());
        private ErrorSeverityLookup aWarning = ValidationPortalDbMigrationConfiguration.ErrorSeverityLookups.First(sev => sev.CodeValue == ErrorSeverity.Warning.ToString());
        protected readonly ValidationPortalDbContext _portalDbContext;
        const int MaxPageSize = 10000;
        const int AutocompleteSuggestionCount = 8;

        public ValidationResultsService(ValidationPortalDbContext portalDbContext)
        {
            _portalDbContext = portalDbContext;
        }

        public ValidationReportDetails GetValidationReportDetails(int validationReportId)
        {
            var reportDetails = _portalDbContext.ValidationReportDetails.Include(vrds => vrds.ValidationReportSummary).FirstOrDefault(vrd => vrd.ValidationReportSummaryId == validationReportId);
            reportDetails.CompletedWhen = reportDetails.CompletedWhen.HasValue ? reportDetails.CompletedWhen.Value.ToLocalTime() : reportDetails.CompletedWhen;
            reportDetails.ValidationReportSummary.RequestedWhen = reportDetails.ValidationReportSummary.RequestedWhen.ToLocalTime();
            return reportDetails;
        }

        public List<ValidationReportSummary> GetValidationReportSummaries(string edOrgId)
        {
            var reportSummaryList = _portalDbContext.ValidationReportSummaries.Where(vrs => vrs.EdOrgId == edOrgId).ToList();
            reportSummaryList.ForEach(rsum => 
            {
                rsum.CompletedWhen = rsum.CompletedWhen?.ToLocalTime();
                rsum.RequestedWhen = rsum.RequestedWhen.ToLocalTime();
            });
            return reportSummaryList;
        }

        public List<string> AutocompleteErrorFilter(ValidationErrorFilter filterSpecification)
        {
            // First limit to errors/warnings of one execution of the rules engine.
            var filteredErrorQuery = _portalDbContext.ValidationErrorSummaries.Where(er0 => er0.ValidationReportDetailsId == filterSpecification.reportDetailsId);
            IQueryable<string> autocompleteQuery;
            switch (filterSpecification.autocompleteColumn)
            {
                case "studentname":
                    autocompleteQuery = filteredErrorQuery.Select(er2 => er2.StudentFullName);
                    break;
                case "school":
                    autocompleteQuery = filteredErrorQuery.Select(er2 => er2.School);
                    break;
                case "grade":
                    autocompleteQuery = filteredErrorQuery.Select(er2 => er2.Grade);
                    break;
                case "rule":
                    autocompleteQuery = filteredErrorQuery.Select(er2 => er2.ErrorCode);
                    break;
                case "errortext":
                    autocompleteQuery = filteredErrorQuery.Select(er2 => er2.ErrorText);
                    break;
                default:
                    return null;
            }
            return autocompleteQuery.Where(sg => sg.Contains(filterSpecification.autocompleteText)).Distinct().Take(AutocompleteSuggestionCount).ToList();
        }

        public FilteredValidationErrors GetFilteredValidationErrorTableData(ValidationErrorFilter filterSpecification)
        {
            // First limit to errors/warnings of one execution of the rules engine.
            var filteredErrorQuery = _portalDbContext.ValidationErrorSummaries.Where(er0 => er0.ValidationReportDetailsId == filterSpecification.reportDetailsId);

            #region FIlter on search texts
            var columnNames = filterSpecification.filterColumns ?? new string[0];
            var searchTexts = filterSpecification.filterTexts ?? new string[0];
            for (int colIndex = 0; colIndex < columnNames.Length; colIndex++)
            {
                if (searchTexts.Length > colIndex && !string.IsNullOrWhiteSpace(searchTexts[colIndex]))
                {
                    var normalizedSearchText = searchTexts[colIndex].Trim().ToLowerInvariant();
                    switch (columnNames[colIndex])
                    {
                        case "studentid":
                            filteredErrorQuery = filteredErrorQuery.Where(er2 => er2.StudentUniqueId.Contains(normalizedSearchText));
                            break;
                        case "studentname":
                            filteredErrorQuery = filteredErrorQuery.Where(er2 => er2.StudentFullName.Contains(normalizedSearchText));
                            break;
                        case "component":
                            filteredErrorQuery = filteredErrorQuery.Where(er2 => er2.Component.Contains(normalizedSearchText));
                            break;
                        case "school":
                            filteredErrorQuery = filteredErrorQuery.Where(er2 => er2.School.Contains(normalizedSearchText));
                            break;
                        case "grade":
                            filteredErrorQuery = filteredErrorQuery.Where(er2 => er2.Grade.Contains(normalizedSearchText));
                            break;
                        case "rule":
                            filteredErrorQuery = filteredErrorQuery.Where(er2 => er2.ErrorCode.Contains(normalizedSearchText));
                            break;
                        case "errortext":
                            filteredErrorQuery = filteredErrorQuery.Where(er2 => er2.ErrorText.Contains(normalizedSearchText));
                            break;
                        case "errortype":
                            if (normalizedSearchText == "error")
                            {
                                filteredErrorQuery = filteredErrorQuery.Where(er2 => er2.SeverityId == (int)(ErrorSeverity.Error));
                            }
                            else if (normalizedSearchText == "warning")
                            {
                                filteredErrorQuery = filteredErrorQuery.Where(er2 => er2.SeverityId == (int)(ErrorSeverity.Warning));
                            }
                            break;
                        default:
                            // Do nothing
                            break;
                    }
                }
            }
            #endregion Filter on search texts

            #region Sort results
            var sortColumnNames = filterSpecification.sortColumns ?? new string[0];
            var sortDirections = filterSpecification.sortDirections ?? new string[0];
            var isDescending = false;
            var isSecondarySortColumn = false;
            for (int colIndex = 0; colIndex < sortColumnNames.Length; colIndex++)
            {
                // Ascending is the default sort direction
                isDescending = (sortDirections.Length > colIndex) && (sortDirections[colIndex] ?? String.Empty).ToLowerInvariant().StartsWith("d");

                switch (sortColumnNames[colIndex])
                {
                    case "studentid":
                        if (!isDescending && !isSecondarySortColumn) { filteredErrorQuery = filteredErrorQuery.OrderBy(er2 => er2.StudentUniqueId); }
                        else if (isDescending && !isSecondarySortColumn) { filteredErrorQuery = filteredErrorQuery.OrderByDescending(er2 => er2.StudentUniqueId); }
                        else if (!isDescending && isSecondarySortColumn) { filteredErrorQuery = (filteredErrorQuery as IOrderedQueryable<ValidationErrorSummary>).ThenBy(er2 => er2.StudentUniqueId); }
                        else if (isDescending && isSecondarySortColumn) { filteredErrorQuery = (filteredErrorQuery as IOrderedQueryable<ValidationErrorSummary>).ThenByDescending(er2 => er2.StudentUniqueId); }
                        else { break; }
                        isSecondarySortColumn = true;
                        break;
                    case "studentname":
                        if (!isDescending && !isSecondarySortColumn) { filteredErrorQuery = filteredErrorQuery.OrderBy(er2 => er2.StudentFullName); }
                        else if (isDescending && !isSecondarySortColumn) { filteredErrorQuery = filteredErrorQuery.OrderByDescending(er2 => er2.StudentFullName); }
                        else if (!isDescending && isSecondarySortColumn) { filteredErrorQuery = (filteredErrorQuery as IOrderedQueryable<ValidationErrorSummary>).ThenBy(er2 => er2.StudentFullName); }
                        else if (isDescending && isSecondarySortColumn) { filteredErrorQuery = (filteredErrorQuery as IOrderedQueryable<ValidationErrorSummary>).ThenByDescending(er2 => er2.StudentFullName); }
                        else { break; }
                        isSecondarySortColumn = true;
                        break;
                    case "component":
                        if (!isDescending && !isSecondarySortColumn) { filteredErrorQuery = filteredErrorQuery.OrderBy(er2 => er2.Component); }
                        else if (isDescending && !isSecondarySortColumn) { filteredErrorQuery = filteredErrorQuery.OrderByDescending(er2 => er2.Component); }
                        else if (!isDescending && isSecondarySortColumn) { filteredErrorQuery = (filteredErrorQuery as IOrderedQueryable<ValidationErrorSummary>).ThenBy(er2 => er2.Component); }
                        else if (isDescending && isSecondarySortColumn) { filteredErrorQuery = (filteredErrorQuery as IOrderedQueryable<ValidationErrorSummary>).ThenByDescending(er2 => er2.Component); }
                        else { break; }
                        isSecondarySortColumn = true;
                        break;
                    case "school":
                        if (!isDescending && !isSecondarySortColumn) { filteredErrorQuery = filteredErrorQuery.OrderBy(er2 => er2.School); }
                        else if (isDescending && !isSecondarySortColumn) { filteredErrorQuery = filteredErrorQuery.OrderByDescending(er2 => er2.School); }
                        else if (!isDescending && isSecondarySortColumn) { filteredErrorQuery = (filteredErrorQuery as IOrderedQueryable<ValidationErrorSummary>).ThenBy(er2 => er2.School); }
                        else if (isDescending && isSecondarySortColumn) { filteredErrorQuery = (filteredErrorQuery as IOrderedQueryable<ValidationErrorSummary>).ThenByDescending(er2 => er2.School); }
                        else { break; }
                        isSecondarySortColumn = true;
                        break;
                    case "grade":
                        if (!isDescending && !isSecondarySortColumn) { filteredErrorQuery = filteredErrorQuery.OrderBy(er2 => er2.Grade); }
                        else if (isDescending && !isSecondarySortColumn) { filteredErrorQuery = filteredErrorQuery.OrderByDescending(er2 => er2.Grade); }
                        else if (!isDescending && isSecondarySortColumn) { filteredErrorQuery = (filteredErrorQuery as IOrderedQueryable<ValidationErrorSummary>).ThenBy(er2 => er2.Grade); }
                        else if (isDescending && isSecondarySortColumn) { filteredErrorQuery = (filteredErrorQuery as IOrderedQueryable<ValidationErrorSummary>).ThenByDescending(er2 => er2.Grade); }
                        else { break; }
                        isSecondarySortColumn = true;
                        break;
                    case "rule":
                        if (!isDescending && !isSecondarySortColumn) { filteredErrorQuery = filteredErrorQuery.OrderBy(er2 => er2.ErrorCode); }
                        else if (isDescending && !isSecondarySortColumn) { filteredErrorQuery = filteredErrorQuery.OrderByDescending(er2 => er2.ErrorCode); }
                        else if (!isDescending && isSecondarySortColumn) { filteredErrorQuery = (filteredErrorQuery as IOrderedQueryable<ValidationErrorSummary>).ThenBy(er2 => er2.ErrorCode); }
                        else if (isDescending && isSecondarySortColumn) { filteredErrorQuery = (filteredErrorQuery as IOrderedQueryable<ValidationErrorSummary>).ThenByDescending(er2 => er2.ErrorCode); }
                        else { break; }
                        isSecondarySortColumn = true;
                        break;
                    case "errortext":
                        if (!isDescending && !isSecondarySortColumn) { filteredErrorQuery = filteredErrorQuery.OrderBy(er2 => er2.ErrorText); }
                        else if (isDescending && !isSecondarySortColumn) { filteredErrorQuery = filteredErrorQuery.OrderByDescending(er2 => er2.ErrorText); }
                        else if (!isDescending && isSecondarySortColumn) { filteredErrorQuery = (filteredErrorQuery as IOrderedQueryable<ValidationErrorSummary>).ThenBy(er2 => er2.ErrorText); }
                        else if (isDescending && isSecondarySortColumn) { filteredErrorQuery = (filteredErrorQuery as IOrderedQueryable<ValidationErrorSummary>).ThenByDescending(er2 => er2.ErrorText); }
                        else { break; }
                        isSecondarySortColumn = true;
                        break;
                    case "errortype":
                        if (!isDescending && !isSecondarySortColumn) { filteredErrorQuery = filteredErrorQuery.OrderBy(er2 => er2.SeverityId); }
                        else if (isDescending && !isSecondarySortColumn) { filteredErrorQuery = filteredErrorQuery.OrderByDescending(er2 => er2.SeverityId); }
                        else if (!isDescending && isSecondarySortColumn) { filteredErrorQuery = (filteredErrorQuery as IOrderedQueryable<ValidationErrorSummary>).ThenBy(er2 => er2.SeverityId); }
                        else if (isDescending && isSecondarySortColumn) { filteredErrorQuery = (filteredErrorQuery as IOrderedQueryable<ValidationErrorSummary>).ThenByDescending(er2 => er2.SeverityId); }
                        else { break; }
                        isSecondarySortColumn = true;
                        break;
                    default:
                        // Do nothing
                        break;
                }
            }
            // If still unsorted - add default sorting, necessary for LINQ-to-Entities paging to function correctly.
            if (!isSecondarySortColumn)
            {
                filteredErrorQuery = filteredErrorQuery.OrderBy(er2 => er2.ErrorCode);
            }
            #endregion Sort results

            #region Count results
            var result = new FilteredValidationErrors { TotalFilteredErrorCount = filteredErrorQuery.Count() };
            #endregion Count results

            #region Page results & Sanity checks

            #region Page Size
            if (filterSpecification.pageSize <= 0 || filterSpecification.pageSize > MaxPageSize) filterSpecification.pageSize = MaxPageSize;
            result.PageSize = filterSpecification.pageSize;
            #endregion Page Size

            #region Record Offset
            // Record offset can be no farther than the last page, and no lower than 0.
            result.RecordOffset = Math.Max(filterSpecification.pageStartingOffset, 0);
            // maxRecordOffset is the start of the last page.
            var maxRecordOffset = ((result.TotalFilteredErrorCount - 1) / filterSpecification.pageSize) * filterSpecification.pageSize;
            result.RecordOffset = Math.Min(maxRecordOffset, result.RecordOffset);
            #endregion Record Offset

            #region Page Number
            var recordsRemaining = result.TotalFilteredErrorCount - result.RecordOffset;
            result.PageNumber = (result.RecordOffset / result.PageSize) + 1;
            #endregion Page Number

            result.TotalPagesCount = (result.TotalFilteredErrorCount / result.PageSize) + ((result.TotalFilteredErrorCount % result.PageSize == 0) ? 0 : 1);
            var maxToReturn = Math.Min(recordsRemaining, filterSpecification.pageSize);
            result.FilteredErrorSummariesPage = filteredErrorQuery.Skip(result.RecordOffset).Take(maxToReturn).ToList();

            #endregion Page results

            return result;
        }

        // TODO: ********** AUTOCOMPLETE ************
    }
}
