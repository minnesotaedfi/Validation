using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
using ValidationWeb.Database;
using ValidationWeb.Models;
using ValidationWeb.Services.Interfaces;
using ValidationWeb.Utility;

namespace ValidationWeb.Services.Implementations
{
    public class AppUserService : IAppUserService
    {
        public const string SessionItemName = "Session";

        protected IDbContextFactory<ValidationPortalDbContext> ValidationPortalDataContextFactory { get; set; }
        
        protected IHttpContextProvider HttpContextProvider { get; set; }
        
        protected ILoggingService LoggingService { get; set; }
        
        public AppUserService(
            IDbContextFactory<ValidationPortalDbContext> validationPortalDataContextFactory, 
            IHttpContextProvider httpContextProvider,
            ILoggingService loggingService)
        {
            ValidationPortalDataContextFactory = validationPortalDataContextFactory;
            HttpContextProvider = httpContextProvider;
            LoggingService = loggingService;
        }
        
        public AppUserSession GetSession()
        {
            return HttpContextProvider.CurrentHttpContext.Items[SessionItemName] as AppUserSession;
        }

        public ValidationPortalIdentity GetUser()
        {
            return GetSession().UserIdentity;
        }

        public void UpdateFocusedEdOrg(string newFocusedEdOrgId)
        {
            LoggingService.LogDebugMessage($"Attempting to update focused Ed Org ID to {newFocusedEdOrgId}.");
            try
            {
                using (var validationPortalDataContext = ValidationPortalDataContextFactory.Create())
                {
                    var desiredEdOrgId = int.Parse(newFocusedEdOrgId);
                    var sessionObj = GetSession();
                    if (sessionObj?.UserIdentity.AuthorizedEdOrgs.FirstOrDefault(eo => eo.Id == desiredEdOrgId) == null)
                    {
                        return; // todo: why silently fail? 
                    }

                    validationPortalDataContext.AppUserSessions
                        .First(x => x.Id == sessionObj.Id)
                        .FocusedEdOrgId = desiredEdOrgId;   

                    validationPortalDataContext.SaveChanges();
                }
            }
            catch(Exception ex)
            {
                LoggingService.LogErrorMessage($"An error occurred when updating focused Ed Org ID to {newFocusedEdOrgId}: {ex.ChainInnerExceptionMessages()}");
            }
        }

        public void UpdateFocusedSchoolYear(int newFocusedSchoolYearId)
        {
            LoggingService.LogDebugMessage($"Attempting to update focused School Year ID to {newFocusedSchoolYearId.ToString()}.");
            try
            {
                using (var validationPortalDataContext = ValidationPortalDataContextFactory.Create())
                {
                    var sessionObj = GetSession();

                    // todo: null ref
                    int? validSchoolYearId = validationPortalDataContext.SchoolYears
                        .FirstOrDefault(sy => sy.Enabled && sy.Id == newFocusedSchoolYearId)?.Id;

                    if (sessionObj == null || !validSchoolYearId.HasValue)
                    {
                        LoggingService.LogErrorMessage("Unable to set an invalid school year");
                        return; // todo: another silent fail! 
                    }

                    validationPortalDataContext.AppUserSessions
                        .First(x => x.Id == sessionObj.Id)  
                        .FocusedSchoolYearId = validSchoolYearId.Value;
                    
                    validationPortalDataContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                LoggingService.LogErrorMessage($"An error occurred when updating focused School Year ID to {newFocusedSchoolYearId.ToString()}: {ex.ChainInnerExceptionMessages()}");
            }
        }

        public void UpdateFocusedProgramArea(int newFocusedProgramAreaId)
        {
            LoggingService.LogDebugMessage($"Attempting to update focused Program Area ID to {newFocusedProgramAreaId.ToString()}.");
            try
            {
                using (var validationPortalDataContext = ValidationPortalDataContextFactory.Create())
                {
                    var sessionObj = GetSession();

                    // todo: null ref
                    int? validProgramAreaId = validationPortalDataContext.ProgramAreas
                        .FirstOrDefault(x => x.Id == newFocusedProgramAreaId)?.Id;

                    if (sessionObj == null || !validProgramAreaId.HasValue)
                    {
                        LoggingService.LogErrorMessage("Unable to set an invalid Program Area");
                        return; // todo: another silent fail! 
                    }

                    validationPortalDataContext.AppUserSessions
                        .First(x => x.Id == sessionObj.Id)
                        .FocusedProgramAreaId = validProgramAreaId.Value;

                    validationPortalDataContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                LoggingService.LogErrorMessage($"An error occurred when updating focused School Year ID to {newFocusedProgramAreaId.ToString()}: {ex.ChainInnerExceptionMessages()}");
            }
        }
    }
}