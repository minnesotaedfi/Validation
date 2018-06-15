using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValidationWeb
{
    /// <summary>
    /// This class should be kept in sync with the enum above. It will provide an easier way to create a human-readable description in reports from the database, 
    /// since only the ID is stored with the entity, and you would like to see the name in a report. The Entity Framework "Seed" method is used to update these
    /// values in the database.
    /// </summary>
    public partial class EnumLookup
    {
        public virtual int Id { get; set; } // 0
        public virtual string CodeValue { get; set; } // School
        public virtual string Description { get; set; }  // <Longer description of what a school is - when needed, or "pretty" (includes spaces and special characters) version of the CodeValue.>
    }
}