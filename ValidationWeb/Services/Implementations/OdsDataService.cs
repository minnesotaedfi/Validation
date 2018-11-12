using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ValidationWeb.Services
{
    public class OdsDataService : IOdsDataService
    {
        public readonly ILoggingService _loggingService;
        public OdsDataService(ILoggingService loggingService)
        {
            _loggingService = loggingService;
        }

        public List<DemographicsCountReportQuery> GetDistrictAncestryRaceCounts(int? districtEdOrgId, string fourDigitOdsDbYear)
        {
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
                                DemographicsCount = System.Convert.ToInt32(reader[DemographicsCountReportQuery.DistinctEnrollmentCountColumnName]),
                                EnrollmentCount = System.Convert.ToInt32(reader[DemographicsCountReportQuery.DistinctDemographicsCountColumnName]),
                                AncestryGivenCount = System.Convert.ToInt32(reader[DemographicsCountReportQuery.AncestryGivenCountColumnName]),
                                RaceGivenCount = System.Convert.ToInt32(reader[DemographicsCountReportQuery.RaceGivenCountColumnName])
                            });
                        }
                    }
                    return reportData;
                }
                catch (Exception ex)
                {
                    _loggingService.LogErrorMessage($"While compiling report on ancestry/ethnic origin supplied counts: {ex.ChainInnerExceptionMessages()}");
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
                    _loggingService.LogErrorMessage($"While compiling report on multiple enrollments: {ex.ChainInnerExceptionMessages()}");
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
                    studentProgsCmd.Parameters["@distid"].Value = districtEdOrgId;
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
                    _loggingService.LogErrorMessage($"While compiling report on student characteristics and programs: {ex.ChainInnerExceptionMessages()}");
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
                                    : System.Convert.ToDateTime(reader[ChangeOfEnrollmentReportQuery.CurrentEdOrgExitDateColumnName]),
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
                    _loggingService.LogErrorMessage($"While compiling report on changes of enrollment: {ex.ChainInnerExceptionMessages()}");
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
                    _loggingService.LogErrorMessage($"While compiling report on residents enrolled elsewhere: {ex.ChainInnerExceptionMessages()}");
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

        List<ResidentsEnrolledElsewhereReportQuery> GetResidentsEnrolledElsewhereStudentDrillDown(int? districtEdOrgId, string fourDigitOdsDbYear, string uniqueStudentId)
    }
}