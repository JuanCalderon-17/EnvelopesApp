using cdpTracker_Api.Model;
using System.ComponentModel.DataAnnotations;

namespace cdpTracker_Api.Models
{
    public class Envelope
    {
        public int Id { get; set; }

        [Required]
        [StringLength(4, MinimumLength = 4)] // Ensures exactly 4 digits
        public string Code { get; set; } = string.Empty;

        [Required]
        public decimal Amount { get; set; }

        public DateTime RecordedAt { get; set; } = DateTime.UtcNow;

        // Foreign Key to Worker
        public int WorkerId { get; set; }
        public Worker Worker { get; set; } = null!;
    }
}