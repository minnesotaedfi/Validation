using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValidationWeb
{
    public class AdminIndexViewModel
    {
        public AppUserSession AppUserSession { get; set; }
        /// <summary>
        /// Ed Orgs the use is allowed to see.
        /// </summary>
        public IEnumerable<EdOrg> AuthorizedEdOrgs { get; set; }
        /// <summary>
        /// Ed Org currently selected (being acting on when a user takes actions designed to affect a single EdOrg)
        /// </summary>
        public EdOrg FocusedEdOrg { get; set; }
        public IEnumerable<SchoolYear> YearsOpenForDataSubmission { get; set; }
    }
}