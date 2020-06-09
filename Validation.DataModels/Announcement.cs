using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Validation.DataModels;

namespace ValidationWeb.Models
{
    [Table("validation.Announcement")]
    public class Announcement
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        
        /// <summary>
        /// Gets or sets the announcement's priority. A higher number should be displayed before announcements with lower numbers.
        /// </summary>
        public int Priority { get; set; }
        
        /// <summary>
        /// Gets or sets the text of the Announcement. Each line ending should be treated as a separate paragraph when rendered.
        /// </summary>
        [Required(ErrorMessage = "Please enter message")]
        public string Message { get; set; }
        
        /// <summary>
        /// Gets or sets how to contact the originator.
        /// </summary>
        public string ContactInfo { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether Is Emergency. When true, should be displayed with text or color indicating action is needed.
        /// </summary>
        public bool IsEmergency { get; set; }

        /// <summary>
        /// Gets or sets link url. When displayed in HTML, clicking on the announcement should open another browser tab and take the user to this link.
        /// </summary>
        public string LinkUrl { get; set; }
        
        /// <summary>
        /// Gets or sets Local time (and date) after which the Announcement will not be displayed and may be deleted.
        /// </summary>
        [Required(ErrorMessage = "Please enter date")]
        public DateTime Expiration { get; set; }

        [Required]
        [ForeignKey("ProgramArea")]
        public int ProgramAreaId { get; set; }

        public ProgramAreaLookup ProgramArea { get; set; }
    }
}