using System.Collections.Generic;
using Engine.Language;

namespace Engine.Models
{
    public class Rule
    {
        private readonly ISchemaProvider _schemaProvider;
        public string RuleId { get; }
        public string RulesetId { get; }
        public List<string> CollectionIds { get; } = new List<string>();
        public List<string> Components { get; } = new List<string>();
        public string Sql { get; }
        public string ExecSql => $"INSERT INTO [{_schemaProvider.Value}].[RuleValidationDetail] " +
                                 $"SELECT @RuleValidationId, * FROM ({Sql}) _sql";

        public Rule(string ruleId, string rulesetId, ISchemaProvider schemaProvider, IEnumerable<string> collectionIds = null, string sql = null, IEnumerable<string> components = null)
        {
            _schemaProvider = schemaProvider;
            RuleId = ruleId;
            RulesetId = rulesetId;
            CollectionIds.AddRange(collectionIds ?? new List<string>());
            Components.AddRange(components ?? new List<string>());
            Sql = sql;
        }
    }
}
