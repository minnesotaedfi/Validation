using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using ValidationWeb.Services;

namespace ValidationWeb
{
    public partial class RawOdsDbContext : DbContext
    {
        // Use of connection string ensures that a new database won't be created by default.
        public RawOdsDbContext(string fourDigitYear) : base(new OdsConfigurationValues().GetRawOdsConnectionString(fourDigitYear)) {}

        static RawOdsDbContext()
        {
            // Fixes a known bug in which EntityFramework.SqlServer.dll is not copied into consumer even when CopyLocal is True.
            var includeSqlServerDLLInConsumer = System.Data.Entity.SqlServer.SqlProviderServices.Instance;
        }
        // public virtual DbSet<Announcement> Announcements { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}

