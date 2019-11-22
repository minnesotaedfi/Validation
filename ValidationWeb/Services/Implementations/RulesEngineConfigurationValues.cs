using System.Collections.Generic;
using System.Configuration;
using System.Linq;

using ValidationWeb.Services.Interfaces;

namespace ValidationWeb.Services.Implementations
{
    public class RulesEngineConfigurationValues : IRulesEngineConfigurationValues
    {
        private static readonly string _rulesFileFolder;
        private static readonly string _ruleEngineResultsSchema;
        private static readonly string _ruleEngineResultsConnectionString;
        private static readonly int _rulesExecutionTimeout;
        private static readonly string _sqlRulesFileFolder;
		private static readonly List<string> _rulesTableExclusions;

        static RulesEngineConfigurationValues()
        {
            _rulesFileFolder = ConfigurationManager.AppSettings["RulesFileFolder"];
            _ruleEngineResultsSchema = ConfigurationManager.AppSettings["RuleEngineResultsSchema"];
            _ruleEngineResultsConnectionString = ConfigurationManager.ConnectionStrings["ValidationPortalDbContext"]?.ToString();
            _sqlRulesFileFolder = ConfigurationManager.ConnectionStrings["SqlRulesFileFolder"]?.ToString();
            int.TryParse(ConfigurationManager.AppSettings["RulesExecutionTimeout"], out _rulesExecutionTimeout);
			_rulesTableExclusions = ConfigurationManager.AppSettings["RulesTableExclusions"].Split(',').ToList();
        }

        public string RulesFileFolder => _rulesFileFolder;

        public string RuleEngineResultsSchema => _ruleEngineResultsSchema;

        public string RuleEngineResultsConnectionString => _ruleEngineResultsConnectionString;

        public int RulesExecutionTimeout => _rulesExecutionTimeout;

        public List<string> RulesTableExclusions => _rulesTableExclusions;
		public string SqlRulesFileFolder => _sqlRulesFileFolder;
    }
}