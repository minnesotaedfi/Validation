using System.Data.Entity.Migrations;

namespace ValidationWeb.Database
{
    internal sealed class RawOdsDbMigrationConfiguration : DbMigrationsConfiguration<RawOdsDbContext>
    {
        public RawOdsDbMigrationConfiguration()
        {
            AutomaticMigrationsEnabled = false;
            AutomaticMigrationDataLossAllowed = false;

            // warning! don't refactor or "fix" this. it doesn't have to be our namespace
            ContextKey = "ValidationWeb.Database.RawOdsDbContext";
        }
    }
}

