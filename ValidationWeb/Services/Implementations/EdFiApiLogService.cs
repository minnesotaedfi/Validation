using System;
using System.Collections.Generic;
using System.Linq;

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

        public IEnumerable<Log> GetIdentityIssues()
        {
            using (var dbContext = DbContextFactory.Create())
            {
                return dbContext.Logs
                    .ToList()
                    .Where(x => 
                        new Uri(x.Url).PathAndQuery.StartsWith($"/{ApiName}/identity/", StringComparison.OrdinalIgnoreCase));
            }
        }

        public IEnumerable<Log> GetApiErrors()
        {
            using (var dbContext = DbContextFactory.Create())
            {
                return dbContext.Logs
                    .ToList()
                    .Where(x => 
                        new Uri(x.Url).PathAndQuery.StartsWith($"/{ApiName}/data/v3/", StringComparison.OrdinalIgnoreCase) ||
                        new Uri(x.Url).PathAndQuery.StartsWith($"/{ApiName}/oauth/", StringComparison.OrdinalIgnoreCase));
            }
        }
    }
}