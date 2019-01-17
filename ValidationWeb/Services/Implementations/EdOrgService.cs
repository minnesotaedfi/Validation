namespace ValidationWeb.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Migrations;
    using System.Data.SqlClient;
    using System.Linq;

    public class EdOrgService : IEdOrgService
    {
        public EdOrgService(
            IDbContextFactory<ValidationPortalDbContext> validationPortalDataContextFactory,
            IAppUserService appUserService,
            ISchoolYearService schoolYearService,
            ILoggingService loggingService)
        {
            ValidationPortalDataContextFactory = validationPortalDataContextFactory;
            AppUserService = appUserService;
            SchoolYearService = schoolYearService;
            LoggingService = loggingService;
        }

        protected IDbContextFactory<ValidationPortalDbContext> ValidationPortalDataContextFactory { get; set; }

        protected IAppUserService AppUserService { get; set; }

        protected ILoggingService LoggingService { get; set; }

        protected ISchoolYearService SchoolYearService { get; set; }

        public List<EdOrg> GetAuthorizedEdOrgs()
        {
            // todo! refactor- this almost doesn't need to exist ... it's in user identity already
            var userIdentity = AppUserService.GetSession().UserIdentity;
            return userIdentity.AuthorizedEdOrgs.OrderBy(eo => eo.OrganizationName).ToList();
        }

        public List<EdOrg> GetAllEdOrgs()
        {
            using (var validationPortalDataContext = ValidationPortalDataContextFactory.Create())
            {
                return validationPortalDataContext.EdOrgs.ToList();
            }
        }

        public EdOrg GetEdOrgById(int edOrgId, int schoolYearId)
        {
            using (var validationPortalDataContext = ValidationPortalDataContextFactory.Create())
            {
                var schoolYear = SchoolYearService.GetSchoolYearById(schoolYearId);

                LoggingService.LogDebugMessage($"EdOrg cache: {validationPortalDataContext.EdOrgs.Count()} currently in ValidationPortal database");

                if (!validationPortalDataContext.EdOrgs.Any())
                {
                    LoggingService.LogDebugMessage($"Refreshing EdOrg cache");
                    RefreshEdOrgCache(schoolYear);
                }

                var result = validationPortalDataContext.EdOrgs.FirstOrDefault(eo => eo.Id == edOrgId);
                if (result != null)
                {
                    return result;
                }

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
                        LoggingService.LogErrorMessage(
                            $"While reading Ed Org description (ID# {edOrgId}, school year {schoolYear.ToString()}): {ex.ChainInnerExceptionMessages()}");
                    }
                    finally
                    {
                        if (conn != null && conn.State != System.Data.ConnectionState.Closed)
                        {
                            try
                            {
                                conn.Close();
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }

                if (result != null)
                {
                    validationPortalDataContext.EdOrgs.AddOrUpdate(result);
                    validationPortalDataContext.SaveChanges();
                    return result;
                }

                throw new ApplicationException(
                    $"The Ed Org with ID# {edOrgId}, school year {schoolYear.ToString()}, was not found.");
            }
        }

        public void RefreshEdOrgCache(SchoolYear schoolYear)
        {
            string fourDigitOdsDbYear = schoolYear.EndYear;
            var edOrgsExtractedFromODS = new List<EdOrg>();
            using (var odsRawDbContext = new RawOdsDbContext(fourDigitOdsDbYear))
            {
                var conn = odsRawDbContext.Database.Connection;
                try
                {
                    conn.Open();
                    var edOrgQueryCmd = conn.CreateCommand();
                    edOrgQueryCmd.CommandType = System.Data.CommandType.Text;
                    edOrgQueryCmd.CommandText = EdOrgQuery.AllEdOrgQuery;
                    edOrgsExtractedFromODS.AddRange(ReadEdOrgs(edOrgQueryCmd, schoolYear.Id).ToList());
                }
                catch (Exception ex)
                {
                    LoggingService.LogErrorMessage($"While trying to add all ODS Ed Org descriptions to the Validation Portal Database Cache: school year {fourDigitOdsDbYear}, error: {ex.ChainInnerExceptionMessages()}");
                }
                finally
                {
                    if (conn != null && conn.State != System.Data.ConnectionState.Closed)
                    {
                        try
                        {
                            conn.Close();
                        }
                        catch (Exception) { }   // todo: never this
                    }
                }
            }

            using (var validationPortalDataContext = ValidationPortalDataContextFactory.Create())
            {
                foreach (var singleEdOrg in edOrgsExtractedFromODS)
                {
                    validationPortalDataContext.EdOrgs.AddOrUpdate(singleEdOrg);
                }

                LoggingService.LogDebugMessage($"EdOrgCache: saving changes");
                validationPortalDataContext.SaveChanges();
            }
        }

        public SingleEdOrgByIdQuery GetSingleEdOrg(int edOrgId, int schoolYearId)
        {
            SingleEdOrgByIdQuery result = null;

            var schoolYear = SchoolYearService.GetSchoolYearById(schoolYearId);
            using (var odsRawDbContext = new RawOdsDbContext(schoolYear.EndYear))
            {
                var conn = odsRawDbContext.Database.Connection;
                try
                {
                    conn.Open();
                    var edOrgQueryCmd = conn.CreateCommand();
                    edOrgQueryCmd.CommandType = System.Data.CommandType.Text;
                    edOrgQueryCmd.CommandText = SingleEdOrgByIdQuery.EdOrgQuery;
                    edOrgQueryCmd.Parameters.Add(new SqlParameter("@edOrgId", System.Data.SqlDbType.Int));
                    edOrgQueryCmd.Parameters["@edOrgId"].Value = edOrgId;

                    using (var reader = edOrgQueryCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            result = new SingleEdOrgByIdQuery
                            {
                                Id = int.Parse(reader[SingleEdOrgByIdQuery.IdColumnName].ToString()),
                                ShortOrganizationName = reader[SingleEdOrgByIdQuery.OrganizationShortNameColumnName].ToString(),
                                OrganizationName = reader[SingleEdOrgByIdQuery.OrganizationNameColumnName].ToString(),
                                StateOrganizationId = reader[SingleEdOrgByIdQuery.StateOrganizationIdColumnName].ToString()
                            };
                        }
                    }
                }
                catch (Exception ex)
                {
                    LoggingService.LogErrorMessage(
                        $"While reading Ed Org description (ID# {edOrgId}, school year {schoolYear.ToString()}): {ex.ChainInnerExceptionMessages()}");
                }
                finally
                {
                    if (conn != null && conn.State != System.Data.ConnectionState.Closed)
                    {
                        try
                        {
                            conn.Close();
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }

            return result;
        }

        private List<EdOrg> ReadEdOrgs(DbCommand edOrgQueryCmd, int schoolYearId)
        {
            var result = new List<EdOrg>();
            try
            {
                LoggingService.LogDebugMessage($"EdOrg cache: Adding EdOrgs from year id {schoolYearId}");

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

                        LoggingService.LogDebugMessage($"Adding {theId}");
                    }
                }
            }
            catch (Exception ex)
            {
                LoggingService.LogErrorMessage($"An error occurred while trying to read the Ed Org information from the Ed Fi ODS referring to the school year with ID {schoolYearId}: {ex.ChainInnerExceptionMessages()}");
            }
            LoggingService.LogDebugMessage($"Added {result.Count} EdOrgs");

            return result;
        }
    }
}
