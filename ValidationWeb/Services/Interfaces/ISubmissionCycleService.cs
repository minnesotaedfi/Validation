using System;
using System.Collections.Generic;
using System.Web.Mvc;

using Validation.DataModels;

using ValidationWeb.Models;

namespace ValidationWeb.Services.Interfaces
{
    public interface ISubmissionCycleService
    {
        void DeleteSubmissionCycle(int id);

        IList<SubmissionCycle> GetSubmissionCycles(ProgramArea programArea = null);

        IEnumerable<SubmissionCycle> GetSubmissionCyclesOpenToday(ProgramArea programArea = null);

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