using System.Collections.Generic;

using Validation.DataModels;

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
        
        public ICollection<ValidationRulesView> ValidationRulesViews { get; set; }
    }
}