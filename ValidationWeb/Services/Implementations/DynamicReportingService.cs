using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;

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
            ISchoolYearService schoolYearService)
        {
            ValidationPortalDataContextFactory = validationPortalDataContextFactory;
            SchoolYearDbContextFactory = schoolYearDbContextFactory;
            SchoolYearService = schoolYearService;
        }

        protected IDbContextFactory<ValidationPortalDbContext> ValidationPortalDataContextFactory { get; set; }

        protected ISchoolYearDbContextFactory SchoolYearDbContextFactory { get; set; }

        protected ISchoolYearService SchoolYearService { get; set; }

        public IEnumerable<DynamicReportDefinition> GetReportDefinitions()
        {
            using (var validationPortalContext = ValidationPortalDataContextFactory.Create())
            {
                return validationPortalContext.DynamicReportDefinitions
                    .Include(x => x.Fields)
                    .Include(x => x.Fields.Select(y => y.Field))
                    .Include(x => x.SchoolYear)
                    .Include(x => x.RulesView)
                    .ToList();
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
                    .Single(x => x.Id == id);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Security",
            "CA2100:Review SQL queries for security vulnerabilities",
            Justification = "Values are taken from the database schema, not user input")]
        public IList<dynamic> GetReportData(DynamicReportRequest request, int districtId)
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
                queryCommand.CommandType = System.Data.CommandType.Text;
                queryCommand.CommandText = $"SELECT {fieldNames} FROM {viewName}";

                if (selectedFields.Any(x =>
                    x.Field.Name.Equals("DistrictId", StringComparison.InvariantCultureIgnoreCase)))
                {
                    queryCommand.CommandText += $" where [DistrictId] = {districtId}";
                }

                using (var reader = queryCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var dynamicObject = new ExpandoObject() as IDictionary<string, object>;

                        foreach (var field in selectedFields)
                        {
                            var fieldDescription = !string.IsNullOrWhiteSpace(field.Description) ? field.Description : field.Field.Name;
                            dynamicObject.Add(fieldDescription, reader[field.Field.Name].ToString());
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
                queryCommand.CommandText = @"select OBJECT_SCHEMA_NAME(v.object_id) [Schema], v.name [Name] FROM sys.views as v where OBJECT_SCHEMA_NAME(v.object_id)=@schemaName";
                queryCommand.Parameters.Add(new SqlParameter("@schemaName", "rules"));

                using (var reader = queryCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var rulesView = new ValidationRulesView
                        {
                            Enabled = true,
                            Schema = reader["Schema"].ToString(),
                            Name = reader["Name"].ToString(),
                            SchoolYearId = schoolYearId
                        };

                        viewsForYear.Add(rulesView);
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
