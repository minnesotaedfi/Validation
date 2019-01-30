namespace ValidationWeb.Models
{
    using Newtonsoft.Json;

    [JsonObject]
    public class RecordsResponseFormData
    {
        [JsonProperty("requestId")]
        public int RequestId { get; set; }

        [JsonProperty("studentId")]
        public string StudentId { get; set; }

        [JsonProperty("responding-user-id")]
        public string RespondingUserId { get; set; }

        //[JsonProperty("responding-district-id")]
        //public int RespondingDistrictId { get; set; }

        [JsonProperty("check-assessment-sent")]
        [JsonConverter(typeof(BoolConverter))]
        public bool CheckAssessment { get; set; }

        [JsonProperty("check-cumulative-sent")]
        [JsonConverter(typeof(BoolConverter))]
        public bool CheckCumulative { get; set; }

        [JsonProperty("check-discipline-sent")]
        [JsonConverter(typeof(BoolConverter))]
        public bool CheckDiscipline { get; set; }

        [JsonProperty("check-iep-sent")]
        [JsonConverter(typeof(BoolConverter))]
        public bool CheckIEP { get; set; }

        [JsonProperty("check-evaluation-sent")]
        [JsonConverter(typeof(BoolConverter))]
        public bool CheckEvaluation { get; set; }

        [JsonProperty("check-immunization-sent")]
        [JsonConverter(typeof(BoolConverter))]
        public bool CheckImmunization { get; set; }
    }
}