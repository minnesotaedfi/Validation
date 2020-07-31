using System;
using System.ComponentModel.DataAnnotations.Schema;

using ValidationWeb.Database;

namespace Validation.DataModels
{
    //public enum ProgramArea
    //{
    //    Marss = 1, 
    //    Mccc = 2, 
    //    Ee = 3
    //}

    [Serializable]
    [Table("validation.ProgramArea")]
    public class ProgramArea : EnumLookup
    {
    }
}
