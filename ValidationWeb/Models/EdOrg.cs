using System;
using System.Collections.Generic;
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
            AppUsers = new HashSet<AppUser>();
        }

        [ForeignKey("Parent")]
        public EdOrg ParentEdOrgId { get; set; }
        public EdOrg Parent { get; set; }

        [ForeignKey("EdOrgTypeLookup")]
        public int EdOrgTypeLookupId { get; set; }
        public EdOrgTypeLookup Type { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }

        public ICollection<Announcement> Announcements { get; set; }
        public ICollection<AppUser> AppUsers { get; set; }

        public bool TryGetEdOrgType(out EdOrgType edOrgType)
        {
            return Enum.TryParse<EdOrgType>(Type.CodeValue, true, out edOrgType);
        }
    }
}