using System;
using System.ComponentModel.DataAnnotations.Schema;

using ValidationWeb.Database;

namespace Validation.DataModels
{
    public enum ProgramArea
    {
        Marss = 0, 
        Mccc = 1, 
        Ee = 2
    }

    [Serializable]
    [Table("validation.ProgramArea")]
    public class ProgramAreaLookup : EnumLookup
    {
    }
}
