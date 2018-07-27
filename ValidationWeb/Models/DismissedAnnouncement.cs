using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ValidationWeb
{
    [Table("validation.DemissedAnnouncement")]
    public class DismissedAnnouncement
    {
        [Key]
        [Column(Order = 0)]
        public string AppUserSessionId { get; set; }
        public AppUserSession AppUserSession { get; set; }
        [Key]
        [Column(Order = 1)]
        public int AnnouncementId { get; set; }
    }
}