using System.ComponentModel.DataAnnotations;

namespace cdpTracker_Api.DTOs
{
    public class CreateEnvelopeRequest
    {
        [Required(ErrorMessage = "The Envelope Code is mandatory.")]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "The Code must have 4 digits")]
        public string Code { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 1000000, ErrorMessage = "The Amount must be greater than zero")]
        public decimal Amount { get; set; }

        [Required]
        public int WorkerId { get; set; }
    }
}