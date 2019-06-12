using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ValidationWeb.Models
{
    [Table("validation.RulesField")]

    public class ValidationRulesField
    {
        public ValidationRulesField()
        {
        }
        
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public bool Enabled { get; set; }

        [ForeignKey("RulesView")]
        public int RulesViewId { get; set; }

        public ValidationRulesView RulesView { get; set; }
    }
}
