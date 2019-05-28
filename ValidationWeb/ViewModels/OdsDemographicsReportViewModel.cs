using System.Collections.Generic;
using ValidationWeb.Database.Queries;
using ValidationWeb.Models;

namespace ValidationWeb.ViewModels
{
    public class OdsDemographicsReportViewModel
    {
        public ValidationPortalIdentity User { get; set; }

        public int EdOrgId { get; set; }

        public string FourDigitSchoolYear { get; set; }

        public int? DistrictToDisplay { get; set; }

        public bool IsStudentDrillDown { get; set; }

        public string EdOrgName { get; set; }

        public List<DemographicsCountReportQuery> Results { get; set; }

        public bool IsStateMode { get; set; }

        public int? SchoolId { get; set; }

        public string SchoolName { get; set; }

        public int? DrillDownColumnIndex { get; set; }

        public OrgType OrgType { get; set; }
    }
}