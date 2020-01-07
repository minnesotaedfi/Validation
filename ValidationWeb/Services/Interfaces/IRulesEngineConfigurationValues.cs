using System.Collections.Generic;

namespace ValidationWeb.Services.Interfaces
{
    public interface IRulesEngineConfigurationValues
    {
        string RulesFileFolder { get; }

        string RuleEngineResultsSchema { get; }

        string RuleEngineResultsConnectionString { get; }

        int RulesExecutionTimeout { get; }
        
        string SqlRulesFileFolder { get; set; }
        
        List<string> RulesTableExclusions { get; }

        bool SaveGeneratedRulesSqlToFiles { get; }
    }
}