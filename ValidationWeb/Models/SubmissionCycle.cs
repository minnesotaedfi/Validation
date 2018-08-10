using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ValidationWeb
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
        public int SubmissionCycleId { get; set; }

        public string CollectionId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public override string ToString()
        {
            return StartDate.ToString("d") + " - " + EndDate.ToString("d");
        }
    }
}