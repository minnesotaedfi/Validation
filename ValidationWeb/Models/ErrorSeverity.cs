using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ValidationWeb
{
    public enum ErrorSeverity
    {
        Error = 0,
        Warning = 1
    }

    [Table("validation.ErrorSeverityLookup")]
    public class ErrorSeverityLookup : EnumLookup { }
}