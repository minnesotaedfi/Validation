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

        protected override void Seed(ValidationPortalDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
        }
    }
}

