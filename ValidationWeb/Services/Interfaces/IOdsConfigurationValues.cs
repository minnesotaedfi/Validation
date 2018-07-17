using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValidationWeb.Services
{
    public interface IOdsConfigurationValues
    {
        string GetRawOdsConnectionString(string fourDigitYear);
        string GetValidatedOdsConnectionString(string fourDigitYear);
    }
}