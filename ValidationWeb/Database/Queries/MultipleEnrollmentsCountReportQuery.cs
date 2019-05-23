using ValidationWeb.Database.Queries;

namespace ValidationWeb
{
    public class MultipleEnrollmentsCountReportQuery
    {
        public const string MultipleEnrollmentsCountQuery = "rules.MultipleEnrollmentReport";
        public const string MultipleEnrollmentsStudentDetailsQuery = "rules.MultipleEnrollmentStudentDetailsReport";
        public const string OrgTypeColumnName = "OrgType";
        public const string EdOrgIdColumnName = "EdOrgId";
        public const string SchoolNameColumnName = "SchoolName";
        public const string TotalEnrollmentCountColumnName = "TotalEnrollmentCount";
        public const string DistinctEnrollmentCountColumnName = "DistinctEnrollmentCount";
        public const string EnrolledInOtherSchoolsCountColumnName = "EnrolledInOtherSchoolsCount";
        public const string EnrolledInOtherDistrictsCountColumnName = "EnrolledInOtherDistrictsCount";

        public OrgType OrgType { get; set; }

        public int? EdOrgId { get; set; }

        public string LEASchool { get; set; }

        public int TotalEnrollmentCount { get; set; }

        public int DistinctEnrollmentCount { get; set; }

        public int EnrolledInOtherSchoolsCount { get; set; }

        public int EnrolledInOtherDistrictsCount { get; set; }
    }
}