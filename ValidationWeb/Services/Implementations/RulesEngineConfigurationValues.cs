namespace ValidationWeb.Services
{
    using System.Configuration;

    public class RulesEngineConfigurationValues : IRulesEngineConfigurationValues
    {
        private static readonly string _rulesFileFolder;
        private static readonly string _ruleEngineResultsSchema;
        private static readonly string _ruleEngineResultsConnectionString;

        static RulesEngineConfigurationValues()
        {
            _rulesFileFolder = ConfigurationManager.AppSettings["RulesFileFolder"]?.ToString();
            _ruleEngineResultsSchema = ConfigurationManager.AppSettings["RuleEngineResultsSchema"]?.ToString();
            _ruleEngineResultsConnectionString = ConfigurationManager.ConnectionStrings["ValidationPortalDbContext"]?.ToString();
        }

        public string RulesFileFolder => _rulesFileFolder;

        public string RuleEngineResultsSchema => _ruleEngineResultsSchema;

        public string RuleEngineResultsConnectionString => _ruleEngineResultsConnectionString;
    }
}