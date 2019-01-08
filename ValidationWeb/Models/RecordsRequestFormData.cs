namespace ValidationWeb.Models
{
    using Newtonsoft.Json;

    [JsonObject]
    public class RecordsRequestFormData
    {
        [JsonProperty("requestId")]
        public int RequestId { get; set; }

        [JsonProperty("studentId")]
        public string StudentId { get; set; }

        [JsonProperty("requesting-user-id")]
        public string RequestingUserId { get; set; }

        [JsonProperty("requesting-district-id")]
        public string RequestingDistrictId { get; set; }

        [JsonProperty("responding-district-id")]
        public string RespondingDistrictId { get; set; }

        [JsonProperty("check-assessment")]
        [JsonConverter(typeof(BoolConverter))]
        public bool CheckAssessment { get; set; }

        [JsonProperty("check-cumulative")]
        [JsonConverter(typeof(BoolConverter))]
        public bool CheckCumulative { get; set; }

        [JsonProperty("check-discipline")]
        [JsonConverter(typeof(BoolConverter))]
        public bool CheckDiscipline { get; set; }

        [JsonProperty("check-iep")]
        [JsonConverter(typeof(BoolConverter))]
        public bool CheckIEP { get; set; }

        [JsonProperty("check-evaluation")]
        [JsonConverter(typeof(BoolConverter))]
        public bool CheckEvaluation { get; set; }

        [JsonProperty("check-immunization")]
        [JsonConverter(typeof(BoolConverter))]
        public bool CheckImmunization { get; set; }

        [JsonProperty("transmittal-instructions")]
        public string TransmittalInstructions { get; set; }
    }
}