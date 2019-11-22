using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Engine.Models;

using ValidationWeb.Database;
using ValidationWeb.Database.Queries;
using ValidationWeb.Models;
using ValidationWeb.Services.Interfaces;
using ValidationWeb.Utility;

namespace ValidationWeb.Services.Implementations
{
    public class RulesEngineService : IRulesEngineService
    {
        protected readonly IRulesEngineConfigurationValues EngineConfig;

        protected readonly Model EngineObjectModel;

        protected readonly IAppUserService AppUserService;

        protected readonly IEdOrgService EdOrgService;

        protected readonly ISchoolYearService SchoolYearService;

        protected readonly ILoggingService LoggingService;

        public readonly IDbContextFactory<ValidationPortalDbContext> DbContextFactory;

        public readonly ISchoolYearDbContextFactory OdsDbContextFactory;

        private readonly IManualRuleExecutionService ManualRuleExecutionService;

        public RulesEngineService(
            IAppUserService appUserService,
            IEdOrgService edOrgService,
            ISchoolYearService schoolYearService,
            IRulesEngineConfigurationValues engineConfig,
            ILoggingService loggingService,
            IDbContextFactory<ValidationPortalDbContext> dbContextFactory,
            ISchoolYearDbContextFactory odsDbContextFactory,
            Model engineObjectModel,
            IManualRuleExecutionService manualRuleExecutionService
            )
        {
            AppUserService = appUserService;
            EdOrgService = edOrgService;
            SchoolYearService = schoolYearService;
            LoggingService = loggingService;
            EngineConfig = engineConfig;
            DbContextFactory = dbContextFactory;
            OdsDbContextFactory = odsDbContextFactory;
            EngineObjectModel = engineObjectModel;
            ManualRuleExecutionService = manualRuleExecutionService;
        }

        public ValidationReportSummary SetupValidationRun(SubmissionCycle submissionCycle, string collectionId)
        {
            if (submissionCycle?.SchoolYearId == null)
            {
                throw new ArgumentException("Submission cycle is null or contains null SchoolYearId", nameof(submissionCycle));
            }

            var schoolYear = SchoolYearService.GetSchoolYearById(submissionCycle.SchoolYearId.Value);
            string fourDigitOdsDbYear = schoolYear.EndYear;

            ValidationReportSummary newReportSummary;
            using (var odsRawDbContext = OdsDbContextFactory.CreateWithParameter(fourDigitOdsDbYear))
            {
                LoggingService.LogDebugMessage(
                    $"Connecting to the Ed Fi ODS {fourDigitOdsDbYear} to run the Rules Engine. Submitting the RulesValidation run ID.");

                // Add a new execution of the Validation Engine to the ODS database, (required by the Engine) and get an ID back representing this execution.
                var newRuleValidationExecution = new RuleValidation { CollectionId = collectionId };
                odsRawDbContext.RuleValidations.Add(newRuleValidationExecution);
                odsRawDbContext.SaveChanges();

                LoggingService.LogDebugMessage(
                    $"Successfully submitted RuleValidationId {newRuleValidationExecution.RuleValidationId.ToString()} to the Rules Engine database table.");

                // Add a new execution of the Validation Engine to the Validation database, (required by the Portal) and get an ID back representing this execution.

                /* todo: using this (Id, SchoolYearId) as a PK - this isn't reliable because it comes from the ods's id. 
                    we can stomp other execution runs from other districts etc. the ID is the identity column in another database. 
                    it doesn't know about what we're doing ... change the ID to the ods's execution id and set up our own identity column
                    that's independent (and change all references to this "id" 
                */

                newReportSummary = new ValidationReportSummary
                {
                    Collection = collectionId,
                    CompletedWhen = null,
                    ErrorCount = null,
                    WarningCount = null,
                    TotalCount = 0,
                    RuleValidationId = newRuleValidationExecution.RuleValidationId,
                    EdOrgId = AppUserService.GetSession().FocusedEdOrgId,
                    SchoolYearId = schoolYear.Id,
                    InitiatedBy = AppUserService.GetUser().FullName,
                    RequestedWhen = DateTime.UtcNow,
                    Status = "In Progress - Starting"
                };

                LoggingService.LogDebugMessage(
                    $"Successfully submitted Validation Report Summary ID {newReportSummary.ValidationReportSummaryId} " +
                    $"to the Validation Portal database for Rules Validation Run {newRuleValidationExecution.RuleValidationId.ToString()}.");
            }

            using (var validationDbContext = DbContextFactory.Create())
            {
                validationDbContext.ValidationReportSummaries.Add(newReportSummary);
                validationDbContext.SaveChanges();
            }

            return newReportSummary;
        }

