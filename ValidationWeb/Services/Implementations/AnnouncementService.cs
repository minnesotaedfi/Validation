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

        public List<Announcement> GetAnnoucements(bool includePreviouslyDismissedAnnouncements = false)
        {
            var dismissedAnnouncementIds = _appUserService.GetSession().DismissedAnnouncements.Select(da => da.AnnouncementId).ToArray();
            var edOrgIds = _appUserService.GetSession().UserIdentity.AuthorizedEdOrgs.Select(aeo => aeo.Id).ToArray();
            var allAnnouncements = _validationPortalDataContext.Announcements.ToList();
            var announcements = allAnnouncements.Where(ann =>
                (ann.LimitToEdOrgs.Count == 0) ||
                (ann.LimitToEdOrgs.Select(lte => lte.Id).Intersect(edOrgIds).Count() > 0
                && !dismissedAnnouncementIds.Contains(ann.Id))
                ).OrderByDescending(ann => ann.Priority).ToList();

            return announcements;
        }
    }
}