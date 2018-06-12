using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValidationWeb.Services
{
    public class FakeAppUserService : IAppUserService
    {
        public AppUser GetCurrentAppUser()
        {
            return new AppUser
            {
                AuthorizedEdOrgs = new FakeEdOrgService().GetEdOrgs(),
                Id = -1,
                Name = "Jane Educator"
            };
        }

        public AppUserSession GetSession(int sessionId)
        {
            return new AppUserSession
            {
                DismissedAnnouncements = Enumerable.Empty<DismissedAnnouncement>().ToList(),
                FocusedEdOrg = new FakeEdOrgService().GetEdOrgs().FirstOrDefault(edo => edo.Id == "ISD 622")
            };
        }
    }
}