using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ValidationWeb.Models
{
    [Table("validation.DynamicReportField")]

    public class DynamicReportField
    {        
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        public string Description { get; set; }

        public bool Enabled { get; set; }

        [ForeignKey("Field")]
        public int ValidationRulesFieldId { get; set; }
        
        public ValidationRulesField Field { get; set; }
    }
}