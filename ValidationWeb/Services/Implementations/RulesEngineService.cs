using Engine.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ValidationWeb.Services
{
    public class RulesEngineService : IRulesEngineService
    {
        protected readonly ValidationPortalDbContext _dbContext;
        protected readonly IRulesEngineConfigurationValues _engineConfig;
        protected readonly Model _engineObjectModel;
        protected readonly IAppUserService _appUserService;
        protected readonly ISchoolYearService _schoolYearService;

        public RulesEngineService(
            IAppUserService appUserService,
            ISchoolYearService schoolYearService,
            IRulesEngineConfigurationValues engineConfig,
            ValidationPortalDbContext dbContext,
            Model engineObjectModel)
        {
            _dbContext = dbContext;
            _appUserService = appUserService;
            _schoolYearService = schoolYearService;
            _engineConfig = engineConfig;
            _engineObjectModel = engineObjectModel;
        }

        public ValidationReportSummary RunEngine(string fourDigitOdsDbYear, string collectionId)
        {
            ValidationReportSummary newReportSummary = null;
            using (var _odsRawDbContext = new RawOdsDbContext(fourDigitOdsDbYear))
            {
                // Run the rules - This code is adapted from an example in the Rule Engine project.
                // Add a new Validation run to the database, and get an ID back representing this run.
                var newRuleValidationExecution = new RuleValidation { CollectionId = collectionId };
                _odsRawDbContext.RuleValidations.Add(newRuleValidationExecution);
                _odsRawDbContext.SaveChanges();

                newReportSummary = new ValidationReportSummary
                {
                    Collection = collectionId,
                    CompletedWhen = null,
                    ErrorCount = null,
                    WarningCount = null,
                    Id = newRuleValidationExecution.RuleValidationId,
                    EdOrgId = _appUserService.GetSession().FocusedEdOrgId,
                    SchoolYear = _schoolYearService.GetSubmittableSchoolYears().FirstOrDefault(sy => sy.StartYear == fourDigitOdsDbYear),
                    InitiatedBy = _appUserService.GetUser().FullName,
                    RequestedWhen = DateTime.UtcNow,
                    Status = "In Progress"
                };
                _dbContext.ValidationReportSummaries.Add(newReportSummary);
                _dbContext.SaveChanges();

                // Now, store each Ruleset ID and Rule ID that the engine will run.
                var rules = _engineObjectModel.GetRules(collectionId).ToArray();
                var ruleComponents = rules.SelectMany(r => r.Components.Distinct().Select(c => new { r.RulesetId, r.RuleId, Component = c }));
                foreach (var singleRuleNeedingToBeValidated in ruleComponents)
                {
                    _odsRawDbContext.RuleValidationRuleComponents.Add(new RuleValidationRuleComponent
                    {
                        RuleValidationId = newRuleValidationExecution.RuleValidationId,
                        RulesetId = singleRuleNeedingToBeValidated.RulesetId,
                        RuleId = singleRuleNeedingToBeValidated.RuleId,
                        Component = singleRuleNeedingToBeValidated.Component
                    });
                }
                _odsRawDbContext.SaveChanges();

                foreach (var rule in rules)
                {
                    var detailParams = new List<SqlParameter> { new SqlParameter("@RuleValidationId", newRuleValidationExecution.RuleValidationId) };
                    detailParams.AddRange(_engineObjectModel.GetParameters(collectionId).Select(x => new SqlParameter(x.ParameterName, x.Value)));
                    _odsRawDbContext.Database.CommandTimeout = 60;
                    var result = _odsRawDbContext.Database.ExecuteSqlCommand(rule.ExecSql, detailParams.ToArray());
                }

                newReportSummary.CompletedWhen = DateTime.UtcNow;
                newReportSummary.ErrorCount = _odsRawDbContext.RuleValidationDetails.Where(rvd => rvd.RuleValidation.RuleValidationId == newRuleValidationExecution.RuleValidationId && rvd.IsError).Count();
                newReportSummary.WarningCount = _odsRawDbContext.RuleValidationDetails.Where(rvd => rvd.RuleValidation.RuleValidationId == newRuleValidationExecution.RuleValidationId && !rvd.IsError).Count();
                newReportSummary.Status = "Completed";

                _dbContext.SaveChanges();
            }
            return newReportSummary;
        }

        public List<Collection> GetCollections()
        {
            return _engineObjectModel.Collections.ToList();
        }
    }
}