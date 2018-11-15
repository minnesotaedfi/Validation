using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValidationWeb
{
    public class StudentDrillDownViewModel
    {
        public string ReportName { get; set; } = string.Empty;
        public ValidationPortalIdentity User { get; set; }
        public int EdOrgId { get; set; }
        public string EdOrgName { get; set; }
        public List<StudentDrillDownQuery> Results { get; set; }
        public bool IsStateMode { get; set; }
    }
}