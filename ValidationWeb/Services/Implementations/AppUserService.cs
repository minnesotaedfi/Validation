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
            GetSession().DismissedAnnouncements.Add(_validationPortalDataContext.Announcements.First(ann => ann.Id == announcementId));
            _validationPortalDataContext.SaveChanges();
        }

        public AppUserSession GetSession()
        {
            return _httpContextProvider.CurrentHttpContext.Items[SessionItemName] as AppUserSession;
        }

        public ValidationPortalIdentity GetUser()
        {
            return GetSession().UserIdentity;
        }
    }
}