using System.ComponentModel.DataAnnotations;

namespace cdpTracker_Api.DTOs
{
    public class CreateEnvelopeRequest
    {
        [Required(ErrorMessage = "El código del sobre es obligatorio.")]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "El código debe tener exactamente 4 caracteres.")]
        public string Code { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 1000000, ErrorMessage = "El monto debe ser mayor a cero.")]
        public decimal Amount { get; set; }

        [Required]
        public int WorkerId { get; set; }
    }
}