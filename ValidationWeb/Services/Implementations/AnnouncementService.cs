using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValidationWeb.Services
{
    public class AnnouncementService : IAnnouncementService
    {
        public List<Announcement> GetAnnoucements()
        {
            return new List<Announcement>
            {
                new Announcement
                {
                    Priority = 0,
                    ContactInfo = "info@education.mn.gov",
                    IsEmergency = false,
                    LinkUrl = "https://education.mn.gov/",
                    Message = "You may know about the nice Department of Education web page. But have you been there lately?\r\nWhy don't you go have a look now?"
                },
                new Announcement
                {
                    Priority = 1,
                    ContactInfo = "info@education.mn.gov",
                    IsEmergency = true,
                    LinkUrl = "https://education.mn.gov/",
                    Message = "A volcano erupted in Hawaii!"
                }
            };
        }
    }
}