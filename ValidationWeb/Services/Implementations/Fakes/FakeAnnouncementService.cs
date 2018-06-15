using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValidationWeb.Services
{
    public class FakeAnnouncementService : IAnnouncementService
    {
        protected readonly IEdOrgService _edOrgService;
        protected readonly IAppUserService _appUserService;

        private static List<Announcement> _fakeAnnouncements = new List<Announcement>
            {
                new Announcement
                {
                    Id = 0,
                    Expiration = DateTime.UtcNow.AddHours(1),
                    Priority = 0,
                    ContactInfo = "info@education.mn.gov",
                    IsEmergency = false,
                    LinkUrl = "https://education.mn.gov/",
                    Message = "You may know about the nice Department of Education web page. But have you been there lately?\r\nWhy don't you go have a look now?"
                },
                new Announcement
                {
                    Id = 1,
                    Expiration = DateTime.UtcNow.AddHours(1),
                    Priority = 1,
                    ContactInfo = "info@education.mn.gov",
                    IsEmergency = true,
                    LinkUrl = "https://education.mn.gov/",
                    Message = "A volcano erupted in Hawaii!"
                },
                // Specific to one district
                new Announcement
                {
                    Id = 2,
                    Expiration = DateTime.UtcNow.AddHours(1),
                    Priority = 0,
                    ContactInfo = "info@education.mn.gov",
                    IsEmergency = false,
                    LinkUrl = "https://education.mn.gov/",
                    Message = "Commissioner expresses pride in ISD 622.",
                    LimitToEdOrgs = new HashSet<EdOrg> { new EdOrg { Id = "ISD 622" } }
                },
                // Expired - shouldn't appear
                new Announcement
                {
                    Id = 3,
                    Expiration = DateTime.UtcNow.AddHours(-1),
                    Priority = 0,
                    ContactInfo = "info@education.mn.gov",
                    IsEmergency = false,
                    LinkUrl = "https://education.mn.gov/",
                    Message = "Good idea: Prepare for the Fall submission date."
                }
            };

        public FakeAnnouncementService(IEdOrgService edOrgService, IAppUserService appUserService)
        {
            _edOrgService = edOrgService;
            _appUserService = appUserService;
        }

        public List<Announcement> GetAnnoucements(int sessionId, bool includePreviouslyDismissedAnnouncements = false)
        {
            var dismissedAnnouncementIds = _appUserService.GetSession(sessionId).DismissedAnnouncements.Select(da => da.Id).ToArray();
            var edOrgIds = _appUserService.GetCurrentAppUser().AuthorizedEdOrgs.Select(aeo => aeo.Id).ToArray();
            return _fakeAnnouncements.Where(ann => includePreviouslyDismissedAnnouncements
            || (
                 (ann.LimitToEdOrgs == null || ann.LimitToEdOrgs.Any(lte => edOrgIds.Contains(lte.Id)))
                 && (!dismissedAnnouncementIds.Contains(ann.Id))
               )
            ).OrderByDescending(ann => ann.Priority).ToList();
        }
    }
}