        public Task RunValidationAsync(SubmissionCycle submissionCycle, long ruleValidationId)
        {
            if (submissionCycle?.SchoolYearId == null)
            {
                throw new ArgumentException("Submission cycle is null, or SchoolYearId is null", nameof(submissionCycle));
            }

            var schoolYear = SchoolYearService.GetSchoolYearById(submissionCycle.SchoolYearId.Value);
            string fourDigitOdsDbYear = schoolYear.EndYear;

            LoggingService.LogInfoMessage($"===== Starting validation run for year {fourDigitOdsDbYear}, ruleValidationId {ruleValidationId}");

            return Task.Factory
                .StartNew(() => RunValidation(submissionCycle, ruleValidationId))
                .ContinueWith(task =>
                    {
                        LoggingService.LogInfoMessage($"===== Completed validation run for year {fourDigitOdsDbYear}, ruleValidationId {ruleValidationId}");

                        if (task.Exception != null)
                        {
                            LoggingService.LogErrorMessage(task.Exception.Flatten().ChainInnerExceptionMessages());
                        }
                    });
        }

        public void RunValidation(SubmissionCycle submissionCycle, long ruleValidationId)
        {
            if (submissionCycle?.SchoolYearId == null)
            {
                throw new ArgumentException("Submission cycle is null, or SchoolYearId is null", nameof(submissionCycle));
            }

            var schoolYear = SchoolYearService.GetSchoolYearById(submissionCycle.SchoolYearId.Value);
            var fourDigitOdsDbYear = schoolYear.EndYear;

            using (var odsRawDbContext = OdsDbContextFactory.CreateWithParameter(fourDigitOdsDbYear))
            {
                using (var validationDbContext = DbContextFactory.Create())
                {
                    var newReportSummary = validationDbContext
                        .ValidationReportSummaries
                        .Include(x => x.SchoolYear)
                        .FirstOrDefault(x =>
                            x.ValidationReportSummaryId == ruleValidationId &&
                            x.SchoolYearId == schoolYear.Id);

                    if (newReportSummary == null)
                    {
                        throw new InvalidOperationException($"Unable to find report summary {ruleValidationId} for year {schoolYear.Id}");
                    }

                    var newRuleValidationExecution = odsRawDbContext.RuleValidations
                        .FirstOrDefault(x =>
                            x.RuleValidationId == newReportSummary.RuleValidationId);

                    if (newRuleValidationExecution == null)
                    {
                        throw new InvalidOperationException($"Unable to find execution {ruleValidationId}");
                    }

                    var collectionId = newRuleValidationExecution.CollectionId;

                    // todo: check collectionId for null/empty and why is it a string!? 

                    // Now, store each Ruleset ID and Rule ID that the engine will run. Save it in the Engine database.
                    LoggingService.LogDebugMessage($"Getting the rules to run for the chosen collection {collectionId}.");
                    var rules = EngineObjectModel.GetRules(collectionId).ToArray();
                    var ruleComponents = rules.SelectMany(
                        r => r.Components.Distinct().Select(
                            c => new
                            {
                                r.RulesetId,
                                r.RuleId,
                                Component = c
                            }));

                    foreach (var singleRuleNeedingToBeValidated in ruleComponents)
                    {
                        odsRawDbContext.RuleValidationRuleComponents.Add(
                            new RuleValidationRuleComponent
                            {
                                RuleValidationId = newRuleValidationExecution.RuleValidationId,
                                RulesetId = singleRuleNeedingToBeValidated.RulesetId,
                                RuleId = singleRuleNeedingToBeValidated.RuleId,
                                Component = singleRuleNeedingToBeValidated.Component
                            });
                    }

                    odsRawDbContext.SaveChanges();
                    LoggingService.LogDebugMessage($"Saved the rules to run for the chosen collection {collectionId}.");

                    // The ValidationReportDetails is one-for-one with the ValidationReportSummary - it should be refactored away. It contains the error/warning details.
                    LoggingService.LogDebugMessage(
                        $"Adding additional Validation Report details to the Validation Portal database for EdOrgID {newReportSummary.EdOrgId}.");

                    var newReportDetails = new ValidationReportDetails
                    {
                        CollectionName = collectionId,
                        SchoolYearId = newReportSummary.SchoolYear.Id,
                        DistrictName = $"{EdOrgService.GetEdOrgById(newReportSummary.EdOrgId, newReportSummary.SchoolYear.Id).OrganizationName} ({newReportSummary.EdOrgId.ToString()})",
                        ValidationReportSummaryId = newReportSummary.ValidationReportSummaryId
                    };
                    validationDbContext.ValidationReportDetails.Add(newReportDetails);
                    try
                    {
                        validationDbContext.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        LoggingService.LogErrorMessage(ex.ChainInnerExceptionMessages());
                    }

                    LoggingService.LogDebugMessage(
                        $"Successfully added additional Validation Report details to the Validation Portal database for EdOrgID {newReportSummary.EdOrgId}.");

                    // Execute each individual rule.
                    List<RulesEngineExecutionException> rulesEngineExecutionExceptions = new List<RulesEngineExecutionException>();

                    for (var i = 0; i < rules.Length; i++)
                    {
                        var rule = rules[i];

                        try
                        {
                            //Execute the SQL files in here? We have the RuleSetName and the Rule Id
                            //e.g.  RuleSetId = MultipleEnrollment
                            //      RuleId = 10.10.6175
                            var toExecute = ManualRuleExecutionService.GetManualSqlFile(rule.RulesetId, rule.RuleId);


                            // By default, rules are run against ALL districts in the Ed Fi ODS. This line filters for multi-district/multi-tenant ODS's.
                            rule.AddDistrictWhereFilter(newReportSummary.EdOrgId);

                            LoggingService.LogDebugMessage($"Executing Rule {rule.RuleId}.");
                            LoggingService.LogDebugMessage($"Executing Rule SQL {rule.Sql}.");

                            var detailParams = new List<SqlParameter>
                                           {
                                               new SqlParameter(
                                                   "@RuleValidationId",
                                                   newRuleValidationExecution.RuleValidationId)
                                           };

                            detailParams.AddRange(
                                EngineObjectModel.GetParameters(collectionId)
                                    .Select(x => new SqlParameter(x.ParameterName, x.Value)));
                            
                            odsRawDbContext.Database.CommandTimeout = EngineConfig.RulesExecutionTimeout;

                            var result = odsRawDbContext.Database.ExecuteSqlCommand(rule.ExecSql, detailParams.ToArray<object>());

                            LoggingService.LogDebugMessage($"Executing Rule {rule.RuleId} rows affected = {result}.");

                            // Record the results of this rule in the Validation Portal database, accompanied by more detailed information.
                            PopulateErrorDetailsFromViews(
                                rule,
                                odsRawDbContext,
                                newRuleValidationExecution.RuleValidationId,
                                newReportDetails.Id);

                            newReportSummary.Status = $"In Progress - {(int)((float)i / rules.Length * 100)}% complete";
                            validationDbContext.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            rulesEngineExecutionExceptions.Add(
                                new RulesEngineExecutionException
                                {
                                    RuleId = rule.RuleId,
                                    Sql = rule.Sql,
                                    ExecSql = rule.ExecSql,
                                    DataSourceName =
                                        $"Database Server: {odsRawDbContext.Database.Connection.DataSource}{Environment.NewLine} " +
                                        $"Database: {odsRawDbContext.Database.Connection.Database}",
                                    ChainedErrorMessages = ex.ChainInnerExceptionMessages()
                                });
                        }
                    }

                    LoggingService.LogDebugMessage("Counting errors and warnings.");
                    newReportSummary.CompletedWhen = DateTime.UtcNow;

                    newReportSummary.ErrorCount = odsRawDbContext.RuleValidationDetails.Count(
                        rvd => rvd.RuleValidation.RuleValidationId == newRuleValidationExecution.RuleValidationId
                               && rvd.IsError);

                    newReportSummary.WarningCount = odsRawDbContext.RuleValidationDetails.Count(
                        rvd => rvd.RuleValidation.RuleValidationId == newRuleValidationExecution.RuleValidationId
                               && !rvd.IsError);

                    var hasExecutionErrors = rulesEngineExecutionExceptions.Count > 0;
                    newReportSummary.Status = hasExecutionErrors
                                                  ? $"Completed - {rulesEngineExecutionExceptions.Count} rules did not execute, ask an administrator to check the log for errors, Report Summary Number {newReportSummary.ValidationReportSummaryId.ToString()}"
                                                  : "Completed";
                    LoggingService.LogDebugMessage($"Saving status {newReportSummary.Status}.");

                    // Log Execution Errors
                    LoggingService.LogErrorMessage(
                        GetLogExecutionErrorsMessage(rulesEngineExecutionExceptions, newReportSummary.ValidationReportSummaryId));

                    newReportDetails.CompletedWhen = newReportDetails.CompletedWhen ?? DateTime.UtcNow;
                    validationDbContext.SaveChanges();
                    LoggingService.LogDebugMessage("Saved status.");
                }
            }
        }

