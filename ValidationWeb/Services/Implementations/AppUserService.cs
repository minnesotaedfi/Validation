using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValidationWeb.Services
{
    public class AppUserService : IAppUserService
    {
        public const string SessionItemName = "Session";
        protected readonly ValidationPortalDbContext _validationPortalDataContext;
        protected readonly IHttpContextProvider _httpContextProvider;
        protected readonly ILoggingService _loggingService;


        public AppUserService(
            ValidationPortalDbContext validationPortalDataContext, 
            IHttpContextProvider httpContextProvider,
            ILoggingService loggingService)
        {
            _validationPortalDataContext = validationPortalDataContext;
            _httpContextProvider = httpContextProvider;
            _loggingService = loggingService;
        }

        public void DismissAnnouncement(int announcementId)
        {
            var session = GetSession();
            session.DismissedAnnouncements.Add(new DismissedAnnouncement { AnnouncementId = announcementId, AppUserSessionId = session.Id });
            _validationPortalDataContext.SaveChanges();
            UpdateUserSession(session);
        }

        public AppUserSession GetSession()
        {
            return _httpContextProvider.CurrentHttpContext.Items[SessionItemName] as AppUserSession;
        }

        public ValidationPortalIdentity GetUser()
        {
            return GetSession().UserIdentity;
        }

        public void UpdateFocusedEdOrg(string newFocusedEdOrgId)
        {
            _loggingService.LogDebugMessage($"Attempting to update focused Ed Org ID to {newFocusedEdOrgId}.");
            try
            {
                var desiredEdOrgId = int.Parse(newFocusedEdOrgId);
                var sessionObj = GetSession();
                if (sessionObj == null || sessionObj.UserIdentity.AuthorizedEdOrgs.FirstOrDefault(eo => eo.Id == desiredEdOrgId) == null)
                {
                    return;
                }
                _validationPortalDataContext.AppUserSessions.Where(sess => sess.Id == sessionObj.Id).First().FocusedEdOrgId = desiredEdOrgId;
                _validationPortalDataContext.SaveChanges();
            }
            catch(Exception ex)
            {
                _loggingService.LogErrorMessage($"An error occurred when updating focused Ed Org ID to {newFocusedEdOrgId}: {ex.ChainInnerExceptionMessages()}");
            }
        }

        public void UpdateFocusedSchoolYear(int newFocusedSchoolYearId)
        {
            _loggingService.LogDebugMessage($"Attempting to update focused School Year ID to {newFocusedSchoolYearId.ToString()}.");
            try
            {
                var sessionObj = GetSession();
                int? validSchoolYearId = _validationPortalDataContext.SchoolYears.FirstOrDefault(sy => sy.Enabled && sy.Id == newFocusedSchoolYearId).Id;
                if (sessionObj == null || !(validSchoolYearId.HasValue))
                {
                    return;
                }
                _validationPortalDataContext.AppUserSessions.Where(sess => sess.Id == sessionObj.Id).First().FocusedSchoolYearId = validSchoolYearId.Value;
                _validationPortalDataContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _loggingService.LogErrorMessage($"An error occurred when updating focused School Year ID to {newFocusedSchoolYearId.ToString()}: {ex.ChainInnerExceptionMessages()}");
            }
        }

        
        private void UpdateUserSession(AppUserSession session)
        {
            _httpContextProvider.CurrentHttpContext.Items[SessionItemName] = session;
        }
    }
}