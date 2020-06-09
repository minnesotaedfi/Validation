using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Validation.DataModels;

namespace ValidationWeb.Models
{
    [Table("validation.SubmissionCycle")]
    public class SubmissionCycle
    {
        public SubmissionCycle() { }

        public SubmissionCycle(string collectionId, DateTime startDate, DateTime endDate)
        {
            CollectionId = collectionId;
            StartDate = startDate;
            EndDate = endDate;
        }

        // Should be changed to Id
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        public string CollectionId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }
        
        [Required]
        public DateTime EndDate { get; set; }
        public int? SchoolYearId { get; set; }
        
        [NotMapped]
        public string SchoolYearDisplay { get; set; }

        [Required]
        [ForeignKey("ProgramArea")]
        public int ProgramAreaId { get; set; }

        public ProgramAreaLookup ProgramArea { get; set; }

        public override string ToString()
        {
            return StartDate.ToString("d") + " - " + EndDate.ToString("d");
        }
    }
}