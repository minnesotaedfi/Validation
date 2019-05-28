using System;
using System.ComponentModel.DataAnnotations.Schema;
using ValidationWeb.Database;

namespace ValidationWeb.Models
{
    public enum EdOrgType
    {
        /// <summary>
        /// A School
        /// </summary>
        School = 0,

        /// <summary>
        /// A District
        /// </summary>
        District = 1,

        /// <summary>
        /// A Region
        /// </summary>
        Region = 2,

        /// <summary>
        /// A State
        /// </summary>
        State = 3
    }

    [Serializable]
    [Table("validation.EdOrgTypeLookup")]
    public class EdOrgTypeLookup : EnumLookup
    {
    }
}