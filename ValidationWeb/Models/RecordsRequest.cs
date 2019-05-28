using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ValidationWeb.Models
{
    /// <summary>
    /// Describes the current status of a records request
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum RecordsRequestStatus
    {
        /// <summary>
        /// The request has been created but no response has been indicated.
        /// </summary>
        Requested = 0, 

        /// <summary>
        /// The responding district has indicated sending some (but not all) of the requested records
        /// </summary>
        PartialResponse = 1,

        /// <summary>
        /// The responding district has indicated sending all of the requested records
        /// </summary>
        ResponseResolved = 2,

        /// <summary>
        /// The requesting district has acknowledged the response
        /// </summary>
        ResolutionAcknowledged = 3
    }

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

        public RecordsRequestStatus? Status { get; set; }

        public string StudentId { get; set; }

        public int SchoolYearId { get; set; }

        public string TransmittalInstructions { get; set; }

        public int RequestingDistrict { get; set; }

        [NotMapped]
        public string RequestingDistrictName { get; set; }

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