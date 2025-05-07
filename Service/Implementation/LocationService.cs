using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.LocationDto;
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

        public async Task<LocationDetailsDto> Add(string? locationName = null, string? address = null, string? city = null, string? country = null, int limit = 1, int selection = 0)
        {
            return new(await _locationRepository.Add(locationName, address, city, country, limit, selection));
        }

        public async Task<LocationDetailsDto?> CheckIfExists(string? locationName = null, string? address = null, string? city = null, string? country = null)
        {
            var location = await _locationRepository.CheckIfExists(locationName, address, city, country);
            
            if (location == null)
                return null;

            return new(location);
        }

        public async Task<bool> Delete(int id)
        {
            return await _locationRepository.Delete(id);
        }

        public async Task<List<LocationDetailsDto>?> FindNew(string? locationName = null, string? address = null, string? city = null, string? country = null, int limit = 1)
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

        public async Task<List<Location>?> GetLocationsByCityId(int id)
        {
            return await _locationRepository.GetLocationsByCityId(id);
        }

        public async Task<Location?> Update(LocationUpdateDto location)
        {
            return await _locationRepository.Update(location);
        }
    }
}
