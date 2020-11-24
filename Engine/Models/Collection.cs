using System;
using System.Collections.Generic;

namespace Engine.Models
{
    public class Collection
    {
        public string CollectionId { get; }

        public string ToFriendlyName() => CollectionId.Replace('_', ' ');

        internal Model Model { get; set; }
        
        private readonly List<string> _rulesetIds = new List<string>();

        private readonly List<string> _ruleIds = new List<string>();

        public IReadOnlyList<string> RulesetIds => _rulesetIds;

        public IReadOnlyList<string> RuleIds => _ruleIds;

        public Collection(string collectionId)
        {
            CollectionId = collectionId;
        }

        public void AddRulesetReference(string rulesetId)
        {
            if (string.IsNullOrEmpty(rulesetId) || _rulesetIds.Contains(rulesetId))
            {
                throw new ArgumentException($"Duplicate or null RulesetId: {rulesetId}", nameof(rulesetId));
            }
            
            _rulesetIds.Add(rulesetId);
        }

        public void AddRuleReference(string ruleId)
        {
            if (string.IsNullOrEmpty(ruleId) || _ruleIds.Contains(ruleId))
            {
                throw new ArgumentException($"Duplicate RuleId: {ruleId}", nameof(ruleId));
            }

            _ruleIds.Add(ruleId);
        }
    }
}
