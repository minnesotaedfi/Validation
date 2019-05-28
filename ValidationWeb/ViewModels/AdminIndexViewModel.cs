using System.Collections.Generic;
using Engine.Models;
using ValidationWeb.Models;

namespace ValidationWeb.ViewModels
{
    public class AdminIndexViewModel
    {
        public AppUserSession AppUserSession { get; set; }

        /// <summary>
        /// Gets or sets Ed Orgs the use is allowed to see.
        /// </summary>
        public IEnumerable<EdOrg> AuthorizedEdOrgs { get; set; }

        /// <summary>
        /// Gets or sets the Ed Org currently selected (being acting on when a user takes actions designed to affect a single EdOrg)
        /// </summary>
        public EdOrg FocusedEdOrg { get; set; }

        public IEnumerable<SchoolYear> YearsOpenForDataSubmission { get; set; }

        public IEnumerable<Collection> RuleCollections { get; set; }
        
        public IEnumerable<SubmissionCycle> SubmissionCycles { get; set; }
        
        public IEnumerable<Announcement> Announcements { get; set; }
    }
}