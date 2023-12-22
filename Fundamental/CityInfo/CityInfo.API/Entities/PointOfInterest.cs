using System.ComponentModel.DataAnnotations;

namespace CityInfo.API.Entities
{
    public class PointOfInterest
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Description { get; set; }
        public City City { get; set; }
        public int CityId { get; set; }
        public PointOfInterest(string name)
        {
            Name = name;
        }
    }
}