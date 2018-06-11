using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValidationWeb
{
    public class Announcement
    {
        /// <summary>
        /// A higher number should be displayed before announcements with lower numbers.
        /// </summary>
        public int Priority { get; set; }
        /// <summary>
        /// Text of the Annoucement. Each line ending should be treated as a separate paragraph when rendered.
        /// </summary>
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
        public DateTime Expiration { get; set; }
        /// <summary>
        /// If not null, then the annoucement will only be displayed to a user with access to at least one of these EdOrgs.
        /// </summary>
        public List<EdOrg> LimitToEdOrgs { get; set; }
    }
}