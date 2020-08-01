using System;
using System.Collections.Generic;

using Validation.DataModels;

using ValidationWeb.Models;

namespace ValidationWeb.Services.Interfaces
{
    public interface IAnnouncementService
    {
        List<Announcement> GetAnnouncements(ProgramArea programArea = null);
        
        Announcement GetAnnouncement(int id);
        
        void DeleteAnnouncement(int announcementId);

        void SaveAnnouncement(Announcement announcement);
    }
}
