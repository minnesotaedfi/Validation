using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ValidationWeb
{
    [Table("validation.AppUserSession")]
    public class AppUserSession
    {
        public AppUserSession()
        {
            DismissedAnnouncements = new HashSet<Announcement>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Logged in user.
        /// </summary>
        [ForeignKey("AppUser")]
        public int AppUserId { get; set; }
        public AppUser AppUser { get; set; }

        /// <summary>
        /// This is the EdOrg that the user is acting on/viewing ... chosen in the application/website.
        /// </summary>
        [ForeignKey("FocusedEdOrg")]
        public string FocusedEdOrgId { get; set; }
        public EdOrg FocusedEdOrg { get; set; }

        /// <summary>
        /// Announcements that have been dismissed by the user during this session.
        /// </summary>
        public ICollection<Announcement> DismissedAnnouncements { get; set; }
    }
}