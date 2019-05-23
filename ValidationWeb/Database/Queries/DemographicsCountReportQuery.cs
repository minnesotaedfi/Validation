namespace ValidationWeb.Database.Queries
{
    public class DemographicsCountReportQuery
    {
        public const string DistrictAncestryRaceCountsQuery = "rules.RaceAEOReport";
        public const string DistrictAncestryRaceCountsStudentDetailsQuery = "rules.RaceAEOStudentDetailsReport";
        public const string OrgTypeColumnName = "OrgType";
        public const string EdOrgIdColumnName = "EdOrgId";
        public const string LEASchoolColumnName = "SchoolName";
        public const string DistinctEnrollmentCountColumnName = "DistinctEnrollmentCount";
        public const string DistinctDemographicsCountColumnName = "DistinctDemographicsCount";
        public const string RaceGivenCountColumnName = "RaceGivenCount";
        public const string AncestryGivenCountColumnName = "AncestryGivenCount";

        public OrgType OrgType { get; set; }

        public int? EdOrgId { get; set; }

        public string LEASchool { get; set; }

        public int EnrollmentCount { get; set; }

        public int DemographicsCount { get; set; }

        public int RaceGivenCount { get; set; }

        public int AncestryGivenCount { get; set; }
    }
}