using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace ValidationWeb.Services
{
    public class RulesEngineConfigurationValues : IRulesEngineConfigurationValues
    {
        private static string _rulesFileFolder;
        private static string _ruleEngineResultsSchema;
        private static string _ruleEngineResultsConnectionString;

        static RulesEngineConfigurationValues()
        {
            _rulesFileFolder = ConfigurationManager.AppSettings["RulesFileFolder"]?.ToString();
            _ruleEngineResultsSchema = ConfigurationManager.AppSettings["RuleEngineResultsSchema"]?.ToString();
            _ruleEngineResultsConnectionString = ConfigurationManager.ConnectionStrings["ValidationPortalDbContext"]?.ToString();
        }

        public string RulesFileFolder
        {
            get
            {
                return _rulesFileFolder;
            }
        }

        public string RuleEngineResultsSchema
        {
            get
            {
                return _ruleEngineResultsSchema;
            }
        }

        public string RuleEngineResultsConnectionString
        {
            get
            {
                return _ruleEngineResultsConnectionString;
            }
        }
    }
}