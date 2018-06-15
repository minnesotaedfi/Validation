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
            ContextKey = "ValidationWeb.ValidationPortalDbContext";
        }

        public static EdOrgTypeLookup[] EdOrgTypeLookups = new EdOrgTypeLookup[]
        {
            new EdOrgTypeLookup { Id = 0, CodeValue = "School", Description = "School" },
            new EdOrgTypeLookup { Id = 1, CodeValue = "District", Description = "District" },
            new EdOrgTypeLookup { Id = 2, CodeValue = "Region", Description = "Region" },
            new EdOrgTypeLookup { Id = 3, CodeValue = "State", Description = "State" }
        };

        public static ErrorSeverityLookup[] ErrorSeverityLookups = new ErrorSeverityLookup[]
        {
            new ErrorSeverityLookup { Id = 0, CodeValue = "Error", Description = "Error" },
            new ErrorSeverityLookup { Id = 1, CodeValue = "Warning", Description = "Warning" }
        };

        /// <summary>
        /// Called after migration.
        /// </summary>
        /// <param name="context">Represents the ValidationPortalDb database connection.</param>
        protected override void Seed(ValidationPortalDbContext context)
        {
            context.EdOrgTypeLookup.AddOrUpdate(EdOrgTypeLookups);
            context.ErrorSeverityLookup.AddOrUpdate(ErrorSeverityLookups);
        }
    }
}

