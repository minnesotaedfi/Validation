using System.Collections.Generic;
using ValidationWeb.Models;

namespace ValidationWeb.ViewModels
{
    public class AdminDynamicReportViewModel
    {
        public AdminDynamicReportViewModel()
        {
            ValidationRulesViews = new List<ValidationRulesView>();
        }

        public DynamicReportDefinition DynamicReportDefinition { get; set; }
        
        public int? ReportSchoolYearId { get; set; }
        public ICollection<ValidationRulesView> ValidationRulesViews { get; set; }

    }
}