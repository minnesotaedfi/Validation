namespace ValidationWeb
{
    using System.Collections.Generic;

    using ValidationWeb.ViewModels;

    public class StudentDrillDownViewModel
    {
        public string ReportName { get; set; } = string.Empty;

        public ValidationPortalIdentity User { get; set; }

        public int EdOrgId { get; set; }

        public string EdOrgName { get; set; }

        public List<StudentDrillDownQuery> Results { get; set; }

        public bool IsStateMode { get; set; }

        public OrgType OrgType { get; set; }

        public string FourDigitSchoolYear { get; set; }

        public int? SchoolId { get; set; }

        public int? DrillDownColumnIndex { get; set; }

        public string SchoolName { get; set; }

        public StudentDrillDownReportType ReportType { get; set; }
    }
}