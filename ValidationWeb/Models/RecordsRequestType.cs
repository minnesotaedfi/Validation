namespace ValidationWeb
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    public enum RecordsRequestType
    {
        Assessment = 0,
        Cumulative = 1,
        Discipline = 2, 
        IEP = 3, 
        Evaluation = 4, 
        Immunizations = 5
    }

    [Serializable]
    [Table("validation.RecordsRequestTypeLookup")]
    public class RecordsRequestTypeLookup : EnumLookup { }
}