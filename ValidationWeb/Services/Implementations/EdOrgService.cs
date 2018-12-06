using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;

namespace ValidationWeb.Services
{
    public class EdOrgService : IEdOrgService
    {
        protected readonly ValidationPortalDbContext _validationPortalDataContext;
        protected readonly IAppUserService _appUserService;
        protected readonly ILoggingService _loggingService;
        protected readonly ISchoolYearService _schoolYearService;

        public EdOrgService(ValidationPortalDbContext validationPortalDataContext, 
            IAppUserService appUserService,
            ISchoolYearService schoolYearService,
            ILoggingService loggingService)
        {
            _validationPortalDataContext = validationPortalDataContext;
            _appUserService = appUserService;
            _schoolYearService = schoolYearService;
            _loggingService = loggingService;
        }

        public List<EdOrg> GetEdOrgs()
        {
            return _appUserService.GetSession().UserIdentity.AuthorizedEdOrgs.OrderBy(eo => eo.OrganizationName).ToList();
        }

        public EdOrg GetEdOrgById(int edOrgId, int schoolYearId)
        {
            var result = _validationPortalDataContext.EdOrgs.FirstOrDefault(eo => eo.Id == edOrgId);
            if (result != null)
            {
                return result;
            }
            var schoolYear = _schoolYearService.GetSchoolYearById(schoolYearId);
            using (var _odsRawDbContext = new RawOdsDbContext(schoolYear.EndYear))
            {
                var conn = _odsRawDbContext.Database.Connection;
                try
                {
                    conn.Open();
                    var edOrgQueryCmd = conn.CreateCommand();
                    edOrgQueryCmd.CommandType = System.Data.CommandType.Text;
                    edOrgQueryCmd.CommandText = EdOrgQuery.SingleEdOrgsQuery;
                    edOrgQueryCmd.Parameters.Add(new SqlParameter("@lea_id", System.Data.SqlDbType.Int));
                    edOrgQueryCmd.Parameters["@lea_id"].Value = edOrgId;
                    result = ReadEdOrgs(edOrgQueryCmd, schoolYearId).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    _loggingService.LogErrorMessage($"While reading Ed Org description (ID# {edOrgId}, school year {schoolYear.ToString()}): {ex.ChainInnerExceptionMessages()}");
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
            if (result != null)
            {
                _validationPortalDataContext.EdOrgs.AddOrUpdate(result);
                _validationPortalDataContext.SaveChanges();
                return result;
            }
            throw new ApplicationException($"The Ed Org with ID# {edOrgId}, school year {schoolYear.ToString()}, was not found.");
        }

        public void RefreshEdOrgCache(int fourDigitOdsDbYear)
        {
            var edOrgsExtractedFromODS = new List<EdOrg>();
            using (var _odsRawDbContext = new RawOdsDbContext(fourDigitOdsDbYear.ToString()))
            {
                var conn = _odsRawDbContext.Database.Connection;
                try
                {
                    conn.Open();
                    var edOrgQueryCmd = conn.CreateCommand();
                    edOrgQueryCmd.CommandType = System.Data.CommandType.Text;
                    edOrgQueryCmd.CommandText = EdOrgQuery.SingleEdOrgsQuery;
                    edOrgQueryCmd.Parameters.Add(new SqlParameter("@lea_id", System.Data.SqlDbType.Int));
                    edOrgsExtractedFromODS.AddRange(ReadEdOrgs(edOrgQueryCmd, fourDigitOdsDbYear).ToList());
                }
                catch (Exception ex)
                {
                    _loggingService.LogErrorMessage($"While trying to add all ODS Ed Org descriptions to the Validation Portal Database Cache: school year {fourDigitOdsDbYear}, error: {ex.ChainInnerExceptionMessages()}");
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
            foreach (var singleEdOrg in edOrgsExtractedFromODS)
            {
                _validationPortalDataContext.EdOrgs.AddOrUpdate();
            }
            _validationPortalDataContext.SaveChanges();
        }

        private List<EdOrg> ReadEdOrgs(DbCommand edOrgQueryCmd, int schoolYearId)
        {
            var result = new List<EdOrg>();
            try
            {
                using (var reader = edOrgQueryCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var theId = int.Parse(reader[EdOrgQuery.IdColumnName].ToString());
                        var theStateAgency = reader[EdOrgQuery.StateOrganizationIdColumnName] as int?;
                        var isStateAgency = (theStateAgency != null) && (theId == theStateAgency);
                        result.Add(new EdOrg
                        {
                            Id = theId,
                            OrganizationName = reader[EdOrgQuery.OrganizationNameColumnName].ToString(),
                            OrganizationShortName = reader[EdOrgQuery.OrganizationShortNameColumnName].ToString(),
                            StateOrganizationId = theStateAgency,
                            ParentId = reader[EdOrgQuery.ParentIdColumnName] as int?,
                            StateLevelOrganizationId = reader[EdOrgQuery.StateLevelOrganizationIdColumnName] as int?,
                            SchoolYearId = schoolYearId,
                            IsStateLevelEdOrg = isStateAgency,
                            OrgTypeCodeValue = reader[EdOrgQuery.OrganizationShortNameColumnName].ToString(),
                            OrgTypeShortDescription = reader[EdOrgQuery.OrganizationShortNameColumnName].ToString(),
                            EdOrgTypeLookupId = isStateAgency ? (int)EdOrgType.State : (int)EdOrgType.District
                        });
                    }
                }
            }
            catch(Exception ex)
            {
                _loggingService.LogErrorMessage($"An error occurred while trying to read the Ed Org information from the Ed Fi ODS referring to the school year with ID {schoolYearId}: {ex.ChainInnerExceptionMessages()}");
            }
            return result;
        }
    }
}
