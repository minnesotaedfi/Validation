using Validation.DataModels;

using ValidationWeb.Models;

namespace ValidationWeb.ViewModels
{
    public class DynamicReportViewModel
    {
        public EdOrg FocusedEdOrg { get; set; }
        public SchoolYear SchoolYear { get; set; }
        public ValidationPortalIdentity User { get; set; }
        public ProgramAreaLookup FocusedProgramArea { get; set; }
    }
}