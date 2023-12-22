using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.DbContexts
{
    public class CityInfoContext : DbContext
    {
        public DbSet<City> Cities { get; set; }
        public DbSet<PointOfInterest> PointsOfInterests { get; set; }

        public CityInfoContext(DbContextOptions<CityInfoContext> options) 
            : base(options)
        {

        }
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer();
        //    base.OnConfiguring(optionsBuilder);
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<City>()
                .HasData(new City("New York City")
                {
                    Id = 1,
                    Description = "the one that with big park."
                },
                new City("Antwerp")
                {
                    Id = 2,
                    Description = "the one with cathedral that was never really"
                },
                new City("Paris")
                {
                    Id = 3,
                    Description = "The one htat with big tower."
                });

            modelBuilder.Entity<PointOfInterest>()
                 .HasData(
                        new PointOfInterest("Central Park")
                        {
                            Id = 1,
                            CityId = 1,
                            Description = "The most visited urban park in the united states."
                        },
                        new PointOfInterest("Empire State Building")
                        {
                            Id = 2,
                            CityId = 1,
                            Description = "A 102-story skyscrapper located in Midtown Manhattan"
                        },
                        new PointOfInterest("Cathedral of our lady")
                        {
                            Id = 3,
                            CityId = 2,
                            Description = "a Gothic style cathedral, conceived by architects Jan and Pietersburg"
                        },

                        new PointOfInterest("Antwerp Central Station")
                        {
                            Id = 4,
                            CityId = 2,
                            Description = "The finest example of railway architecture in Belgium."
                        },
                        new PointOfInterest("Eiffel Tower")
                        {
                            Id = 5,
                            CityId = 3,
                            Description = "A wrought iron lattice tower on the Champ de Mars"
                        },
                        new PointOfInterest("The Louvre")
                        {
                            Id = 6,
                            CityId = 3,
                            Description = "The world's largest museum"
                        });
            base.OnModelCreating(modelBuilder);
        }
    }
}
