using System.Collections.Generic;
using System.Threading.Tasks;

using Engine.Models;

using ValidationWeb.Models;

namespace ValidationWeb.Services.Interfaces
{
    public interface IRulesEngineService
    {
        ValidationReportSummary SetupValidationRun(SubmissionCycle submissionCycle, string collectionId);

        List<Collection> GetCollections();

        void RunValidation(SubmissionCycle submissionCycle, long ruleValidationId);

        Task RunValidationAsync(SubmissionCycle submissionCycle, long ruleValidationId);

        void DeleteOldValidationRuns(SubmissionCycle submissionCycle); 
    }
}