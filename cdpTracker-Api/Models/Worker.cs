namespace cdpTracker_Api.Models
{
    public class Worker
    {
        public int Id { get; set; } 
        public string Name { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public KioskLocation Kiosko { get; set; }
        public UserRole Role { get; set; }

        public ICollection<Envelope> Envelopes { get; set; } = new List<Envelope>();
    }

    public enum KioskLocation { K2, K3, K5 }
    public enum UserRole { Worker, Manager }
}