using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using ValidationWeb.Database;
using ValidationWeb.Database.Queries;
using ValidationWeb.Services.Interfaces;
using ValidationWeb.Utility;

namespace ValidationWeb.Services.Implementations
{
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
                    var command = conn.CreateCommand();
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.CommandText = DemographicsCountReportQuery.DistrictAncestryRaceCountsQuery;
                    command.Parameters.Add(new SqlParameter("@distid", System.Data.SqlDbType.Int));
                    command.Parameters["@distid"].Value = districtEdOrgId ?? (object)DBNull.Value;

                    var reportData = new List<DemographicsCountReportQuery>();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int? edOrgValue = null;
                            var edOrgIdObj = reader[DemographicsCountReportQuery.EdOrgIdColumnName];
                            if (!(edOrgIdObj is DBNull))
                            {
                                edOrgValue = Convert.ToInt32(reader[DemographicsCountReportQuery.EdOrgIdColumnName]);
                            }

                            reportData.Add(new DemographicsCountReportQuery
                            {
                                OrgType = (OrgType)Convert.ToInt32(reader[DemographicsCountReportQuery.OrgTypeColumnName]),
                                EdOrgId = edOrgValue,
                                LEASchool = reader[DemographicsCountReportQuery.LEASchoolColumnName].ToString(),
                                EnrollmentCount = Convert.ToInt32(reader[DemographicsCountReportQuery.DistinctEnrollmentCountColumnName]),
                                DemographicsCount = Convert.ToInt32(reader[DemographicsCountReportQuery.DistinctDemographicsCountColumnName]),
                                AncestryGivenCount = Convert.ToInt32(reader[DemographicsCountReportQuery.AncestryGivenCountColumnName]),
                                RaceGivenCount = Convert.ToInt32(reader[DemographicsCountReportQuery.RaceGivenCountColumnName])
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
                        conn.Close();
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
                    var command = conn.CreateCommand();
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.CommandText = MultipleEnrollmentsCountReportQuery.MultipleEnrollmentsCountQuery;
                    command.Parameters.Add(new SqlParameter("@distid", System.Data.SqlDbType.Int));
                    command.Parameters["@distid"].Value = districtEdOrgId ?? (object)DBNull.Value;
                    var reportData = new List<MultipleEnrollmentsCountReportQuery>();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int? edOrgValue = null;
                            var edOrgIdObj = reader[MultipleEnrollmentsCountReportQuery.EdOrgIdColumnName];
                            if (!(edOrgIdObj is DBNull))
                            {
                                edOrgValue = Convert.ToInt32(reader[MultipleEnrollmentsCountReportQuery.EdOrgIdColumnName]);
                            }

                            reportData.Add(new MultipleEnrollmentsCountReportQuery
                            {
                                OrgType = (OrgType)Convert.ToInt32(reader[MultipleEnrollmentsCountReportQuery.OrgTypeColumnName]),
                                EdOrgId = edOrgValue,
                                LEASchool = reader[MultipleEnrollmentsCountReportQuery.SchoolNameColumnName].ToString(),
                                TotalEnrollmentCount = Convert.ToInt32(reader[MultipleEnrollmentsCountReportQuery.TotalEnrollmentCountColumnName]),
                                DistinctEnrollmentCount = Convert.ToInt32(reader[MultipleEnrollmentsCountReportQuery.DistinctEnrollmentCountColumnName]),
                                EnrolledInOtherSchoolsCount = Convert.ToInt32(reader[MultipleEnrollmentsCountReportQuery.EnrolledInOtherSchoolsCountColumnName]),
                                EnrolledInOtherDistrictsCount = Convert.ToInt32(reader[MultipleEnrollmentsCountReportQuery.EnrolledInOtherDistrictsCountColumnName])
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
                        conn.Close();
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
                    var command = conn.CreateCommand();
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.CommandText = StudentProgramsCountReportQuery.StudentProgramsCountQuery;
                    command.Parameters.Add(new SqlParameter("@distid", System.Data.SqlDbType.Int));
                    command.Parameters["@distid"].Value = districtEdOrgId ?? (object)DBNull.Value;
                    var reportData = new List<StudentProgramsCountReportQuery>();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int? edOrgValue = null;
                            var edOrgIdObj = reader[DemographicsCountReportQuery.EdOrgIdColumnName];
                            if (!(edOrgIdObj is DBNull))
                            {
                                edOrgValue = Convert.ToInt32(reader[DemographicsCountReportQuery.EdOrgIdColumnName]);
                            }

                            reportData.Add(new StudentProgramsCountReportQuery
                            {
                                OrgType = (OrgType)Convert.ToInt32(reader[StudentProgramsCountReportQuery.OrgTypeColumnName]),
                                EdOrgId = edOrgValue,
                                LEASchool = reader[StudentProgramsCountReportQuery.LEASchoolColumnName].ToString(),
                                DemographicsCount = Convert.ToInt32(reader[StudentProgramsCountReportQuery.DistinctEnrollmentCountColumnName]),
                                DistinctEnrollmentCount = Convert.ToInt32(reader[StudentProgramsCountReportQuery.DistinctEnrollmentCountColumnName]),
                                DistinctDemographicsCount = Convert.ToInt32(reader[StudentProgramsCountReportQuery.DistinctDemographicsCountColumnName]),
                                ADParentCount = Convert.ToInt32(reader[StudentProgramsCountReportQuery.ADParentCountColumnName]),
                                IndianNativeCount = Convert.ToInt32(reader[StudentProgramsCountReportQuery.IndianNativeCountColumnName]),
                                MigrantCount = Convert.ToInt32(reader[StudentProgramsCountReportQuery.MigrantCountColumnName]),
                                HomelessCount = Convert.ToInt32(reader[StudentProgramsCountReportQuery.HomelessCountColumnName]),
                                ImmigrantCount = Convert.ToInt32(reader[StudentProgramsCountReportQuery.ImmigrantCountColumnName]),
                                EnglishLearnerIdentifiedCount = Convert.ToInt32(reader[StudentProgramsCountReportQuery.EnglishLearnerIdentifiedCountColumnName]),
                                EnglishLearnerServedCount = Convert.ToInt32(reader[StudentProgramsCountReportQuery.EnglishLearnerServedCountColumnName]),
                                RecentEnglishCount = Convert.ToInt32(reader[StudentProgramsCountReportQuery.RecentEnglishCountColumnName]),
                                SLIFECount = Convert.ToInt32(reader[StudentProgramsCountReportQuery.SLIFECountColumnName]),
                                IndependentStudyCount = Convert.ToInt32(reader[StudentProgramsCountReportQuery.IndependentStudyCountColumnName]),
                                Section504Count = Convert.ToInt32(reader[StudentProgramsCountReportQuery.Section504CountColumnName]),
                                Title1PartACount = Convert.ToInt32(reader[StudentProgramsCountReportQuery.Title1PartACountColumnName]),
                                FreeReducedCount = Convert.ToInt32(reader[StudentProgramsCountReportQuery.FreeReducedColumnName])
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
                        conn.Close();
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
                    var command = conn.CreateCommand();
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.CommandText = ChangeOfEnrollmentReportQuery.ChangeOfEnrollmentQuery;
                    command.Parameters.Add(new SqlParameter("@distid", System.Data.SqlDbType.Int));
                    command.Parameters["@distid"].Value = districtEdOrgId;
                    var reportData = new List<ChangeOfEnrollmentReportQuery>();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            reportData.Add(new ChangeOfEnrollmentReportQuery
                            {
                                IsCurrentDistrict = Convert.ToBoolean(reader[ChangeOfEnrollmentReportQuery.IsCurrentDistrictColumnName]),
                                CurrentDistEdOrgId = Convert.ToInt32(reader[ChangeOfEnrollmentReportQuery.CurrentDistEdOrgIdColumnName]),
                                CurrentDistrictName = reader[ChangeOfEnrollmentReportQuery.CurrentDistrictNameColumnName].ToString(),
                                CurrentSchoolEdOrgId = Convert.ToInt32(reader[ChangeOfEnrollmentReportQuery.CurrentSchoolEdOrgIdColumnName]),
                                CurrentSchoolName = reader[ChangeOfEnrollmentReportQuery.CurrentSchoolNameColumnName].ToString(),
                                CurrentEdOrgEnrollmentDate = (reader[ChangeOfEnrollmentReportQuery.CurrentEdOrgEnrollmentDateColumnName] is DBNull)
                                    ? (DateTime?)null
                                    : Convert.ToDateTime(reader[ChangeOfEnrollmentReportQuery.CurrentEdOrgEnrollmentDateColumnName]),
                                CurrentEdOrgExitDate = (reader[ChangeOfEnrollmentReportQuery.CurrentEdOrgExitDateColumnName] is DBNull)
                                    ? (DateTime?)null
                                    : Convert.ToDateTime(reader[ChangeOfEnrollmentReportQuery.CurrentEdOrgExitDateColumnName]),
                                CurrentGrade = reader[ChangeOfEnrollmentReportQuery.CurrentGradeColumnName].ToString(),
                                PastDistEdOrgId = Convert.ToInt32(reader[ChangeOfEnrollmentReportQuery.PastDistEdOrgIdColumnName]),
                                PastDistrictName = reader[ChangeOfEnrollmentReportQuery.PastDistrictNameColumnName].ToString(),
                                PastSchoolEdOrgId = Convert.ToInt32(reader[ChangeOfEnrollmentReportQuery.PastSchoolEdOrgIdColumnName]),
                                PastSchoolName = reader[ChangeOfEnrollmentReportQuery.PastSchoolNameColumnName].ToString(),
                                PastEdOrgEnrollmentDate = (reader[ChangeOfEnrollmentReportQuery.PastEdOrgEnrollmentDateColumnName] is DBNull)
                                    ? (DateTime?)null
                                    : Convert.ToDateTime(reader[ChangeOfEnrollmentReportQuery.PastEdOrgEnrollmentDateColumnName]),
                                PastEdOrgExitDate = (reader[ChangeOfEnrollmentReportQuery.PastEdOrgExitDateColumnName] is DBNull)
                                    ? (DateTime?)null
                                    : Convert.ToDateTime(reader[ChangeOfEnrollmentReportQuery.PastEdOrgExitDateColumnName]),
                                PastGrade = reader[ChangeOfEnrollmentReportQuery.PastGradeColumnName].ToString(),
                                StudentID = reader[ChangeOfEnrollmentReportQuery.StudentIDColumnName].ToString(),
                                StudentLastName = reader[ChangeOfEnrollmentReportQuery.StudentLastNameColumnName].ToString(),
                                StudentFirstName = reader[ChangeOfEnrollmentReportQuery.StudentFirstNameColumnName].ToString(),
                                StudentMiddleName = reader[ChangeOfEnrollmentReportQuery.StudentMiddleNameColumnName].ToString(),
                                StudentBirthDate = (reader[ChangeOfEnrollmentReportQuery.StudentBirthDateColumnName] is DBNull)
                                    ? (DateTime?)null
                                    : Convert.ToDateTime(reader[ChangeOfEnrollmentReportQuery.StudentBirthDateColumnName]),
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
                        conn.Close();
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
                    var command = conn.CreateCommand();
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.CommandText = ResidentsEnrolledElsewhereReportQuery.ResidentsEnrolledElsewhereQuery;
                    command.Parameters.Add(new SqlParameter("@distid", System.Data.SqlDbType.Int));
                    command.Parameters["@distid"].Value = districtEdOrgId ?? (object)DBNull.Value;
                    var reportData = new List<ResidentsEnrolledElsewhereReportQuery>();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int? edOrgValue = null;
                            var edOrgIdObj = reader[ResidentsEnrolledElsewhereReportQuery.EdOrgIdColumnName];
                            if (!(edOrgIdObj is DBNull))
                            {
                                edOrgValue = Convert.ToInt32(reader[ResidentsEnrolledElsewhereReportQuery.EdOrgIdColumnName]);
                            }

                            reportData.Add(new ResidentsEnrolledElsewhereReportQuery
                            {
                                OrgType = (OrgType)Convert.ToInt32(reader[ResidentsEnrolledElsewhereReportQuery.OrgTypeColumnName]),
                                EdOrgId = edOrgValue,
                                EdOrgName = reader[ResidentsEnrolledElsewhereReportQuery.EdOrgNameColumnName].ToString(),
                                DistrictOfEnrollmentId = Convert.ToInt32(reader[ResidentsEnrolledElsewhereReportQuery.DistrictOfEnrollmentIdColumnName]),
                                DistrictOfEnrollmentName = reader[ResidentsEnrolledElsewhereReportQuery.DistrictOfEnrollmentNameColumnName].ToString(),
                                ResidentsEnrolled = Convert.ToInt32(reader[ResidentsEnrolledElsewhereReportQuery.ResidentsEnrolledColumnName])
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
                        conn.Close();
                    }
                }
            }
        }

        public List<StudentDrillDownQuery> GetDistrictAncestryRaceStudentDrillDown(OrgType orgType, int? schoolEdOrgId, int? districtEdOrgId, int? columnIndex, string fourDigitOdsDbYear)
        {
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
                    queryCmd.Parameters["@schoolid"].Value = schoolEdOrgId.HasValue && orgType == OrgType.School ? schoolEdOrgId.Value : (object)DBNull.Value;
                    queryCmd.Parameters.Add(new SqlParameter("@distid", System.Data.SqlDbType.Int));
                    queryCmd.Parameters["@distid"].Value = districtEdOrgId.HasValue && orgType == OrgType.District ? districtEdOrgId.Value : (object)DBNull.Value;
                    queryCmd.Parameters.Add(new SqlParameter("@columnIndex", System.Data.SqlDbType.Int));
                    queryCmd.Parameters["@columnIndex"].Value = columnIndex ?? (object)DBNull.Value;

                    List<StudentDrillDownQuery> result;
                    using (var reader = queryCmd.ExecuteReader())
                    {
                        result = ReadStudentDrillDownDataReader(reader);
                    }

                    return result;
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
                        conn.Close();
                    }
                }
            }
        }

        public List<StudentDrillDownQuery> GetMultipleEnrollmentStudentDrillDown(
            OrgType orgType,
            int? schoolEdOrgId,
            int? districtEdOrgId,
            int? columnIndex,
            string fourDigitOdsDbYear)
        {
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
                    queryCmd.Parameters["@schoolid"].Value = schoolEdOrgId.HasValue &&
                                                             orgType == OrgType.School ? schoolEdOrgId.Value : (object)DBNull.Value;

                    queryCmd.Parameters.Add(new SqlParameter("@distid", System.Data.SqlDbType.Int));
                    queryCmd.Parameters["@distid"].Value = districtEdOrgId.HasValue &&
                                                           (orgType == OrgType.District || orgType == OrgType.School) ? districtEdOrgId.Value : (object)DBNull.Value;

                    queryCmd.Parameters.Add(new SqlParameter("@columnIndex", System.Data.SqlDbType.Int));
                    queryCmd.Parameters["@columnIndex"].Value = columnIndex ?? (object)DBNull.Value;

                    List<StudentDrillDownQuery> result;
                    using (var reader = queryCmd.ExecuteReader())
                    {
                        result = ReadStudentDrillDownDataReader(reader);
                    }

                    return result;
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
                        conn.Close();
                    }
                }
            }
        }

