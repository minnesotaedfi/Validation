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
        bool AddAnnouncement(int priority, string message, string contactInfo, string linkUrl, string expirationDateStr);
        bool AddAnnouncementFromBody(Announcement announcement);
    }
}
