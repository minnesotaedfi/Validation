using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValidationWeb
{
    public class ValidationErrorSummary
    {
        public string Component { get; set; }
        public ErrorSeverity Severity { get; set; }
        public Student Student { get; set; }
        public string School { get; set; }
        public string Grade { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorText { get; set; }
    }
}