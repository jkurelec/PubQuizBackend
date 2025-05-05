using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto;
using PubQuizBackend.Repository.Interface;
using PubQuizBackend.Service.Interface;

namespace PubQuizBackend.Service.Implementation
{
    public class LocationService : ILocationService
    {
        private readonly ILocationRepository _locationRepository;

        public LocationService(ILocationRepository locationRepository)
        {
            _locationRepository = locationRepository;
        }

        public async Task<LocationDetailsDto> Add(string? locationName = null, string? address = null, string? city = null, string? country = null, int limit = 1)
        {
            return await _locationRepository.Add(locationName, address, city, country, limit);
        }

        public async Task<LocationDetailsDto?> CheckIfExists(string? locationName = null, string? address = null, string? city = null, string? country = null, int limit = 1)
        {
            return await _locationRepository.CheckIfExists(locationName, address, city, country, limit);
        }
        public async Task<bool> Delete(int id)
        {
            return await _locationRepository.Delete(id);
        }

        public async Task<List<LocationDetailsDto>?> FindNew(string locationName, string address, string city, string country, int limit = 1)
        {
            return await _locationRepository.FindNew(locationName, address, city, country, limit);
        }

        public async Task<List<Location>> GetAllLocations()
        {
            return await _locationRepository.GetAllLocations();
        }

        public async Task<Location?> GetLocationById(int id)
        {
            return await _locationRepository.GetLocationById(id);
        }

        public async Task<Location?> GetLocationByName(string name)
        {
            return await _locationRepository.GetLocationByName(name);
        }

        public async Task<List<Location>> GetLocationsByCityId(int id)
        {
            return await _locationRepository.GetLocationsByCityId(id);
        }

        public async Task<bool> Update(Location Location)
        {
            return await _locationRepository.Update(Location);
        }
    }
}
