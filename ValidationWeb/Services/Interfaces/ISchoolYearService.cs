using System.Collections.Generic;
using ValidationWeb.Models;

namespace ValidationWeb.Services.Interfaces
{
    public interface ISchoolYearService
    {
        IList<SchoolYear> GetSubmittableSchoolYears();
        
        Dictionary<int, string> GetSubmittableSchoolYearsDictionary();
        
        SchoolYear GetSchoolYearById(int id);
        
        void SetSubmittableSchoolYears(IEnumerable<SchoolYear> years);

        bool UpdateErrorThresholdValue(
            int id,
            decimal thresholdValue);

        bool AddNewSchoolYear(
            string startDate,
            string endDate);

        bool ValidateYears(
            string startDate,
            string endDate);

        bool RemoveSchoolYear(int id);
    }
}