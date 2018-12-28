using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValidationWeb
{
    public class OdsMultipleEnrollmentsReportViewModel
    {
        public ValidationPortalIdentity User { get; set; }

        public int EdOrgId { get; set; }

        public string EdOrgName { get; set; }

        public bool IsStateMode { get; set; }

        public string FourDigitSchoolYear { get; set; }

        public int? SchoolId { get; set; }

        public string SchoolName { get; set; }
    }
}