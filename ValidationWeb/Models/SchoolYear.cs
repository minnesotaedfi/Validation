using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ValidationWeb.Models
{
    [Table("validation.SchoolYear")]
    public class SchoolYear : IEquatable<SchoolYear>
    {
        public SchoolYear()
        {
        }

        public SchoolYear(string startYear, string endYear, bool enabled = true)
        {
            StartYear = startYear;
            EndYear = endYear;
            Enabled = enabled;
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        
        public bool Enabled { get; set; }

        public decimal? ErrorThreshold { get; set; }

        public string StartYear { get; }

        public string EndYear { get; }

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

        public bool Equals(SchoolYear other)
        {
            return other != null && Id == other.Id && string.Equals(StartYear, other.StartYear) && string.Equals(EndYear, other.EndYear);
        }

        public override bool Equals(object obj)
        {
            var schoolYear = obj as SchoolYear;
            if (schoolYear != null)
            {
                return Equals(schoolYear);
            }

            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id;
                hashCode = (hashCode * 397) ^ (StartYear != null ? StartYear.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (EndYear != null ? EndYear.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}