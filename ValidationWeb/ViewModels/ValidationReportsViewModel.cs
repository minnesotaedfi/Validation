using System.Collections.Generic;
using Engine.Models;

using Validation.DataModels;

using ValidationWeb.Models;

namespace ValidationWeb.ViewModels
{
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

        public ProgramArea FocusedProgramArea { get; set; }

        public bool ReadOnly { get; set; }
    }
}