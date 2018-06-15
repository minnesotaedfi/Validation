using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValidationWeb.Services
{
    public class AnnouncementService : IAnnouncementService
    {
        protected readonly ValidationPortalDbContext _validationPortalDataContext;
        protected readonly IAppUserService _appUserService;

        public AnnouncementService(
            ValidationPortalDbContext validationPortalDataContext,
            IAppUserService appUserService)
        {
            _validationPortalDataContext = validationPortalDataContext;
            _appUserService = appUserService;
        }

        public List<Announcement> GetAnnoucements(int sessionId, bool includePreviouslyDismissedAnnouncements = false)
        {
            var dismissedAnnouncementIds = _appUserService.GetSession(sessionId).DismissedAnnouncements.Select(da => da.Id).ToArray();
            var edOrgIds = _appUserService.GetCurrentAppUser().AuthorizedEdOrgs.Select(aeo => aeo.Id).ToArray(); 
            return _validationPortalDataContext.Announcements.Where(ann => includePreviouslyDismissedAnnouncements 
            || ( 
                 (ann.LimitToEdOrgs == null || ann.LimitToEdOrgs.Any(lte => edOrgIds.Contains(lte.Id))) 
                 && (! dismissedAnnouncementIds.Contains(ann.Id))
               )
            ).OrderByDescending(ann => ann.Priority).ToList();
        }
    }
}