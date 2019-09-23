using System;
using System.Collections.Generic;
using System.Configuration;

using ValidationWeb.Models;
using ValidationWeb.Services.Interfaces;

namespace ValidationWeb.Services.Implementations
{
    public class AppSettingsFileConfigurationValues : IConfigurationValues
    {
        private static readonly string _appId;
        private static readonly string _authenticationServerRedirectUrl;
        private static readonly string _authorizationStoredProcedureName;
        private static readonly string _singleSignOnDatabaseConnectionString;
        private static readonly int _sessionTimeoutInMinutes;
        private static readonly bool _useFakeViewModelData;
        private static readonly bool _isSsoSimulated;
        private static readonly List<SchoolYear> _seedSchoolYears = new List<SchoolYear>();
        private static readonly string _environmentName;
        private static readonly string _marssComparisonUrl;
        private static readonly string _ediamProfileLink;
        private static readonly string _ediamLogoutLink;
        private static readonly string _ediamUnauthorizedLink;


        static AppSettingsFileConfigurationValues()
        {
            _appId = ConfigurationManager.AppSettings["SsoAppId"];
            _authenticationServerRedirectUrl = ConfigurationManager.AppSettings["AuthenticationServerRedirectUrl"];
            _authorizationStoredProcedureName = ConfigurationManager.AppSettings["AuthorizationStoredProcedureName"];
            _singleSignOnDatabaseConnectionString = ConfigurationManager.ConnectionStrings["SingleSignOnDatabase"]?.ConnectionString;
            _useFakeViewModelData = ConfigurationManager.AppSettings["UseFakeViewModelData"] == "true";
            _isSsoSimulated = ConfigurationManager.AppSettings["UseSimulatedSSO"] == "true";

            if (!int.TryParse(ConfigurationManager.AppSettings["SessionTimeoutInMinutes"], out _sessionTimeoutInMinutes))
            {
                _sessionTimeoutInMinutes = 30; // Default to 30 minutes.
            }

            // todo: this needs to not be hidden down in this constructor 
            var seedSchoolYears = ConfigurationManager.AppSettings["SeedSchoolYears"];
            if (!string.IsNullOrWhiteSpace(seedSchoolYears))
            {
                var seedSchoolYearsCandidates = seedSchoolYears.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var schoolYearCandidate in seedSchoolYearsCandidates)
                {
                    int schoolYearInteger;
                    if (int.TryParse(schoolYearCandidate.Trim(), out schoolYearInteger) && schoolYearInteger > 1900 && schoolYearInteger < 2400)
                    {
                        _seedSchoolYears.Add(new SchoolYear((schoolYearInteger - 1).ToString(), schoolYearInteger.ToString()));
                    }
                }
            }

            _environmentName = ConfigurationManager.AppSettings["EnvironmentName"];
            _marssComparisonUrl = ConfigurationManager.AppSettings["MarssComparisonUrl"];

            _ediamLogoutLink = ConfigurationManager.AppSettings["EdiamLogoutLink"];
            _ediamProfileLink = ConfigurationManager.AppSettings["EdiamProfileLink"];
            _ediamUnauthorizedLink = ConfigurationManager.AppSettings["EdiamUnauthorizedLink"];
        }

        public string AppId => _appId;

        public string AuthenticationServerRedirectUrl => _authenticationServerRedirectUrl;

        public string AuthorizationStoredProcedureName => _authorizationStoredProcedureName;

        public string SingleSignOnDatabaseConnectionString => _singleSignOnDatabaseConnectionString;

        public int SessionTimeoutInMinutes => _sessionTimeoutInMinutes;

        /// <summary>
        /// The FakeViewModelData value should be either "false" of omitted in production. 
        /// Only if the value is present and is exactly "true" will fake, hardcoded ViewModel data be used for the UI.
        /// </summary>
        public bool UseFakeViewModelData => _useFakeViewModelData;

        public bool UseSimulatedSSO => _isSsoSimulated;

        public List<SchoolYear> SeedSchoolYears => _seedSchoolYears;

        public string EnvironmentName => _environmentName;

        public string MarssComparisonUrl => _marssComparisonUrl;

        public string EdiamProfileLink => _ediamProfileLink;

        public string EdiamLogoutLink => _ediamLogoutLink;

        public string EdiamUnauthorizedLink => _ediamUnauthorizedLink;
    }
}