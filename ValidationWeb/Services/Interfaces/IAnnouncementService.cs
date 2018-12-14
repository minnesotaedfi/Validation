using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValidationWeb.Services
{
    public interface IAnnouncementService
    {
        List<Announcement> GetAnnouncements(bool includePreviouslyDismissedAnnouncements = false);
        Announcement GetAnnouncement(int id);
        void DeleteAnnouncement(int announcementId);
        void SaveAnnouncement(int announcementId, int priority, string message, string contactInfo, string linkUrl, DateTime expirationDate);
    }
}
