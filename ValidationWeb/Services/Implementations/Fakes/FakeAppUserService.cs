using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValidationWeb.Services
{
    public class FakeAppUserService : IAppUserService
    {
        private static List<AppUserSession> _openSessions = new List<AppUserSession>();

        protected readonly IEdOrgService _edOrgService;

        private AppUserSession currentSession = null;

        public FakeAppUserService(IEdOrgService edOrgService)
        {
            _edOrgService = edOrgService;
        }

        public AppUser GetCurrentAppUser(int sessionId)
        {
            return GetSession(sessionId).AppUser;
        }

        /// <summary>
        /// Returns SessionId number.
        /// </summary>
        /// <returns></returns>
        public int CreateAppUserSession(int appUserId)
        {
            var specialEdOrg = _edOrgService.GetEdOrgById("ISD 622");
            var allOrgs = _edOrgService.GetEdOrgs();
            var initalAppUser = new AppUser { Id = -1, Name = "Jane Educator", AuthorizedEdOrgs = allOrgs };

            currentSession = new AppUserSession
            {
                Id = (new Random()).Next(1, 1000000),
                AppUser = initalAppUser,
                AppUserId = initalAppUser.Id,
                FocusedEdOrg = specialEdOrg,
                FocusedEdOrgId = specialEdOrg.Id
            };

            _openSessions.Add(currentSession);

            return currentSession.Id;
        }

        public AppUserSession GetSession(int sessionId)
        {
            return _openSessions.First(sess => sess.Id == sessionId);
        }

        public void DismissAnnouncement(int sessionId, int announcementId)
        {
            _openSessions.First(sess => sess.Id == sessionId).DismissedAnnouncements.Add(new Announcement { Id = announcementId });
        }
    }
}