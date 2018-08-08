using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValidationWeb.Services
{
    public class SchoolYearService : ISchoolYearService
    {
        protected readonly ValidationPortalDbContext _validationPortalDataContext;
        public SchoolYearService(ValidationPortalDbContext validationPortalDataContext)
        {
            _validationPortalDataContext = validationPortalDataContext;
        }
        public IList<SchoolYear> GetSubmittableSchoolYears()
        {
            return _validationPortalDataContext.SchoolYears.ToList();
        }
        public void SetSubmittableSchoolYears(IEnumerable<SchoolYear> years)
        {
            _validationPortalDataContext.SchoolYears.RemoveRange(_validationPortalDataContext.SchoolYears.ToArray()) ;
            _validationPortalDataContext.SaveChanges();
            _validationPortalDataContext.SchoolYears.AddRange(years);
            _validationPortalDataContext.SaveChanges();
        }

        public bool UpdateErrorThresholdValue(int id, decimal thresholdValue)
        {
            var schoolYearRecord = _validationPortalDataContext.SchoolYears.FirstOrDefault(schoolYear => schoolYear.Id == id);
            if (schoolYearRecord == null)
                return false;

            schoolYearRecord.ErrorThreshold = thresholdValue;
            _validationPortalDataContext.SaveChanges();

            return true;
        }

        public bool AddNewSchoolYear(string startDate, string endDate)
        {
            if (string.IsNullOrEmpty(startDate) || string.IsNullOrEmpty(endDate))
                return false;

            var newSchoolYear = new SchoolYear(startDate, endDate);
            _validationPortalDataContext.SchoolYears.Add(newSchoolYear);
            _validationPortalDataContext.SaveChanges();

            return true;
        }

        public bool RemoveSchoolYear(int id)
        {
            var schoolYearRecord = _validationPortalDataContext.SchoolYears.FirstOrDefault(schoolYear => schoolYear.Id == id);

            if (schoolYearRecord == null)
                return false;

            _validationPortalDataContext.SchoolYears.Remove(schoolYearRecord);
            return true;
        }
    }
}