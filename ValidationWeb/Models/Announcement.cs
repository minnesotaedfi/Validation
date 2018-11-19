using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ValidationWeb
{
    [Table("validation.Announcement")]
    public partial class Announcement
    {
        public Announcement()
        {
            LimitToEdOrgs = new HashSet<EdOrg>();
            AppUserSessions = new HashSet<AppUserSession>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// A higher number should be displayed before announcements with lower numbers.
        /// </summary>
        //[Required(ErrorMessage = "Please enter priority")]
        [Range (0, 10)]
        public int Priority { get; set; }
        /// <summary>
        /// Text of the Annoucement. Each line ending should be treated as a separate paragraph when rendered.
        /// </summary>
        [Required(ErrorMessage = "Please enter message")]
        public string Message { get; set; }
        /// <summary>
        /// How to contact the originator.
        /// </summary>

        public string ContactInfo { get; set; }
        /// <summary>
        /// When true, should be displayed with text or color indicating action is needed.
        /// </summary>
        public bool IsEmergency { get; set; }
        /// <summary>
        /// When displayed in HTML, clicking on the annoucement should open another browser tab and take the user to this link.
        /// </summary>
        
        public string LinkUrl { get; set; }
        /// <summary>
        /// Local time (and date) after which the Announcement will not be displayed and may be deleted.
        /// </summary>
        [Required(ErrorMessage = "Please enter date")]
        public DateTime Expiration { get; set; }
        /// <summary>
        /// If not null, then the annoucement will only be displayed to a user with access to at least one of these EdOrgs.
        /// </summary>
        public ICollection<EdOrg> LimitToEdOrgs { get; set; }
        /// <summary>
        /// Sessions that have dismissed this announcement.
        /// </summary>
        public ICollection<AppUserSession> AppUserSessions { get; set; }
    }
}