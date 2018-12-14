using System.Data.Entity;

namespace ValidationWeb
{
    public interface IValidationPortalDbContext
    {
        DbSet<Announcement> Announcements { get; set; }
        DbSet<AppUserSession> AppUserSessions { get; set; }
        DbSet<DismissedAnnouncement> DismissedAnnouncements { get; set; }
        DbSet<EdOrg> EdOrgs { get; set; }
        DbSet<EdOrgTypeLookup> EdOrgTypeLookup { get; set; }
        DbSet<ErrorSeverityLookup> ErrorSeverityLookup { get; set; }
        DbSet<SchoolYear> SchoolYears { get; set; }
        DbSet<SubmissionCycle> SubmissionCycles { get; set; }
        DbSet<ValidationErrorSummary> ValidationErrorSummaries { get; set; }
        DbSet<ValidationReportDetails> ValidationReportDetails { get; set; }
        DbSet<ValidationReportSummary> ValidationReportSummaries { get; set; }

        int SaveChanges();
    }
}