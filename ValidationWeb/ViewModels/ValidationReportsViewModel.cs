namespace ValidationWeb
{
    using System.Collections.Generic;

    using Engine.Models;
    using ValidationWeb.Filters;

    public class ValidationReportsViewModel
    {
        public string DistrictName { get; set; }

        public ValidationPortalIdentity TheUser { get; set; }

        public IEnumerable<ValidationReportSummary> ReportSummaries { get; set; }

        public List<Collection> RulesCollections { get; set; }

        public List<SchoolYear> SchoolYears { get; set; }

        public IList<SubmissionCycle> SubmissionCycles { get; set; }

        public int FocusedEdOrgId { get; set; }

        public int FocusedSchoolYearId { get; set; }

        public bool ReadOnly { get; set; }
    }
}