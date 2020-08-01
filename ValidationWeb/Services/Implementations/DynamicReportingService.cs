using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;

using Validation.DataModels;

using ValidationWeb.Database;
using ValidationWeb.Models;
using ValidationWeb.Services.Interfaces;

namespace ValidationWeb.Services.Implementations
{
    public class DynamicReportingService : IDynamicReportingService
    {
        public DynamicReportingService(
            IDbContextFactory<ValidationPortalDbContext> validationPortalDataContextFactory,
            ISchoolYearDbContextFactory schoolYearDbContextFactory,
            ISchoolYearService schoolYearService,
            ILoggingService loggingService,
            IRulesEngineConfigurationValues rulesEngineConfigurationValues)
        {
            ValidationPortalDataContextFactory = validationPortalDataContextFactory;
            SchoolYearDbContextFactory = schoolYearDbContextFactory;
            SchoolYearService = schoolYearService;
            LoggingService = loggingService;
            RulesEngineConfigurationValues = rulesEngineConfigurationValues;
        }

        private IDbContextFactory<ValidationPortalDbContext> ValidationPortalDataContextFactory { get; }
        private ISchoolYearDbContextFactory SchoolYearDbContextFactory { get; }
        private ISchoolYearService SchoolYearService { get; }
        private ILoggingService LoggingService { get; }
        private IRulesEngineConfigurationValues RulesEngineConfigurationValues { get; }

        public IEnumerable<DynamicReportDefinition> GetReportDefinitions(ProgramArea programArea = null)
        {
            using (var validationPortalContext = ValidationPortalDataContextFactory.Create())
            {
                var result = validationPortalContext.DynamicReportDefinitions
                    .Include(x => x.Fields)
                    .Include(x => x.Fields.Select(y => y.Field))
                    .Include(x => x.SchoolYear)
                    .Include(x => x.RulesView)
                    .Include(x => x.ProgramArea)
                    .ToList();

                if (programArea != null)
                {
                    result = result.Where(x => x.ProgramAreaId == programArea.Id).ToList();
                }

                return result;
            }
        }

        public DynamicReportDefinition GetReportDefinition(int id)
        {
            using (var validationPortalContext = ValidationPortalDataContextFactory.Create())
            {
                return validationPortalContext.DynamicReportDefinitions
                    .Include(x => x.RulesView)
                    .Include(x => x.Fields)
                    .Include(x => x.Fields.Select(y => y.Field))
                    .Include(x => x.SchoolYear)
                    .Include(x => x.ProgramArea)
                    .Single(x => x.Id == id);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Security",
            "CA2100:Review SQL queries for security vulnerabilities",
            Justification = "Values are taken from the database schema, not user input")]
        public IList<dynamic> GetReportData(DynamicReportRequest request, int? districtId)
        {
            var reportDefinition = GetReportDefinition(request.ReportDefinitionId);

            // todo: implement user ordering, this at least mimics the admin definition screen 
            var selectedFields = reportDefinition.Fields
                .Where(x => request.SelectedFields.Contains(x.Id.ToString()))
                .OrderBy(x => x.Field.Id)
                .ToList();

            var viewName = $"[{reportDefinition.RulesView.Schema}].[{reportDefinition.RulesView.Name}]";
            var fieldNames = string.Join(", ", selectedFields.Select(x => $"[{x.Field.Name}]"));

            var report = new List<dynamic>();

            using (var schoolYearContext = SchoolYearDbContextFactory.CreateWithParameter(reportDefinition.SchoolYear.EndYear))
            {
                var connection = schoolYearContext.Database.Connection;
                connection.Open();

                var queryCommand = connection.CreateCommand();
                queryCommand.CommandTimeout = 500;
                queryCommand.CommandType = System.Data.CommandType.Text;
                queryCommand.CommandText = $"SELECT {fieldNames} FROM {viewName}";

                if (!reportDefinition.IsOrgLevelReport || districtId.HasValue)
                {
                    queryCommand.CommandText += $" where [DistrictId] = {districtId}";
                }

                LoggingService.LogInfoMessage($"Executing dynamic report SQL: {queryCommand.CommandText}");
                using (var reader = queryCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var dynamicObject = new ExpandoObject() as IDictionary<string, object>;

                        foreach (var field in selectedFields)
                        {
                            var fieldDescription = !string.IsNullOrWhiteSpace(field.Description) ? field.Description : field.Field.Name;
                            dynamicObject.Add(fieldDescription, reader[field.Field.Name]);
                        }

                        report.Add(dynamicObject);
                    }
                }
            }

            return report;
        }

        public void EnableReportDefinition(int id)
        {
            using (var validationPortalContext = ValidationPortalDataContextFactory.Create())
            {
                var report = validationPortalContext.DynamicReportDefinitions
                    .SingleOrDefault(x => x.Id == id);

                if (report == null)
                {
                    throw new InvalidOperationException($"Unable to find a report definition with id {id}");
                }

                report.Enabled = true;
                validationPortalContext.SaveChanges();
            }
        }

        public void DisableReportDefinition(int id)
        {
            using (var validationPortalContext = ValidationPortalDataContextFactory.Create())
            {
                var report = validationPortalContext.DynamicReportDefinitions
                    .SingleOrDefault(x => x.Id == id);

                if (report == null)
                {
                    throw new InvalidOperationException($"Unable to find a report definition with id {id}");
                }

                report.Enabled = false;
                validationPortalContext.SaveChanges();
            }
        }

