using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ValidationWeb.Models
{
    [Table("validation.RulesView")]

    public class ValidationRulesView
    {
        public ValidationRulesView()
        {
            RulesFields = new HashSet<ValidationRulesField>();
        }
        
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        public string Schema { get; set; }

        public string Name { get; set; }

        public bool Enabled { get; set; }

        [ForeignKey("SchoolYear")]
        public int SchoolYearId { get; set; }

        public SchoolYear SchoolYear { get; set; }

        public ICollection<ValidationRulesField> RulesFields { get; set; }

    }
}