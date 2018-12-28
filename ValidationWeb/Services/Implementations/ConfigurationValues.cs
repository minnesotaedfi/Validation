using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace ValidationWeb.Services
{
    internal class AppSettingsFileConfigurationValues : IConfigurationValues
    {
        private static string _appId;
        private static string _authenticationServerRedirectUrl;
        private static string _authorizationStoredProcedureName;
        private static string _singleSignOnDatabaseConnectionString;
        private static int _sessionTimeoutInMinutes;
        private static bool _useFakeViewModelData;
        private static bool _isSsoSimulated;
        private static string _ssoSimulatedUserName;
        private static List<SchoolYear> _seedSchoolYears = new List<SchoolYear>();

        static AppSettingsFileConfigurationValues()
        {
            _appId = ConfigurationManager.AppSettings["SsoAppId"]?.ToString();
            _authenticationServerRedirectUrl = ConfigurationManager.AppSettings["AuthenticationServerRedirectUrl"]?.ToString();
            _authorizationStoredProcedureName = ConfigurationManager.AppSettings["AuthorizationStoredProcedureName"]?.ToString();
            _singleSignOnDatabaseConnectionString = ConfigurationManager.ConnectionStrings["SingleSignOnDatabase"]?.ConnectionString;
            _useFakeViewModelData = ConfigurationManager.AppSettings["UseFakeViewModelData"] == "true";
            _isSsoSimulated = ConfigurationManager.AppSettings["UseSimulatedSSO"] == "true";
            _ssoSimulatedUserName = ConfigurationManager.AppSettings["SimulatedUserName"]?.ToString();
            if (!int.TryParse(ConfigurationManager.AppSettings["SessionTimeoutInMinutes"], out _sessionTimeoutInMinutes))
            {
                _sessionTimeoutInMinutes = 30; // Default to 30 minutes.
            }
            var seedSchoolYears = ConfigurationManager.AppSettings["SeedSchoolYears"];
            if (! string.IsNullOrWhiteSpace(seedSchoolYears))
            {
                var seedSchoolYearsCandidates = seedSchoolYears.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach(var schoolYearCandidate in seedSchoolYearsCandidates)
                {
                    int schoolYearInteger;
                    if (int.TryParse(schoolYearCandidate.Trim(), out schoolYearInteger) && schoolYearInteger > 1900 && schoolYearInteger < 2400)
                    {
                        _seedSchoolYears.Add(new SchoolYear((schoolYearInteger - 1).ToString(), schoolYearInteger.ToString()));
                    }
                }
            }
        }

        public string AppId
        {
            get
            {
                return _appId;
            }
        }

        public string AuthenticationServerRedirectUrl
        {
            get
            {
                return _authenticationServerRedirectUrl;
            }
        }

        public string AuthorizationStoredProcedureName
        {
            get
            {
                return _authorizationStoredProcedureName;
            }
        }

        public string SingleSignOnDatabaseConnectionString
        {
            get
            {
                return _singleSignOnDatabaseConnectionString;
            }
        }

        public int SessionTimeoutInMinutes
        {
            get
            {
                return _sessionTimeoutInMinutes;
            }
        }

        /// <summary>
        /// The FakeViewModelData value should be either "false" of omitted in production. 
        /// Only if the value is present and is exactly "true" will fake, hardcoded ViewModel data be used for the UI.
        /// </summary>
        public bool UseFakeViewModelData
        {
            get
            {
                return _useFakeViewModelData;
            }
        }

        public bool UseSimulatedSSO
        {
            get
            {
                return _isSsoSimulated;
            }
        }
        public string SimulatedUserName
        {
            get
            {
                return _ssoSimulatedUserName;
            }
        }
        public List<SchoolYear> SeedSchoolYears
        {
            get
            {
                return _seedSchoolYears;
            }
        }
    }
}