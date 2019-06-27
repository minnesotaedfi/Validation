using System.Configuration;
using ValidationWeb.Services.Interfaces;

namespace ValidationWeb.Services.Implementations
{
    public class OdsConfigurationValues : IOdsConfigurationValues
    {
        private static readonly string _rawOdsConnectionStringTemplate;

        static OdsConfigurationValues()
        {
            _rawOdsConnectionStringTemplate = ConfigurationManager.ConnectionStrings["RawOdsDbContext"]?.ToString();
        }

        public string GetRawOdsConnectionString(string fourDigitYear)
        {
            return string.Format(_rawOdsConnectionStringTemplate, fourDigitYear);
        }
    }
}