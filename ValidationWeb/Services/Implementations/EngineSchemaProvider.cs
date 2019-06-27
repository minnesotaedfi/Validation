using Engine.Language;
using ValidationWeb.Services.Interfaces;

namespace ValidationWeb.Services.Implementations
{
    public class EngineSchemaProvider : ISchemaProvider
    {
        public EngineSchemaProvider(IRulesEngineConfigurationValues rulesEngineConfigurationValues)
        {
            Value = rulesEngineConfigurationValues.RuleEngineResultsSchema;
        }

        public string Value { get; }
    }
}