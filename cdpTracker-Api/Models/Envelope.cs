using cdpTracker_Api.Models;
using System.ComponentModel.DataAnnotations;

namespace cdpTracker_Api.Models
{
    public class Envelope
    {
        public int Id { get; set; }

        [Required]
        [StringLength(4, MinimumLength = 4)]
        public int Code { get; set; } = int.Empty;

        [Required]
        public decimal Amount { get; set; }

        public DateTime RecordedAt { get; set; } = DateTime.UtcNow;

        // Foreign Key: This links the envelope to a specific Worker
        public int WorkerId { get; set; }
        public Worker Worker { get; set; } = null!;
    }
}