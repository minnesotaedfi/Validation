using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ValidationWeb
{
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

