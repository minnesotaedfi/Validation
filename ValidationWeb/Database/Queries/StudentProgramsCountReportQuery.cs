namespace ValidationWeb.Database.Queries
{
    public class StudentProgramsCountReportQuery
    {
        public const string StudentProgramsCountQuery = "rules.StudentProgramsReport";
        public const string StudentProgramsStudentDetailsReport = "rules.StudentProgramsStudentDetailsReport";

        public const string OrgTypeColumnName = "OrgType";
        public const string EdOrgIdColumnName = "EdOrgId";
        public const string LEASchoolColumnName = "SchoolName";
        public const string DistrictEdOrgIdColumnName = "DistrictEdOrgId";
        public const string DistrictNameColumnName = "DistrictName";
        public const string DistinctEnrollmentCountColumnName = "DistinctEnrollmentCount";
        public const string DistinctDemographicsCountColumnName = "DistinctDemographicsCount";
        public const string ADParentCountColumnName = "ADParentCount";
        public const string IndianNativeCountColumnName = "IndianNativeCount";
        public const string MigrantCountColumnName = "MigrantCount";
        public const string HomelessCountColumnName = "HomelessCount";
        public const string ImmigrantCountColumnName = "ImmigrantCount";
        public const string EnglishLearnerIdentifiedCountColumnName = "EnglishLearnerIdentifiedCount";
        public const string EnglishLearnerServedCountColumnName = "EnglishLearnerServedCount";
        public const string RecentEnglishCountColumnName = "RecentEnglishCount";
        public const string SLIFECountColumnName = "SLIFECount";
        public const string IndependentStudyCountColumnName = "IndependentStudyCount";
        public const string Section504CountColumnName = "Section504Count";
        public const string Title1PartACountColumnName = "Title1PartACount";
        public const string FreeReducedColumnName = "FreeReducedCount";

        public OrgType OrgType { get; set; }

        public int? EdOrgId { get; set; }

        public string LEASchool { get; set; }

        public int DistinctEnrollmentCount { get; set; }

        public int DistinctDemographicsCount { get; set; }

        public int DemographicsCount { get; set; }

        public int ADParentCount { get; set; }

        public int IndianNativeCount { get; set; }

        public int MigrantCount { get; set; }

        public int HomelessCount { get; set; }

        public int ImmigrantCount { get; set; }

        public int RecentEnglishCount { get; set; }

        public int SLIFECount { get; set; }

        public int EnglishLearnerIdentifiedCount { get; set; }

        public int EnglishLearnerServedCount { get; set; }

        public int IndependentStudyCount { get; set; }

        public int Section504Count { get; set; }

        public int Title1PartACount { get; set; }

        public int FreeReducedCount { get; set; }
    }
}
