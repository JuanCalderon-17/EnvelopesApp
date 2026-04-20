using Microsoft.EntityFrameworkCore;
using cdpTracker_Api.Models;

namespace cdpTracker_Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Worker> Workers { get; set; }
        public DbSet<Envelope> Envelopes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Worker>().HasData(
                new Worker
                {
                    Id = 1,
                    Name = "Kiosko 2",
                    PasswordHash = "$2a$11$iE/mpYk0zTDDKPDtfhxPyOZ2lFfxnlAGP/sn0p.yR3PQZtG7e3OS2",
                    Kiosko = KioskLocation.K2,
                    Role = UserRole.Worker
                },
                new Worker
                {
                    Id = 2,
                    Name = "Kiosko 3",
                    PasswordHash = "$2a$11$Qhy6d5hyPmjk5kIKhJ8kSek.0wObNSSsHzE6B83DFnnNW6R1cN9sm",
                    Kiosko = KioskLocation.K3,
                    Role = UserRole.Worker
                },
                new Worker
                {
                    Id = 3,
                    Name = "Kiosko 5",
                    PasswordHash = "$2a$11$i8X0D4..nZMXolIfRO9RZOS9NZUclVLIZydpOjlej.S7D2m5fUnU.",
                    Kiosko = KioskLocation.K5,
                    Role = UserRole.Worker
                }
            );
        }
    }
}
