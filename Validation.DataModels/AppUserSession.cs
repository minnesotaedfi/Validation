using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ValidationWeb.Models
{
    [Table("validation.AppUserSession")]
    public class AppUserSession
    {
        [Key]
        [StringLength(64)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the Logged in user.
        /// </summary>
        [NotMapped]
        public ValidationPortalIdentity UserIdentity { get; set; }

        public DateTime ExpiresUtc { get; set; }

        /// <summary>
        /// Gets or sets the EdOrg that the user is acting on/viewing ... chosen in the application/website.
        /// </summary>
        public int FocusedEdOrgId { get; set; }

        /// <summary>
        /// Gets or sets SchoolYear that the user is acting on/viewing ... chosen in the application/website.
        /// </summary>
        public int FocusedSchoolYearId { get; set; }

        public int FocusedProgramAreaId { get; set; }
    }
}