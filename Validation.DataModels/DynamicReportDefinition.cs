using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using ValidationWeb.Models;

namespace Validation.DataModels
{
    [Table("validation.DynamicReportDefinition")]
    public class DynamicReportDefinition
    {
        public DynamicReportDefinition()
        {
            Fields = new HashSet<DynamicReportField>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool Enabled { get; set; }

        [ForeignKey("SchoolYear")]
        public int SchoolYearId { get; set; }

        public SchoolYear SchoolYear { get; set; }

        [ForeignKey("RulesView")]
        public int ValidationRulesViewId { get; set; }

        public ValidationRulesView RulesView { get; set; }

        public ICollection<DynamicReportField> Fields { get; set; }

        public bool IsOrgLevelReport { get; set; }

        [Required]
        [ForeignKey("ProgramArea")]
        public int? ProgramAreaId { get; set; }

        public ProgramArea ProgramArea { get; set; }
    }
}