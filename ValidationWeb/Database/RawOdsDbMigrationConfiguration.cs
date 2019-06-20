using System.Data.Entity.Migrations;

namespace ValidationWeb.Database
{
    internal sealed class RawOdsDbMigrationConfiguration : DbMigrationsConfiguration<RawOdsDbContext>
    {
        public RawOdsDbMigrationConfiguration()
        {
            AutomaticMigrationsEnabled = false;
            AutomaticMigrationDataLossAllowed = false;
            ContextKey = "ValidationWeb.Database.RawOdsDbContext";
        }
    }
}

