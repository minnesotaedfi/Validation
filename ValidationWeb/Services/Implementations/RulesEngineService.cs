using Engine.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
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
        protected readonly IEdOrgService _edOrgService;
        protected readonly ISchoolYearService _schoolYearService;
        protected readonly ILoggingService _loggingService;

        public RulesEngineService(
            IAppUserService appUserService,
            IEdOrgService edOrgService,
            ISchoolYearService schoolYearService,
            IRulesEngineConfigurationValues engineConfig,
            ILoggingService loggingService,
            ValidationPortalDbContext dbContext,
            Model engineObjectModel
            )
        {
            _dbContext = dbContext;
            _appUserService = appUserService;
            _edOrgService = edOrgService;
            _schoolYearService = schoolYearService;
            _loggingService = loggingService;
            _engineConfig = engineConfig;
            _engineObjectModel = engineObjectModel;
        }

        public ValidationReportSummary RunEngine(string fourDigitOdsDbYear, string collectionId)
        {
            ValidationReportSummary newReportSummary = null;
            using (var _odsRawDbContext = new RawOdsDbContext(fourDigitOdsDbYear))
            {
                // Run the rules - This code is adapted from an example in the Rule Engine project.

                #region Add a new execution of the Validation Engine to the ODS database, (required by the Engine) and get an ID back representing this execution.
                var newRuleValidationExecution = new RuleValidation { CollectionId = collectionId };
                _odsRawDbContext.RuleValidations.Add(newRuleValidationExecution);
                _odsRawDbContext.SaveChanges();
                #endregion Add a new execution of the Validation Engine to the ODS database, (required by the Engine) and get an ID back representing this execution.

                #region Add a new execution of the Validation Engine to the Validation database, (required by the Portal) and get an ID back representing this execution.
                newReportSummary = new ValidationReportSummary
                {
                    Collection = collectionId,
                    CompletedWhen = null,
                    ErrorCount = null,
                    WarningCount = null,
                    TotalCount = 0,
                    Id = newRuleValidationExecution.RuleValidationId,
                    EdOrgId = _appUserService.GetSession().FocusedEdOrgId,
                    SchoolYear = _schoolYearService.GetSubmittableSchoolYears().FirstOrDefault(sy => sy.StartYear == fourDigitOdsDbYear),
                    InitiatedBy = _appUserService.GetUser().FullName,
                    RequestedWhen = DateTime.UtcNow,
                    Status = "In Progress"
                };
                _dbContext.ValidationReportSummaries.Add(newReportSummary);
                _dbContext.SaveChanges();
                #endregion Add a new execution of the Validation Engine to the Validation database, (required by the Portal) and get an ID back representing this execution.

                #region Now, store each Ruleset ID and Rule ID that the engine will run. Save it in the Engine database.
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
                #endregion Now, store each Ruleset ID and Rule ID that the engine will run. Save it in the Engine database.

                #region The ValidationReportDetails is one-for-one with the ValidationReportSummary - it should be refactored away. It contains the error/warning details.
                var newReportDetails = new ValidationReportDetails
                {
                    CollectionName = collectionId,
                    DistrictName = $"{_edOrgService.GetEdOrgById(newReportSummary.EdOrgId, newReportSummary.SchoolYear.Id).OrganizationName} ({newReportSummary.EdOrgId.ToString()})",
                    ValidationReportSummaryId = newReportSummary.Id
                };
                _dbContext.ValidationReportDetails.Add(newReportDetails);
                _dbContext.SaveChanges();
                #endregion The ValidationReportDetails is one-for-one with the ValidationReportSummary - it should be refactored away. It contains the error/warning details.

                #region Execute each individual rule.
                foreach (var rule in rules)
                {
                    var detailParams = new List<SqlParameter> { new SqlParameter("@RuleValidationId", newRuleValidationExecution.RuleValidationId) };
                    detailParams.AddRange(_engineObjectModel.GetParameters(collectionId).Select(x => new SqlParameter(x.ParameterName, x.Value)));
                    _odsRawDbContext.Database.CommandTimeout = 60;
                    var result = _odsRawDbContext.Database.ExecuteSqlCommand(rule.ExecSql, detailParams.ToArray());

                    #region Record the results of this rule in the Validation Portal database, accompanied by more detailed information.
                    PopulateErrorDetailsFromViews(rule, _odsRawDbContext, newRuleValidationExecution.RuleValidationId, newReportDetails.Id);
                    #endregion Record the results of this rule in the Validation Portal database, accompanied by more detailed information.
                }
                #endregion Execute each individual rule.

                newReportSummary.CompletedWhen = DateTime.UtcNow;
                newReportSummary.ErrorCount = _odsRawDbContext.RuleValidationDetails.Where(rvd => rvd.RuleValidation.RuleValidationId == newRuleValidationExecution.RuleValidationId && rvd.IsError).Count();
                newReportSummary.WarningCount = _odsRawDbContext.RuleValidationDetails.Where(rvd => rvd.RuleValidation.RuleValidationId == newRuleValidationExecution.RuleValidationId && !rvd.IsError).Count();
                newReportSummary.Status = "Completed";

                // TODO: Fix this with real values
                newReportDetails.CompletedWhen = newReportDetails.CompletedWhen ?? DateTime.UtcNow;
                _dbContext.SaveChanges();
            }
            return newReportSummary;
        }

        public List<Collection> GetCollections()
        {
            return _engineObjectModel.Collections.ToList();
        }

        /// <summary>
        /// For a single rule, on a single execution of the Rules Engine, records the resulting errors and warnings accompanied by enhanced information.
        /// </summary>
        /// <param name="rule">The single rule whose corresponding errors and warnings will be recorded in the Validation Portal's Database.</param>
        /// <param name="rawOdsContext">A connection to the Ed Fi ODS database from which details about the error and the entity the error relates to. 
        /// Used as a source to fill in details about Individual errors and warnings.</param>
        /// <param name="rulesExecutionId">The single execution of the Rules Engine which serves as the scope of the paticular errors and warnings that will be
        /// transferred to the Validation Portal database. This ID is used by the Rules engine database.</param>
        /// <param name="reportDetailId">The single execution of the Rules Engine which serves as the scope of the paticular errors and warnings that will be
        /// transferred to the Validation Portal database. This ID is used by the Validation Portal database.</param>
        private void PopulateErrorDetailsFromViews(Rule rule, RawOdsDbContext rawOdsContext, long rulesExecutionId, int reportDetailId)
        {
            try
            {
                var errorSummaries = new List<ValidationErrorSummary>();

                // Retrieve what the Rules Engine recorded - errors or warnings for this particular rule, on this particular execution.
                var queryResults = rawOdsContext.RuleValidationDetails.Where(rvd => rvd.RuleValidationId == rulesExecutionId && rvd.RuleId == rule.RuleId);

                var conn = rawOdsContext.Database.Connection;
                try
                {
                    conn.Open();
                    var studentQueryCmd = conn.CreateCommand();
                    studentQueryCmd.CommandType = System.Data.CommandType.Text;
                    studentQueryCmd.CommandText = StudentDataFromId.StudentDataQueryFromId;
                    studentQueryCmd.Parameters.Add(new SqlParameter("@student_unique_id", System.Data.SqlDbType.NVarChar, 32));

                    foreach (var queryResult in queryResults.ToArray())
                    {
                        studentQueryCmd.Parameters["@student_unique_id"].Value = queryResult.Id.ToString();
                        var singleStudentData = new List<StudentDataFromId>();
                        using (var reader = studentQueryCmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var entryDateValue = System.Convert.IsDBNull(reader[StudentDataFromId.EntryDateColumnName]) ? (DateTime?)null : Convert.ToDateTime(reader[StudentDataFromId.EntryDateColumnName]);
                                var exitWithdrawDateValue = System.Convert.IsDBNull(reader[StudentDataFromId.ExitWithdrawDateColumnName]) ? (DateTime?)null : Convert.ToDateTime(reader[StudentDataFromId.ExitWithdrawDateColumnName]);
                                singleStudentData.Add(new StudentDataFromId
                                {
                                    EntryDate = entryDateValue,
                                    ExitWithdrawDate = exitWithdrawDateValue,
                                    FirstName = reader[StudentDataFromId.FirstNameColumnName].ToString(),
                                    GradeLevel = reader[StudentDataFromId.GradeLevelColumnName].ToString(),
                                    LastSurname = reader[StudentDataFromId.LastSurnameColumnName].ToString(),
                                    MiddleName = reader[StudentDataFromId.MiddleNameColumnName].ToString(),
                                    NameOfInstitution = reader[StudentDataFromId.NameOfInstitutionColumnName].ToString(),
                                    SchoolId = reader[StudentDataFromId.SchoolIdColumnName].ToString(),
                                });
                            }
                        }

                        // var sqlCountStatement = $"SELECT Count([Id]) FROM [rules].[{componentName}]";

                        #region Record the error (warning) with additional details taken from the ODS database.
                        errorSummaries.Add(new ValidationErrorSummary
                        {
                            StudentUniqueId = queryResult.Id.ToString(),
                            StudentFullName = StudentDataFromId.GetStudentFullName(singleStudentData),
                            SeverityId = (queryResult.IsError ? (int)ErrorSeverity.Error : (int)ErrorSeverity.Warning),
                            Component = rule.Components[0],
                            ErrorCode = rule.RuleId,
                            ErrorText = queryResult.Message,
                            ValidationReportDetailsId = reportDetailId,
                            ErrorEnrollmentDetails = new HashSet<ValidationErrorEnrollmentDetail>(singleStudentData.Select(
                                    ssd => new ValidationErrorEnrollmentDetail
                                    {
                                        School = ssd.NameOfInstitution,
                                        SchoolId = ssd.SchoolId,
                                        Grade = ssd.GradeLevel,
                                        DateEnrolled = ssd.EntryDate,
                                        DateWithdrawn = ssd.ExitWithdrawDate
                                    }
                                )
                            ),
                       });
                        #endregion Record the error (warning) with additional details taken from the ODS database.
                    }
                }
                catch (Exception ex)
                {
                    _loggingService.LogErrorMessage($"While reading student data to add to error/warning information during an execution of the validation engine, and error occurred: {ex.ChainInnerExceptionMessages()}");
                }
                finally
                {
                    if (conn != null && conn.State != System.Data.ConnectionState.Closed)
                    {
                        try
                        {
                            conn.Close();
                        }
                        catch (Exception) { }
                    }
                }
                _dbContext.ValidationErrorSummaries.AddRange(errorSummaries);
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _loggingService.LogErrorMessage($"Error when compiling details of validation error {rule.RuleId ?? string.Empty}: {ex.ChainInnerExceptionMessages()}");
            }
        }
    }
}