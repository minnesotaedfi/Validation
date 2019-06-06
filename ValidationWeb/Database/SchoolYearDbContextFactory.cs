using ValidationWeb.Services.Interfaces;

namespace ValidationWeb.Database
{
    public class SchoolYearDbContextFactory : ISchoolYearDbContextFactory
    {
        public SchoolYearDbContextFactory(IOdsConfigurationValues odsConfigurationValues)
        {
            OdsOdsConfigurationValues = odsConfigurationValues;
        }

        public IOdsConfigurationValues OdsOdsConfigurationValues { get; set; }

        public RawOdsDbContext CreateWithParameter(string schoolYear)
        {
            return new RawOdsDbContext(OdsOdsConfigurationValues, schoolYear);
        }
    }
}