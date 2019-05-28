using System.Configuration;
using ValidationWeb.Services.Interfaces;

namespace ValidationWeb.Services.Implementations
{
    public class RulesEngineConfigurationValues : IRulesEngineConfigurationValues
    {
        private static readonly string _rulesFileFolder;
        private static readonly string _ruleEngineResultsSchema;
        private static readonly string _ruleEngineResultsConnectionString;

        static RulesEngineConfigurationValues()
        {
            _rulesFileFolder = ConfigurationManager.AppSettings["RulesFileFolder"];
            _ruleEngineResultsSchema = ConfigurationManager.AppSettings["RuleEngineResultsSchema"];
            _ruleEngineResultsConnectionString = ConfigurationManager.ConnectionStrings["ValidationPortalDbContext"]?.ToString();
        }

        public string RulesFileFolder => _rulesFileFolder;

        public string RuleEngineResultsSchema => _ruleEngineResultsSchema;

        public string RuleEngineResultsConnectionString => _ruleEngineResultsConnectionString;
    }
}