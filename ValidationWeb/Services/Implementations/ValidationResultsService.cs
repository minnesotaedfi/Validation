using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

using Validation.DataModels;

using ValidationWeb.Database;
using ValidationWeb.Models;
using ValidationWeb.Services.Interfaces;

namespace ValidationWeb.Services.Implementations
{
    public class ValidationResultsService : IValidationResultsService
    {
        private const int MaxPageSize = 10000;
        private const int AutocompleteSuggestionCount = 8;
        private readonly ISubmissionCycleService SubmissionCycleService;
        private readonly ILoggingService LoggingService;
        private readonly IDbContextFactory<ValidationPortalDbContext> DbContextFactory;
        
        public ValidationResultsService(
            ISubmissionCycleService submissionCycleService,
            ILoggingService loggingService, 
            IDbContextFactory<ValidationPortalDbContext> dbContextFactory)
        {
            SubmissionCycleService = submissionCycleService;
            LoggingService = loggingService;
            DbContextFactory = dbContextFactory;
        }

        public ValidationReportDetails GetValidationReportDetails(int validationReportId)
        {
            LoggingService.LogDebugMessage($"Retrieving Validation Report Details for report with ID number: {validationReportId.ToString()}");
            using (var portalDbContext = DbContextFactory.Create())
            {
                var reportDetails = portalDbContext.ValidationReportDetails
                    .Include(x => x.ValidationReportSummary)
                    .Include(x => x.ValidationReportSummary.SchoolYear)
                    .FirstOrDefault(x => x.ValidationReportSummaryId == validationReportId);

                if (reportDetails != null)
                {
                    reportDetails.CompletedWhen =
                        reportDetails.CompletedWhen?.ToLocalTime() ?? reportDetails.CompletedWhen;

                    reportDetails.ValidationReportSummary.RequestedWhen =
                        reportDetails.ValidationReportSummary.RequestedWhen.ToLocalTime();

                    LoggingService.LogDebugMessage(
                        $"Successfully retrieved Validation Report Details for report with ID number: {validationReportId.ToString()}");
                }

                return reportDetails;
            }
        }

        public IEnumerable<ValidationReportSummary> GetValidationReportSummaries(
            int edOrgId,
            ProgramAreaLookup programArea = null)
        {
            LoggingService.LogDebugMessage($"Retrieving a list of Validation Reports for the Ed Org ID number: {edOrgId.ToString()}");
            using (var portalDbContext = DbContextFactory.Create())
            {
                //todo: fix overall tracking of collections, this is bad
                var collections = SubmissionCycleService.GetSubmissionCycles(programArea)
                    .Select(x => x.CollectionId);

                var reportSummaryList = portalDbContext.ValidationReportSummaries
                    .Where(x => x.EdOrgId == edOrgId)
                    .Include(x => x.SchoolYear)
                    .Where(x => collections.Contains(x.Collection))
                    .ToList();

                // todo: fix this, use UTC across the board server-side
                reportSummaryList.ForEach(
                    x =>
                        {
                            x.CompletedWhen = x.CompletedWhen?.ToLocalTime();
                            x.RequestedWhen = x.RequestedWhen.ToLocalTime();
                        });

                LoggingService.LogDebugMessage(
                    $"Successfully retrieved a list of Validation Reports for the Ed Org ID number: {edOrgId.ToString()}");

                return reportSummaryList;
            }
        }

