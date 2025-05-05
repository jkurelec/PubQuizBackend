using PubQuizBackend.Model.DbModel;

namespace PubQuizBackend.Model.Dto
{
    public class LocationDetailsDto
    {
        public LocationDetailsDto() {}

        public LocationDetailsDto(Location location)
        {
            Id = location.Id;
            Name = location.Name;
            Address = location.Address;
            PostalCodeId = location.PostalCodeId;
            PostalCode = location.PostalCode.Code;
            CityId = location.CityId;
            City = location.City.Name;
            Country = location.City.Country.Name;
            CountryCode = location.City.Country.CountryCode;
            Lat = location.Lat ?? 0;
            Lon = location.Lon ?? 0;
        }

        public int? Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public int? PostalCodeId { get; set; }
        public string PostalCode { get; set; }
        public int? CityId { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }

        public Location ToLocation(PostalCode? postalCode = null)
        {
            var location = new Location
            {
                Name = Name,
                Address = Address,
                Lat = Lat,
                Lon = Lon,
                PostalCode = postalCode
                ?? new PostalCode
                {
                    Id = (int)PostalCodeId!,
                    Code = PostalCode,
                    City = new City
                    {
                        Id = (int)CityId!,
                        Name = City,
                        Country = new Country
                        {
                            Name = Country,
                            CountryCode = CountryCode
                        }
                    }
                },
                CityId = postalCode != null ? postalCode.CityId : (int)CityId!
            };

            if (Id.HasValue)
                location.Id = Id.Value;

            return location;
        }
    }
}
