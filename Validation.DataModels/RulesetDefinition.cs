using System.Collections.Generic;

namespace Validation.DataModels
{
    public class RulesetDefinition
    {
        public RulesetDefinition()
        {
            RuleDefinitions = new List<RuleDefinition>();
        }

        public string Name { get; set; }
        public ICollection<RuleDefinition> RuleDefinitions { get; }
    }

}
