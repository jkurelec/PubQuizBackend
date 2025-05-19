using PubQuizBackend.Model.DbModel;

namespace PubQuizBackend.Model.Dto.LocationDto
{
    public class LocationBriefDto
    {
        public LocationBriefDto() { }

        public LocationBriefDto(Location location)
        {
            Id = location.Id;
            Name = location.Name;
            Address = location.Address;
            City = location.City.Name;
            Country = location.City.Country.Name;
        }

        public int? Id { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Country { get; set; } = null!;
    }
}
