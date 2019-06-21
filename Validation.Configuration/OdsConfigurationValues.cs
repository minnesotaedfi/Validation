using System.Configuration;
using ValidationWeb.Services.Interfaces;

namespace ValidationWeb.Services.Implementations
{
    public class OdsConfigurationValues : IOdsConfigurationValues
    {
        private static readonly string _rawOdsConnectionStringTemplate;
        private static readonly string _validatedOdsConnectionStringTemplate;

        static OdsConfigurationValues()
        {
            _rawOdsConnectionStringTemplate = ConfigurationManager.ConnectionStrings["RawOdsDbContext"]?.ToString();
            _validatedOdsConnectionStringTemplate = ConfigurationManager.ConnectionStrings["ValidatedOdsDbContext"]?.ToString();
        }

        public string GetRawOdsConnectionString(string fourDigitYear)
        {
            return string.Format(_rawOdsConnectionStringTemplate, fourDigitYear);
        }

        //public string GetValidatedOdsConnectionString(string fourDigitYear)
        //{
        //    return string.Format(_validatedOdsConnectionStringTemplate, fourDigitYear);
        //}
    }
}