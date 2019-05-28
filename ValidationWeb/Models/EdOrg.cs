using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ValidationWeb.Models
{
    [Serializable]
    [Table("validation.EdOrg")]
    public class EdOrg
    {
        public EdOrg()
        {
            Announcements = new HashSet<Announcement>();
        }

        /// <summary>
        /// Gets or sets Id. Not database generated - but comes from ODS - comes from assignment via code, or bulk-loading this table.
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public int Id { get; set; }
        
        public string OrganizationName { get; set; }
        
        public string OrganizationShortName { get; set; }
        
        public int? StateOrganizationId { get; set; }
        
        public int? ParentId { get; set; }
        
        public int? StateLevelOrganizationId { get; set; }
        
        // Orgs can be different depending on the particular ODS you are referencing. The ODS is determined by the School Year (2019 means 2019-2020).
        public int SchoolYearId { get; set; }
        
        public bool IsStateLevelEdOrg { get; set; }

        public string OrgTypeCodeValue { get; set; }

        public string OrgTypeShortDescription { get; set; }

        [ForeignKey("Type")]
        public int EdOrgTypeLookupId { get; set; }

        public EdOrgTypeLookup Type { get; set; }

        public ICollection<Announcement> Announcements { get; set; }

        public bool TryGetEdOrgType(out EdOrgType edOrgType)
        {
            return Enum.TryParse(Type.CodeValue, true, out edOrgType);
        }

        public override bool Equals(object obj)
        {
            return (obj as EdOrg)?.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}