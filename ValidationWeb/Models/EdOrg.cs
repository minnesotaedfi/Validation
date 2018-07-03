using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ValidationWeb
{
    [Table("validation.EdOrg")]
    public class EdOrg
    {
        public EdOrg()
        {
            Announcements = new HashSet<Announcement>();
        }

        /// <summary>
        /// Not database generated - but comes from ODS - comes from assignment via code, or bulk-loading this table.
        /// </summary>
        [Key]
        public string Id { get; set; }
        public string OrganizationName { get; set; }
        public string StateOrganizationId { get; set; }
        public string FormattedOrganizationId { get; set; }
        public string DistrictName { get; set; }

        [ForeignKey("Parent")]
        public EdOrg ParentEdOrgId { get; set; }
        public EdOrg Parent { get; set; }

        [ForeignKey("EdOrgTypeLookup")]
        public int EdOrgTypeLookupId { get; set; }
        public EdOrgTypeLookup Type { get; set; }

        public ICollection<Announcement> Announcements { get; set; }

        public bool TryGetEdOrgType(out EdOrgType edOrgType)
        {
            return Enum.TryParse<EdOrgType>(Type.CodeValue, true, out edOrgType);
        }
    }
}