using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;

using Validation.DataModels;

using ValidationWeb.Database;
using ValidationWeb.Models;
using ValidationWeb.Services.Interfaces;
using ValidationWeb.Utility;

namespace ValidationWeb.Services.Implementations
{
    public class AnnouncementService : IAnnouncementService
    {
        protected readonly IDbContextFactory<ValidationPortalDbContext> DbContextFactory;

        protected readonly IAppUserService AppUserService;

        protected readonly ILoggingService LoggingService;

        public AnnouncementService(
            IDbContextFactory<ValidationPortalDbContext> dbContextFactory,
            IAppUserService appUserService,
            ILoggingService loggingService)
        {
            DbContextFactory = dbContextFactory;
            AppUserService = appUserService;
            LoggingService = loggingService;
        }

        public List<Announcement> GetAnnouncements(ProgramArea programArea = null)
        {
            var announcements = new List<Announcement>();
            try
            {
                using (var dbContext = DbContextFactory.Create())
                {
                    announcements = dbContext.Announcements.OrderByDescending(ann => ann.Priority).ToList();
                    if (programArea != null)
                    {
                        announcements = announcements.Where(x => x.ProgramAreaId == programArea.Id).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                LoggingService.LogErrorMessage($"An error occurred while retrieving announcements: {ex.ChainInnerExceptionMessages()}");
            }

            return announcements;
        }

        public Announcement GetAnnouncement(int id)
        {
            Announcement announcement;

            try
            {
                using (var dbContext = DbContextFactory.Create())
                {
                    announcement = dbContext.Announcements.First(x => x.Id == id);
                }
            }
            catch (Exception ex)
            {
                LoggingService.LogErrorMessage($"An error occurred while retrieving announcement by id {id}: {ex.ChainInnerExceptionMessages()}");
                throw new Exception("An error occurred while retrieving announcement", ex); 
            }

            return announcement;
        }

        public void SaveAnnouncement(int announcementId, int priority, string message, string contactInfo, string linkUrl, DateTime expirationDate)
        {
            try
            {
                if (announcementId > 0)
                {
                    SaveExistingAnnouncement(announcementId, priority, message, contactInfo, linkUrl, expirationDate);
                }
                else
                {
                    SaveNewAnnouncement(priority, message, contactInfo, linkUrl, expirationDate);
                }
            }
            catch (Exception ex)
            {
                LoggingService.LogErrorMessage($"An error occurred while saving announcement with announcementId = {announcementId}, priority = {priority}, message = {message}, contactInfo = {contactInfo}, linkUrl = {linkUrl}, expirationDate = {expirationDate}: {ex.ChainInnerExceptionMessages()}");
                throw new Exception("An error occurred while saving announcement.");
            }
        }

        public void DeleteAnnouncement(int announcementId)
        {
            using (var dbContext = DbContextFactory.Create())
            {
                var announcement = dbContext.Announcements.FirstOrDefault(a => a.Id == announcementId);
                if (announcement == null)
                {
                    throw new Exception($"Could not delete an announcement because announcement with ID {announcementId} was not found");
                }

                dbContext.Announcements.Remove(announcement);
                dbContext.SaveChanges();
            }
        }

        private void SaveExistingAnnouncement(
            int announcementId,
            int priority,
            string message,
            string contactInfo,
            string linkUrl,
            DateTime expirationDate)
        {
            using (var dbContext = DbContextFactory.Create())
            {
                var announcement = dbContext.Announcements.FirstOrDefault(a => a.Id == announcementId);
                if (announcement == null)
                {
                    throw new Exception(
                        $"Could not save an announcement because announcement with ID {announcementId} was not found");
                }

                announcement.Priority = priority;
                announcement.Message = message;
                announcement.ContactInfo = contactInfo;
                announcement.LinkUrl = linkUrl;
                announcement.Expiration = expirationDate;

                dbContext.SaveChanges();
            }
        }

        private void SaveNewAnnouncement(
            int priority,
            string message,
            string contactInfo,
            string linkUrl,
            DateTime expirationDate)
        {
            using (var dbContext = DbContextFactory.Create())
            {
                var announcement = new Announcement
                {
                    Priority = priority,
                    Message = message,
                    ContactInfo = contactInfo,
                    LinkUrl = linkUrl,
                    IsEmergency = false,
                    Expiration = expirationDate
                };

                dbContext.Announcements.Add(announcement);
                dbContext.SaveChanges();
            }
        }
    }
}