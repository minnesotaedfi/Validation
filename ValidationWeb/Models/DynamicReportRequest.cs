using System.Collections.Generic;

namespace ValidationWeb.Models
{
    public class DynamicReportRequest
    {
        public DynamicReportRequest()
        {
            SelectedFields = new List<string>();
        }

        public int ReportDefinitionId { get; set; }

        public int SchoolYearId { get; set; }

        public ICollection<string> SelectedFields { get; set; }
    }
}