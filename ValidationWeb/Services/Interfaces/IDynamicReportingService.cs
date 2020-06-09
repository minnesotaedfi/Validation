using System.Collections.Generic;

using Validation.DataModels;

using ValidationWeb.Models;

namespace ValidationWeb.Services.Interfaces
{
    public interface IDynamicReportingService
    {
        IEnumerable<ValidationRulesView> GetRulesViews(int schoolYearId);

        void DeleteViewsAndRulesForSchoolYear(int schoolYearId);
         
        void UpdateViewsAndRulesForSchoolYear(int schoolYearId);
        
        IEnumerable<DynamicReportDefinition> GetReportDefinitions(ProgramAreaLookup programArea = null);
        
        void SaveReportDefinition(DynamicReportDefinition reportDefinition);

        void UpdateReportDefinition(DynamicReportDefinition newReportDefinition);
        
        DynamicReportDefinition GetReportDefinition(int id);

        IList<dynamic> GetReportData(DynamicReportRequest request, int? districtId);

        void EnableReportDefinition(int id);
        
        void DisableReportDefinition(int id);

        void DeleteReportDefinition(int id);
    }
}