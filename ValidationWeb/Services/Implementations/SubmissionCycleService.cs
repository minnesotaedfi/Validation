using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ValidationWeb.Services
{
    public class SubmissionCycleService : ISubmissionCycleService
    {
        protected readonly ValidationPortalDbContext _validationPortalDataContext;
        private readonly ISchoolYearService _schoolYearService;
        protected readonly ILoggingService _loggingService;

        public SubmissionCycleService(ValidationPortalDbContext validationPortalDataContext,
            ISchoolYearService schoolYearService,
                    ILoggingService loggingService)
        { 
            _validationPortalDataContext = validationPortalDataContext;
            _schoolYearService = schoolYearService;
            _loggingService = loggingService;
        }

        public IList<SubmissionCycle> GetSubmissionCycles()
        {
            foreach (var submissionCycle in _validationPortalDataContext.SubmissionCycles)
            {
                var schoolYear = _validationPortalDataContext.SchoolYears.FirstOrDefault(x => x.Id == submissionCycle.SchoolYearId);
                if (schoolYear != null)
                {
                    submissionCycle.SchoolYearDisplay = schoolYear.ToString();
                }
            }
            return _validationPortalDataContext.SubmissionCycles.ToList();
        }

        public SubmissionCycle GetSubmissionCycle(int id)
        {
            return _validationPortalDataContext.SubmissionCycles.FirstOrDefault(submissionCycle => submissionCycle.Id == id);
        }

        public bool AddSubmissionCycle(SubmissionCycle submissionCycle)
        {
            _validationPortalDataContext.SubmissionCycles.Add(submissionCycle);
            _validationPortalDataContext.SaveChanges();

            return true;
        }

        public bool AddSubmissionCycle(string collectionId, DateTime startDate, DateTime endDate)
        {
            if (collectionId == null)
                return false;

            var newSubmissionCycle = new SubmissionCycle(collectionId, startDate, endDate);
            _validationPortalDataContext.SubmissionCycles.Add(newSubmissionCycle);
            _validationPortalDataContext.SaveChanges();

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
                    _validationPortalDataContext.SubmissionCycles.Add(submissionCycle);
                    _validationPortalDataContext.SaveChanges();
                }
                else
                {
                    var existingSubmissionCycle = _validationPortalDataContext.SubmissionCycles.FirstOrDefault(s => s.Id == submissionCycle.Id);
                    if (existingSubmissionCycle == null)
                    {
                        throw new Exception($"SubmissionCycle with id {submissionCycle.Id} does not exist.");
                    }
                    //existingSubmissionCycle = submissionCycle;
                    existingSubmissionCycle.CollectionId = submissionCycle.CollectionId;
                    existingSubmissionCycle.StartDate = submissionCycle.StartDate;
                    existingSubmissionCycle.EndDate = submissionCycle.EndDate;
                    existingSubmissionCycle.SchoolYearId = submissionCycle.SchoolYearId;
                    _validationPortalDataContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                string strMessage = "";
                if (submissionCycle != null)
                {
                    strMessage = $" with id = { submissionCycle.Id }, StartDate = { submissionCycle.StartDate }, EndDate = { submissionCycle.EndDate}, SchoolYearID = { submissionCycle.SchoolYearId }";
                }

                _loggingService.LogErrorMessage($"An error occurred while saving announcement {strMessage}: {ex.ChainInnerExceptionMessages()}");
                throw new Exception("An error occurred while saving announcement.");
            }
        }

        public IList<SubmissionCycle> GetSubmissionCyclesByCollectionId(string collectionId)
        {
            if (collectionId == null)
                return null;

            return _validationPortalDataContext.SubmissionCycles.Where(submissionCycle => submissionCycle.CollectionId == collectionId).ToList();
        }

        public List<SelectListItem> GetSchoolYearsSelectList(SubmissionCycle submissionCycle = null)
        {
            var schoolYearsEnumerable = _schoolYearService.GetSubmittableSchoolYearsDictionary().OrderByDescending(x => x.Value);

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
            var duplicateCycle = _validationPortalDataContext.SubmissionCycles
                .FirstOrDefault(x => x.SchoolYearId == submissionCycle.SchoolYearId && x.CollectionId == submissionCycle.CollectionId);
            return duplicateCycle;
        }

        public void DeleteSubmissionCycle(int Id)
        {
            var submissionCycle = _validationPortalDataContext.SubmissionCycles.FirstOrDefault(a => a.Id == Id);
            if (submissionCycle == null)
            {
                throw new Exception($"Could not delete a submission cycle because submission cycle with ID {Id} was not found");
            }
            _validationPortalDataContext.SubmissionCycles.Remove(submissionCycle);
            _validationPortalDataContext.SaveChanges();
        }

    }
}