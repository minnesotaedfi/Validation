namespace ValidationWeb
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [ComplexType]
    [Table("validation.RecordsRequestDetail")]
    public class RecordsRequestDetail
    {
        public int Id { get; set; }
   
        public bool Requested { get; set; }

        public bool Sent { get; set; }

        public string RequestingUserId { get; set; }

        public int RequestingDistrictId { get; set; }

        public string RespondingUserId { get; set; }

        public int RespondingDistrictId { get; set; }
    }
}

