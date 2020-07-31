using System.Data.Entity;

using Validation.DataModels;

using ValidationWeb.Models;

namespace ValidationWeb.Database
{
    public class ValidationPortalDbContext : DbContext
    {
        static ValidationPortalDbContext()
        {
            // Fixes a known bug in which EntityFramework.SqlServer.dll is not copied into consumer even when CopyLocal is True.
            var includeSqlServerDLLInConsumer = System.Data.Entity.SqlServer.SqlProviderServices.Instance;
        }

        // Use of "name=" ensures that a new database won't be created by default.
        public ValidationPortalDbContext()
            : base("name=ValidationPortalDbContext")
        {
            System.Data.Entity.Database.SetInitializer(
                new MigrateDatabaseToLatestVersion<ValidationPortalDbContext, ValidationPortalDbMigrationConfiguration>(true));

            Configuration.LazyLoadingEnabled = false;
          
            #if DEBUG
            Database.Log = x => System.Diagnostics.Debug.Write(x);
            #endif
        }

        public virtual DbSet<Announcement> Announcements { get; set; }

        public virtual DbSet<AppUserSession> AppUserSessions { get; set; }

        public virtual DbSet<EdOrg> EdOrgs { get; set; }

        public virtual DbSet<EdOrgTypeLookup> EdOrgTypeLookup { get; set; }

        public virtual DbSet<ErrorSeverityLookup> ErrorSeverityLookup { get; set; }

        public virtual DbSet<RecordsRequest> RecordsRequests { get; set; }

        public virtual DbSet<SchoolYear> SchoolYears { get; set; }

        public virtual DbSet<SubmissionCycle> SubmissionCycles { get; set; }

        public virtual DbSet<ValidationErrorSummary> ValidationErrorSummaries { get; set; }

        public virtual DbSet<ValidationReportDetails> ValidationReportDetails { get; set; }

        public virtual DbSet<ValidationReportSummary> ValidationReportSummaries { get; set; }

        public virtual DbSet<ValidationRulesView> ValidationRulesViews { get; set; }

        public virtual DbSet<ValidationRulesField> ValidationRulesFields { get; set; }
        
        public virtual DbSet<DynamicReportDefinition> DynamicReportDefinitions { get; set; }

        public virtual DbSet<DynamicReportField> DynamicReportFields { get; set; }

        public virtual DbSet<ProgramArea> ProgramAreas { get; set; }

        protected new virtual void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<ValidationRulesView>()
            //    .HasRequired(x => x.SchoolYear)
            //    .WithRequiredDependent()
            //    .WillCascadeOnDelete(false);
        }
    }
}

