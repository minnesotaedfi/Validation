using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValidationWeb
{
    public class ValidationReportSummary
    {
        public DateTime RequestedWhen { get; set; }
        public string Collection { get; set; }
        public int ValidationResultsId { get; set; }
        public string InitiatedBy { get; set; }
        public string Status { get; set; }
        public DateTime CompletedWhen { get; set; }
        public int ErrorCount { get; set; }
        public int WarningCount { get; set; }
    }
}