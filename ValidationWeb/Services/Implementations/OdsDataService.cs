namespace ValidationWeb.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Data.Entity.Infrastructure;
    using System.Data.SqlClient;
    using System.Linq;

    using ValidationWeb.Models;
    using ValidationWeb.Utility;

    public class OdsDataService : IOdsDataService
    {
        public readonly ILoggingService LoggingService;

        public readonly IDbContextFactory<ValidationPortalDbContext> DbContextFactory;

        public OdsDataService(
            ILoggingService loggingService, 
            IDbContextFactory<ValidationPortalDbContext> dbContextFactory)
        {
            LoggingService = loggingService;
            DbContextFactory = dbContextFactory;
        }

        public List<DemographicsCountReportQuery> GetDistrictAncestryRaceCounts(int? districtEdOrgId, string fourDigitOdsDbYear)
        {
            // todo: dependency-inject these data contexts
            using (var rawOdsContext = new RawOdsDbContext(fourDigitOdsDbYear))
            {
                var conn = rawOdsContext.Database.Connection;
                try
                {
                    conn.Open();
                    var ancestryQueryCmd = conn.CreateCommand();
                    ancestryQueryCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    ancestryQueryCmd.CommandText = DemographicsCountReportQuery.DistrictAncestryRaceCountsQuery;
                    ancestryQueryCmd.Parameters.Add(new SqlParameter("@distid", System.Data.SqlDbType.Int));
                    ancestryQueryCmd.Parameters["@distid"].Value = districtEdOrgId.HasValue ? (object)districtEdOrgId.Value : (object)DBNull.Value;
                    var reportData = new List<DemographicsCountReportQuery>();
                    using (var reader = ancestryQueryCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int? theEdOrgValue = null;
                            var edOrgIdObj = reader[DemographicsCountReportQuery.EdOrgIdColumnName];
                            if (!(edOrgIdObj is DBNull))
                            {
                                theEdOrgValue = System.Convert.ToInt32(reader[DemographicsCountReportQuery.EdOrgIdColumnName]);
                            }
                            reportData.Add(new DemographicsCountReportQuery
                            {
                                OrgType = (OrgType)(System.Convert.ToInt32(reader[DemographicsCountReportQuery.OrgTypeColumnName])),
                                EdOrgId = theEdOrgValue,
                                LEASchool = reader[DemographicsCountReportQuery.LEASchoolColumnName].ToString(),
                                EnrollmentCount = System.Convert.ToInt32(reader[DemographicsCountReportQuery.DistinctEnrollmentCountColumnName]),
                                DemographicsCount = System.Convert.ToInt32(reader[DemographicsCountReportQuery.DistinctDemographicsCountColumnName]),
                                AncestryGivenCount = System.Convert.ToInt32(reader[DemographicsCountReportQuery.AncestryGivenCountColumnName]),
                                RaceGivenCount = System.Convert.ToInt32(reader[DemographicsCountReportQuery.RaceGivenCountColumnName])
                            });
                        }
                    }
                    return reportData;
                }
                catch (Exception ex)
                {
                    LoggingService.LogErrorMessage($"While compiling report on ancestry/ethnic origin supplied counts: {ex.ChainInnerExceptionMessages()}");
                    throw;
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
            }
        }

        public List<MultipleEnrollmentsCountReportQuery> GetMultipleEnrollmentCounts(int? districtEdOrgId, string fourDigitOdsDbYear)
        {
            using (var rawOdsContext = new RawOdsDbContext(fourDigitOdsDbYear))
            {
                var conn = rawOdsContext.Database.Connection;
                try
                {
                    conn.Open();
                    var multipleEnrolledQueryCmd = conn.CreateCommand();
                    multipleEnrolledQueryCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    multipleEnrolledQueryCmd.CommandText = MultipleEnrollmentsCountReportQuery.MultipleEnrollmentsCountQuery;
                    multipleEnrolledQueryCmd.Parameters.Add(new SqlParameter("@distid", System.Data.SqlDbType.Int));
                    multipleEnrolledQueryCmd.Parameters["@distid"].Value = districtEdOrgId.HasValue ? (object)districtEdOrgId.Value : (object)DBNull.Value;
                    var reportData = new List<MultipleEnrollmentsCountReportQuery>();
                    using (var reader = multipleEnrolledQueryCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int? theEdOrgValue = null;
                            var edOrgIdObj = reader[MultipleEnrollmentsCountReportQuery.EdOrgIdColumnName];
                            if (!(edOrgIdObj is DBNull))
                            {
                                theEdOrgValue = System.Convert.ToInt32(reader[MultipleEnrollmentsCountReportQuery.EdOrgIdColumnName]);
                            }
                            reportData.Add(new MultipleEnrollmentsCountReportQuery
                            {
                                OrgType = (OrgType)(System.Convert.ToInt32(reader[MultipleEnrollmentsCountReportQuery.OrgTypeColumnName])),
                                EdOrgId = theEdOrgValue,
                                LEASchool = reader[MultipleEnrollmentsCountReportQuery.SchoolNameColumnName].ToString(),
                                TotalEnrollmentCount = System.Convert.ToInt32(reader[MultipleEnrollmentsCountReportQuery.TotalEnrollmentCountColumnName]),
                                DistinctEnrollmentCount = System.Convert.ToInt32(reader[MultipleEnrollmentsCountReportQuery.DistinctEnrollmentCountColumnName]),
                                EnrolledInOtherSchoolsCount = System.Convert.ToInt32(reader[MultipleEnrollmentsCountReportQuery.EnrolledInOtherSchoolsCountColumnName]),
                                EnrolledInOtherDistrictsCount = System.Convert.ToInt32(reader[MultipleEnrollmentsCountReportQuery.EnrolledInOtherDistrictsCountColumnName])
                            });
                        }
                    }
                    return reportData;
                }
                catch (Exception ex)
                {
                    LoggingService.LogErrorMessage($"While compiling report on multiple enrollments: {ex.ChainInnerExceptionMessages()}");
                    throw;
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
            }
        }

        public List<StudentProgramsCountReportQuery> GetStudentProgramsCounts(int? districtEdOrgId, string fourDigitOdsDbYear)
        {
            using (var rawOdsContext = new RawOdsDbContext(fourDigitOdsDbYear))
            {
                var conn = rawOdsContext.Database.Connection;
                try
                {
                    conn.Open();
                    var studentProgsCmd = conn.CreateCommand();
                    studentProgsCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    studentProgsCmd.CommandText = StudentProgramsCountReportQuery.StudentProgramsCountQuery;
                    studentProgsCmd.Parameters.Add(new SqlParameter("@distid", System.Data.SqlDbType.Int));
                    studentProgsCmd.Parameters["@distid"].Value = districtEdOrgId.HasValue ? (object)districtEdOrgId.Value : (object)DBNull.Value;
                    var reportData = new List<StudentProgramsCountReportQuery>();
                    using (var reader = studentProgsCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int? theEdOrgValue = null;
                            var edOrgIdObj = reader[DemographicsCountReportQuery.EdOrgIdColumnName];
                            if (!(edOrgIdObj is DBNull))
                            {
                                theEdOrgValue = System.Convert.ToInt32(reader[DemographicsCountReportQuery.EdOrgIdColumnName]);
                            }
                            reportData.Add(new StudentProgramsCountReportQuery
                            {
                                OrgType = (OrgType)(System.Convert.ToInt32(reader[StudentProgramsCountReportQuery.OrgTypeColumnName])),
                                EdOrgId = theEdOrgValue,
                                LEASchool = reader[StudentProgramsCountReportQuery.LEASchoolColumnName].ToString(),
                                DemographicsCount = System.Convert.ToInt32(reader[StudentProgramsCountReportQuery.DistinctEnrollmentCountColumnName]),
                                DistinctEnrollmentCount = System.Convert.ToInt32(reader[StudentProgramsCountReportQuery.DistinctEnrollmentCountColumnName]),
                                DistinctDemographicsCount = System.Convert.ToInt32(reader[StudentProgramsCountReportQuery.DistinctDemographicsCountColumnName]),
                                ADParentCount = System.Convert.ToInt32(reader[StudentProgramsCountReportQuery.ADParentCountColumnName]),
                                IndianNativeCount = System.Convert.ToInt32(reader[StudentProgramsCountReportQuery.IndianNativeCountColumnName]),
                                MigrantCount = System.Convert.ToInt32(reader[StudentProgramsCountReportQuery.MigrantCountColumnName]),
                                HomelessCount = System.Convert.ToInt32(reader[StudentProgramsCountReportQuery.HomelessCountColumnName]),
                                ImmigrantCount = System.Convert.ToInt32(reader[StudentProgramsCountReportQuery.ImmigrantCountColumnName]),
                                EnglishLearnerIdentifiedCount = System.Convert.ToInt32(reader[StudentProgramsCountReportQuery.EnglishLearnerIdentifiedCountColumnName]),
                                EnglishLearnerServedCount = System.Convert.ToInt32(reader[StudentProgramsCountReportQuery.EnglishLearnerServedCountColumnName]),
                                RecentEnglishCount = System.Convert.ToInt32(reader[StudentProgramsCountReportQuery.RecentEnglishCountColumnName]),
                                SLIFECount = System.Convert.ToInt32(reader[StudentProgramsCountReportQuery.SLIFECountColumnName]),
                                IndependentStudyCount = System.Convert.ToInt32(reader[StudentProgramsCountReportQuery.IndependentStudyCountColumnName]),
                                Section504Count = System.Convert.ToInt32(reader[StudentProgramsCountReportQuery.Section504CountColumnName]),
                                Title1PartACount = System.Convert.ToInt32(reader[StudentProgramsCountReportQuery.Title1PartACountColumnName]),
                                FreeReducedCount = System.Convert.ToInt32(reader[StudentProgramsCountReportQuery.FreeReducedColumnName])
                            });
                        }
                    }
                    return reportData;
                }
                catch (Exception ex)
                {
                    LoggingService.LogErrorMessage($"While compiling report on student characteristics and programs: {ex.ChainInnerExceptionMessages()}");
                    throw;
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
            }
        }

        public List<ChangeOfEnrollmentReportQuery> GetChangeOfEnrollmentReport(int districtEdOrgId, string fourDigitOdsDbYear)
        {
            using (var rawOdsContext = new RawOdsDbContext(fourDigitOdsDbYear))
            {
                var conn = rawOdsContext.Database.Connection;
                try
                {
                    conn.Open();
                    var chgOfEnrollmentQueryCmd = conn.CreateCommand();
                    chgOfEnrollmentQueryCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    chgOfEnrollmentQueryCmd.CommandText = ChangeOfEnrollmentReportQuery.ChangeOfEnrollmentQuery;
                    chgOfEnrollmentQueryCmd.Parameters.Add(new SqlParameter("@distid", System.Data.SqlDbType.Int));
                    chgOfEnrollmentQueryCmd.Parameters["@distid"].Value = districtEdOrgId;
                    var reportData = new List<ChangeOfEnrollmentReportQuery>();
                    using (var reader = chgOfEnrollmentQueryCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            reportData.Add(new ChangeOfEnrollmentReportQuery
                            {
                                IsCurrentDistrict = System.Convert.ToBoolean(reader[ChangeOfEnrollmentReportQuery.IsCurrentDistrictColumnName]),
                                CurrentDistEdOrgId = System.Convert.ToInt32(reader[ChangeOfEnrollmentReportQuery.CurrentDistEdOrgIdColumnName]),
                                CurrentDistrictName = reader[ChangeOfEnrollmentReportQuery.CurrentDistrictNameColumnName].ToString(),
                                CurrentSchoolEdOrgId = System.Convert.ToInt32(reader[ChangeOfEnrollmentReportQuery.CurrentSchoolEdOrgIdColumnName]),
                                CurrentSchoolName = reader[ChangeOfEnrollmentReportQuery.CurrentSchoolNameColumnName].ToString(),
                                CurrentEdOrgEnrollmentDate = (reader[ChangeOfEnrollmentReportQuery.CurrentEdOrgEnrollmentDateColumnName] is DBNull)
                                    ? (DateTime?)null
                                    : System.Convert.ToDateTime(reader[ChangeOfEnrollmentReportQuery.CurrentEdOrgEnrollmentDateColumnName]),
                                CurrentEdOrgExitDate = (reader[ChangeOfEnrollmentReportQuery.CurrentEdOrgExitDateColumnName] is DBNull)
                                    ? (DateTime?)null
                                    : System.Convert.ToDateTime(reader[ChangeOfEnrollmentReportQuery.CurrentEdOrgExitDateColumnName]),
                                CurrentGrade = reader[ChangeOfEnrollmentReportQuery.CurrentGradeColumnName].ToString(),
                                PastDistEdOrgId = System.Convert.ToInt32(reader[ChangeOfEnrollmentReportQuery.PastDistEdOrgIdColumnName]),
                                PastDistrictName = reader[ChangeOfEnrollmentReportQuery.PastDistrictNameColumnName].ToString(),
                                PastSchoolEdOrgId = System.Convert.ToInt32(reader[ChangeOfEnrollmentReportQuery.PastSchoolEdOrgIdColumnName]),
                                PastSchoolName = reader[ChangeOfEnrollmentReportQuery.PastSchoolNameColumnName].ToString(),
                                PastEdOrgEnrollmentDate = (reader[ChangeOfEnrollmentReportQuery.PastEdOrgEnrollmentDateColumnName] is DBNull)
                                    ? (DateTime?)null
                                    : System.Convert.ToDateTime(reader[ChangeOfEnrollmentReportQuery.PastEdOrgEnrollmentDateColumnName]),
                                PastEdOrgExitDate = (reader[ChangeOfEnrollmentReportQuery.PastEdOrgExitDateColumnName] is DBNull)
                                    ? (DateTime?)null
                                    : System.Convert.ToDateTime(reader[ChangeOfEnrollmentReportQuery.PastEdOrgExitDateColumnName]),
                                PastGrade = reader[ChangeOfEnrollmentReportQuery.PastGradeColumnName].ToString(),
                                StudentID = reader[ChangeOfEnrollmentReportQuery.StudentIDColumnName].ToString(),
                                StudentLastName = reader[ChangeOfEnrollmentReportQuery.StudentLastNameColumnName].ToString(),
                                StudentFirstName = reader[ChangeOfEnrollmentReportQuery.StudentFirstNameColumnName].ToString(),
                                StudentMiddleName = reader[ChangeOfEnrollmentReportQuery.StudentMiddleNameColumnName].ToString(),
                                StudentBirthDate = (reader[ChangeOfEnrollmentReportQuery.StudentBirthDateColumnName] is DBNull)
                                    ? (DateTime?)null
                                    : System.Convert.ToDateTime(reader[ChangeOfEnrollmentReportQuery.StudentBirthDateColumnName]),
                            });
                        }
                    }

                    return reportData;
                }
                catch (Exception ex)
                {
                    LoggingService.LogErrorMessage($"While compiling report on changes of enrollment: {ex.ChainInnerExceptionMessages()}");
                    throw;
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
            }
        }

        public List<ResidentsEnrolledElsewhereReportQuery> GetResidentsEnrolledElsewhereReport(int? districtEdOrgId, string fourDigitOdsDbYear)
        {
            using (var rawOdsContext = new RawOdsDbContext(fourDigitOdsDbYear))
            {
                var conn = rawOdsContext.Database.Connection;
                try
                {
                    conn.Open();
                    var residentsElseWhereQueryCmd = conn.CreateCommand();
                    residentsElseWhereQueryCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    residentsElseWhereQueryCmd.CommandText = ResidentsEnrolledElsewhereReportQuery.ResidentsEnrolledElsewhereQuery;
                    residentsElseWhereQueryCmd.Parameters.Add(new SqlParameter("@distid", System.Data.SqlDbType.Int));
                    residentsElseWhereQueryCmd.Parameters["@distid"].Value = districtEdOrgId.HasValue ? (object)districtEdOrgId.Value : (object)DBNull.Value;
                    var reportData = new List<ResidentsEnrolledElsewhereReportQuery>();
                    using (var reader = residentsElseWhereQueryCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int? theEdOrgValue = null;
                            var edOrgIdObj = reader[ResidentsEnrolledElsewhereReportQuery.EdOrgIdColumnName];
                            if (!(edOrgIdObj is DBNull))
                            {
                                theEdOrgValue = System.Convert.ToInt32(reader[ResidentsEnrolledElsewhereReportQuery.EdOrgIdColumnName]);
                            }
                            reportData.Add(new ResidentsEnrolledElsewhereReportQuery
                            {
                                OrgType = (OrgType)(System.Convert.ToInt32(reader[ResidentsEnrolledElsewhereReportQuery.OrgTypeColumnName])),
                                EdOrgId = theEdOrgValue,
                                EdOrgName = reader[ResidentsEnrolledElsewhereReportQuery.EdOrgNameColumnName].ToString(),
                                DistrictOfEnrollmentId = System.Convert.ToInt32(reader[ResidentsEnrolledElsewhereReportQuery.DistrictOfEnrollmentIdColumnName]),
                                DistrictOfEnrollmentName = reader[ResidentsEnrolledElsewhereReportQuery.DistrictOfEnrollmentNameColumnName].ToString(),
                                ResidentsEnrolled = System.Convert.ToInt32(reader[ResidentsEnrolledElsewhereReportQuery.ResidentsEnrolledColumnName])
                            });
                        }
                    }
                    return reportData;
                }
                catch (Exception ex)
                {
                    LoggingService.LogErrorMessage($"While compiling report on residents enrolled elsewhere: {ex.ChainInnerExceptionMessages()}");
                    throw;
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
            }
        }

        public List<StudentDrillDownQuery> GetDistrictAncestryRaceStudentDrillDown(OrgType orgType, int? schoolEdOrgId, int? districtEdOrgId, int? columnIndex, string fourDigitOdsDbYear)
        {
            var returnedList = new List<StudentDrillDownQuery>();
            using (var rawOdsContext = new RawOdsDbContext(fourDigitOdsDbYear))
            {
                var conn = rawOdsContext.Database.Connection;
                try
                {
                    conn.Open();
                    var queryCmd = conn.CreateCommand();
                    queryCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    queryCmd.CommandText = DemographicsCountReportQuery.DistrictAncestryRaceCountsStudentDetailsQuery;
                    queryCmd.Parameters.Add(new SqlParameter("@schoolid", System.Data.SqlDbType.Int));
                    queryCmd.Parameters["@schoolid"].Value = schoolEdOrgId.HasValue && orgType == OrgType.School ? (object)schoolEdOrgId.Value : (object)DBNull.Value;
                    queryCmd.Parameters.Add(new SqlParameter("@distid", System.Data.SqlDbType.Int));
                    queryCmd.Parameters["@distid"].Value = districtEdOrgId.HasValue && orgType == OrgType.District ? (object)districtEdOrgId.Value : (object)DBNull.Value;
                    queryCmd.Parameters.Add(new SqlParameter("@columnIndex", System.Data.SqlDbType.Int));
                    queryCmd.Parameters["@columnIndex"].Value = columnIndex.HasValue ? (object)columnIndex.Value : (object)DBNull.Value;
                    using (var reader = queryCmd.ExecuteReader())
                    {
                        returnedList = ReadStudentDrillDownDataReader(reader);
                    }
                    return returnedList;
                }
                catch (Exception ex)
                {
                    LoggingService.LogErrorMessage($"While providing student details on race and ethnic origin: {ex.ChainInnerExceptionMessages()}");
                    throw;
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
            }
        }

        public List<StudentDrillDownQuery> GetMultipleEnrollmentStudentDrillDown(OrgType orgType, int? schoolEdOrgId, int? districtEdOrgId, int? columnIndex, string fourDigitOdsDbYear)
        {
            var returnedList = new List<StudentDrillDownQuery>();
            using (var rawOdsContext = new RawOdsDbContext(fourDigitOdsDbYear))
            {
                var conn = rawOdsContext.Database.Connection;
                try
                {
                    conn.Open();
                    var queryCmd = conn.CreateCommand();
                    queryCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    queryCmd.CommandText = MultipleEnrollmentsCountReportQuery.MultipleEnrollmentsStudentDetailsQuery;
                    queryCmd.Parameters.Add(new SqlParameter("@schoolid", System.Data.SqlDbType.Int));
                    queryCmd.Parameters["@schoolid"].Value = schoolEdOrgId.HasValue && orgType == OrgType.School ? (object) schoolEdOrgId.Value : (object) DBNull.Value;
                    queryCmd.Parameters.Add(new SqlParameter("@distid", System.Data.SqlDbType.Int));
                    queryCmd.Parameters["@distid"].Value = districtEdOrgId.HasValue && orgType == OrgType.District ? (object) districtEdOrgId.Value : (object) DBNull.Value;
                    queryCmd.Parameters.Add(new SqlParameter("@columnIndex", System.Data.SqlDbType.Int));
                    queryCmd.Parameters["@columnIndex"].Value = columnIndex.HasValue? (object) columnIndex.Value : (object) DBNull.Value;
                    using (var reader = queryCmd.ExecuteReader())
                    {
                        returnedList = ReadStudentDrillDownDataReader(reader);
}
                    return returnedList;
                }
                catch (Exception ex)
                {
                    LoggingService.LogErrorMessage($"While providing student details on multiple enrollments: {ex.ChainInnerExceptionMessages()}");
                    throw;
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
            }
        }

        public List<StudentDrillDownQuery> GetStudentProgramsStudentDrillDown(OrgType orgType, int? schoolEdOrgId, int? districtEdOrgId, int? columnIndex, string fourDigitOdsDbYear)
        {
            var returnedList = new List<StudentDrillDownQuery>();
            using (var rawOdsContext = new RawOdsDbContext(fourDigitOdsDbYear))
            {
                var conn = rawOdsContext.Database.Connection;
                try
                {
                    conn.Open();
                    var queryCmd = conn.CreateCommand();
                    queryCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    queryCmd.CommandText = StudentProgramsCountReportQuery.StudentProgramsStudentDetailsReport;
                    queryCmd.Parameters.Add(new SqlParameter("@schoolid", System.Data.SqlDbType.Int));
                    queryCmd.Parameters["@schoolid"].Value = schoolEdOrgId.HasValue && orgType == OrgType.School ? (object)schoolEdOrgId.Value : (object)DBNull.Value;
                    queryCmd.Parameters.Add(new SqlParameter("@distid", System.Data.SqlDbType.Int));
                    queryCmd.Parameters["@distid"].Value = districtEdOrgId.HasValue && orgType == OrgType.District ? (object)districtEdOrgId.Value : (object)DBNull.Value;
                    queryCmd.Parameters.Add(new SqlParameter("@columnIndex", System.Data.SqlDbType.Int));
                    queryCmd.Parameters["@columnIndex"].Value = columnIndex.HasValue ? (object)columnIndex.Value : (object)DBNull.Value;
                    using (var reader = queryCmd.ExecuteReader())
                    {
                        returnedList = ReadStudentDrillDownDataReader(reader);
                    }
                    return returnedList;
                }
                catch (Exception ex)
                {
                    LoggingService.LogErrorMessage($"While providing student details on program participation and student characteristics: {ex.ChainInnerExceptionMessages()}");
                    throw;
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
            }
        }

        public List<StudentDrillDownQuery> GetResidentsEnrolledElsewhereStudentDrillDown(int? districtEdOrgId, string fourDigitOdsDbYear)
        {
            var returnedList = new List<StudentDrillDownQuery>();
            using (var rawOdsContext = new RawOdsDbContext(fourDigitOdsDbYear))
            {
                var conn = rawOdsContext.Database.Connection;
                try
                {
                    conn.Open();
                    var residentsElseWhereQueryCmd = conn.CreateCommand();
                    residentsElseWhereQueryCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    residentsElseWhereQueryCmd.CommandText = ResidentsEnrolledElsewhereReportQuery.ResidentsEnrolledElsewhereStudentDetailsQuery;
                    residentsElseWhereQueryCmd.Parameters.Add(new SqlParameter("@distid", System.Data.SqlDbType.Int));
                    residentsElseWhereQueryCmd.Parameters["@distid"].Value = districtEdOrgId.HasValue ? (object)districtEdOrgId.Value : (object)DBNull.Value;
                    using (var reader = residentsElseWhereQueryCmd.ExecuteReader())
                    {
                        returnedList = ReadStudentDrillDownDataReader(reader);
                    }
                    return returnedList;
                }
                catch (Exception ex)
                {
                    LoggingService.LogErrorMessage($"While providing student details on residents enrolled elsewhere: {ex.ChainInnerExceptionMessages()}");
                    throw;
                }
                finally
                {
                    if (conn != null && conn.State != System.Data.ConnectionState.Closed)
                    {
                        try
                        {
                            conn.Close();
                        }
                        catch (Exception) { }   // todo: remove all these
                    }
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

                    if (formData.CheckAssessment)
                    {
                        studentRecord.AssessmentResults.Requested = true;
                        studentRecord.AssessmentResults.RequestingUserId = studentRecord.RequestingUser;
                        studentRecord.AssessmentResults.RequestingDistrictId = studentRecord.RequestingDistrict;
                    }

                    if (formData.CheckCumulative)
                    {
                        studentRecord.CumulativeFiles.Requested = true;
                        studentRecord.CumulativeFiles.RequestingUserId = studentRecord.RequestingUser;
                        studentRecord.CumulativeFiles.RequestingDistrictId = studentRecord.RequestingDistrict;
                    }

                    if (formData.CheckDiscipline)
                    {
                        studentRecord.DisciplineRecords.Requested = true;
                        studentRecord.DisciplineRecords.RequestingUserId = studentRecord.RequestingUser;
                        studentRecord.DisciplineRecords.RequestingDistrictId = studentRecord.RequestingDistrict;
                    }

                    if (formData.CheckEvaluation)
                    {
                        studentRecord.EvaluationSummary.Requested = true;
                        studentRecord.EvaluationSummary.RequestingUserId = studentRecord.RequestingUser;
                        studentRecord.EvaluationSummary.RequestingDistrictId = studentRecord.RequestingDistrict;
                    }

                    if (formData.CheckIEP)
                    {
                        studentRecord.IEP.Requested = true;
                        studentRecord.IEP.RequestingUserId = studentRecord.RequestingUser;
                        studentRecord.IEP.RequestingDistrictId = studentRecord.RequestingDistrict;
                    }

                    if (formData.CheckImmunization)
                    {
                        studentRecord.Immunizations.Requested = true;
                        studentRecord.Immunizations.RequestingUserId = studentRecord.RequestingUser;
                        studentRecord.Immunizations.RequestingDistrictId = studentRecord.RequestingDistrict;
                    }

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

        protected void SetRecordRequester(RecordsRequest recordsRequest, RecordsRequestFormData formData)
        {

        }

        protected void SetRecordResponder(RecordsRequest recordsRequest, RecordsRequestFormData formData)
        {

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

                    studentRecord.RespondingDistrict = formData.RespondingDistrictId;
                    studentRecord.RespondingUser = formData.RespondingUserId;

                    if (formData.CheckAssessment)
                    {
                        studentRecord.AssessmentResults.Sent = true;
                        studentRecord.AssessmentResults.RespondingUserId = studentRecord.RespondingUser;
                        studentRecord.AssessmentResults.RespondingDistrictId = studentRecord.RespondingDistrict;
                    }

                    if (formData.CheckCumulative)
                    {
                        studentRecord.CumulativeFiles.Sent = true;
                        studentRecord.CumulativeFiles.RespondingUserId = studentRecord.RespondingUser;
                        studentRecord.CumulativeFiles.RespondingDistrictId = studentRecord.RespondingDistrict;
                    }

                    if (formData.CheckDiscipline)
                    {
                        studentRecord.DisciplineRecords.Sent = true;
                        studentRecord.DisciplineRecords.RespondingUserId = studentRecord.RespondingUser;
                        studentRecord.DisciplineRecords.RespondingDistrictId = studentRecord.RespondingDistrict;
                    }

                    if (formData.CheckEvaluation)
                    {
                        studentRecord.EvaluationSummary.Sent = true;
                        studentRecord.EvaluationSummary.RespondingUserId = studentRecord.RespondingUser;
                        studentRecord.EvaluationSummary.RespondingDistrictId = studentRecord.RespondingDistrict;
                    }

                    if (formData.CheckIEP)
                    {
                        studentRecord.IEP.Sent = true;
                        studentRecord.IEP.RespondingUserId = studentRecord.RespondingUser;
                        studentRecord.IEP.RespondingDistrictId = studentRecord.RespondingDistrict;
                    }

                    if (formData.CheckImmunization)
                    {
                        studentRecord.Immunizations.Sent = true;
                        studentRecord.Immunizations.RespondingUserId = studentRecord.RespondingUser;
                        studentRecord.Immunizations.RespondingDistrictId = studentRecord.RespondingDistrict;
                    }

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
        
        protected List<StudentDrillDownQuery> ReadStudentDrillDownDataReader(DbDataReader reader)
        {
            var recordsReturned = new List<StudentDrillDownQuery>();
            while (reader.Read())
            {
                int? schoolId = null;
                var schoolIdObj = reader[StudentDrillDownQuery.SchoolIdColumnName];
                if (!(schoolIdObj is DBNull))
                {
                    schoolId = System.Convert.ToInt32(reader[StudentDrillDownQuery.SchoolIdColumnName]);
                }

                int? districtId = null;
                if (reader.HasColumn(StudentDrillDownQuery.DistrictIdColumnName))
                {
                    var districtIdObj = reader[StudentDrillDownQuery.DistrictIdColumnName];
                    if (!(districtIdObj is DBNull))
                    {
                        districtId = System.Convert.ToInt32(reader[StudentDrillDownQuery.DistrictIdColumnName]);
                    }
                }

                DateTime? enrolledDate = null;
                if (reader.HasColumn(StudentDrillDownQuery.EnrolledDateColumnName))
                {
                    var enrolledDateObj = reader[StudentDrillDownQuery.EnrolledDateColumnName];
                    if (!(enrolledDateObj is DBNull))
                    {
                        enrolledDate = System.Convert.ToDateTime(reader[StudentDrillDownQuery.EnrolledDateColumnName]);
                    }
                }

                DateTime? withdrawDate = null;
                if (reader.HasColumn(StudentDrillDownQuery.WithdrawDateColumnName))
                {
                    var withdrawDateObj = reader[StudentDrillDownQuery.WithdrawDateColumnName];
                    if (!(withdrawDateObj is DBNull))
                    {
                        withdrawDate = System.Convert.ToDateTime(reader[StudentDrillDownQuery.WithdrawDateColumnName]);
                    }
                }

                recordsReturned.Add(new StudentDrillDownQuery
                {
                    StudentId = reader[StudentDrillDownQuery.StudentIdColumnName].ToString(),
                    StudentFirstName = reader[StudentDrillDownQuery.StudentFirstNameColumnName].ToString(),
                    StudentMiddleName = reader[StudentDrillDownQuery.StudentMiddleNameColumnName].ToString(),
                    StudentLastName = reader[StudentDrillDownQuery.StudentLastNameColumnName].ToString(),
                    DistrictId = districtId,
                    DistrictName = reader.HasColumn(StudentDrillDownQuery.DistrictNameColumnName) ? reader[StudentDrillDownQuery.DistrictNameColumnName].ToString() : string.Empty,
                    SchoolId = schoolId,
                    SchoolName = reader[StudentDrillDownQuery.SchoolNameColumnName].ToString(),
                    EnrolledDate = enrolledDate,
                    WithdrawDate = withdrawDate,
                    Grade = reader.HasColumn(StudentDrillDownQuery.GradeColumnName) ? reader[StudentDrillDownQuery.GradeColumnName].ToString() : string.Empty,
                    SpecialEdStatus = reader.HasColumn(StudentDrillDownQuery.SpecialEdStatusColumnName) ? reader[StudentDrillDownQuery.SpecialEdStatusColumnName].ToString() : string.Empty
                });
            }

            return recordsReturned;
        }
    }
}