        public List<string> AutocompleteErrorFilter(ValidationErrorFilter filterSpecification)
        {
            using (var portalDbContext = DbContextFactory.Create())
            {
                // First limit to errors/warnings of one execution of the rules engine.
                var filteredErrorQuery = portalDbContext.ValidationErrorSummaries
                    .Include(x => x.ErrorEnrollmentDetails)
                    .Where(x => x.ValidationReportDetailsId == filterSpecification.reportDetailsId);

                IQueryable<string> autocompleteQuery;
                switch (filterSpecification.autocompleteColumn)
                {
                    case "studentid":
                        autocompleteQuery = filteredErrorQuery.Select(er2 => er2.StudentUniqueId);
                        break;
                    case "studentname":
                        autocompleteQuery = filteredErrorQuery.Select(er2 => er2.StudentFullName);
                        break;
                    case "school":
                        autocompleteQuery = filteredErrorQuery.SelectMany(
                            er2 => er2.ErrorEnrollmentDetails.Select(eed => eed.School));
                        break;
                    case "schoolid":
                        autocompleteQuery = filteredErrorQuery.SelectMany(
                            er2 => er2.ErrorEnrollmentDetails.Select(eed => eed.SchoolId));
                        break;
                    case "grade":
                        autocompleteQuery = filteredErrorQuery.SelectMany(
                            er2 => er2.ErrorEnrollmentDetails.Select(eed => eed.Grade));
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

                return autocompleteQuery
                    .Where(sg => sg.Contains(filterSpecification.autocompleteText))
                    .Distinct()
                    .Take(AutocompleteSuggestionCount).ToList();
            }
        }

        public FilteredValidationErrors GetFilteredValidationErrorTableData(ValidationErrorFilter filterSpecification)
        {
            using (var portalDbContext = DbContextFactory.Create())
            {
                // First limit to errors/warnings of one execution of the rules engine.
                var filteredErrorQuery = portalDbContext.ValidationErrorSummaries
                    .Include(ves0 => ves0.ErrorEnrollmentDetails).Where(
                        er0 => er0.ValidationReportDetailsId == filterSpecification.reportDetailsId);

                #region Filter on search texts

                var columnNames = filterSpecification.filterColumns ?? Array.Empty<string>();
                var searchTexts = filterSpecification.filterTexts ?? Array.Empty<string>();

                for (var colIndex = 0; colIndex < columnNames.Length; colIndex++)
                {
                    if (searchTexts.Length > colIndex && !string.IsNullOrWhiteSpace(searchTexts[colIndex]))
                    {
                        var normalizedSearchText = searchTexts[colIndex].Trim().ToLowerInvariant();
                        switch (columnNames[colIndex])
                        {
                            case "studentid":
                                filteredErrorQuery = filteredErrorQuery.Where(
                                    er2 => er2.StudentUniqueId.Contains(normalizedSearchText));
                                break;
                            case "studentname":
                                filteredErrorQuery = filteredErrorQuery.Where(
                                    er2 => er2.StudentFullName.Contains(normalizedSearchText));
                                break;
                            case "component":
                                filteredErrorQuery = filteredErrorQuery.Where(
                                    er2 => er2.Component.Contains(normalizedSearchText));
                                break;
                            case "school":
                                filteredErrorQuery = filteredErrorQuery.Where(
                                    er2 => er2.ErrorEnrollmentDetails.Select(eed => eed.School)
                                        .Contains(normalizedSearchText));
                                break;
                            case "schoolid":
                                filteredErrorQuery = filteredErrorQuery.Where(
                                    er2 => er2.ErrorEnrollmentDetails.Select(eed => eed.SchoolId)
                                        .Contains(normalizedSearchText));
                                break;
                            case "grade":
                                filteredErrorQuery = filteredErrorQuery.Where(
                                    er2 => er2.ErrorEnrollmentDetails.Select(eed => eed.Grade)
                                        .Contains(normalizedSearchText));
                                break;
                            case "rule":
                                filteredErrorQuery = filteredErrorQuery.Where(
                                    er2 => er2.ErrorCode.Contains(normalizedSearchText));
                                break;
                            case "errortext":
                                filteredErrorQuery = filteredErrorQuery.Where(
                                    er2 => er2.ErrorText.Contains(normalizedSearchText));
                                break;
                            case "errortype":
                                if (normalizedSearchText == "error")
                                {
                                    filteredErrorQuery = filteredErrorQuery.Where(
                                        er2 => er2.SeverityId == (int)(ErrorSeverity.Error));
                                }
                                else if (normalizedSearchText == "warning")
                                {
                                    filteredErrorQuery = filteredErrorQuery.Where(
                                        er2 => er2.SeverityId == (int)(ErrorSeverity.Warning));
                                }

                                break;
                        }
                    }
                }

                #endregion Filter on search texts

                #region Sort results

                var sortColumnNames = filterSpecification.sortColumns ?? new string[0];
                var sortDirections = filterSpecification.sortDirections ?? new string[0];
                var isSecondarySortColumn = false;
                for (int colIndex = 0; colIndex < sortColumnNames.Length; colIndex++)
                {
                    // Ascending is the default sort direction
                    var isDescending = (sortDirections.Length > colIndex)
                                       && (sortDirections[colIndex] ?? string.Empty).ToLowerInvariant().StartsWith("d");

                    switch (sortColumnNames[colIndex])
                    {
                        case "studentid":
                            if (!isDescending && !isSecondarySortColumn)
                            {
                                filteredErrorQuery = filteredErrorQuery.OrderBy(er2 => er2.StudentUniqueId);
                            }
                            else if (isDescending && !isSecondarySortColumn)
                            {
                                filteredErrorQuery = filteredErrorQuery.OrderByDescending(er2 => er2.StudentUniqueId);
                            }
                            else if (!isDescending && isSecondarySortColumn)
                            {
                                filteredErrorQuery =
                                    (filteredErrorQuery as IOrderedQueryable<ValidationErrorSummary>).ThenBy(
                                        er2 => er2.StudentUniqueId);
                            }
                            else if (isDescending && isSecondarySortColumn)
                            {
                                filteredErrorQuery =
                                    (filteredErrorQuery as IOrderedQueryable<ValidationErrorSummary>).ThenByDescending(
                                        er2 => er2.StudentUniqueId);
                            }
                            else
                            {
                                break;
                            }

                            isSecondarySortColumn = true;
                            break;
                        case "studentname":
                            if (!isDescending && !isSecondarySortColumn)
                            {
                                filteredErrorQuery = filteredErrorQuery.OrderBy(er2 => er2.StudentFullName);
                            }
                            else if (isDescending && !isSecondarySortColumn)
                            {
                                filteredErrorQuery = filteredErrorQuery.OrderByDescending(er2 => er2.StudentFullName);
                            }
                            else if (!isDescending && isSecondarySortColumn)
                            {
                                filteredErrorQuery =
                                    (filteredErrorQuery as IOrderedQueryable<ValidationErrorSummary>).ThenBy(
                                        er2 => er2.StudentFullName);
                            }
                            else if (isDescending && isSecondarySortColumn)
                            {
                                filteredErrorQuery =
                                    (filteredErrorQuery as IOrderedQueryable<ValidationErrorSummary>).ThenByDescending(
                                        er2 => er2.StudentFullName);
                            }
                            else
                            {
                                break;
                            }

                            isSecondarySortColumn = true;
                            break;
                        case "component":
                            if (!isDescending && !isSecondarySortColumn)
                            {
                                filteredErrorQuery = filteredErrorQuery.OrderBy(er2 => er2.Component);
                            }
                            else if (isDescending && !isSecondarySortColumn)
                            {
                                filteredErrorQuery = filteredErrorQuery.OrderByDescending(er2 => er2.Component);
                            }
                            else if (!isDescending && isSecondarySortColumn)
                            {
                                filteredErrorQuery =
                                    (filteredErrorQuery as IOrderedQueryable<ValidationErrorSummary>).ThenBy(
                                        er2 => er2.Component);
                            }
                            else if (isDescending && isSecondarySortColumn)
                            {
                                filteredErrorQuery =
                                    (filteredErrorQuery as IOrderedQueryable<ValidationErrorSummary>).ThenByDescending(
                                        er2 => er2.Component);
                            }
                            else
                            {
                                break;
                            }

                            isSecondarySortColumn = true;
                            break;
                        case "school":
                            if (!isDescending && !isSecondarySortColumn)
                            {
                                filteredErrorQuery = filteredErrorQuery.OrderBy(
                                    er2 => er2.ErrorEnrollmentDetails.OrderBy(eed => eed.School)
                                        .Select(eed => eed.School).FirstOrDefault());
                            }
                            else if (isDescending && !isSecondarySortColumn)
                            {
                                filteredErrorQuery = filteredErrorQuery.OrderByDescending(
                                    er2 => er2.ErrorEnrollmentDetails.OrderByDescending(eed => eed.School)
                                        .Select(eed => eed.School).FirstOrDefault());
                            }
                            else if (!isDescending && isSecondarySortColumn)
                            {
                                filteredErrorQuery =
                                    (filteredErrorQuery as IOrderedQueryable<ValidationErrorSummary>).ThenBy(
                                        er2 => er2.ErrorEnrollmentDetails.OrderBy(eed => eed.School)
                                            .Select(eed => eed.School).FirstOrDefault());
                            }
                            else if (isDescending && isSecondarySortColumn)
                            {
                                filteredErrorQuery =
                                    (filteredErrorQuery as IOrderedQueryable<ValidationErrorSummary>).ThenByDescending(
                                        er2 => er2.ErrorEnrollmentDetails.OrderByDescending(eed => eed.School)
                                            .Select(eed => eed.School).FirstOrDefault());
                            }
                            else
                            {
                                break;
                            }

                            isSecondarySortColumn = true;
                            break;
                        case "schoolid":
                            if (!isDescending && !isSecondarySortColumn)
                            {
                                filteredErrorQuery = filteredErrorQuery.OrderBy(
                                    er2 => er2.ErrorEnrollmentDetails.OrderBy(eed => eed.SchoolId)
                                        .Select(eed => eed.SchoolId).FirstOrDefault());
                            }
                            else if (isDescending && !isSecondarySortColumn)
                            {
                                filteredErrorQuery = filteredErrorQuery.OrderByDescending(
                                    er2 => er2.ErrorEnrollmentDetails.OrderByDescending(eed => eed.SchoolId)
                                        .Select(eed => eed.SchoolId).FirstOrDefault());
                            }
                            else if (!isDescending && isSecondarySortColumn)
                            {
                                filteredErrorQuery =
                                    (filteredErrorQuery as IOrderedQueryable<ValidationErrorSummary>).ThenBy(
                                        er2 => er2.ErrorEnrollmentDetails.OrderBy(eed => eed.SchoolId)
                                            .Select(eed => eed.SchoolId).FirstOrDefault());
                            }
                            else if (isDescending && isSecondarySortColumn)
                            {
                                filteredErrorQuery =
                                    (filteredErrorQuery as IOrderedQueryable<ValidationErrorSummary>).ThenByDescending(
                                        er2 => er2.ErrorEnrollmentDetails.OrderByDescending(eed => eed.SchoolId)
                                            .Select(eed => eed.SchoolId).FirstOrDefault());
                            }
                            else
                            {
                                break;
                            }

                            isSecondarySortColumn = true;
                            break;
                        case "dateenrolled":
                            if (!isDescending && !isSecondarySortColumn)
                            {
                                filteredErrorQuery = filteredErrorQuery.OrderBy(
                                    er2 => er2.ErrorEnrollmentDetails.OrderBy(eed => eed.DateEnrolled)
                                        .Select(eed => eed.DateEnrolled).FirstOrDefault());
                            }
                            else if (isDescending && !isSecondarySortColumn)
                            {
                                filteredErrorQuery = filteredErrorQuery.OrderByDescending(
                                    er2 => er2.ErrorEnrollmentDetails.OrderByDescending(eed => eed.DateEnrolled)
                                        .Select(eed => eed.DateEnrolled).FirstOrDefault());
                            }
                            else if (!isDescending && isSecondarySortColumn)
                            {
                                filteredErrorQuery =
                                    (filteredErrorQuery as IOrderedQueryable<ValidationErrorSummary>).ThenBy(
                                        er2 => er2.ErrorEnrollmentDetails.OrderBy(eed => eed.DateEnrolled)
                                            .Select(eed => eed.DateEnrolled).FirstOrDefault());
                            }
                            else if (isDescending && isSecondarySortColumn)
                            {
                                filteredErrorQuery =
                                    (filteredErrorQuery as IOrderedQueryable<ValidationErrorSummary>).ThenByDescending(
                                        er2 => er2.ErrorEnrollmentDetails.OrderByDescending(eed => eed.DateEnrolled)
                                            .Select(eed => eed.DateEnrolled).FirstOrDefault());
                            }
                            else
                            {
                                break;
                            }

                            isSecondarySortColumn = true;
                            break;
                        case "datewithdrawn":
                            if (!isDescending && !isSecondarySortColumn)
                            {
                                filteredErrorQuery = filteredErrorQuery.OrderBy(
                                    er2 => er2.ErrorEnrollmentDetails.OrderBy(eed => eed.DateWithdrawn)
                                        .Select(eed => eed.DateWithdrawn).FirstOrDefault());
                            }
                            else if (isDescending && !isSecondarySortColumn)
                            {
                                filteredErrorQuery = filteredErrorQuery.OrderByDescending(
                                    er2 => er2.ErrorEnrollmentDetails.OrderByDescending(eed => eed.DateWithdrawn)
                                        .Select(eed => eed.DateWithdrawn).FirstOrDefault());
                            }
                            else if (!isDescending && isSecondarySortColumn)
                            {
                                filteredErrorQuery =
                                    (filteredErrorQuery as IOrderedQueryable<ValidationErrorSummary>).ThenBy(
                                        er2 => er2.ErrorEnrollmentDetails.OrderBy(eed => eed.DateWithdrawn)
                                            .Select(eed => eed.DateWithdrawn).FirstOrDefault());
                            }
                            else if (isDescending && isSecondarySortColumn)
                            {
                                filteredErrorQuery =
                                    (filteredErrorQuery as IOrderedQueryable<ValidationErrorSummary>).ThenByDescending(
                                        er2 => er2.ErrorEnrollmentDetails.OrderByDescending(eed => eed.DateWithdrawn)
                                            .Select(eed => eed.DateWithdrawn).FirstOrDefault());
                            }
                            else
                            {
                                break;
                            }

                            isSecondarySortColumn = true;
                            break;
                        case "grade":
                            if (!isDescending && !isSecondarySortColumn)
                            {
                                filteredErrorQuery = filteredErrorQuery.OrderBy(
                                    er2 => er2.ErrorEnrollmentDetails.OrderBy(eed => eed.Grade).Select(eed => eed.Grade)
                                        .FirstOrDefault());
                            }
                            else if (isDescending && !isSecondarySortColumn)
                            {
                                filteredErrorQuery = filteredErrorQuery.OrderByDescending(
                                    er2 => er2.ErrorEnrollmentDetails.OrderByDescending(eed => eed.Grade)
                                        .Select(eed => eed.Grade).FirstOrDefault());
                            }
                            else if (!isDescending && isSecondarySortColumn)
                            {
                                filteredErrorQuery =
                                    (filteredErrorQuery as IOrderedQueryable<ValidationErrorSummary>).ThenBy(
                                        er2 => er2.ErrorEnrollmentDetails.OrderBy(eed => eed.Grade)
                                            .Select(eed => eed.Grade).FirstOrDefault());
                            }
                            else if (isDescending && isSecondarySortColumn)
                            {
                                filteredErrorQuery =
                                    (filteredErrorQuery as IOrderedQueryable<ValidationErrorSummary>).ThenByDescending(
                                        er2 => er2.ErrorEnrollmentDetails.OrderByDescending(eed => eed.Grade)
                                            .Select(eed => eed.Grade).FirstOrDefault());
                            }
                            else
                            {
                                break;
                            }

                            isSecondarySortColumn = true;
                            break;
                        case "rule":
                            if (!isDescending && !isSecondarySortColumn)
                            {
                                filteredErrorQuery = filteredErrorQuery.OrderBy(er2 => er2.ErrorCode);
                            }
                            else if (isDescending && !isSecondarySortColumn)
                            {
                                filteredErrorQuery = filteredErrorQuery.OrderByDescending(er2 => er2.ErrorCode);
                            }
                            else if (!isDescending && isSecondarySortColumn)
                            {
                                filteredErrorQuery =
                                    (filteredErrorQuery as IOrderedQueryable<ValidationErrorSummary>).ThenBy(
                                        er2 => er2.ErrorCode);
                            }
                            else if (isDescending && isSecondarySortColumn)
                            {
                                filteredErrorQuery =
                                    (filteredErrorQuery as IOrderedQueryable<ValidationErrorSummary>).ThenByDescending(
                                        er2 => er2.ErrorCode);
                            }
                            else
                            {
                                break;
                            }

                            isSecondarySortColumn = true;
                            break;
                        case "errortext":
                            if (!isDescending && !isSecondarySortColumn)
                            {
                                filteredErrorQuery = filteredErrorQuery.OrderBy(er2 => er2.ErrorText);
                            }
                            else if (isDescending && !isSecondarySortColumn)
                            {
                                filteredErrorQuery = filteredErrorQuery.OrderByDescending(er2 => er2.ErrorText);
                            }
                            else if (!isDescending && isSecondarySortColumn)
                            {
                                filteredErrorQuery =
                                    (filteredErrorQuery as IOrderedQueryable<ValidationErrorSummary>).ThenBy(
                                        er2 => er2.ErrorText);
                            }
                            else if (isDescending && isSecondarySortColumn)
                            {
                                filteredErrorQuery =
                                    (filteredErrorQuery as IOrderedQueryable<ValidationErrorSummary>).ThenByDescending(
                                        er2 => er2.ErrorText);
                            }
                            else
                            {
                                break;
                            }

                            isSecondarySortColumn = true;
                            break;
                        case "errortype":
                            if (!isDescending && !isSecondarySortColumn)
                            {
                                filteredErrorQuery = filteredErrorQuery.OrderBy(er2 => er2.SeverityId);
                            }
                            else if (isDescending && !isSecondarySortColumn)
                            {
                                filteredErrorQuery = filteredErrorQuery.OrderByDescending(er2 => er2.SeverityId);
                            }
                            else if (!isDescending && isSecondarySortColumn)
                            {
                                filteredErrorQuery =
                                    (filteredErrorQuery as IOrderedQueryable<ValidationErrorSummary>).ThenBy(
                                        er2 => er2.SeverityId);
                            }
                            else if (isDescending && isSecondarySortColumn)
                            {
                                filteredErrorQuery =
                                    (filteredErrorQuery as IOrderedQueryable<ValidationErrorSummary>).ThenByDescending(
                                        er2 => er2.SeverityId);
                            }
                            else
                            {
                                break;
                            }

                            isSecondarySortColumn = true;
                            break;
                        default:
                            // Do nothing
                            break;
                    }
                }

                // IMPORTANT! If still unsorted - add default sorting, necessary for LINQ-to-Entities paging to function correctly.
                // If already sorted, still needs to be secondary-sorted by a UNIQUE field, otherwise rows will be repeated on subsequent pages.
                filteredErrorQuery = !isSecondarySortColumn 
                    ? filteredErrorQuery.OrderBy(x => x.Id) 
                    : ((IOrderedQueryable<ValidationErrorSummary>)filteredErrorQuery).ThenBy(x => x.Id);

                #endregion Sort results

                #region Count results

                var result = new FilteredValidationErrors
                             {
                                 TotalFilteredErrorCount = filteredErrorQuery.Count()
                             };

                #endregion Count results

                #region Page results & Sanity checks

                #region Page Size

                if (filterSpecification.pageSize <= 0 || filterSpecification.pageSize > MaxPageSize)
                    filterSpecification.pageSize = MaxPageSize;
                result.PageSize = filterSpecification.pageSize;

                #endregion Page Size

                #region Record Offset

                // Record offset can be no farther than the last page, and no lower than 0.
                result.RecordOffset = Math.Max(filterSpecification.pageStartingOffset, 0);
                // maxRecordOffset is the start of the last page.
                var maxRecordOffset = ((result.TotalFilteredErrorCount - 1) / filterSpecification.pageSize)
                                      * filterSpecification.pageSize;
                result.RecordOffset = Math.Max(filterSpecification.pageStartingOffset, 0);
                result.RecordOffset = Math.Min(maxRecordOffset, result.RecordOffset);

                #endregion Record Offset

                #region Page Number

                var recordsRemaining = result.TotalFilteredErrorCount - result.RecordOffset;
                result.PageNumber = (result.RecordOffset / result.PageSize) + 1;

                #endregion Page Number

                result.TotalPagesCount = (result.TotalFilteredErrorCount / result.PageSize)
                                         + ((result.TotalFilteredErrorCount % result.PageSize == 0) ? 0 : 1);
                var maxToReturn = Math.Max(Math.Min(recordsRemaining, filterSpecification.pageSize), 0);
                result.FilteredErrorSummariesPage =
                    filteredErrorQuery.Skip(result.RecordOffset).Take(maxToReturn).ToList();

                #endregion Page results

                return result;
            }
        }

        public IList<ValidationErrorSummary> GetValidationErrors(int reportDetailsId)
        {
            using (var portalDbContext = DbContextFactory.Create())
            {
                return portalDbContext.ValidationReportDetails
                    .Include(x => x.ErrorSummaries)
                    .Include(x => x.ErrorSummaries.Select(y => y.Severity))
                    .Include(x => x.ErrorSummaries.Select(y => y.ErrorEnrollmentDetails))
                    .First(x => x.ValidationReportSummaryId == reportDetailsId)
                    .ErrorSummaries
                    .ToList();
            }
        }
    }
}
