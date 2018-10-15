using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ValidationWeb
{
    [Table("validation.ErrorEnrollmentDetail")]
    public class ValidationErrorEnrollmentDetail
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        [ForeignKey("ValidationErrorSummary")]
        public int ValidationErrorSummaryId { get; set; }
        public ValidationErrorSummary ValidationErrorSummary { get; set; }

        public string School { get; set; }
        public string SchoolId { get; set; }
        public DateTime? DateEnrolled { get; set; }
        public DateTime? DateWithdrawn { get; set; }
        public string Grade { get; set; }
    }
}