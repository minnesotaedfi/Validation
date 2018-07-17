using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ValidationWeb
{
    [Table("validation.ReportDetails")]
    public class ValidationReportDetails
    {
        public ValidationReportDetails()
        {
            ErrorSummaries = new HashSet<ValidationErrorSummary>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        public string DistrictName { get; set; }
        public string CollectionName { get; set; }
        public DateTime CompletedWhen { get; set; }

        [ForeignKey("ValidationReportSummary")]
        public long ValidationReportSummaryId { get; set; }
        public ValidationReportSummary ValidationReportSummary { get; set; }

        public ICollection<ValidationErrorSummary> ErrorSummaries { get; set; }

    }
}