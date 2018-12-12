using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ValidationWeb
{
    [Table("validation.ErrorSummary")]
    public class ValidationErrorSummary
    {
        public ValidationErrorSummary()
        {
            ErrorEnrollmentDetails = new HashSet<ValidationErrorEnrollmentDetail>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        public string Component { get; set; }

        [ForeignKey("Severity")]
        public int SeverityId { get; set; }
        public ErrorSeverityLookup Severity { get; set; }

        public ICollection<ValidationErrorEnrollmentDetail> ErrorEnrollmentDetails { get; set; }

        public string ErrorCode { get; set; }
        public string ErrorText { get; set; }


        [ForeignKey("ValidationReportDetails")]
        public int ValidationReportDetailsId { get; set; }
        public int SchoolYearId { get; set; }
        public ValidationReportDetails ValidationReportDetails { get; set; }

        public bool TryGetErrorSeverity(out ErrorSeverity edOrgType)
        {
            return Enum.TryParse<ErrorSeverity>(Severity.CodeValue, true, out edOrgType);
        }
        /// <summary>
        /// The StudentUniqueId is not globally unique, but unique just to one ODS database.
        /// </summary>
        public string StudentUniqueId { get; set; }
        public string StudentFullName { get; set; }
    }
}