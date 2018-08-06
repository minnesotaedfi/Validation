using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ValidationWeb.Services;

namespace ValidationWeb
{
    public class NavMenusViewModel
    {
        public AppUserSession AppUserSession { get; set; }
        /// <summary>
        /// Ed Org currently selected (being acting on when a user takes actions designed to affect a single EdOrg)
        /// </summary>
        public EdOrg FocusedEdOrg { get; set; }
        public SchoolYear FocusedSchoolYear { get; set; }
        public IEnumerable<SchoolYear> SchoolYears { get; set; }
        public IEnumerable<EdOrg> EdOrgs { get; set; }
    }
}