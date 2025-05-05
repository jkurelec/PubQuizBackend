using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto;

namespace PubQuizBackend.Repository.Interface
{
    public interface ILocationRepository
    {
        public Task<List<Location>> GetAllLocations();
        public Task<List<Location>> GetLocationsByCityId(int id);
        public Task<Location?> GetLocationById(int id);
        public Task<Location?> GetLocationByName(string name);
        public Task<LocationDetailsDto> Add(string? locationName = null, string? address = null, string? city = null, string? country = null, int limit = 1);
        public Task<bool> Update(Location Location);
        public Task<bool> Delete(int id);
        public Task<List<LocationDetailsDto>?> FindNew(string locationName, string address, string city, string country, int limit = 1);
        public Task<LocationDetailsDto?> CheckIfExists(string? locationName = null, string? address = null, string? city = null, string? country = null, int limit = 1);
    }
}
