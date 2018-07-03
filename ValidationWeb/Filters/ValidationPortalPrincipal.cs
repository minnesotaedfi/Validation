using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Principal;
using System.ComponentModel.DataAnnotations;

namespace ValidationWeb
{
    public class ValidationPortalPrincipal : IPrincipal
    {
        public ValidationPortalPrincipal(ValidationPortalIdentity ident)
        {
            Identity = ident;
        }
        public IIdentity Identity { get; set; }
        public bool IsInRole(string role)
        {
            var portalIdentity = Identity as ValidationPortalIdentity;
            if (portalIdentity == null)
            {
                return false;
            }
            var roleSought = AppRole.CreateAppRole(role);
            return portalIdentity.AppRole >= roleSought;      
        }
    }

    /// <summary>
    /// A representation of the authenticated Validation Portal user, for use in HTTP Contexts.
    /// </summary>
    public class ValidationPortalIdentity : IIdentity
    {
        public string AuthenticationType { get { return "MNDOE_SSO"; } }
        public bool IsAuthenticated { get { return true; } }
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



