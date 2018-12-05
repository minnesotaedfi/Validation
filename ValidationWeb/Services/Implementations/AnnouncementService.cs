using System;
using System.Collections.Generic;
using System.Linq;

namespace ValidationWeb.Services
{
    public class AnnouncementService : IAnnouncementService
    {
        protected readonly IValidationPortalDbContext _validationPortalDataContext;
        protected readonly IAppUserService _appUserService;
        protected readonly ILoggingService _loggingService;

        public AnnouncementService(
            IValidationPortalDbContext validationPortalDataContext,
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

        public Announcement GetAnnoucement(int id)
        {
            Announcement announcement = null;
            try
            {
                var edOrgIds = _appUserService.GetSession().UserIdentity.AuthorizedEdOrgs.Select(aeo => aeo.Id).ToArray();
                announcement = _validationPortalDataContext.Announcements.Where(ann =>
                    (ann.LimitToEdOrgs.Count == 0) ||
                    (ann.LimitToEdOrgs.Select(lte => lte.Id).Intersect(edOrgIds).Count() > 0))
                    .First(x => x.Id == id);
            }
            catch (Exception ex)
            {
                _loggingService.LogErrorMessage($"An error occurred while retrieving announcement by id {id}: {ex.ChainInnerExceptionMessages()}");
                throw new Exception($"An error occurred while retrieving announcement");
            }
            return announcement;
        }

        public void SaveAnnouncement(int announcementId, int priority, string message, string contactInfo, string linkUrl, DateTime expirationDate)
        {
            try
            {
                if (announcementId > 0)
                    SaveExistingAnnouncement(announcementId, priority, message, contactInfo, linkUrl, expirationDate);
                else
                    SaveNewAnnouncement(priority, message, contactInfo, linkUrl, expirationDate);
            }
            catch (Exception ex)
            {
                _loggingService.LogErrorMessage($"An error occurred while saving announcement with announcementId = {announcementId}, priority = {priority}, message = {message}, contactInfo = {contactInfo}, linkUrl = {linkUrl}, expirationDate = {expirationDate}: {ex.ChainInnerExceptionMessages()}");
                throw new Exception("An error occurred while saving announcement.");
            }
        }

        private void SaveExistingAnnouncement(int announcementId, int priority, string message, string contactInfo, string linkUrl, DateTime expirationDate)
        {
            var announcement = _validationPortalDataContext.Announcements.FirstOrDefault(a => a.Id == announcementId);
            if (announcement == null)
            {
                throw new Exception($"Could not save an announcement because announcement with ID {announcementId} was not found");
            }
            announcement.Priority = priority;
            announcement.Message = message;
            announcement.ContactInfo = contactInfo;
            announcement.LinkUrl = linkUrl;
            announcement.Expiration = expirationDate;

            _validationPortalDataContext.SaveChanges();
        }

        private void SaveNewAnnouncement(int priority, string message, string contactInfo, string linkUrl, DateTime expirationDate)
        {
            Announcement announcement = new Announcement
            {
                Priority = priority,
                Message = message,
                ContactInfo = contactInfo,
                LinkUrl = linkUrl,
                IsEmergency = false,
                Expiration = expirationDate
            };
            _validationPortalDataContext.Announcements.Add(announcement);
            _validationPortalDataContext.SaveChanges();
        }

        public void DeleteAnnoucement(int announcementId)
        {
            var announcement = _validationPortalDataContext.Announcements.FirstOrDefault(a => a.Id == announcementId);
            if (announcement == null)
            {
                throw new Exception($"Could not delete an announcement because announcement with ID {announcementId} was not found");
            }
            _validationPortalDataContext.Announcements.Remove(announcement);
            _validationPortalDataContext.SaveChanges();
        }
    }
}