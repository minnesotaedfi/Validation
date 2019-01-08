namespace ValidationWeb
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("validation.RecordsRequest")]
    public class RecordsRequest
    {
        public RecordsRequest()
        {
            AssessmentResults = new RecordsRequestDetail();
            CumulativeFiles = new RecordsRequestDetail();
            DisciplineRecords = new RecordsRequestDetail();
            IEP = new RecordsRequestDetail();
            EvaluationSummary = new RecordsRequestDetail();
            Immunizations = new RecordsRequestDetail();
        }

        [Key]
        public int Id { get; set; }

        public string StudentId { get; set; }

        public int SchoolYearId { get; set; }

        public string TransmittalInstructions { get; set; }

        public int RequestingDistrict { get; set; }

        public string RequestingUser { get; set; }

        public int RespondingDistrict { get; set; }
        
        public string RespondingUser { get; set; }

        public RecordsRequestDetail AssessmentResults { get; set; }

        public RecordsRequestDetail CumulativeFiles { get; set; }

        public RecordsRequestDetail DisciplineRecords { get; set; }

        public RecordsRequestDetail IEP { get; set; }

        public RecordsRequestDetail EvaluationSummary { get; set; }

        public RecordsRequestDetail Immunizations { get; set; }
    }
}