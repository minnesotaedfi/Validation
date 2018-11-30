using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValidationWeb.Services
{
    public class SubmissionCycleService : ISubmissionCycleService
    {
        protected readonly ValidationPortalDbContext _validationPortalDataContext;
        protected readonly ILoggingService _loggingService;

        public SubmissionCycleService(ValidationPortalDbContext validationPortalDataContext,
                    ILoggingService loggingService)
        {
            _validationPortalDataContext = validationPortalDataContext;
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
                    existingSubmissionCycle = submissionCycle;
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

        /***
        public void SaveSubmissionCycle(int id, int schoolYear_Id, string collectionId, DateTime startDate, DateTime endDate)
        {
            try
            {
                if (id == 0)
                {
                    SaveNewSubmissionCycle(schoolYear_Id, collectionId, startDate, endDate);
                }
                else
                {
                    var existingSubmissionCycle = _validationPortalDataContext.SubmissionCycles.FirstOrDefault(s => s.Id == id);
                    if (existingSubmissionCycle == null)
                    {
                        throw new Exception($"SubmissionCycle with id {id} does not exist.");
                    }
                    SaveExistingSubmissionCycle(id, schoolYear_Id, collectionId, startDate, endDate);
                }
            }
            catch (Exception ex)
            {
                string strMessage = "";
                    strMessage = $" with id = { id }, StartDate = { startDate }, EndDate = { endDate}, SchoolYear_Id = { schoolYear_Id }";
                _loggingService.LogErrorMessage($"An error occurred while saving announcement {strMessage}: {ex.ChainInnerExceptionMessages()}");
                throw new Exception("An error occurred while saving announcement.");
            }
        }

        protected void SaveExistingSubmissionCycle(int id, int schoolYear_Id, string collectionId, DateTime startDate, DateTime endDate)
        {
            SchoolYear schoolYear = _validationPortalDataContext.SchoolYears.FirstOrDefault(x => x.Id == schoolYear_Id);
            SubmissionCycle submissionCycle = _validationPortalDataContext.SubmissionCycles.FirstOrDefault(x => x.Id == id);
            submissionCycle.CollectionId = collectionId;
            submissionCycle.StartDate = startDate;
            submissionCycle.EndDate = endDate;
            //submissionCycle.SchoolYear = schoolYear;
            _validationPortalDataContext.SaveChanges();
        }


        protected void SaveNewSubmissionCycle(int schoolYear_Id, string collectionId, DateTime startDate, DateTime endDate)
        {
            SchoolYear schoolYear = _validationPortalDataContext.SchoolYears.FirstOrDefault(x => x.Id == schoolYear_Id);
            //SubmissionCycle submissionCycle = new SubmissionCycle { CollectionId = collectionId, StartDate = startDate, EndDate = endDate, SchoolYear = schoolYear };
            //_validationPortalDataContext.SubmissionCycles.Add(submissionCycle);
            _validationPortalDataContext.SaveChanges();
        }
        ***/


        public IList<SubmissionCycle> GetSubmissionCyclesByCollectionId(string collectionId)
        {
            if (collectionId == null)
                return null;

            return _validationPortalDataContext.SubmissionCycles.Where(submissionCycle => submissionCycle.CollectionId == collectionId).ToList();
        }

        public bool RemoveSubmissionCycle(int id)
        {
            var submissionRecord = _validationPortalDataContext.SubmissionCycles.FirstOrDefault(submissionCycle => submissionCycle.Id == id);

            if (submissionRecord == null)
                return false;

            _validationPortalDataContext.SubmissionCycles.Remove(submissionRecord);
            _validationPortalDataContext.SaveChanges();

            return true;
        }
    }
}