using System.Collections.Generic;
using ValidationWeb.Models;

namespace ValidationWeb.Services.Interfaces
{
    public interface IValidationRulesViewService
    {
        IEnumerable<ValidationRulesView> GetRulesViews(int schoolYearId);

        void DeleteRulesForSchoolYear(int schoolYearId);
        
        void UpdateRulesForSchoolYear(int schoolYearId);
        
        IEnumerable<string> GetFieldsForView(SchoolYear schoolYear, string schema, string name);
    }
}