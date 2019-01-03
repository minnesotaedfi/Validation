namespace ValidationWeb.Services
{
    using System;
    using System.Data.Entity.Infrastructure;
    using System.Linq;

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

        public void DismissAnnouncement(int announcementId)
        {
            using (var validationPortalDataContext = ValidationPortalDataContextFactory.Create())
            {
                var session = GetSession();
                session.DismissedAnnouncements.Add(
                    new DismissedAnnouncement
                    {
                        AnnouncementId = announcementId,
                        AppUserSessionId = session.Id
                    });

                validationPortalDataContext.SaveChanges();
                UpdateUserSession(session);
            }
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
                    if (sessionObj == null
                        || sessionObj.UserIdentity.AuthorizedEdOrgs.FirstOrDefault(eo => eo.Id == desiredEdOrgId)
                        == null)
                    {
                        return;
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
                        .FirstOrDefault(sy => sy.Enabled && sy.Id == newFocusedSchoolYearId).Id;

                    if (sessionObj == null || !validSchoolYearId.HasValue)
                    {
                        return;
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

        private void UpdateUserSession(AppUserSession session)
        {
            HttpContextProvider.CurrentHttpContext.Items[SessionItemName] = session;
        }
    }
}