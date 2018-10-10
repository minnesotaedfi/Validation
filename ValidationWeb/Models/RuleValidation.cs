using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ValidationWeb
{
    /// <summary>
    /// Represents one occurence of running the entire validation engine for a particular collection of [rulesets & rules]. 
    /// Typically, a collection consists of a set of rulesets, given by their Ruleset ID's. Rulesets, in turn, consist of a list of Rules.
    /// In addition to rulesets, a collection can additionally include individual rules, identified by their Rule ID's, 
    /// regardless of what ruleset they do or don't exist in. 
    /// The structure of this table is given by the Wells Rules Engine
    /// </summary>
    [Table("rules.RuleValidation")]
    public class RuleValidation
    {
        public RuleValidation()
        {
            // Database Default
            RunDateTime = DateTime.UtcNow;
            RuleValidationDetails = new HashSet<RuleValidationDetail>();
            RuleValidationRuleComponents = new HashSet<RuleValidationRuleComponent>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public long RuleValidationId { get; set; }

        [StringLength(50)]
        public string CollectionId { get; set; }

        public DateTime RunDateTime { get; set; }

        public ICollection<RuleValidationDetail> RuleValidationDetails { get; set; }

        public ICollection<RuleValidationRuleComponent> RuleValidationRuleComponents { get; set; }        
    }
}