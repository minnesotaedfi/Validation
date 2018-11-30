using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValidationWeb.Services
{
    public interface ISubmissionCycleService
    {
        IList<SubmissionCycle> GetSubmissionCycles();
        SubmissionCycle GetSubmissionCycle(int id);
        bool AddSubmissionCycle(SubmissionCycle submissionCycle);
        bool AddSubmissionCycle(string collectionId, DateTime startDate, DateTime endDate);
        bool RemoveSubmissionCycle(int id);
        void SaveSubmissionCycle(SubmissionCycle submissionCycle);
        IList<SubmissionCycle> GetSubmissionCyclesByCollectionId(string collectionId);
    }
}