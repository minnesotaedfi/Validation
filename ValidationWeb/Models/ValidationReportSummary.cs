using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ValidationWeb
{
    [Table("validation.ReportSummary")]
    public class ValidationReportSummary
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public DateTime RequestedWhen { get; set; }
        public string Collection { get; set; }
        public string InitiatedBy { get; set; }
        public string Status { get; set; }
        public DateTime CompletedWhen { get; set; }
        public int ErrorCount { get; set; }
        public int WarningCount { get; set; }
    }
}