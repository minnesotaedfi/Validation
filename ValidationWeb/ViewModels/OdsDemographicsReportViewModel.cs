using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValidationWeb
{
    public class OdsDemographicsReportViewModel
    {
        public ValidationPortalIdentity User { get; set; }
        public string EdOrgId { get; set; }
        public string EdOrgName { get; set; }
        public List<DemographicsCountReportQuery> Results { get; set; }
    }
}