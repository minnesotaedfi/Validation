using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValidationWeb
{
    public class AppUserSession
    {
        public EdOrg FocusedEdOrg { get; set; }
        public List<DismissedAnnouncement> DismissedAnnouncements { get; set; }
    }
}