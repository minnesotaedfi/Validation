using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ValidationWeb
{
    public partial class ValidationPortalDbContext : DbContext, IValidationPortalDbContext
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
        public virtual DbSet<Announcement> Announcements { get; set; }
        public virtual DbSet<AppUserSession> AppUserSessions { get; set; }
        public virtual DbSet<DismissedAnnouncement> DismissedAnnouncements { get; set; }
        public virtual DbSet<EdOrg> EdOrgs { get; set; }
        public virtual DbSet<EdOrgTypeLookup> EdOrgTypeLookup { get; set; }
        public virtual DbSet<ErrorSeverityLookup> ErrorSeverityLookup { get; set; }
        public virtual DbSet<SchoolYear> SchoolYears { get; set; }
        public virtual DbSet<SubmissionCycle> SubmissionCycles { get; set; }
        public virtual DbSet<ValidationErrorSummary> ValidationErrorSummaries { get; set; }
        public virtual DbSet<ValidationReportDetails> ValidationReportDetails { get; set; }
        public virtual DbSet<ValidationReportSummary> ValidationReportSummaries { get; set; }

        public override int SaveChanges()
        {
            return ((DbContext)this).SaveChanges();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Announcement>()
                .HasMany<EdOrg>(ann => ann.LimitToEdOrgs)
                .WithMany(eo => eo.Announcements)
                .Map(cs =>
                {
                    cs.MapLeftKey("AnnouncementId");
                    cs.MapRightKey("EdOrgId");
                    cs.ToTable("validation.AnnouncementEdOrg");
                });

            modelBuilder.Entity<AppUserSession>()
                .HasMany<DismissedAnnouncement>(aus => aus.DismissedAnnouncements)
                .WithRequired(dann => dann.AppUserSession)
                .HasForeignKey(aus => aus.AppUserSessionId)
                .WillCascadeOnDelete();
        }
    }
}

