using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValidationWeb
{
    public class DismissedAnnouncement
    {
        public int AppUserId { get; set; }
        public int AnnouncementUserId { get; set; }
        public DateTime PurgeAfterDate { get; set; }
    }
}