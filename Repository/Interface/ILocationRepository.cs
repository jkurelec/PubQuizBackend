using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.LocationDto;

namespace PubQuizBackend.Repository.Interface
{
    public interface ILocationRepository
    {
        Task<List<Location>> GetAll();
        Task<List<Location>> GetByCityId(int id);
        Task<Location> GetById(int id);
        Task<Location> GetByName(string name);
        Task<Location> Add(string? locationName = null, string? address = null, string? city = null, string? country = null, int limit = 1, int selection = 0);
        Task<Location> Update(LocationUpdateDto Location);
        Task<bool> Delete(int id);
        Task<List<LocationDetailedDto>> FindNew(string? locationName = null, string? address = null, string? city = null, string? country = null, int limit = 1);
        Task<Location?> CheckIfExists(string? locationName = null, string? address = null, string? city = null, string? country = null);
    }
}
