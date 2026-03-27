using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using cdpTracker_Api.Models;

namespace cdpTracker_Api.Data
{
    public class AppDbContext : DbContext
    {
        //class contructor,it takes in the options and passes it to the base class constructor
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // this are the tables in the database, each DbSet represents a table in the database and the type parameter is the model class that represents the table
        public DbSet<Worker> Workers { get; set; }
        public DbSet<Envelope> Envelopes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //seed a default worker to the database, this is useful for testing and development purposes
            modelBuilder.Entity<Worker>().HasData(new Worker
            {
                Id = 1,
                Name = "Admin Manager",
                PasswordHash = "hashed_admin_pass",
                Kiosko = KioskLocation.K2,
                Role = UserRole.Manager
            });


            //seed a default worker to the database, this is useful for testing and development purposes
            modelBuilder.Entity<Worker>().HasData(new Worker
            {
                    Id = 2,
                    Name = "Juan Calderon",
                    PasswordHash = "hashed_john_pass",
                    Kiosko = KioskLocation.K3, 
                    Role = UserRole.Worker
            });
        }
    }
}