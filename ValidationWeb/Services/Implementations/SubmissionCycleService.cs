using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValidationWeb.Services
{
    public class SubmissionCycleService : ISubmissionCycleService
    {
        protected readonly ValidationPortalDbContext _validationPortalDataContext;

        public SubmissionCycleService(ValidationPortalDbContext validationPortalDataContext)
        {
            _validationPortalDataContext = validationPortalDataContext;
        }

        public IList<SubmissionCycle> GetSubmissionCycles()
        {
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