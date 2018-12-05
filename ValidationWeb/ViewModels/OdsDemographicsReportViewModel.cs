namespace ValidationWeb
{
    using System.Collections.Generic;

    public class OdsDemographicsReportViewModel
    {
        public ValidationPortalIdentity User { get; set; }

        public int EdOrgId { get; set; }

        public string EdOrgName { get; set; }

        public List<DemographicsCountReportQuery> Results { get; set; }

        public bool IsStateMode { get; set; }

        public int? SchoolId { get; set; }

        public string SchoolName { get; set; }
    }
}