using System.Collections.Generic;

namespace ValidationWeb.Models
{
    public class FilteredValidationErrors
    {
        public IList<ValidationErrorSummary> FilteredErrorSummariesPage { get; set; }
        
        public int TotalFilteredErrorCount { get; set; }

        public int PageSize { get; set; }
        
        public int PageNumber { get; set; }
        
        public int RecordOffset { get; set; }
        
        public int TotalPagesCount { get; set; }
    }
}