        public List<StudentDrillDownQuery> GetStudentProgramsStudentDrillDown(OrgType orgType, int? schoolEdOrgId, int? districtEdOrgId, int? columnIndex, string fourDigitOdsDbYear)
        {
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
                    queryCmd.Parameters["@schoolid"].Value = schoolEdOrgId.HasValue && orgType == OrgType.School ? schoolEdOrgId.Value : (object)DBNull.Value;
                    queryCmd.Parameters.Add(new SqlParameter("@distid", System.Data.SqlDbType.Int));
                    queryCmd.Parameters["@distid"].Value = districtEdOrgId.HasValue && orgType == OrgType.District ? districtEdOrgId.Value : (object)DBNull.Value;
                    queryCmd.Parameters.Add(new SqlParameter("@columnIndex", System.Data.SqlDbType.Int));
                    queryCmd.Parameters["@columnIndex"].Value = columnIndex ?? (object)DBNull.Value;
                    List<StudentDrillDownQuery> result;
                    using (var reader = queryCmd.ExecuteReader())
                    {
                        result = ReadStudentDrillDownDataReader(reader);
                    }

                    return result;
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
                        conn.Close();
                    }
                }
            }
        }

        public List<StudentDrillDownQuery> GetResidentsEnrolledElsewhereStudentDrillDown(int? districtEdOrgId, string fourDigitOdsDbYear)
        {
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
                    residentsElseWhereQueryCmd.Parameters["@distid"].Value = districtEdOrgId ?? (object)DBNull.Value;
                    List<StudentDrillDownQuery> result;
                    using (var reader = residentsElseWhereQueryCmd.ExecuteReader())
                    {
                        result = ReadStudentDrillDownDataReader(reader);
                    }

                    return result;
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
                        conn.Close();
                    }
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
                    schoolId = Convert.ToInt32(reader[StudentDrillDownQuery.SchoolIdColumnName]);
                }

                int? districtId = null;
                if (reader.HasColumn(StudentDrillDownQuery.DistrictIdColumnName))
                {
                    var districtIdObj = reader[StudentDrillDownQuery.DistrictIdColumnName];
                    if (!(districtIdObj is DBNull))
                    {
                        districtId = Convert.ToInt32(reader[StudentDrillDownQuery.DistrictIdColumnName]);
                    }
                }

                DateTime? enrolledDate = null;
                if (reader.HasColumn(StudentDrillDownQuery.EnrolledDateColumnName))
                {
                    var enrolledDateObj = reader[StudentDrillDownQuery.EnrolledDateColumnName];
                    if (!(enrolledDateObj is DBNull))
                    {
                        enrolledDate = Convert.ToDateTime(reader[StudentDrillDownQuery.EnrolledDateColumnName]);
                    }
                }

                DateTime? withdrawDate = null;
                if (reader.HasColumn(StudentDrillDownQuery.WithdrawDateColumnName))
                {
                    var withdrawDateObj = reader[StudentDrillDownQuery.WithdrawDateColumnName];
                    if (!(withdrawDateObj is DBNull))
                    {
                        withdrawDate = Convert.ToDateTime(reader[StudentDrillDownQuery.WithdrawDateColumnName]);
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