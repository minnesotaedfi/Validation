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
        ValidationReportSummary SetupValidationRun(string fourDigitOdsDbYear, string collectionId);
        List<Collection> GetCollections();

        void RunValidation(string fourDigitOdsDbYear, long ruleValidationId);

        Task RunValidationAsync(string fourDigitOdsDbYear, long ruleValidationId);
    }
}