using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValidationWeb
{
    public class SchoolYear
    {
        public SchoolYear() { }
        public SchoolYear(string startYear, string endYear)
        {
            StartYear = startYear;
            EndYear = endYear;
        }
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