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
        protected readonly ISchoolYearService _schoolYearService;

        public FakeAppUserService(IEdOrgService edOrgService, ISchoolYearService schoolYearService)
        {
            _edOrgService = edOrgService;
            _schoolYearService = schoolYearService;
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
                DismissedAnnouncements = new HashSet<DismissedAnnouncement>(),
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
                FocusedEdOrgId = specialEdOrg.Id,
                FocusedSchoolYearId = (_schoolYearService.GetSubmittableSchoolYears().Count() > 0) 
                    ? _schoolYearService.GetSubmittableSchoolYears().FirstOrDefault().Id 
                    : 0
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
            var theSession = GetSession();
            theSession.DismissedAnnouncements.Add(new DismissedAnnouncement { AnnouncementId = announcementId,  AppUserSessionId = theSession.Id });
        }
    }
}