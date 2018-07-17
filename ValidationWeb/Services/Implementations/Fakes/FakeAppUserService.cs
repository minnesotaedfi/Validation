using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValidationWeb.Services
{
    public class FakeAppUserService : IAppUserService
    {
        private static List<AppUserSession> _openSessions = new List<AppUserSession>();
        private AppUserSession currentSession = null;
        protected readonly IEdOrgService _edOrgService;

        public FakeAppUserService(IEdOrgService edOrgService)
        {
            _edOrgService = edOrgService;
        }
        public ValidationPortalIdentity GetUser()
        {
            var allOrgs = _edOrgService.GetEdOrgs();
            return new ValidationPortalIdentity
            {
                AppRole = AppRole.Administrator,
                AuthorizedEdOrgs = allOrgs,
                Email = "jane.educator@education.mn.edu",
                FirstName = "Jane",
                MiddleName = "Anne",
                LastName = "Educator",
                FullName = "Jane A. Educator",
                Name = "Jane",
                UserId = "Jane"
            };
        }

        /// <summary>
        /// Returns SessionId number.
        /// </summary>
        /// <returns></returns>
        private AppUserSession CreateAppUserSession()
        {
            var specialEdOrg = _edOrgService.GetEdOrgById("ISD 622");
            var allOrgs = _edOrgService.GetEdOrgs();

            currentSession = new AppUserSession
            {
                Id = (new Random()).Next(1, 1000000).ToString(),
                DismissedAnnouncements = new HashSet<Announcement>(),
                ExpiresUtc = DateTime.UtcNow.AddHours(1),
                UserIdentity = new ValidationPortalIdentity
                {
                    AppRole = AppRole.Administrator,
                    AuthorizedEdOrgs = allOrgs,
                    Email = "jane.educator@education.mn.edu",
                    FirstName = "Jane",
                    MiddleName = "Anne",
                    LastName = "Educator",
                    FullName = "Jane A. Educator",
                    Name = "Jane",
                    UserId = "Jane"
                },
                FocusedEdOrgId = specialEdOrg.Id
            };

            _openSessions.Add(currentSession);

            return currentSession;
        }

        public AppUserSession GetSession()
        {
            return _openSessions.FirstOrDefault() ?? CreateAppUserSession();
        }

        public void DismissAnnouncement(int announcementId)
        {
            GetSession().DismissedAnnouncements.Add(new Announcement { Id = announcementId });
        }
    }
}