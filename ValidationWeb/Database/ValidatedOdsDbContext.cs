using System.Data.Entity;
using ValidationWeb.Services.Implementations;

namespace ValidationWeb.Database
{
    public class ValidatedOdsDbContext : DbContext
    {
        static ValidatedOdsDbContext()
        {
            // Fixes a known bug in which EntityFramework.SqlServer.dll is not copied into consumer even when CopyLocal is True.
            var includeSqlServerDLLInConsumer = System.Data.Entity.SqlServer.SqlProviderServices.Instance;
        }

        // Use of "name=" ensures that a new database won't be created by default.
        public ValidatedOdsDbContext(string fourDigitYear) : base(new OdsConfigurationValues().GetRawOdsConnectionString(fourDigitYear)) { }


        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);
        //}
    }
}

