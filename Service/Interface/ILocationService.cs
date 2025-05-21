using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.LocationDto;

namespace PubQuizBackend.Service.Interface
{
    public interface ILocationService
    {
        public Task<List<LocationDetailedDto>> GetAll();
        public Task<List<LocationDetailedDto>> GetByCityId(int id);
        public Task<LocationDetailedDto> GetById(int id);
        public Task<LocationDetailedDto> GetByName(string name);
        public Task<LocationDetailedDto> Add(string? locationName = null, string? address = null, string? city = null, string? country = null, int limit = 1, int selection = 0);
        public Task<Location> Update(LocationUpdateDto updatedLocation);
        public Task<bool> Delete(int id);
        public Task<List<LocationDetailedDto>> FindNew(string? locationName = null, string? address = null, string? city = null, string? country = null, int limit = 1);
        public Task<LocationDetailedDto> CheckIfExists(string? locationName = null, string? address = null, string? city = null, string? country = null);
    }
}
