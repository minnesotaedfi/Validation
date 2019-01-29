using Engine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValidationWeb.Services
{
    using System.Threading.Tasks;

    public interface IRulesEngineService
    {
        ValidationReportSummary SetupValidationRun(SubmissionCycle submissionCycle, string collectionId);
        List<Collection> GetCollections();

        void RunValidation(SubmissionCycle submissionCycle, long ruleValidationId);

        Task RunValidationAsync(SubmissionCycle submissionCycle, long ruleValidationId);
    }
}