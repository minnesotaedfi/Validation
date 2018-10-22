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

        public List<DemographicsCountReportQuery> GetDistrictAncestryRaceCounts(int districtEdOrgId, string fourDigitOdsDbYear)
        {
            using (var rawOdsContext = new RawOdsDbContext(fourDigitOdsDbYear))
            {
                var conn = rawOdsContext.Database.Connection;
                try
                {
                    conn.Open();
                    var ancestryQueryCmd = conn.CreateCommand();
                    ancestryQueryCmd.CommandType = System.Data.CommandType.Text;
                    ancestryQueryCmd.CommandText = DemographicsCountReportQuery.DistrictAncestryRaceCountsQuery;
                    ancestryQueryCmd.Parameters.Add(new SqlParameter("@distid", System.Data.SqlDbType.Int));
                    ancestryQueryCmd.Parameters["@distid"].Value = districtEdOrgId;
                    var reportData = new List<DemographicsCountReportQuery>();
                    using (var reader = ancestryQueryCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            reportData.Add(new DemographicsCountReportQuery
                            {
                                SchoolName = reader[DemographicsCountReportQuery.SchoolNameColumnName].ToString(),
                                DistrictName = reader[DemographicsCountReportQuery.DistrictNameColumnName].ToString(),
                                DemographicsCount = System.Convert.ToInt32(reader[DemographicsCountReportQuery.DemographicsCountColumnName]),
                                EnrollmentCount = System.Convert.ToInt32(reader[DemographicsCountReportQuery.EnrollmentCountColumnName]),
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

        public List<MultipleEnrollmentsCountReportQuery> GetMultipleEnrollmentCounts(int districtEdOrgId, string fourDigitOdsDbYear)
        {
            using (var rawOdsContext = new RawOdsDbContext(fourDigitOdsDbYear))
            {
                var conn = rawOdsContext.Database.Connection;
                try
                {
                    conn.Open();
                    var ancestryQueryCmd = conn.CreateCommand();
                    ancestryQueryCmd.CommandType = System.Data.CommandType.Text;
                    ancestryQueryCmd.CommandText = MultipleEnrollmentsCountReportQuery.MultipleEnrollmentsCountQuery;
                    ancestryQueryCmd.Parameters.Add(new SqlParameter("@distid", System.Data.SqlDbType.Int));
                    ancestryQueryCmd.Parameters["@distid"].Value = districtEdOrgId;
                    var reportData = new List<MultipleEnrollmentsCountReportQuery>();
                    using (var reader = ancestryQueryCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            reportData.Add(new MultipleEnrollmentsCountReportQuery
                            {
                                SchoolName = reader[MultipleEnrollmentsCountReportQuery.SchoolNameColumnName].ToString(),
                                DistrictName = reader[MultipleEnrollmentsCountReportQuery.DistrictNameColumnName].ToString(),
                                EnrollmentCount = System.Convert.ToInt32(reader[MultipleEnrollmentsCountReportQuery.EnrollmentCountColumnName]),
                                MultiWithinDistrictCount = System.Convert.ToInt32(reader[MultipleEnrollmentsCountReportQuery.MultiWithinDistrictCountColumnName]),
                                MultiOutsideDistrictCount = System.Convert.ToInt32(reader[MultipleEnrollmentsCountReportQuery.MultiOutsideDistrictCountColumnName])
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

        public List<StudentProgramsCountReportQuery> GetStudentProgramsCounts(int districtEdOrgId, string fourDigitOdsDbYear)
        {
            using (var rawOdsContext = new RawOdsDbContext(fourDigitOdsDbYear))
            {
                var conn = rawOdsContext.Database.Connection;
                try
                {
                    conn.Open();
                    var studentProgsCmd = conn.CreateCommand();
                    studentProgsCmd.CommandType = System.Data.CommandType.Text;
                    studentProgsCmd.CommandText = StudentProgramsCountReportQuery.StudentProgramsCountQuery;
                    studentProgsCmd.Parameters.Add(new SqlParameter("@distid", System.Data.SqlDbType.Int));
                    studentProgsCmd.Parameters["@distid"].Value = districtEdOrgId;
                    var reportData = new List<StudentProgramsCountReportQuery>();
                    using (var reader = studentProgsCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            reportData.Add(new StudentProgramsCountReportQuery
                            {
                                SchoolName = reader[StudentProgramsCountReportQuery.SchoolNameColumnName].ToString(),
                                DistrictName = reader[StudentProgramsCountReportQuery.DistrictNameColumnName].ToString(),
                                EnrollmentCount = System.Convert.ToInt32(reader[StudentProgramsCountReportQuery.EnrollmentCountColumnName]),
                                DemographicsCount = System.Convert.ToInt32(reader[StudentProgramsCountReportQuery.DemographicsCountColumnName]),
                                ADParentCount = System.Convert.ToInt32(reader[StudentProgramsCountReportQuery.ADParentCountColumnName]),
                                IndianNativeCount = System.Convert.ToInt32(reader[StudentProgramsCountReportQuery.IndianNativeCountColumnName]),
                                MigrantCount = System.Convert.ToInt32(reader[StudentProgramsCountReportQuery.MigrantCountColumnName]),
                                HomelessCount = System.Convert.ToInt32(reader[StudentProgramsCountReportQuery.HomelessCountColumnName]),
                                ImmigrantCount = System.Convert.ToInt32(reader[StudentProgramsCountReportQuery.ImmigrantCountColumnName]),
                                EnglishLearnerCount = System.Convert.ToInt32(reader[StudentProgramsCountReportQuery.EnglishLearnerCountColumnName]),
                                EnglishLearnerServedCount = System.Convert.ToInt32(reader[StudentProgramsCountReportQuery.EnglishLearnerServedCountColumnName]),
                                SLIFECount = System.Convert.ToInt32(reader[StudentProgramsCountReportQuery.SLIFECountColumnName]),
                                IndependentStudyCount = System.Convert.ToInt32(reader[StudentProgramsCountReportQuery.IndependentStudyCountColumnName]),
                                Section504Count = System.Convert.ToInt32(reader[StudentProgramsCountReportQuery.Section504CountColumnName]),
                                Title1PartACount = System.Convert.ToInt32(reader[StudentProgramsCountReportQuery.Title1PartACountColumnName])
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
    }
}