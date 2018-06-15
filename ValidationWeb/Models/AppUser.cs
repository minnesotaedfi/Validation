using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ValidationWeb
{
    [Table("validation.AppUser")]
    public class AppUser
    {
        public AppUser()
        {
            AuthorizedEdOrgs = new HashSet<EdOrg>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<EdOrg> AuthorizedEdOrgs { get; set; }
    }
}