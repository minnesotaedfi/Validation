namespace Engine.Models
{
    public class RuleErrors
    {
        public long Id { get; set; }
        public string RuleId { get; set; }
        public bool IsError { get; set; }
        public string Message { get; set; }
    }
}