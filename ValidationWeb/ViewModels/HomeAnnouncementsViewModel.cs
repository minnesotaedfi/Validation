using System.Collections.Generic;
using ValidationWeb.Models;

namespace ValidationWeb.ViewModels
{
    public class HomeAnnouncementsViewModel
    {
        public IEnumerable<Announcement> Announcements { get; set; }

        /// <summary>
        /// Gets or sets Ed Orgs the use is allowed to see.
        /// </summary>
        public IEnumerable<EdOrg> AuthorizedEdOrgs { get; set; }
        
        /// <summary>
        /// Gets or sets Subset of Ed Orgs currently chosen for viewing.
        /// </summary>
        public IEnumerable<EdOrg> FilteredEdOrgs { get; set; }
        
        public IEnumerable<SchoolYear> YearsOpenForDataSubmission { get; set; }
    }
}