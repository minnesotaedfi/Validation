using ValidationWeb.Models;

namespace ValidationWeb.ViewModels
{
    public class OdsReportsViewModel
    {
        public ValidationPortalIdentity User { get; set; }

        public int EdOrgId { get; set; }

        public string EdOrgName { get; set; }
    }
}