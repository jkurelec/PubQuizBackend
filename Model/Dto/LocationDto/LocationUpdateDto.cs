namespace PubQuizBackend.Model.Dto.LocationDto
{
    public class LocationUpdateDto
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public int? PostalCodeId { get; set; }
        public int? CityId { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
    }
}
