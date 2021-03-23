using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;

using ValidationWeb.Database;
using ValidationWeb.Models;
using ValidationWeb.Services.Interfaces;

namespace ValidationWeb.Services.Implementations
{
    public class SchoolYearService : ISchoolYearService
    {
        public SchoolYearService(IDbContextFactory<ValidationPortalDbContext> validationPortalDataContextFactory)
        {
            ValidationPortalDataContextFactory = validationPortalDataContextFactory;
        }

        private IDbContextFactory<ValidationPortalDbContext> ValidationPortalDataContextFactory { get; }

        public IList<SchoolYear> GetSubmittableSchoolYears()
        {
            using (var validationPortalDataContext = ValidationPortalDataContextFactory.Create())
            {
                return validationPortalDataContext.SchoolYears
                    .Where(x => x.Enabled)
                    .Where(x => x.Visible)
                    .ToList();
            }
        }

        public IList<SchoolYear> GetAllSchoolYears()
        {
            using (var validationPortalDataContext = ValidationPortalDataContextFactory.Create()) 
            {
                return validationPortalDataContext.SchoolYears
                    .Where(x => x.Enabled)
                    .ToList();
            }
        }

        public Dictionary<int, string> GetSubmittableSchoolYearsDictionary()
        {
            using (var validationPortalDataContext = ValidationPortalDataContextFactory.Create())
            {
                return validationPortalDataContext.SchoolYears
                    .ToDictionary(x => x.Id, x => x.ToString());
            }
        }

        public void SetSubmittableSchoolYears(IEnumerable<SchoolYear> years)
        {
            using (var validationPortalDataContext = ValidationPortalDataContextFactory.Create())
            {
                validationPortalDataContext.SchoolYears.RemoveRange(validationPortalDataContext.SchoolYears.ToArray());
                validationPortalDataContext.SaveChanges();

                validationPortalDataContext.SchoolYears.AddRange(years);
                validationPortalDataContext.SaveChanges();
            }
        }

        public bool UpdateErrorThresholdValue(int id, decimal thresholdValue)
        {
            using (var validationPortalDataContext = ValidationPortalDataContextFactory.Create())
            {
                var schoolYearRecord = validationPortalDataContext.SchoolYears.FirstOrDefault(schoolYear => schoolYear.Id == id);

                if (schoolYearRecord == null)
                {
                    return false;
                }

                schoolYearRecord.ErrorThreshold = thresholdValue;
                validationPortalDataContext.SaveChanges();

                return true;
            }
        }

        public bool AddNewSchoolYear(string startDate, string endDate)
        {
            if (string.IsNullOrEmpty(startDate) || string.IsNullOrEmpty(endDate))
            {
                return false;
            }

            if (!ValidateYears(startDate, endDate))
            {
                return false;
            }

            var newSchoolYear = new SchoolYear(startDate, endDate);

            using (var validationPortalDataContext = ValidationPortalDataContextFactory.Create())
            {
                validationPortalDataContext.SchoolYears.Add(newSchoolYear);
                validationPortalDataContext.SaveChanges();
            }

            return true;
        }

        // AddNewSchoolYear Validator to make sure the dates are just one year apart
        public bool ValidateYears(string startDate, string endDate)
        {
            int startDateCheck;
            var didParse = Int32.TryParse(startDate, out startDateCheck);

            if (!didParse)
            {
                return false;
            }

            startDateCheck = startDateCheck + 1; 
            return startDateCheck.ToString() == endDate;
        }

        public bool RevealSchoolYear(int id)
        {
            using (var validationPortalDataContext = ValidationPortalDataContextFactory.Create())
            {
                var schoolYearRecord = validationPortalDataContext.SchoolYears.FirstOrDefault(schoolYear => schoolYear.Id == id);

                if (schoolYearRecord == null)
                {
                    return false;
                }

                schoolYearRecord.Visible = true;
                validationPortalDataContext.SaveChanges();
                return true;
            }
        }

        public bool HideSchoolYear(int id)
        {
            using (var validationPortalDataContext = ValidationPortalDataContextFactory.Create())
            {
                var schoolYearRecord = validationPortalDataContext.SchoolYears.FirstOrDefault(schoolYear => schoolYear.Id == id);

                if (schoolYearRecord == null)
                {
                    return false;
                }

                // validationPortalDataContext.SchoolYears.Remove(schoolYearRecord);
                schoolYearRecord.Visible = false;
                validationPortalDataContext.SaveChanges();
                return true;
            }
        }

        public SchoolYear GetSchoolYearById(int id)
        {
            using (var validationPortalDataContext = ValidationPortalDataContextFactory.Create())
            {
                return validationPortalDataContext.SchoolYears.FirstOrDefault(schoolYear => schoolYear.Id == id);
            }
        }
    }
}
