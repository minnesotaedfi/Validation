using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValidationWeb.Services
{
    public interface IAnnouncementService
    {
        List<Announcement> GetAnnoucements(bool includePreviouslyDismissedAnnouncements = false);
        Announcement GetAnnoucement(int id);
        void DeleteAnnoucement(int announcementId);
        void SaveAnnouncement(int announcementId, int priority, string message, string contactInfo, string linkUrl, DateTime expirationDate);
    }
}
