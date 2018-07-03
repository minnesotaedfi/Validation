using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValidationWeb.Services
{
    public interface IConfigurationValues
    {
        bool UseFakeViewModelData { get; }
        string AuthenticationServerRedirectUrl { get; }
    }
}