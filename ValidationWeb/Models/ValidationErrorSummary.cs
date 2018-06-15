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
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        public string Component { get; set; }

        [ForeignKey("Severity")]
        public int SeverityId { get; set; }
        public ErrorSeverityLookup Severity { get; set; }

        [ForeignKey("Student")]
        public int StudentId { get; set; }
        public Student Student { get; set; }

        public string School { get; set; }
        public string Grade { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorText { get; set; }

        public bool TryGetErrorSeverity(out ErrorSeverity edOrgType)
        {
            return Enum.TryParse<ErrorSeverity>(Severity.CodeValue, true, out edOrgType);
        }
    }
}