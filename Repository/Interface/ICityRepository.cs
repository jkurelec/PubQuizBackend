using PubQuizBackend.Model.DbModel;

namespace PubQuizBackend.Repository.Interface
{
    public interface ICityRepository
    {
        Task<List<City>> GetAllCities();
        Task<List<City>> GetCitiesByCountryId(int id);
        Task<City> GetCityById(int id);
        Task<City?> GetCityByName(string name);
        Task<City> AddCity(City city);
        Task<bool> UpdateCity(City city);
        Task<bool> DeleteCity(City city);
    }
}
