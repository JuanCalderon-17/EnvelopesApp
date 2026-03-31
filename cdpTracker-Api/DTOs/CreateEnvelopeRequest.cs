namespace cdpTracker_Api.DTOs
{
    public class CreateEnvelopeRequest
    {
        public string Code { get; set; } = string.Empty;
        public decimal Amount { get; set; } 
        public int WorkerId { get; set; }
    }
}

