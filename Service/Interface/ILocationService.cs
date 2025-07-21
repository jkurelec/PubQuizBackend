using PubQuizBackend.Model.DbModel;
using PubQuizBackend.Model.Dto.LocationDto;

namespace PubQuizBackend.Service.Interface
{
    public interface ILocationService
    {
        Task<List<LocationDetailedDto>> GetAll();
        Task<List<LocationDetailedDto>> GetByCityId(int id);
        Task<LocationDetailedDto> GetById(int id);
        Task<LocationDetailedDto> GetByName(string name);
        Task<LocationDetailedDto> Add(string? locationName = null, string? address = null, string? city = null, string? country = null, int limit = 1, int selection = 0);
        Task<Location> Update(LocationUpdateDto updatedLocation);
        Task<bool> Delete(int id);
        Task<List<LocationDetailedDto>> FindNew(string? locationName = null, string? address = null, string? city = null, string? country = null, int limit = 1);
        Task<LocationDetailedDto?> CheckIfExists(string? locationName = null, string? address = null, string? city = null, string? country = null);
        Task<List<LocationDetailedDto>> SearchByText(string searchText, int limit = 10);
    }
}
