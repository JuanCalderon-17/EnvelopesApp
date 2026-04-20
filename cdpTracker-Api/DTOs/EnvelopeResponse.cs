namespace cdpTracker_Api.DTOs
{
    public class EnvelopeResponse
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime RecordedAt { get; set; }
        public int WorkerId { get; set; }
        public string WorkerName { get; set; } = string.Empty;
        public string Kiosko { get; set; } = string.Empty;
    }
}
