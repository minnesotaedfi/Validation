using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValidationWeb
{
    public class ValidationReportsViewModel
    {
        public string DistrictName { get; set; }
        public IEnumerable<ValidationReportSummary> ReportSummaries { get; set; }
    }
}