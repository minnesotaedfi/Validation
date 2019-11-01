using System.Collections.Generic;

using ValidationWeb.Models;

namespace ValidationWeb.Services.Interfaces
{
    public interface IConfigurationValues
    {
        string AppId { get; }
        string AuthenticationServerRedirectUrl { get; }
        string AuthorizationStoredProcedureName { get; }
        string SingleSignOnDatabaseConnectionString { get; }
        int SessionTimeoutInMinutes { get; }
        /// <summary>
        /// The FakeViewModelData value should be either "false" of omitted in production. 
        /// Only if the value is present and is exactly "true" will fake, hardcoded ViewModel data be used for the UI.
        /// </summary>
        bool UseFakeViewModelData { get; }
        bool UseSimulatedSSO { get; }
        List<SchoolYear> SeedSchoolYears { get; }

        string EnvironmentName { get; }

        string MarssComparisonUrl { get; }
        
        string MarssWesUrl { get; }
        
        string EdiamProfileLink { get; }

        string EdiamLogoutLink { get; }

        string EdiamUnauthorizedLink { get; }
    }
}