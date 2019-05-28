using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ValidationWeb.Models
{
    /// <summary>
    /// Represents a single error or warning. The structure of this table is given by the Wells Rules Engine.
    /// </summary>
    [Table("rules.RuleValidationDetail")]
    public class RuleValidationDetail
    {
        [Key]
        [Column(Order = 0)]
        public long RuleValidationId { get; set; }

        public RuleValidation RuleValidation { get; set; }

        [Key]
        [Column(Order = 1)]
        public long Id { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(50)]
        public string RuleId { get; set; }

        public bool IsError { get; set; }

        public string Message { get; set; }
    }
}

