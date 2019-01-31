using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ValidationWeb
{
    [Table("validation.ReportSummary")]
    public class ValidationReportSummary
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public long ValidationReportSummaryId { get; set; }
        
        [Key, Column(Order = 1)]
        [ForeignKey("SchoolYear")]
        public int SchoolYearId { get; set; }

        [Required]
        [Index]
        public int EdOrgId { get; set; }

        public long RuleValidationId { get; set; }

        public SchoolYear SchoolYear { get; set; }

        public DateTime RequestedWhen { get; set; }

        public string Collection { get; set; }

        public string InitiatedBy { get; set; }

        public string Status { get; set; }

        public DateTime? CompletedWhen { get; set; }

        public int? ErrorCount { get; set; }

        public int? WarningCount { get; set; }

        public long? TotalCount { get; set; }
    }
}