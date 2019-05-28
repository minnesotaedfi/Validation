using System.ComponentModel.DataAnnotations.Schema;
using ValidationWeb.Database;

namespace ValidationWeb.Models
{
    public enum ErrorSeverity
    {
        Error = 0,
        Warning = 1
    }

    [Table("validation.ErrorSeverityLookup")]
    public class ErrorSeverityLookup : EnumLookup { }
}