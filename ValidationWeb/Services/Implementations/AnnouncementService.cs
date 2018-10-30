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
        protected readonly ILoggingService _loggingService;

        public AnnouncementService(
            ValidationPortalDbContext validationPortalDataContext,
            IAppUserService appUserService,
            ILoggingService loggingService)
        {
            _validationPortalDataContext = validationPortalDataContext;
            _appUserService = appUserService;
            _loggingService = loggingService;
        }

        public List<Announcement> GetAnnoucements(bool includePreviouslyDismissedAnnouncements = false)
        {
            var announcements = new List<Announcement>();
            try
            {
                var dismissedAnnouncementIds = _appUserService.GetSession().DismissedAnnouncements.Select(da => da.AnnouncementId).ToArray();
                var edOrgIds = _appUserService.GetSession().UserIdentity.AuthorizedEdOrgs.Select(aeo => aeo.Id).ToArray();
                var allAnnouncements = _validationPortalDataContext.Announcements.ToList();
                announcements = allAnnouncements.Where(ann =>
                    (ann.LimitToEdOrgs.Count == 0) ||
                    (ann.LimitToEdOrgs.Select(lte => lte.Id).Intersect(edOrgIds).Count() > 0
                    && !dismissedAnnouncementIds.Contains(ann.Id))
                    ).OrderByDescending(ann => ann.Priority).ToList();
            }
            catch(Exception ex)
            {
                _loggingService.LogErrorMessage($"An error occurred while retrieving announcements: {ex.ChainInnerExceptionMessages()}");
            }
            return announcements;
        }
    }
}