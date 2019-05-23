using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using ValidationWeb.Database;
using ValidationWeb.Services;

namespace ValidationWeb
{
    using ValidationWeb.Services.Implementations;

    internal sealed class ValidationPortalDbMigrationConfiguration : DbMigrationsConfiguration<ValidationPortalDbContext>
    {
        private readonly IConfigurationValues _config = new AppSettingsFileConfigurationValues();

        public ValidationPortalDbMigrationConfiguration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            ContextKey = "ValidationWeb.ValidationPortalDbContext";
        }

        public static EdOrgTypeLookup[] EdOrgTypeLookups = new[]
        {
            new EdOrgTypeLookup { Id = (int)EdOrgType.School, CodeValue = "School", Description = "School" },
            new EdOrgTypeLookup { Id = (int)EdOrgType.District, CodeValue = "District", Description = "District" },
            new EdOrgTypeLookup { Id = (int)EdOrgType.Region, CodeValue = "Region", Description = "Region" },
            new EdOrgTypeLookup { Id = (int)EdOrgType.State, CodeValue = "State", Description = "State" }
        };

        public static ErrorSeverityLookup[] ErrorSeverityLookups = new ErrorSeverityLookup[]
        {
            new ErrorSeverityLookup { Id = (int)ErrorSeverity.Error, CodeValue = "Error", Description = "Error" },
            new ErrorSeverityLookup { Id = (int)ErrorSeverity.Warning, CodeValue = "Warning", Description = "Warning" }
        };

        public static RecordsRequestTypeLookup[] RecordsRequestTypeLookups = new RecordsRequestTypeLookup[]
        {
            new RecordsRequestTypeLookup { Id = (int)RecordsRequestType.Assessment, CodeValue = "Assessment", Description = "Assessment"},
            new RecordsRequestTypeLookup { Id = (int)RecordsRequestType.Cumulative, CodeValue = "Cumulative", Description = "Cumulative"},
            new RecordsRequestTypeLookup { Id = (int)RecordsRequestType.Discipline, CodeValue = "Discipline", Description = "Discipline"},
            new RecordsRequestTypeLookup { Id = (int)RecordsRequestType.IEP, CodeValue = "IEP", Description = "IEP"},
            new RecordsRequestTypeLookup { Id = (int)RecordsRequestType.Evaluation, CodeValue = "Evaluation", Description = "Evaluation"},
            new RecordsRequestTypeLookup { Id = (int)RecordsRequestType.Immunizations, CodeValue = "Immunizations", Description = "Immunizations"}
        };

        /// <summary>
        /// Called after migration.
        /// </summary>
        /// <param name="context">Represents the ValidationPortalDb database connection.</param>
        protected override void Seed(ValidationPortalDbContext context)
        {
            context.EdOrgTypeLookup.AddOrUpdate(EdOrgTypeLookups);
            context.ErrorSeverityLookup.AddOrUpdate(ErrorSeverityLookups);
            
            // why would it be < 0 ? todo 
            if (_config.SeedSchoolYears != null && !context.SchoolYears.Any()) 
            {
                foreach (var schoolYearToSeedIfMissing in _config.SeedSchoolYears)
                {
                    if (context.SchoolYears.FirstOrDefault(sy =>
                        sy.StartYear != schoolYearToSeedIfMissing.StartYear && 
                        sy.EndYear != schoolYearToSeedIfMissing.EndYear) == null)
                    {
                        context.SchoolYears.AddOrUpdate(schoolYearToSeedIfMissing);
                    }
                }
            }
#if DEBUG
            if (context.Announcements != null && !context.Announcements.Any())
            {
                context.Announcements.AddOrUpdate(
                    new Announcement
                    {
                        Priority = 0,
                        Message = "This message is stored in the application database.",
                        ContactInfo = "info@education.mn.gov",
                        IsEmergency = false,
                        LinkUrl = "http://education.mn.gov/",
                        Expiration = new DateTime(2019, 4, 1)
                    },
                    new Announcement
                    {
                        Priority = 0,
                        Message = "Another message.",
                        ContactInfo = "info@education.mn.gov",
                        IsEmergency = false,
                        LinkUrl = "http://education.mn.gov/",
                        Expiration = new DateTime(2019, 4, 1)
                    });
            }
#endif
        }
    }
}

