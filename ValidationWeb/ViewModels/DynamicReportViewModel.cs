using System.Collections.Generic;
using ValidationWeb.Models;

namespace ValidationWeb.ViewModels
{
    public class DynamicReportViewModel
    {
        /// <summary>
        /// Gets or sets Ed Org currently selected (being acting on when a user takes actions designed to affect a single EdOrg)
        /// </summary>
        public EdOrg FocusedEdOrg { get; set; }

        public SchoolYear SchoolYear { get; set; }
    }
}