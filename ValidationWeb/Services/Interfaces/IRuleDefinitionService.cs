using System.Collections.Generic;

using Validation.DataModels;

namespace ValidationWeb.Services.Implementations
{
    public interface IRuleDefinitionService
    {
        IEnumerable<RulesetDefinition> GetRulesetDefinitions();
    }
}