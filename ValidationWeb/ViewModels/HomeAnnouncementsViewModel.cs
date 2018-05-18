using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValidationWeb
{
    public class HomeAnnouncementsViewModel
    {
        public IEnumerable<Announcement> Announcements { get; set; }
        /// <summary>
        /// Ed Orgs the use is allowed to see.
        /// </summary>
        public IEnumerable<EdOrg> AuthorizedEdOrgs { get; set; }
        /// <summary>
        /// Subset of Ed Orgs currently chosen for viewing.
        /// </summary>
        public IEnumerable<EdOrg> FilteredEdOrgs { get; set; }
        public IEnumerable<SchoolYear> YearsOpenForDataSubmission { get; set; }
    }
}