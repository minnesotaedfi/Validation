using System;
using System.Collections.Generic;
using System.Linq;
//using System.Web;

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

        public Announcement GetAnnoucement(int id)
        {
            //var announcements = new List<Announcement>();
            Announcement announcement = null;
            try
            {
                var edOrgIds = _appUserService.GetSession().UserIdentity.AuthorizedEdOrgs.Select(aeo => aeo.Id).ToArray();
                //var allAnnouncements = _validationPortalDataContext.Announcements.ToList();
                announcement = _validationPortalDataContext.Announcements.Where(ann =>
                    (ann.LimitToEdOrgs.Count == 0) ||
                    (ann.LimitToEdOrgs.Select(lte => lte.Id).Intersect(edOrgIds).Count() > 0))
                    //.FirstOrDefault(a => a.Id == id);
                    .First(x => x.Id == id);
            }
            catch (Exception ex)
            {
                _loggingService.LogErrorMessage($"An error occurred while retrieving announcement by id {id}: {ex.ChainInnerExceptionMessages()}");
            }
            return announcement;
        }


        //public bool AddAnnouncement(int priority, string message, string contactInfo, string linkUrl, string expirationDateStr)
        public bool AddAnnouncementFromBody(Announcement announcement)
        {
            /***
            List<string> invalidFieldNames = new List<string>();
            if (string.IsNullOrEmpty(announcement.Message))
                invalidFieldNames.Add("Message");
            if (string.IsNullOrEmpty(announcement.Expiration))
                invalidFieldNames.Add("Expiration Date");
            if (invalidFieldNames.Count() > 0)
            {
                _loggingService.LogErrorMessage($"Could not create an announcement because the following announcement fields were null: {String.Join(",", invalidFieldNames)}");
                return false;
            }

            DateTime expirationDate;
            var dateValid = DateTime.TryParse(expirationDateStr, out expirationDate);
            if (!dateValid)
            {
                _loggingService.LogErrorMessage($"Could not create an announcement because Expiration Date {expirationDateStr} is not a valid date.");
                return false;
            }

            Announcement announcement = new Announcement
            {
                Priority = priority,
                Message = message,
                ContactInfo = contactInfo,
                LinkUrl = linkUrl,
                IsEmergency = false,
                Expiration = expirationDate
            };
            ***/
            _validationPortalDataContext.Announcements.Add(announcement);
            _validationPortalDataContext.SaveChanges();

            return true;
        }


        //public bool SaveAnnouncement(int announcementId, int priority, string message, string contactInfo, string linkUrl, string expirationDateStr)
        public bool SaveAnnouncement(int announcementId, int priority, string message, string contactInfo, string linkUrl, DateTime expirationDate)
        {
            List<string> invalidFieldNames = new List<string>();
            if (string.IsNullOrEmpty(message))
                invalidFieldNames.Add("Message");
            //if (string.IsNullOrEmpty(expirationDateStr))
                //invalidFieldNames.Add("Expiration Date");
            if (invalidFieldNames.Count() > 0)
            {
                string errorMessage = $"Could not save an announcement because the following announcement fields were null: {String.Join(",", invalidFieldNames)}";
                _loggingService.LogErrorMessage(errorMessage);
                throw new Exception(errorMessage);
            }

            /**
            DateTime expirationDate;
            var dateValid = DateTime.TryParse(expirationDateStr, out expirationDate);
            if (!dateValid)
            {
                string errorMessage = $"Could not save an announcement because Expiration Date {expirationDateStr} is not a valid date.";
                _loggingService.LogErrorMessage(errorMessage);
                throw new Exception(errorMessage);
            }
    ***/

            if (announcementId > 0)
                SaveExistingAnnouncement(announcementId, priority, message, contactInfo, linkUrl, expirationDate);
            else
                SaveNewAnnouncement(priority, message, contactInfo, linkUrl, expirationDate);
            return true;
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
    }
}