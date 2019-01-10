using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValidationWeb
{
    public class ResidentsEnrolledElsewhereReportQuery
    {
        public const string ResidentsEnrolledElsewhereQuery = "rules.ResidentsEnrolledElsewhereReport";

        public const string ResidentsEnrolledElsewhereStudentDetailsQuery = "rules.ResidentsEnrolledElsewhereStudentDetailsReport";
        
        public const string OrgTypeColumnName = "OrgType";
        
        public const string EdOrgIdColumnName = "EdOrgId";
        
        public const string EdOrgNameColumnName = "EdOrgName";
        
        public const string DistrictOfEnrollmentIdColumnName = "DistrictOfEnrollmentId";
        
        public const string DistrictOfEnrollmentNameColumnName = "DistrictOfEnrollmentName";
        
        public const string ResidentsEnrolledColumnName = "ResidentsEnrolled";

        public OrgType OrgType { get; set; }

        public int? EdOrgId { get; set; }
        
        public string EdOrgName { get; set; }
        
        public int DistrictOfEnrollmentId { get; set; }
        
        public string DistrictOfEnrollmentName { get; set; }
        
        public int ResidentsEnrolled { get; set; }
    }
}
