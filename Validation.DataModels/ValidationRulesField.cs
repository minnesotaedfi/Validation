using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        public int DisplayOrder { get; set; }
    }
}
