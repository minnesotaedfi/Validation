using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValidationWeb
{
    public class ValidationReportDetailsViewModel
    {
        public string DistrictName { get; set; }
        public string CollectionName { get; set; }
        public DateTime CompletedWhen { get; set; }
        public IEnumerable<ValidationErrorSummary> ErrorSummaries { get; set; }
    }
}