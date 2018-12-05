using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;

namespace ValidationWeb
{
    internal sealed class ValidationPortalDbMigrationConfiguration : DbMigrationsConfiguration<ValidationPortalDbContext>
    {
        public ValidationPortalDbMigrationConfiguration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            ContextKey = "ValidationWeb.ValidationPortalDbContext";
        }

        public static EdOrgTypeLookup[] EdOrgTypeLookups = new EdOrgTypeLookup[]
        {
            new EdOrgTypeLookup { Id = (int)EdOrgType.School, CodeValue = "School", Description = "School" },
            new EdOrgTypeLookup { Id = (int)EdOrgType.District, CodeValue = "District", Description = "District" },
            new EdOrgTypeLookup { Id = (int)EdOrgType.Region, CodeValue = "Region", Description = "Region" },
            new EdOrgTypeLookup { Id = (int)EdOrgType.State, CodeValue = "State", Description = "State" }
        };

        public static ErrorSeverityLookup[] ErrorSeverityLookups = new ErrorSeverityLookup[]
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
            if (context.SchoolYears != null && context.SchoolYears.Count() <= 0)
            {
                context.SchoolYears.AddOrUpdate(new SchoolYear("2018", "2019"));
                context.SchoolYears.AddOrUpdate(new SchoolYear("2019", "2020"));
                context.SchoolYears.AddOrUpdate(new SchoolYear("2020", "2021"));
            }
        }
    }
}

