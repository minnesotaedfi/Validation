using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using ValidationWeb.Database;
using ValidationWeb.Models;
using ValidationWeb.Services.Interfaces;
using ValidationWeb.Utility;

namespace ValidationWeb.Services.Implementations
{
    public class RecordsRequestService : IRecordsRequestService
    {
        public readonly ILoggingService LoggingService;

        public readonly IDbContextFactory<ValidationPortalDbContext> DbContextFactory;

        public RecordsRequestService(
            ILoggingService loggingService,
            IDbContextFactory<ValidationPortalDbContext> dbContextFactory)
        {
            LoggingService = loggingService;
            DbContextFactory = dbContextFactory;
        }

        public IEnumerable<RecordsRequest> GetAllRecordsRequests()
        {
            using (var dbContext = DbContextFactory.Create())
            {
                try
                {
                    return dbContext.RecordsRequests.ToList();
                }
                catch (Exception ex)
                {
                    LoggingService.LogErrorMessage($"Unable to retrieve Records Request data: {ex.ChainInnerExceptionMessages()}");
                    throw;
                }
            }
        }

        public RecordsRequest GetRecordsRequestData(int schoolYearId, int edOrgId, string studentId)
        {
            using (var dbContext = DbContextFactory.Create())
            {
                try
                {
                    var studentRecord = dbContext.RecordsRequests.FirstOrDefault(x =>
                                            x.StudentId == studentId &&
                                            x.SchoolYearId == schoolYearId)
                                        ?? new RecordsRequest
                                        {
                                            StudentId = studentId,
                                            SchoolYearId = schoolYearId,
                                            RequestingDistrict = edOrgId
                                        };

                    return studentRecord;
                }
                catch (Exception ex)
                {
                    LoggingService.LogErrorMessage($"Unable to retrieve Records Request data: {ex.ChainInnerExceptionMessages()}");
                    throw;
                }
            }
        }

        public void SaveRecordsRequest(int schoolYearId, RecordsRequestFormData formData)
        {
            using (var dbContext = DbContextFactory.Create())
            {
                try
                {
                    var studentRecord = dbContext.RecordsRequests.FirstOrDefault(x =>
                                            x.StudentId == formData.StudentId &&
                                            x.SchoolYearId == schoolYearId)
                                        ?? new RecordsRequest();

                    studentRecord.StudentId = formData.StudentId;
                    studentRecord.RespondingDistrict = int.Parse(formData.RespondingDistrictId);
                    studentRecord.RequestingDistrict = int.Parse(formData.RequestingDistrictId);
                    studentRecord.RequestingUser = formData.RequestingUserId;
                    studentRecord.TransmittalInstructions = formData.TransmittalInstructions;
                    studentRecord.SchoolYearId = schoolYearId;

                    var requests = new List<KeyValuePair<RecordsRequestDetail, bool>>
                                    {
                                        new KeyValuePair<RecordsRequestDetail, bool>(studentRecord.AssessmentResults, formData.CheckAssessment),
                                        new KeyValuePair<RecordsRequestDetail, bool>(studentRecord.CumulativeFiles, formData.CheckCumulative),
                                        new KeyValuePair<RecordsRequestDetail, bool>(studentRecord.DisciplineRecords, formData.CheckDiscipline),
                                        new KeyValuePair<RecordsRequestDetail, bool>(studentRecord.EvaluationSummary, formData.CheckEvaluation),
                                        new KeyValuePair<RecordsRequestDetail, bool>(studentRecord.IEP, formData.CheckIEP),
                                        new KeyValuePair<RecordsRequestDetail, bool>(studentRecord.Immunizations, formData.CheckImmunization)
                                    };

                    foreach (var pair in requests)
                    {
                        if (pair.Value)
                        {
                            pair.Key.Requested = true;
                            pair.Key.RequestingUserId = studentRecord.RequestingUser;
                            pair.Key.RequestingDistrictId = studentRecord.RequestingDistrict;
                        }
                    }

                    UpdateRecordsRequestStatus(studentRecord);

                    if (studentRecord.Id == 0)
                    {
                        dbContext.RecordsRequests.Add(studentRecord);
                    }
                    else
                    {
                        dbContext.Entry(studentRecord).CurrentValues.SetValues(studentRecord);
                    }

                    dbContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    LoggingService.LogErrorMessage($"Unable to retrieve Records Request data: {ex.ChainInnerExceptionMessages()}");
                    throw;
                }
            }
        }

        public void SaveRecordsResponse(int schoolYearId, RecordsResponseFormData formData)
        {
            using (var dbContext = DbContextFactory.Create())
            {
                try
                {
                    RecordsRequest studentRecord = dbContext.RecordsRequests.FirstOrDefault(x =>
                        x.StudentId == formData.StudentId &&
                        x.SchoolYearId == schoolYearId);

                    if (studentRecord == null)
                    {
                        throw new InvalidOperationException($"Unable to find record request for student ID {formData.StudentId}");
                    }

                    var responses = new List<KeyValuePair<RecordsRequestDetail, bool>>
                                    {
                                        new KeyValuePair<RecordsRequestDetail, bool>(studentRecord.AssessmentResults, formData.CheckAssessment),
                                        new KeyValuePair<RecordsRequestDetail, bool>(studentRecord.CumulativeFiles, formData.CheckCumulative),
                                        new KeyValuePair<RecordsRequestDetail, bool>(studentRecord.DisciplineRecords, formData.CheckDiscipline),
                                        new KeyValuePair<RecordsRequestDetail, bool>(studentRecord.EvaluationSummary, formData.CheckEvaluation),
                                        new KeyValuePair<RecordsRequestDetail, bool>(studentRecord.IEP, formData.CheckIEP),
                                        new KeyValuePair<RecordsRequestDetail, bool>(studentRecord.Immunizations, formData.CheckImmunization)
                                    };

                    foreach (var pair in responses)
                    {
                        if (pair.Value)
                        {
                            pair.Key.Sent = true;
                            pair.Key.RespondingUserId = studentRecord.RespondingUser;
                            pair.Key.RespondingDistrictId = studentRecord.RespondingDistrict;
                        }
                    }

                    UpdateRecordsRequestStatus(studentRecord);

                    dbContext.Entry(studentRecord).CurrentValues.SetValues(studentRecord);
                    dbContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    LoggingService.LogErrorMessage($"Unable to retrieve Records Request data: {ex.ChainInnerExceptionMessages()}");
                    throw;
                }
            }
        }

        public void UpdateRecordsRequestStatus(RecordsRequest recordsRequest)
        {
            var details = new List<RecordsRequestDetail>(
                new[]
                {
                    recordsRequest.DisciplineRecords,
                    recordsRequest.EvaluationSummary,
                    recordsRequest.IEP,
                    recordsRequest.AssessmentResults,
                    recordsRequest.CumulativeFiles,
                    recordsRequest.Immunizations
                });

            if (details.Any(x => x.Requested))
            {
                recordsRequest.Status = RecordsRequestStatus.Requested;

                if (details.Any(x => x.Requested && !x.Sent) && details.Any(x => x.Requested && x.Sent))
                {
                    recordsRequest.Status = RecordsRequestStatus.PartialResponse;
                }

                if (details.All(x => !x.Requested || (x.Requested && x.Sent)))
                {
                    recordsRequest.Status = RecordsRequestStatus.ResponseResolved;
                }
            }
        }
    }
}