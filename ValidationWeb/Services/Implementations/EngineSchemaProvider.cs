using Engine.Language;

namespace ValidationWeb.Services.Implementations
{
    public class EngineSchemaProvider : ISchemaProvider
    {
        private static readonly string _ruleEngineResultsSchema;

        static EngineSchemaProvider()
        {
            _ruleEngineResultsSchema = (new RulesEngineConfigurationValues()).RuleEngineResultsSchema;
        }

        public string Value => _ruleEngineResultsSchema;
    }
}