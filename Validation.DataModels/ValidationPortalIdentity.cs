using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;

namespace ValidationWeb.Models
{
    /// <summary>
    /// A representation of the authenticated Validation Portal user, for use in HTTP Contexts.
    /// </summary>
    [Serializable]
    public class ValidationPortalIdentity : IIdentity
    {
        public string AuthenticationType => "Minnesota DOE SSO";

        public bool IsAuthenticated => true;

        public string Name { get; set; }

        public AppRole AppRole { get; set; }

        [Key]
        public string UserId { get; set; }
        
        public string FirstName { get; set; }
        
        public string MiddleName { get; set; }
        
        public string LastName { get; set; }
        
        public string FullName { get; set; }
        
        public string Email { get; set; }
        
        public ICollection<EdOrg> AuthorizedEdOrgs { get; set; }
    }
}