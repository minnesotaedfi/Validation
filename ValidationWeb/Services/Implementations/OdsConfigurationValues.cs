using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace ValidationWeb.Services
{
    public class OdsConfigurationValues : IOdsConfigurationValues
    {
        private static string _rawOdsConnectionStringTemplate;
        private static string _validatedOdsConnectionStringTemplate;

        static OdsConfigurationValues()
        {
            _rawOdsConnectionStringTemplate = ConfigurationManager.ConnectionStrings["RawOdsDbContext"]?.ToString();
            _validatedOdsConnectionStringTemplate = ConfigurationManager.ConnectionStrings["ValidatedOdsDbContext"]?.ToString();
        }

        public string GetRawOdsConnectionString(string fourDigitYear)
        {
            return string.Format(_rawOdsConnectionStringTemplate, fourDigitYear);
        }

        public string GetValidatedOdsConnectionString(string fourDigitYear)
        {
            return string.Format(_validatedOdsConnectionStringTemplate, fourDigitYear);
        }
    }
}