using System.Collections.Generic;
using ValidationWeb.Models;

namespace ValidationWeb.ViewModels
{
    public class SelectOrgViewModel
    {
        public AppUserSession AppUserSession { get; set; }

        /// <summary>
        /// Gets or sets Ed Orgs the use is allowed to see.
        /// </summary>
        public IEnumerable<EdOrg> AuthorizedEdOrgs { get; set; }

        /// <summary>
        /// Gets or sets Ed Org currently selected (being acting on when a user takes actions designed to affect a single EdOrg)
        /// </summary>
        public EdOrg FocusedEdOrg { get; set; }
    }
}