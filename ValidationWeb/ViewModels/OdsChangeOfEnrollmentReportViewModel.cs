namespace ValidationWeb
{
    using System.Collections.Generic;

    public class OdsChangeOfEnrollmentReportViewModel
    {
        public ValidationPortalIdentity User { get; set; }

        public int EdOrgId { get; set; }

        public string EdOrgName { get; set; }

        public List<ChangeOfEnrollmentReportQuery> Results { get; set; }

        public string FourDigitSchoolYear { get; set; }
    }
}