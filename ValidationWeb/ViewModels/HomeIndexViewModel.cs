using System.Collections.Generic;

using Validation.DataModels;

using ValidationWeb.Models;

namespace ValidationWeb.ViewModels
{
    public class HomeIndexViewModel
    {
        public AppUserSession AppUserSession { get; set; }
        
        public IEnumerable<Announcement> Announcements { get; set; }
        
        /// <summary>
        /// Gets or sets Ed Orgs the use is allowed to see.
        /// </summary>
        public IEnumerable<EdOrg> AuthorizedEdOrgs { get; set; }
        
        /// <summary>
        /// Gets or sets Ed Org currently selected (being acting on when a user takes actions designed to affect a single EdOrg)
        /// </summary>
        public EdOrg FocusedEdOrg { get; set; }

        public ProgramArea FocusedProgramArea { get; set; }
        
        public IEnumerable<SchoolYear> YearsOpenForDataSubmission { get; set; }

        public IEnumerable<RecordsRequest> RecordsRequests { get; set; }

        public IEnumerable<SubmissionCycle> SubmissionCycles { get; set; }
    }
}