        protected string GetLogExecutionErrorsMessage(IList<RulesEngineExecutionException> rulesEngineExecutionExceptions, long reportId)
        {
            var logMessageBuilder = new StringBuilder();
            logMessageBuilder.AppendLine("=================================================");
            logMessageBuilder.AppendLine($"Rules Engine Execution Errors Reported for Validation Report Summary # {reportId.ToString()}:");
            foreach (var execError in rulesEngineExecutionExceptions)
            {
                logMessageBuilder.AppendLine();
                logMessageBuilder.AppendLine($"Rule ID: {execError.RuleId ?? "null"}");
                logMessageBuilder.AppendLine($"Server and Database: {execError.DataSourceName ?? "null"}");
                logMessageBuilder.AppendLine($"SQL: {execError.Sql ?? "null"}");
                logMessageBuilder.AppendLine($"Executed SQL: {execError.ExecSql ?? "null"}");
                logMessageBuilder.AppendLine("Chained Error Message and Stack Trace:");
                logMessageBuilder.AppendLine(execError.ChainedErrorMessages ?? "null");
            }

            logMessageBuilder.AppendLine("=================================================");
            return logMessageBuilder.ToString();
        }

        public List<Collection> GetCollections()
        {
            return EngineObjectModel.Collections.ToList();
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
            LoggingService.LogDebugMessage($"Preparing to populate the error information for rule {rule.RuleId} to the Validation Portal database.");
            try
            {
                var errorSummaries = new List<ValidationErrorSummary>();

                // Retrieve what the Rules Engine recorded - errors or warnings for this particular rule, on this particular execution.
                var queryResults = rawOdsContext.RuleValidationDetails.Where(rvd => rvd.RuleValidationId == rulesExecutionId && rvd.RuleId == rule.RuleId);
                LoggingService.LogDebugMessage($"Successfully retrieved results for rule {rule.RuleId} from the Ed Fi ODS database Rules Validation tables. Retrieving additional information.");
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
                        studentQueryCmd.Parameters["@student_unique_id"].Value = queryResult.Id.ToString("D13");
                        var singleStudentData = new List<StudentDataFromId>();
                        using (var reader = studentQueryCmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var entryDateValue = Convert.IsDBNull(reader[StudentDataFromId.EntryDateColumnName]) ? (DateTime?)null : Convert.ToDateTime(reader[StudentDataFromId.EntryDateColumnName]);
                                var exitWithdrawDateValue = Convert.IsDBNull(reader[StudentDataFromId.ExitWithdrawDateColumnName]) ? (DateTime?)null : Convert.ToDateTime(reader[StudentDataFromId.ExitWithdrawDateColumnName]);
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

                        LoggingService.LogDebugMessage($"Additional info for one student error record retrieved from the ODS Rules Engine database table {rule.RuleId}.");

                        // Record the error (warning) with additional details taken from the ODS database.
                        errorSummaries.Add(new ValidationErrorSummary
                        {
                            StudentUniqueId = queryResult.Id.ToString(), // .ToString("D13"),
                            StudentFullName = StudentDataFromId.GetStudentFullName(singleStudentData),
                            SeverityId = queryResult.IsError ? (int)ErrorSeverity.Error : (int)ErrorSeverity.Warning,
                            Component = rule.Components[0],
                            ErrorCode = rule.RuleId,
                            ErrorText = queryResult.Message,
                            ValidationReportDetailsId = reportDetailId,
                            ErrorEnrollmentDetails = new HashSet<ValidationErrorEnrollmentDetail>(
                                singleStudentData.Select(
                                    ssd => new ValidationErrorEnrollmentDetail
                                    {
                                        School = ssd.NameOfInstitution,
                                        SchoolId = ssd.SchoolId,
                                        Grade = ssd.GradeLevel,
                                        DateEnrolled = ssd.EntryDate,
                                        DateWithdrawn = ssd.ExitWithdrawDate
                                    }))
                        });

                        LoggingService.LogDebugMessage("A record was added to the Validation Portal, but not yet committed.");
                    }
                }
                catch (Exception ex)
                {
                    LoggingService.LogErrorMessage($"While reading student data to add to error/warning information during an execution of the validation engine, and error occurred: {ex.ChainInnerExceptionMessages()}");
                }
                finally
                {
                    if (conn != null && conn.State != System.Data.ConnectionState.Closed)
                    {
                        conn.Close();
                    }
                }

                using (var validationDbContext = DbContextFactory.Create())
                {
                    validationDbContext.ValidationErrorSummaries.AddRange(errorSummaries);
                    validationDbContext.SaveChanges();
                    LoggingService.LogDebugMessage(
                        $"Successfully committed all additional error information for students found with issues referring to rule {rule.RuleId}.");
                }
            }
            catch (Exception ex)
            {
                LoggingService.LogErrorMessage($"Error when compiling details of validation error {rule.RuleId ?? string.Empty}: {ex.ChainInnerExceptionMessages()}");
            }
        }
    }
}