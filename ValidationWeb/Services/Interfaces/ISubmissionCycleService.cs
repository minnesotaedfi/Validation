using System;
using System.Collections.Generic;
using System.Web.Mvc;
using ValidationWeb.Models;

namespace ValidationWeb.Services.Interfaces
{
    public interface ISubmissionCycleService
    {
        void DeleteSubmissionCycle(int Id);

        IList<SubmissionCycle> GetSubmissionCycles();

        IList<SubmissionCycle> GetSubmissionCyclesOpenToday();

        SubmissionCycle GetSubmissionCycle(int id);

        bool AddSubmissionCycle(SubmissionCycle submissionCycle);

        bool AddSubmissionCycle(
            string collectionId,
            DateTime startDate,
            DateTime endDate);

        void SaveSubmissionCycle(SubmissionCycle submissionCycle);

        SubmissionCycle SchoolYearCollectionAlreadyExists(SubmissionCycle submissionCycle);

        IList<SubmissionCycle> GetSubmissionCyclesByCollectionId(string collectionId);

        List<SelectListItem> GetSchoolYearsSelectList(SubmissionCycle submissionCycle = null);
    }
}