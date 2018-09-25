using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ValidationWeb
{
    [Table("validation.SchoolYear")]
    public class SchoolYear
    {
        public SchoolYear() { }

        public SchoolYear(string startYear, string endYear)
        {
            StartYear = startYear;
            EndYear = endYear;
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public bool Enabled { get; set; }
        public decimal? ErrorThreshold { get; set; }

        public string StartYear { get; set; }
        public string EndYear { get; set; }

        public override string ToString()
        {
            var hasStart = ! string.IsNullOrWhiteSpace(StartYear);
            var hasEnd = ! string.IsNullOrWhiteSpace(EndYear);
            if (hasStart && hasEnd)
            {
                return $"{StartYear.Trim()}-{EndYear.Trim()}";
            }
            else if (hasStart)
            {
                return StartYear.Trim();
            }
            else if (hasEnd)
            {
                return EndYear.Trim();
            }
            return "Unknown";
        }
    }
}