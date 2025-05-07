using PubQuizBackend.Model.DbModel;

namespace PubQuizBackend.Repository.Interface
{
    public interface ICityRepository
    {
        public Task<List<City>> GetAllCities();
        public Task<List<City>?> GetCitiesByCountryId(int id);
        public Task<City?> GetCityById(int id);
        public Task<City?> GetCityByName(string name);
        public Task<City> AddCity(City city);
        public Task<bool> UpdateCity(City city);
        public Task<bool> DeleteCity(City city);
    }
}
