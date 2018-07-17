using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ValidationWeb
{
    public enum EdOrgType
    {
        School = 0,
        District = 1,
        Region = 2,
        State = 3
    }

    [Serializable]
    [Table("validation.EdOrgTypeLookup")]
    public class EdOrgTypeLookup : EnumLookup { }
}