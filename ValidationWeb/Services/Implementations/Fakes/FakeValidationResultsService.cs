using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValidationWeb.Services
{
    public class FakeValidationResultsService : IValidationResultsService
    {
        public ValidationReportDetails GetValidationReportDetails(int validationReportId)
        {
            switch (validationReportId)
            {
                case 1:
                    return new ValidationReportDetails
                    {
                        CollectionName = "End of Year Collection",
                        CompletedWhen = new DateTime(2018, 4, 26, 15, 1, 44),
                        DistrictName = "North St. Paul-Maplewood School District",
                        ErrorSummaries = new ValidationErrorSummary[]
                        {
                            new ValidationErrorSummary
                            {
                                Component = "Student Enrollment",
                                Student = new Student { Id = "111122222", FullName = "Maria Doe" },
                                School = "Eagle Point Elementary",
                                Grade = "Four",
                                ErrorCode = "2",
                                ErrorText = "Student grade level is significantly different from expected based on age.",
                                Severity = ErrorSeverity.Warning
                            },
                            new ValidationErrorSummary
                            {
                                Component = "Student Demographics",
                                Student = new Student { Id = "333444555", FullName = "Jonathan Delacroix" },
                                School = "Eagle Point Elementary",
                                Grade = "Six",
                                ErrorCode = "42",
                                ErrorText = "Student race changed from previous years.",
                                Severity = ErrorSeverity.Warning
                            },
                            new ValidationErrorSummary
                            {
                                Component = "Student Enrollment",
                                Student = new Student { Id = "777222111", FullName = "Judy Garland" },
                                School = "Eagle Point Elementary",
                                Grade = "Seven",
                                ErrorCode = "9",
                                ErrorText = "Parent/guardian record missing.",
                                Severity = ErrorSeverity.Error
                            },
                            new ValidationErrorSummary
                            {
                                Component = "Student Enrollment",
                                Student = new Student { Id = "999555222", FullName = "John Glenn" },
                                School = "Eagle Point Elementary",
                                Grade = "Eight",
                                ErrorCode = "16",
                                ErrorText = "Student address zip code not valid.",
                                Severity = ErrorSeverity.Error
                            }
                        }
                    };
                case 2:
                    return new ValidationReportDetails
                    {
                        CollectionName = "End of Year Collection",
                        CompletedWhen = new DateTime(2018, 4, 26, 15, 1, 44),
                        DistrictName = "North St. Paul-Maplewood School District",
                        ErrorSummaries = new ValidationErrorSummary[]
                        {
                            new ValidationErrorSummary
                            {
                                Component = "Student Enrollment",
                                Student = new Student { Id = "111122222", FullName = "Maria Doe" },
                                School = "Eagle Point Elementary",
                                Grade = "Four",
                                ErrorCode = "2",
                                ErrorText = "Student grade level is significantly different from expected based on age.",
                                Severity = ErrorSeverity.Warning
                            },
                            new ValidationErrorSummary
                            {
                                Component = "Student Demographics",
                                Student = new Student { Id = "333444555", FullName = "Jonathan Delacroix" },
                                School = "Eagle Point Elementary",
                                Grade = "Six",
                                ErrorCode = "42",
                                ErrorText = "Student race changed from previous years.",
                                Severity = ErrorSeverity.Warning
                            },
                            new ValidationErrorSummary
                            {
                                Component = "Student Enrollment",
                                Student = new Student { Id = "777222111", FullName = "Judy Garland" },
                                School = "Eagle Point Elementary",
                                Grade = "Seven",
                                ErrorCode = "9",
                                ErrorText = "Parent/guardian record missing.",
                                Severity = ErrorSeverity.Error
                            },
                            new ValidationErrorSummary
                            {
                                Component = "Student Enrollment",
                                Student = new Student { Id = "999555222", FullName = "John Glenn" },
                                School = "Eagle Point Elementary",
                                Grade = "Eight",
                                ErrorCode = "16",
                                ErrorText = "Student address zip code not valid.",
                                Severity = ErrorSeverity.Error
                            }
                        }
                    };
                default:
                    return null;
            }
        }

        public List<ValidationReportSummary> GetValidationReportSummaries(string edOrgId)
        {
            if (edOrgId == "ISD 622")
            {
                return new List<ValidationReportSummary>
                {
                    new ValidationReportSummary
                    {
                        Collection = "End of Year Collection",
                        InitiatedBy = "jsmith@k12.isd622.mn.gov",
                        RequestedWhen = new DateTime(2018, 5, 1, 15, 30, 0),
                        CompletedWhen = new DateTime(2018, 5, 1, 15, 31, 25),
                        ValidationResultsId = 2,
                        Status = "Complete",
                        ErrorCount = 976,
                        WarningCount = 424
                    },
                    new ValidationReportSummary
                    {
                        Collection = "End of Year Collection",
                        InitiatedBy = "mdoe@k12.isd622.mn.gov",
                        RequestedWhen = new DateTime(2018, 4, 26, 14, 59, 0),
                        CompletedWhen = new DateTime(2018, 4, 26, 15, 1, 44),
                        ValidationResultsId = 1,
                        Status = "Complete",
                        ErrorCount = 5038,
                        WarningCount = 3726
                    }
                };
            }
            else return Enumerable.Empty<ValidationReportSummary>().ToList();
        }
    }
}