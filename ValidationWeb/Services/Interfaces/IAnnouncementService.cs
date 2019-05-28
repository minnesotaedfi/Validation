using System;
using System.Collections.Generic;
using ValidationWeb.Models;

namespace ValidationWeb.Services.Interfaces
{
    public interface IAnnouncementService
    {
        List<Announcement> GetAnnouncements();
        
        Announcement GetAnnouncement(int id);
        
        void DeleteAnnouncement(int announcementId);

        void SaveAnnouncement(
            int announcementId,
            int priority,
            string message,
            string contactInfo,
            string linkUrl,
            DateTime expirationDate);
    }
}
