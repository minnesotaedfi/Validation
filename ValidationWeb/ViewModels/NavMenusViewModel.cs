using System.Collections.Generic;

using Validation.DataModels;

using ValidationWeb.Models;

namespace ValidationWeb.ViewModels
{
    public class NavMenusViewModel
    {
        public AppUserSession AppUserSession { get; set; }
        
        /// <summary>
        /// Gets or sets Ed Org currently selected (being acting on when a user takes actions designed to affect a single EdOrg)
        /// </summary>
        public EdOrg FocusedEdOrg { get; set; }

        public SchoolYear FocusedSchoolYear { get; set; }

        public IEnumerable<SchoolYear> SchoolYears { get; set; }

        public IEnumerable<EdOrg> EdOrgs { get; set; }

        public IEnumerable<ProgramArea> ProgramAreas { get; set; }

        public ProgramArea FocusedProgramArea { get; set; }

        public string EdiamProfileLink { get; set; }

        public string EdiamLogoutLink { get; set; }
    }
}