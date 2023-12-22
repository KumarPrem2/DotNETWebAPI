using CityInfo.API.Models;

namespace CityInfo.API
{
    public class CitiesDataStore
    {
        public List<CityDto> Cities { get; set; }

        //public static CitiesDataStore Current { get;} = new CitiesDataStore();
        public CitiesDataStore() 
        {
            Cities = new List<CityDto>()
            {
                new CityDto()
                {
                    Id = 1,
                    Name = "New York City",
                    Description = "the one that with big park.",
                    PointOfInterest = new List<PointOfInterestDto>()
                    {
                        new PointOfInterestDto()
                        {
                            Id = 1,
                            Name = "Central Park",
                            Description = "The most visited urban park in the united states."
                        },
                        new PointOfInterestDto()
                        {
                            Id = 2,
                            Name = "Empire State Building",
                            Description = "A 102-story skyscrapper located in Midtown Manhattan"
                        }
                    } 
                },
                new CityDto()
                {
                    Id = 2,
                    Name = "Antwerp",
                    Description = "the one with cathedral that was never really",
                    PointOfInterest = new List<PointOfInterestDto>()
                    {
                        new PointOfInterestDto()
                        {
                            Id = 3,
                            Name = "Cathedral of our lady",
                            Description = "a Gothic style cathedral, conceived by architects Jan and Pietersburg"
                        },

                        new PointOfInterestDto()
                        {
                            Id = 4,
                            Name = "Antwerp Central Station",
                            Description = "The finest example of railway architecture in Belgium."
                        }
                    }
                },

                new CityDto()
                {
                    Id = 3,
                    Name = "Paris",
                    Description = "The one htat with big tower.",
                    PointOfInterest = new List<PointOfInterestDto>()
                    {
                        new PointOfInterestDto()
                        {
                            Id = 5,
                            Name = "Eiffel Tower",
                            Description = "A wrought iron lattice tower on the Champ de Mars"
                        },
                        new PointOfInterestDto()
                        {
                            Id = 6,
                            Name = "The Louvre",
                            Description = "The world's largest museum"
                        }
                    }
                }
            };    
        }
    }
}
