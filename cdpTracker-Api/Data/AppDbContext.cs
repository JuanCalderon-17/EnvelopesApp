using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

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
    }
}