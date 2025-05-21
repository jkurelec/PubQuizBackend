using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.LocationDto;

namespace PubQuizBackend.Repository.Interface
{
    public interface ILocationRepository
    {
        public Task<List<Location>> GetAll();
        public Task<List<Location>> GetByCityId(int id);
        public Task<Location> GetById(int id);
        public Task<Location> GetByName(string name);
        public Task<Location> Add(string? locationName = null, string? address = null, string? city = null, string? country = null, int limit = 1, int selection = 0);
        public Task<Location> Update(LocationUpdateDto Location);
        public Task<bool> Delete(int id);
        public Task<List<LocationDetailedDto>> FindNew(string? locationName = null, string? address = null, string? city = null, string? country = null, int limit = 1);
        public Task<Location?> CheckIfExists(string? locationName = null, string? address = null, string? city = null, string? country = null);
    }
}
