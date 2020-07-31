using System;
using System.Data.Entity.Migrations;
using System.Linq;

using Validation.DataModels;

using ValidationWeb.Models;
using ValidationWeb.Services.Implementations;
using ValidationWeb.Services.Interfaces;

using EdOrgType = ValidationWeb.Models.EdOrgType;

namespace ValidationWeb.Database
{
    internal sealed class ValidationPortalDbMigrationConfiguration : DbMigrationsConfiguration<ValidationPortalDbContext>
    {
        private readonly IConfigurationValues _config = new AppSettingsFileConfigurationValues();

        public ValidationPortalDbMigrationConfiguration()
        {
            AutomaticMigrationsEnabled = false;
            AutomaticMigrationDataLossAllowed = false; 

            // don't "fix" or change this! or refactor it or anything!
            // else, EF will try to auto-migrate all VP tables.
            // pre-3.1x validation portal used automatic migrations and any existing 
            // migration history will have this context key in it. it doesn't have to
            // be correct, just unique. 
            ContextKey = "ValidationWeb.ValidationPortalDbContext"; 
        }

        public static EdOrgTypeLookup[] EdOrgTypeLookups = 
        {
            new EdOrgTypeLookup { Id = (int)EdOrgType.School, CodeValue = "School", Description = "School" },
            new EdOrgTypeLookup { Id = (int)EdOrgType.District, CodeValue = "District", Description = "District" },
            new EdOrgTypeLookup { Id = (int)EdOrgType.Region, CodeValue = "Region", Description = "Region" },
            new EdOrgTypeLookup { Id = (int)EdOrgType.State, CodeValue = "State", Description = "State" }
        };

        public static ErrorSeverityLookup[] ErrorSeverityLookups = 
        {
            new ErrorSeverityLookup { Id = (int)ErrorSeverity.Error, CodeValue = "Error", Description = "Error" },
            new ErrorSeverityLookup { Id = (int)ErrorSeverity.Warning, CodeValue = "Warning", Description = "Warning" }
        };
        
        /// <summary>
        /// Called after migration.
        /// </summary>
        /// <param name="context">Represents the ValidationPortalDb database connection.</param>
        protected override void Seed(ValidationPortalDbContext context)
        {
            context.EdOrgTypeLookup.AddOrUpdate(EdOrgTypeLookups);
            context.ErrorSeverityLookup.AddOrUpdate(ErrorSeverityLookups);

            if (_config.SeedSchoolYears != null && !context.SchoolYears.Any()) 
            {
                foreach (var schoolYearToSeedIfMissing in _config.SeedSchoolYears)
                {
                    if (context.SchoolYears.FirstOrDefault(sy =>
                        sy.StartYear != schoolYearToSeedIfMissing.StartYear && 
                        sy.EndYear != schoolYearToSeedIfMissing.EndYear) == null)
                    {
                        context.SchoolYears.AddOrUpdate(schoolYearToSeedIfMissing);
                    }
                }
            }
        }
    }
}

