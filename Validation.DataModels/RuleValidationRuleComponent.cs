using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ValidationWeb.Models
{
    /// <summary>
    /// Represents a single rule that needs to be validated, during a single run of the rules engine. 
    /// When running the rules engine, one must specify a particular collection, consisting of many rules. 
    /// For each rule in the collection, each time the collection is run, one instance of RuleValidationRuleComponent 
    /// will be created to track the running of that rule's validation during this particular execution of the Rules Engine.
    /// The structure of this table is given by the Wells Rules Engine
    /// </summary>
    [Table("rules.RuleValidationRuleComponent")]
    public class RuleValidationRuleComponent
    {
        [Key]
        [Column(Order = 0)]
        public long RuleValidationId { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string RulesetId { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(50)]
        public string RuleId { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(50)]
        public string Component { get; set; }

        public RuleValidation RuleValidation { get; set; }
    }
}

