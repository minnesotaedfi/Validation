using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ValidationWeb
{
    /// <summary>
    /// The Entity Framework "Seed" method (ValidationPortalDbMigrationConfiguration) is used to update these values in the database.
    /// </summary>
    public partial class EnumLookup
    {
        [Key]
        public virtual int Id { get; set; } // 0
        public virtual string CodeValue { get; set; } // School
        public virtual string Description { get; set; }  // <Longer description of what a school is - when needed, or "pretty" (includes spaces and special characters) version of the CodeValue.>
    }
}