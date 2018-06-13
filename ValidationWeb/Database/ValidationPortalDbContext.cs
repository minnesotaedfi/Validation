using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ValidationWeb
{
    public partial class ValidationPortalDbContext : DbContext
    {
        // Use of "name=" ensures that a new database won't be created by default.
        public ValidationPortalDbContext()
            : base("name=ValidationPortalDbContext")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ValidationPortalDbContext, ValidationPortalDbMigrationConfiguration>(true));
            Configuration.LazyLoadingEnabled = false;
        }

        static ValidationPortalDbContext()
        {
            // Fixes a known bug in which EntityFramework.SqlServer.dll is not copied into consumer even when CopyLocal is True.
            var includeSqlServerDLLInConsumer = System.Data.Entity.SqlServer.SqlProviderServices.Instance;
        }
        //public virtual DbSet<Client> Clients { get; set; }
        //public virtual DbSet<Token> Tokens { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}

