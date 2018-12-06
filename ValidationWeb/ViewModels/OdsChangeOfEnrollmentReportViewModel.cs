using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValidationWeb
{
    public class OdsChangeOfEnrollmentReportViewModel
    {
        public ValidationPortalIdentity User { get; set; }
        public int EdOrgId { get; set; }
        public string EdOrgName { get; set; }
        public bool ReadOnly { get; set; }
        public List<ChangeOfEnrollmentReportQuery> Results { get; set; }
    }
}