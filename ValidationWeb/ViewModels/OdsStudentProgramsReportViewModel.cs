using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValidationWeb
{
    public class OdsStudentProgramsReportViewModel
    {
        public ValidationPortalIdentity User { get; set; }
        public int EdOrgId { get; set; }
        public string EdOrgName { get; set; }
        public List<StudentProgramsCountReportQuery> Results { get; set; }
        public bool IsStateMode { get; set; }

    }
}