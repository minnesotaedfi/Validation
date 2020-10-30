using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using DataTables.AspNet.Core;
using DataTables.AspNet.Mvc5;

using ValidationWeb.Database;
using ValidationWeb.Models;
using ValidationWeb.Services.Interfaces;

namespace ValidationWeb.Services.Implementations
{
    using System.Data.Entity.Infrastructure;

    public class EdFiApiLogService : IEdFiApiLogService
    {
        public const string ApiName = "EdFi.Ods.WebApi";

        public readonly ILoggingService LoggingService;
        
        public readonly IDbContextFactory<EdFiLogDbContext> DbContextFactory;
        
        public EdFiApiLogService(
            ILoggingService loggingService,
            IDbContextFactory<EdFiLogDbContext> dbContextFactory)
        {
            LoggingService = loggingService;
            DbContextFactory = dbContextFactory;
        }

        public JsonResult GetIdentityIssues(IDataTablesRequest request, string districtId, string year)
        {
            using (var dbContext = DbContextFactory.Create())
            {
                var urlFragment = $"/{ApiName}/identity/";
                var logs = dbContext.Logs
                    .Where(x => 
                        x.Url.Contains(urlFragment) ||
                        (!string.IsNullOrEmpty(x.ResponseBody) && x.ResponseBody.ToLower().Contains("Validation of 'Student' failed".ToLower())))
                    .Where(x => x.District == districtId && x.Year == year);

                return SortAndFilterApiReportData(logs, request);
            }
        }

        public JsonResult GetApiErrors(IDataTablesRequest request, string districtId, string year)
        {
            using (var dbContext = DbContextFactory.Create())
            {
                var apiFragment = $"/{ApiName}/data/v3/";
                var oauthFragment = $"/{ApiName}/oauth/";

                var logs = dbContext.Logs
                    .Where(x => x.Url.Contains(apiFragment) || x.Url.Contains(oauthFragment))
                    .Where(x => x.District == districtId && x.Year == year);

                return SortAndFilterApiReportData(logs, request);
            }
        }

        protected JsonResult SortAndFilterApiReportData(IQueryable<Log> results, IDataTablesRequest request)
        {
            var sortedResults = results.AsEnumerable(); 

            var sortColumn = request.Columns.FirstOrDefault(x => x.Sort != null);
            if (sortColumn != null)
            {
                Func<Log, string> orderingFunctionString = null;
                Func<Log, DateTime?> orderingFunctionNullableDateTime = null;
                Func<Log, int> orderingFunctionInt = null;

                switch (sortColumn.Field)
                {
                    case "id":
                        {
                            orderingFunctionInt = x => x.Id;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? sortedResults.OrderBy(orderingFunctionInt)
                                                : sortedResults.OrderByDescending(orderingFunctionInt);
                            break;
                        }
                    case "date":
                        {
                            orderingFunctionNullableDateTime = x => x.Date;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? sortedResults.OrderBy(orderingFunctionNullableDateTime)
                                                : sortedResults.OrderByDescending(orderingFunctionNullableDateTime);
                            break;
                        }
                    case "thread":
                        {
                            orderingFunctionString = x => x.Thread;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? sortedResults.OrderBy(orderingFunctionString)
                                                : sortedResults.OrderByDescending(orderingFunctionString);
                            break;
                        }
                    case "level":
                        {
                            orderingFunctionString = x => x.Level;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? sortedResults.OrderBy(orderingFunctionString)
                                                : sortedResults.OrderByDescending(orderingFunctionString);
                            break;
                        }
                    case "logger":
                        {
                            orderingFunctionString = x => x.Logger;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? sortedResults.OrderBy(orderingFunctionString)
                                                : sortedResults.OrderByDescending(orderingFunctionString);
                            break;
                        }
                    case "year":
                        {
                            orderingFunctionString = x => x.Year;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? sortedResults.OrderBy(orderingFunctionString)
                                                : sortedResults.OrderByDescending(orderingFunctionString);
                            break;
                        }

                    case "district":
                        {
                            orderingFunctionString = x => x.District;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? sortedResults.OrderBy(orderingFunctionString)
                                                : sortedResults.OrderByDescending(orderingFunctionString);
                            break;
                        }
                    case "method":
                        {
                            orderingFunctionString = x => x.Method;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? sortedResults.OrderBy(orderingFunctionString)
                                                : sortedResults.OrderByDescending(orderingFunctionString);
                            break;
                        }
                    case "url":
                        {
                            orderingFunctionString = x => x.Url;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? sortedResults.OrderBy(orderingFunctionString)
                                                : sortedResults.OrderByDescending(orderingFunctionString);
                            break;
                        }
                    case "responseCode":
                        {
                            orderingFunctionString = x => $"{x.ResponseCode} {x.ResponsePhrase}";
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? sortedResults.OrderBy(orderingFunctionString)
                                                : sortedResults.OrderByDescending(orderingFunctionString);
                            break;
                        }
                    case "responseBody":
                        {
                            orderingFunctionString = x => x.ResponseBody;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? sortedResults.OrderBy(orderingFunctionString)
                                                : sortedResults.OrderByDescending(orderingFunctionString);
                            break;
                        }
                    case "exception":
                        {
                            orderingFunctionString = x => x.Exception;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? sortedResults.OrderBy(orderingFunctionString)
                                                : sortedResults.OrderByDescending(orderingFunctionString);
                            break;
                        }
                    case "requestBody":
                        {
                            orderingFunctionString = x => x.RequestBody;
                            sortedResults = sortColumn.Sort.Direction == SortDirection.Ascending
                                                ? sortedResults.OrderBy(orderingFunctionString)
                                                : sortedResults.OrderByDescending(orderingFunctionString);
                            break;
                        }
                }
            }

            var pagedResults = sortedResults.Skip(request.Start).Take(request.Length);

            var formattedResults = pagedResults.Select(x =>
                new Log
                {
                    Date = x.Date,
                    District = x.District,
                    Year = x.Year,
                    Method = x.Method,
                    Url = new Uri(x.Url).PathAndQuery,
                    ResponseCode = x.ResponseCode,
                    ResponsePhrase = x.ResponsePhrase,
                    ResponseBody = x.ResponseBody,
                    RequestBody = x.RequestBody.Replace(",\"", ", \"")
                }).ToList();

            var response = DataTablesResponse.Create(request, results.Count(), results.Count(), formattedResults);
            var jsonResult = new DataTablesJsonResult(response, JsonRequestBehavior.AllowGet);
            return jsonResult;
        }
    }
}