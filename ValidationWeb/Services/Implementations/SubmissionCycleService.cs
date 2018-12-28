namespace ValidationWeb.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    public class SubmissionCycleService : ISubmissionCycleService
    {
        protected readonly ValidationPortalDbContext ValidationPortalDataContext;
        
        protected readonly ISchoolYearService SchoolYearService;
        
        protected readonly ILoggingService LoggingService;

        public SubmissionCycleService(
            ValidationPortalDbContext validationPortalDataContext,
            ISchoolYearService schoolYearService,
            ILoggingService loggingService)
        { 
            ValidationPortalDataContext = validationPortalDataContext;
            SchoolYearService = schoolYearService;
            LoggingService = loggingService;
        }
        
        public IList<SubmissionCycle> GetSubmissionCycles()
        {
            foreach (var submissionCycle in ValidationPortalDataContext.SubmissionCycles)
            {
                var schoolYear = ValidationPortalDataContext.SchoolYears.FirstOrDefault(x => x.Id == submissionCycle.SchoolYearId);
                if (schoolYear != null)
                {
                    submissionCycle.SchoolYearDisplay = schoolYear.ToString();
                }
            }
            return ValidationPortalDataContext.SubmissionCycles.ToList();
        }

        public IList<SubmissionCycle> GetSubmissionCyclesOpenToday()
        {
            var submissionCyclesOpenToday = ValidationPortalDataContext.SubmissionCycles
                .Where(s => s.StartDate <= DateTime.Now && s.EndDate >= DateTime.Now);
            foreach (var submissionCycle in submissionCyclesOpenToday)
            {
                var schoolYear = ValidationPortalDataContext.SchoolYears.FirstOrDefault(x => x.Id == submissionCycle.SchoolYearId);
                if (schoolYear != null)
                {
                    submissionCycle.SchoolYearDisplay = schoolYear.ToString();
                }
            }
            return submissionCyclesOpenToday.ToList();
        }

        public SubmissionCycle GetSubmissionCycle(int id)
        {
            return ValidationPortalDataContext.SubmissionCycles.FirstOrDefault(submissionCycle => submissionCycle.Id == id);
        }

        public bool AddSubmissionCycle(SubmissionCycle submissionCycle)
        {
            ValidationPortalDataContext.SubmissionCycles.Add(submissionCycle);
            ValidationPortalDataContext.SaveChanges();

            return true;
        }

        public bool AddSubmissionCycle(string collectionId, DateTime startDate, DateTime endDate)
        {
            if (collectionId == null)
                return false;

            var newSubmissionCycle = new SubmissionCycle(collectionId, startDate, endDate);
            ValidationPortalDataContext.SubmissionCycles.Add(newSubmissionCycle);
            ValidationPortalDataContext.SaveChanges();

            return true;
        }

        public void SaveSubmissionCycle(SubmissionCycle submissionCycle)
        {
            try
            {
                if (submissionCycle == null)
                {
                    throw new Exception($"Attempted to save a null SubmissionCycle.");
                }
                if (submissionCycle.Id == 0)
                {
                    ValidationPortalDataContext.SubmissionCycles.Add(submissionCycle);
                    ValidationPortalDataContext.SaveChanges();
                }
                else
                {
                    var existingSubmissionCycle = ValidationPortalDataContext.SubmissionCycles.FirstOrDefault(s => s.Id == submissionCycle.Id);
                    if (existingSubmissionCycle == null)
                    {
                        throw new Exception($"SubmissionCycle with id {submissionCycle.Id} does not exist.");
                    }
                    //existingSubmissionCycle = submissionCycle;
                    existingSubmissionCycle.CollectionId = submissionCycle.CollectionId;
                    existingSubmissionCycle.StartDate = submissionCycle.StartDate;
                    existingSubmissionCycle.EndDate = submissionCycle.EndDate;
                    existingSubmissionCycle.SchoolYearId = submissionCycle.SchoolYearId;
                    ValidationPortalDataContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                string strMessage = "";
                if (submissionCycle != null)
                {
                    strMessage = $" with id = { submissionCycle.Id }, StartDate = { submissionCycle.StartDate }, EndDate = { submissionCycle.EndDate}, SchoolYearID = { submissionCycle.SchoolYearId }";
                }

                LoggingService.LogErrorMessage($"An error occurred while saving announcement {strMessage}: {ex.ChainInnerExceptionMessages()}");
                throw new Exception("An error occurred while saving announcement.");
            }
        }

        public IList<SubmissionCycle> GetSubmissionCyclesByCollectionId(string collectionId)
        {
            if (collectionId == null)
                return null;

            return ValidationPortalDataContext.SubmissionCycles.Where(submissionCycle => submissionCycle.CollectionId == collectionId).ToList();
        }

        public List<SelectListItem> GetSchoolYearsSelectList(SubmissionCycle submissionCycle = null)
        {
            var schoolYearsEnumerable = SchoolYearService.GetSubmittableSchoolYearsDictionary().OrderByDescending(x => x.Value);

            List<SelectListItem> schoolYears = schoolYearsEnumerable.Select(kvPair => new SelectListItem
            {
                Value = kvPair.Key.ToString(),
                Text = kvPair.Value,
                Selected = (submissionCycle != null) && (kvPair.Key == submissionCycle.SchoolYearId) ? true : false
            }).ToList();

            if (submissionCycle != null)
            {
                schoolYears[0].Selected = true;
            }
            return schoolYears;
        }

        public SubmissionCycle SchoolYearCollectionAlreadyExists(SubmissionCycle submissionCycle)
        {
            var duplicateCycle = ValidationPortalDataContext.SubmissionCycles
                .FirstOrDefault(x => x.SchoolYearId == submissionCycle.SchoolYearId && x.CollectionId == submissionCycle.CollectionId);
            return duplicateCycle;
        }

        public void DeleteSubmissionCycle(int Id)
        {
            var submissionCycle = ValidationPortalDataContext.SubmissionCycles.FirstOrDefault(a => a.Id == Id);
            if (submissionCycle == null)
            {
                throw new Exception($"Could not delete a submission cycle because submission cycle with ID {Id} was not found");
            }
            ValidationPortalDataContext.SubmissionCycles.Remove(submissionCycle);
            ValidationPortalDataContext.SaveChanges();
        }

    }
}