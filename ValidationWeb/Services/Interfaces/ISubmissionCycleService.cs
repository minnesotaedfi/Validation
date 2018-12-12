using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ValidationWeb.Services
{
    public interface ISubmissionCycleService
    {
        void DeleteSubmissionCycle(int Id);
        IList<SubmissionCycle> GetSubmissionCycles();
        IList<SubmissionCycle> GetSubmissionCyclesOpenToday();
        SubmissionCycle GetSubmissionCycle(int id);
        bool AddSubmissionCycle(SubmissionCycle submissionCycle);
        bool AddSubmissionCycle(string collectionId, DateTime startDate, DateTime endDate);
        void SaveSubmissionCycle(SubmissionCycle submissionCycle);
        SubmissionCycle SchoolYearCollectionAlreadyExists(SubmissionCycle submissionCycle);
        IList<SubmissionCycle> GetSubmissionCyclesByCollectionId(string collectionId);
        List<SelectListItem> GetSchoolYearsSelectList(SubmissionCycle submissionCycle = null);
    }
}