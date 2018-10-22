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

        public AppUserService(
            ValidationPortalDbContext validationPortalDataContext, 
            IHttpContextProvider httpContextProvider)
        {
            _validationPortalDataContext = validationPortalDataContext;
            _httpContextProvider = httpContextProvider;
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
            var desiredEdOrgId = int.Parse(newFocusedEdOrgId);
            var sessionObj = GetSession();
            if (sessionObj == null || sessionObj.UserIdentity.AuthorizedEdOrgs.FirstOrDefault(eo => eo.Id == desiredEdOrgId) == null)
            {
                return;
            }
            _validationPortalDataContext.AppUserSessions.Where(sess => sess.Id == sessionObj.Id).First().FocusedEdOrgId = desiredEdOrgId;
            _validationPortalDataContext.SaveChanges();
        }

        public void UpdateFocusedSchoolYear(int newFocusedSchoolYearId)
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

        
        private void UpdateUserSession(AppUserSession session)
        {
            _httpContextProvider.CurrentHttpContext.Items[SessionItemName] = session;
        }
    }
}