using System;
using System.Collections.Generic;
using System.Data.Entity;
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
                    announcements = dbContext.Announcements
                        .Include(x => x.ProgramArea)
                        .OrderByDescending(ann => ann.Priority)
                        .ToList();

                    if (programArea != null)
                    {
                        announcements = announcements
                            .Where(x => x.ProgramAreaId == null || x.ProgramAreaId == programArea.Id)
                            .ToList();
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
                    announcement = dbContext.Announcements
                        .Include(x => x.ProgramArea)
                        .First(x => x.Id == id);
                }
            }
            catch (Exception ex)
            {
                LoggingService.LogErrorMessage($"An error occurred while retrieving announcement by id {id}: {ex.ChainInnerExceptionMessages()}");
                throw new Exception("An error occurred while retrieving announcement", ex); 
            }

            return announcement;
        }

        public void SaveAnnouncement(Announcement announcement)
        {
            try
            {
                if (announcement.Id > 0)
                {
                    SaveExistingAnnouncement(announcement);
                }
                else
                {
                    SaveNewAnnouncement(announcement);
                }
            }
            catch (Exception ex)
            {
                LoggingService.LogErrorMessage($"An error occurred while saving announcement with announcementId = {announcement?.Id}: {ex.ChainInnerExceptionMessages()}");
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

        private void SaveExistingAnnouncement(Announcement announcement)
        {
            using (var dbContext = DbContextFactory.Create())
            {
                var existingAnnouncement = dbContext.Announcements.FirstOrDefault(a => a.Id == announcement.Id);
                if (existingAnnouncement == null)
                {
                    throw new Exception($"Could not save an announcement because announcement with ID {announcement.Id} was not found");
                }

                existingAnnouncement.Priority = announcement.Priority;
                existingAnnouncement.Message = announcement.Message;
                existingAnnouncement.ContactInfo = announcement.ContactInfo;
                existingAnnouncement.LinkUrl = announcement.LinkUrl;
                existingAnnouncement.Expiration = announcement.Expiration;
                existingAnnouncement.ProgramAreaId = announcement.ProgramAreaId;

                dbContext.SaveChanges();
            }
        }

        private void SaveNewAnnouncement(Announcement announcement)
        {
            using (var dbContext = DbContextFactory.Create())
            {
                dbContext.Announcements.Add(announcement);
                dbContext.SaveChanges();
            }
        }
    }
}