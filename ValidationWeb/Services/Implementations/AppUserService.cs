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

        private void UpdateUserSession(AppUserSession session)
        {
            _httpContextProvider.CurrentHttpContext.Items[SessionItemName] = session;
        }
    }
}