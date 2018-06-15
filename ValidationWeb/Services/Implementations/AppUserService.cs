using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValidationWeb.Services
{
    public class AppUserService : IAppUserService
    {
        protected readonly ValidationPortalDbContext _validationPortalDataContext;
        protected readonly IEdOrgService _edOrgService;

        public AppUserService(ValidationPortalDbContext validationPortalDataContext, IEdOrgService edOrgService)
        {
            _validationPortalDataContext = validationPortalDataContext;
            _edOrgService = edOrgService;
        }

        public int CreateAppUserSession(int appUserId)
        {
            var allOrgs = _edOrgService.GetEdOrgs();
            var specialEdOrg = _edOrgService.GetEdOrgById("ISD 622");

            var currentSession = new AppUserSession
            {
                AppUser = new AppUser { Id = -1, Name = "Jane Educator", AuthorizedEdOrgs = allOrgs },
                AppUserId = -1,
                FocusedEdOrg = specialEdOrg,
                FocusedEdOrgId = specialEdOrg.Id
            };

            _validationPortalDataContext.AppUserSessions.Add(currentSession);
            _validationPortalDataContext.SaveChanges();

            return currentSession.Id;
        }

        public void DismissAnnouncement(int sessionId, int announcementId)
        {
            GetSession(sessionId).DismissedAnnouncements.Add(_validationPortalDataContext.Announcements.First(ann => ann.Id == announcementId));
        }

        public AppUser GetCurrentAppUser(int sessionId)
        {
            return GetSession(sessionId).AppUser;
        }

        public AppUserSession GetSession(int sessionId)
        {
            return _validationPortalDataContext.AppUserSessions.First(sess => sess.Id == sessionId);
        }
    }
}