        public void DeleteReportDefinition(int id)
        {
            using (var validationPortalContext = ValidationPortalDataContextFactory.Create())
            {
                var report = validationPortalContext.DynamicReportDefinitions
                    .Include(x => x.Fields)
                    .SingleOrDefault(x => x.Id == id);

                if (report == null)
                {
                    throw new InvalidOperationException($"Unable to find a report definition with id {id}");
                }

                validationPortalContext.DynamicReportFields.RemoveRange(report.Fields);
                validationPortalContext.DynamicReportDefinitions.Remove(report);
                validationPortalContext.SaveChanges();
            }
        }

        public void SaveReportDefinition(DynamicReportDefinition reportDefinition)
        {
            using (var validationPortalContext = ValidationPortalDataContextFactory.Create())
            {
                validationPortalContext.DynamicReportDefinitions.AddOrUpdate(reportDefinition);
                validationPortalContext.SaveChanges();
            }
        }

        public void UpdateReportDefinition(DynamicReportDefinition newReportDefinition)
        {
            using (var validationPortalContext = ValidationPortalDataContextFactory.Create())
            {
                var existingReportDefinition = validationPortalContext.DynamicReportDefinitions
                    .Include(x => x.Fields)
                    .Single(x => x.Id == newReportDefinition.Id);

                existingReportDefinition.Name = newReportDefinition.Name?.Trim();
                existingReportDefinition.Description = newReportDefinition.Description?.Trim();
                existingReportDefinition.IsOrgLevelReport = newReportDefinition.IsOrgLevelReport;
                existingReportDefinition.ProgramAreaId = newReportDefinition.ProgramAreaId;

                foreach (var newField in newReportDefinition.Fields)
                {
                    var existingField = existingReportDefinition.Fields.Single(x => x.Id == newField.Id);
                    existingField.Enabled = newField.Enabled;
                    existingField.Description = newField.Description?.Trim();
                }

                validationPortalContext.SaveChanges();
            }
        }

        public IEnumerable<ValidationRulesView> GetRulesViews(int schoolYearId)
        {
            using (var validationPortalContext = ValidationPortalDataContextFactory.Create())
            {
                return validationPortalContext.ValidationRulesViews
                    .Include(x => x.RulesFields)
                    .Where(x => x.SchoolYearId == schoolYearId)
                    .ToList();
            }
        }

        public void DeleteViewsAndRulesForSchoolYear(int schoolYearId)
        {
            using (var validationPortalContext = ValidationPortalDataContextFactory.Create())
            {
                var existingViews = validationPortalContext.ValidationRulesViews
                    .Include(x => x.RulesFields)
                    .Where(x => x.SchoolYearId == schoolYearId);

                validationPortalContext.ValidationRulesViews.RemoveRange(existingViews);
                validationPortalContext.SaveChanges();
            }
        }

        public void UpdateViewsAndRulesForSchoolYear(int schoolYearId)
        {
            DeleteViewsAndRulesForSchoolYear(schoolYearId);

            var viewsForYear = new List<ValidationRulesView>();

            var schoolYear = SchoolYearService.GetSchoolYearById(schoolYearId);
            using (var schoolYearContext = SchoolYearDbContextFactory.CreateWithParameter(schoolYear.EndYear))
            {
                var connection = schoolYearContext.Database.Connection;
                connection.Open();

                var queryCommand = connection.CreateCommand();
                queryCommand.CommandType = System.Data.CommandType.Text;
                queryCommand.CommandText = @"select OBJECT_SCHEMA_NAME(t.object_id) [Schema], t.name [Name] FROM sys.tables as t where OBJECT_SCHEMA_NAME(t.object_id)=@schemaName";
                queryCommand.Parameters.Add(new SqlParameter("@schemaName", "rules"));

                using (var reader = queryCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // check for exclusions here and just continue 
                        var tableName = reader["Name"].ToString();
                        if (!RulesEngineConfigurationValues.RulesTableExclusions.Contains(
                                tableName,
                                StringComparer.OrdinalIgnoreCase))
                        {
                            var rulesView = new ValidationRulesView
                            {
                                Enabled = true,
                                Schema = reader["Schema"].ToString(),
                                Name = tableName,
                                SchoolYearId = schoolYearId
                            };

                            viewsForYear.Add(rulesView);
                        }
                    }
                }
            }

            using (var validationPortalContext = ValidationPortalDataContextFactory.Create())
            {
                foreach (var view in viewsForYear)
                {
                    var fieldNames = GetFieldsForView(schoolYear, view.Schema, view.Name);
                    foreach (var fieldName in fieldNames)
                    {
                        var rulesField = new ValidationRulesField { Enabled = true, Name = fieldName };
                        validationPortalContext.ValidationRulesFields.Add(rulesField);
                        view.RulesFields.Add(rulesField);
                    }
                }

                validationPortalContext.ValidationRulesViews.AddRange(viewsForYear);
                validationPortalContext.SaveChanges();
            }
        }

        protected IEnumerable<string> GetFieldsForView(SchoolYear schoolYear, string schema, string name)
        {
            var fieldsForView = new List<string>();

            using (var schoolYearContext = SchoolYearDbContextFactory.CreateWithParameter(schoolYear.EndYear))
            {
                var connection = schoolYearContext.Database.Connection;
                connection.Open();

                var queryCommand = connection.CreateCommand();
                queryCommand.CommandType = System.Data.CommandType.Text;
                queryCommand.CommandText =
                    @"SELECT COLUMN_NAME FROM information_schema.columns WHERE TABLE_SCHEMA=@schemaName and table_name = @viewName";

                queryCommand.Parameters.Add(new SqlParameter("@schemaName", schema));
                queryCommand.Parameters.Add(new SqlParameter("@viewName", name));

                using (var reader = queryCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        fieldsForView.Add(reader["COLUMN_NAME"].ToString());
                    }
                }
            }

            return fieldsForView;
        }
    }
}
