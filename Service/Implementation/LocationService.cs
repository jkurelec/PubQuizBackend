using PubQuizBackend.Exceptions;
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

        public async Task<LocationDetailedDto> Add(string? locationName = null, string? address = null, string? city = null, string? country = null, int limit = 1, int selection = 0)
        {
            return new(
                await _locationRepository.Add(locationName, address, city, country, limit, selection)
            );
        }

        public async Task<LocationDetailedDto?> CheckIfExists(string? locationName = null, string? address = null, string? city = null, string? country = null)
        {
            var location = await _locationRepository.CheckIfExists(locationName, address, city, country);

            if (location == null)
                return null;

            return new (location);
        }

        public async Task<bool> Delete(int id)
        {
            return await _locationRepository.Delete(id);
        }

        public async Task<List<LocationDetailedDto>> FindNew(string? locationName = null, string? address = null, string? city = null, string? country = null, int limit = 1)
        {
            return await _locationRepository.FindNew(locationName, address, city, country, limit);
        }

        public async Task<List<LocationDetailedDto>> GetAll()
        {
            return await _locationRepository.GetAll().ContinueWith(x => x.Result.Select(x => new LocationDetailedDto(x)).ToList());
        }

        public async Task<LocationDetailedDto> GetById(int id)
        {
            return new(await _locationRepository.GetById(id));
        }

        public async Task<LocationDetailedDto> GetByName(string name)
        {
            return new(await _locationRepository.GetByName(name));
        }

        public async Task<List<LocationDetailedDto>> GetByCityId(int id)
        {
            return await _locationRepository.GetByCityId(id).ContinueWith(x => x.Result.Select(x => new LocationDetailedDto(x)).ToList());
        }

        public async Task<Location> Update(LocationUpdateDto location)
        {
            return await _locationRepository.Update(location);
        }

        public async Task<List<LocationDetailedDto>> SearchByText(string searchText, int limit = 10)
        {
            var locations = await _locationRepository.SearchByText(searchText, limit);
            return locations.Select(l => new LocationDetailedDto(l)).ToList();
        }
    }
}
