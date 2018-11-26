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
                                 $"SELECT @RuleValidationId, Id, RuleId, IsError, Message FROM ({Sql}) _sql WHERE {WhereCondition}";
        protected string WhereCondition { get; set; } = " (1 = 1) "; // Default is effectively NO filter.

        public Rule(string ruleId, string rulesetId, ISchemaProvider schemaProvider, IEnumerable<string> collectionIds = null, string sql = null, IEnumerable<string> components = null)
        {
            _schemaProvider = schemaProvider;
            RuleId = ruleId;
            RulesetId = rulesetId;
            CollectionIds.AddRange(collectionIds ?? new List<string>());
            Components.AddRange(components ?? new List<string>());
            Sql = sql;
        }

        /// <summary>
        /// Will cause the rule to fail if the column <code>DistrictID</code> isn't present in the component view to which the rule is applied.
        /// Special case: If the component view contains "MultipleEnrollments" in the name (without regard to upper/lowercase - case insensitive)
        /// then instead of DistrictID, <code>DistrictIdLeft</code> and <code>DistrictIdRight</code> will be checked instead.
        /// This code is very particular to the State of Minnesota because other implementations of the Rules Engine were one-per-district.
        /// </summary>
        /// <param name="filterCondition">The district ID that will be sought - other districts will not be present in the results.</param>
        public void AddDistrictWhereFilter(int districtId)
        {
            if ((Sql ?? string.Empty).ToUpperInvariant().Contains("MULTIPLEENROLLMENT"))
            {
                WhereCondition = $" (DistrictIdLeft = {districtId.ToString()} OR DistrictIdRight = {districtId.ToString()}) ";
            }
            else
            {
                WhereCondition = $" (DistrictId = {districtId.ToString()}) ";
            }
        }
    }
}
